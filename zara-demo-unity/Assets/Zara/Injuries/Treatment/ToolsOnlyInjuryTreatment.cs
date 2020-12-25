using System;
using System.Collections.Generic;
using System.Linq;
using ZaraEngine.Inventory;
using ZaraEngine.StateManaging;

namespace ZaraEngine.Injuries.Treatment
{
    public class ToolsOnlyInjuryTreatment : IAcceptsStateChange
    {
        private const int MaximumTimeBetweenAppliancesInGameMinutes = 5;

        private List<MedicalAppliancesGroup> _toolsNeeded;
        private readonly List<MedicalAppliancesGroup> _toolsOriginal;
        private DateTime? _lastToolTime;

        public Action OnTreated;

        public ToolsOnlyInjuryTreatment(params string[] tools)
        {
            var groups = new List<MedicalAppliancesGroup>();

            groups.AddRange(tools.Select(x =>
            {
                var g = MedicalAppliancesGroup.GetRelevantMedicalGroup(x);

                if (g == null)
                    return new MedicalAppliancesGroup(x);

                return g;
            }));

            _toolsNeeded = groups.ToList();
            _toolsOriginal = groups.ToList();
        }

        public ToolsOnlyInjuryTreatment(string tool, MedicalAppliancesGroup group, params string[] restTools)
        {
            var tools = new List<MedicalAppliancesGroup>();

            tools.Add(new MedicalAppliancesGroup(tool));
            tools.Add(group);
            tools.AddRange(restTools.Select(x =>
            {
                var g = MedicalAppliancesGroup.GetRelevantMedicalGroup(x);

                if (g == null)
                    return new MedicalAppliancesGroup(x);

                return g;
            }));

            _toolsNeeded = tools.ToList();
            _toolsOriginal = tools.ToList();
        }

        public ToolsOnlyInjuryTreatment(MedicalAppliancesGroup group, params string[] restTools)
        {
            var tools = new List<MedicalAppliancesGroup>();

            tools.Add(group);
            tools.AddRange(restTools.Select(x =>
            {
                var g = MedicalAppliancesGroup.GetRelevantMedicalGroup(x);

                if (g == null)
                    return new MedicalAppliancesGroup(x);

                return g;
            }));

            _toolsNeeded = tools.ToList();
            _toolsOriginal = tools.ToList();
        }

        public ToolsOnlyInjuryTreatment(params MedicalAppliancesGroup[] groups)
        {
            _toolsNeeded = groups.ToList();
            _toolsOriginal = groups.ToList();
        }

        public bool OnApplianceTaken(IGameController gc, InventoryMedicalItemBase tool, BodyParts bodyPart, ActiveInjury injury)
        {
            if (_toolsNeeded.Count == 0)
                return true;

            if (bodyPart != injury.BodyPart)
                return false;

            if (_toolsNeeded[0].IsApplicableToGroup(tool.Name))
            {
                if (_lastToolTime.HasValue)
                {
                    if ((gc.WorldTime.Value - _lastToolTime.Value).TotalMinutes > MaximumTimeBetweenAppliancesInGameMinutes)
                    {
                        _toolsNeeded = _toolsOriginal.ToList();
                        _lastToolTime = null;

                        return false;
                    }
                }

                _lastToolTime = gc.WorldTime;

                _toolsNeeded.RemoveAt(0);

                if (_toolsNeeded.Count == 0)
                {
                    injury.Invert();

                    OnTreated?.Invoke();

                    Events.NotifyAll(l => l.InjuryHealed(injury.Injury));
                }

                return true;
            }

            return false;
        }

        #region State Manage

        public IStateSnippet GetState()
        {
            var state = new ToolsOnlyInjuryTreatmentSnippet
            {
                LastToolTime = _lastToolTime,
                ToolsUsed = _toolsOriginal.Count - _toolsNeeded.Count
            };

            return state;
        }

        public void RestoreState(IStateSnippet savedState)
        {
            var state = (ToolsOnlyInjuryTreatmentSnippet)savedState;

            _lastToolTime = state.LastToolTime;
            _toolsNeeded = _toolsOriginal.ToList();

            for (int i = 0; i < state.ToolsUsed; i++)
            {
                _toolsNeeded.RemoveAt(0);
            }
        }

        #endregion 

    }
}
