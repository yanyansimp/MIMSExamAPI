namespace Domain.Entities
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }

        public List<Packaging> Packagings { get; set; } = new List<Packaging>();
    }
}
