using System;
using ZaraEngine.StateManaging;

namespace ZaraEngine.Inventory
{
    public abstract class WaterVesselItemBase : InventoryInfiniteConsumableToolItemBase
    {

        public int DosesLeft { get; private set; }

        public abstract int DosesCount { get; }

        public abstract int WaterValuePerDose { get; }

        public bool IsSafe { get; private set; }

        public bool IsFull => DosesLeft == DosesCount; 

        public bool IsEmpty =>  DosesLeft == 0; 

        public DateTime? LastFillTime { get; private set; }

        public DateTime? LastDisinfectTime { get; private set; }
        
        public DateTime? LastBoilTime { get; private set; }

        public abstract float VesselWeightInGramms{ get; }

        public abstract float WaterDoseWeightInGramms { get; }

        public override float WeightGrammsPerUnit => WaterDoseWeightInGramms * DosesLeft + VesselWeightInGramms; 

        public string NamePostfix
        {
            get
            {
                var safePostfix = IsSafe ? "$Safe" : "$Unsafe";

                if (DosesLeft == 0)
                    return string.Empty;

                return safePostfix + DosesLeft;
            }
        }

        public void FillUp(DateTime gameTime)
        {
            LastFillTime = gameTime;
            LastDisinfectTime = null;
            LastBoilTime = null;
            IsSafe = false;
            DosesLeft = DosesCount;
        }

        public void TakeAwayOneDose()
        {
            if (DosesLeft <= 0)
                return;

            DosesLeft -= 1;
        }

        public void Disinfect(DateTime gameTime)
        {
            IsSafe = true;
            LastDisinfectTime = gameTime;
        }

        public void Boil(DateTime gameTime)
        {
            IsSafe = true;
            LastDisinfectTime = gameTime;
            LastBoilTime = gameTime;
        }

        #region State Manage

        public override IStateSnippet GetState()
        {
            return new InventoryWaterVesselItemSnippet
            {
                Id = this.Id,
                Count = this.Count,
                ItemType = this.GetType(),
                DosesLeft = this.DosesLeft,
                IsSafe = this.IsSafe,
                LastBoilTime = this.LastBoilTime,
                LastDisinfectTime = this.LastDisinfectTime,
                LastFillTime = this.LastFillTime
            };
        }

        public override void RestoreState(IStateSnippet savedState)
        {
            var state = (InventoryWaterVesselItemSnippet)savedState;

            Count = state.Count;
            DosesLeft = state.DosesLeft;
            IsSafe = state.IsSafe;
            LastBoilTime = state.LastBoilTime;
            LastDisinfectTime = state.LastDisinfectTime;
            LastFillTime = state.LastFillTime;
        }

        #endregion

    }
}
