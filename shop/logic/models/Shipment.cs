namespace shop
{
    public class Products
    {
        private readonly string product;
        private readonly int count;

        public Products(string product, int count)
        {
            this.product = product.ToLower();
            this.count = count;
        }

        public string Product => product;
        public int Count => count;
    }

    public class Shipment
    {
        public readonly string product;
        public readonly int count;
        public readonly float price;

        public Shipment(string product, int count, float price)
        {
            this.product = product.ToLower();
            this.count = count;
            this.price = price;
        }
    }
}