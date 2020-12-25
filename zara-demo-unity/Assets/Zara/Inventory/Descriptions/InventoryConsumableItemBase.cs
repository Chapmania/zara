namespace ZaraEngine.Inventory
{
    public abstract class InventoryConsumableItemBase : InventoryItemBase
    {

        public int SpoiledChanceOfFoodPoisoning { get; protected set; }

        public int GeneralChanceOfFoodPoisoning { get; protected set; }

        public override InventoryController.InventoryItemType[] Type
        {
            get { return new [] { InventoryController.InventoryItemType.Organic }; }
        }
    }
}
