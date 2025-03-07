namespace Domain.Entities
{
    public class Item
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }

        public List<PackagingItem> PackagingItems { get; set; } = new List<PackagingItem>();
    }
}
