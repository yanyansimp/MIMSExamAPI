namespace Domain.Entities
{
    public class PackagingItem
    {
        public int PackagingID { get; set; }
        public Packaging Packaging { get; set; }

        public int ItemID { get; set; }
        public Item Item { get; set; }
    }
}
