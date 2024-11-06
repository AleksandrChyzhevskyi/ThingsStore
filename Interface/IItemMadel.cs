namespace _Development.Scripts.ThingsStore.Model
{
    public interface IItemMadel
    {
        void Create();
        void CompareItemParametersWithInventory();
        void ResetStatsInItem();
    }
}