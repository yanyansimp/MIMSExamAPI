namespace Domain.Entities
{
    public class Packaging
    {
        public int PackagingID { get; set; }
        public int ProductID { get; set; }
        public string PackagingType { get; set; } // e.g., Box, Packet

        public int? ParentPackagingID { get; set; }  // Nullable for root packaging
        public Packaging ParentPackaging { get; set; }
        public List<Packaging> SubPackaging { get; set; } = new List<Packaging>();

        public List<PackagingItem> PackagingItems { get; set; } = new List<PackagingItem>();
    }
}
