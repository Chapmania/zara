using System;
using System.Collections.Generic;
using ZaraEngine.Injuries.Stages;
using ZaraEngine.StateManaging;

namespace ZaraEngine.Injuries
{

    public abstract class InjuryBase: IAcceptsStateChange
    {

        public Guid Id { get; } = Guid.NewGuid();

        protected InjuryBase()
        {
            Stages = new List<InjuryStage>();
        }

        public string Name { get; protected set; }

        public ICollection<InjuryStage> Stages { get; protected set; }

        public void SwapChain(List<InjuryStage> chainCopy)
        {
            Stages = chainCopy;
        }

        #region State Manage

        public abstract IStateSnippet GetState();

        public abstract void RestoreState(IStateSnippet state);

        #endregion

    }
}
