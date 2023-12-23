using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using MySql.Data.MySqlClient;

namespace shop
{
    public struct Pair
    {
        public readonly float price;
        public readonly string store;

        public Pair(float price, string store)
        {
            this.price = price;
            this.store = store;
        }
    }
    public class Service
    {
        private Resource resource;

        public Service(bool useDB)
        {
            Connector connector;
            if (useDB)
            {
                connector = new DBConnector();
                resource = connector.Create();
            }
            else
            {
                connector = new FileConnector();
                resource = connector.Create();
            }
        }

        string ToFormat(string line)
        {
            line.Trim();
            return char.ToUpper(line[0]) + line.Substring(1).ToLower();
        }

        string ToDBFormat(string line)
        {
            return line.ToLower();
        }

        Shipment GetShipment(List<string> storeInfo, string productName)
        {
            Shipment shipment = null;

            foreach (var info in storeInfo)
            {
                string[] data = info.Split('$');
                if (ToFormat(data[0]).Equals(ToFormat(productName)))
                {
                    float price = float.Parse(data[1]);
                    int count = int.Parse(data[2]);

                    shipment = new Shipment(data[0], count, price);
                    break;
                }
            }

            return shipment;
        }

        public void createStore(string store)
        {
            resource.CreateStore(ToDBFormat(store));
        }

        public void CreateProduct(string store, string product, float price)
        {
            resource.CreateProduct(ToDBFormat(product), ToDBFormat(store), price);
        }

        public void DeliverShipment(List<Shipment> shipments, string store)
        {
            resource.DeliverShipment(shipments, ToDBFormat(store));
        }

        public List<string> FindCheapestProductStores(string product)
        {
            List<string> prices = resource.FindCheapestProductStore(product);
            List<string> cheapestStore = new List<string>();

            float minPrice = prices.Min(x => float.Parse(x.Substring(x.IndexOf('$') + 1)));

            for (int i = 0; i < prices.Count; i++)
            {
                string[] info = prices[i].Split('$');
                if (minPrice == float.Parse(info[1]))
                    cheapestStore.Add(ToFormat(info[0]));
            }

            return cheapestStore;
        }

        public List<Products> GetCountProducts(float sum, string store)
        {
            List<string> sqlInfo = resource.GetProductsInfo(store);
            List<Products> products = new List<Products>();

            foreach (var line in sqlInfo)
            {
                string[] infoSet = line.Split('$');
                float price = float.Parse(infoSet[1].Trim());
                string product = ToFormat(infoSet[0]);

                int count = (int)Math.Floor(sum / price);

                if (count > 0) products.Add(new Products(product, count));
            }

            if (products.Count == 0) throw new EmptyResponse();

            return products;
        }

        public float BuyShipment(List<Products> shipment, string store)
        {
            List<string> productsInfo = resource.GetProductsInfo(ToDBFormat(store));
            float sum = 0;

            List<int> productBought = new List<int>();

            foreach (var product in shipment)
            {
                Shipment productInfo = GetShipment(productsInfo, product.Product);
                if (productInfo == null) throw new EmptyResponse();
                if (productInfo.count < product.Count) throw new NotEnoughExceptions();

                sum += productInfo.price * product.Count;
                productBought.Add(product.Count);
            }

            for (int i = 0; i < shipment.Count; i++)
                resource.DecreaseCount(ToDBFormat(shipment[i].Product), ToDBFormat(store), productBought[i]);

            return sum;
        }

        public List<string> FindCheapestStores(List<Products> shipment)
        {
            List<string> stores = resource.GetStoresList();
            List<Pair> pairs = new List<Pair>();

            foreach (var store in stores)
            {
                try
                {
                    pairs.Add(new Pair(BuyShipment(shipment, store), store));
                }
                catch (Exception)
                {
                    continue;
                }
            }

            float minSum = pairs.Min(x => x.price);

            List<string> result = new List<string>();
            foreach (var pair in pairs)
            {
                if (pair.price == minSum) result.Add(ToFormat(pair.store));
            }

            if (result.Count == 0) throw new EmptyResponse();

            return result;
        }

        public List<string> GetStoresList()
        {
            List<string> stores = resource.GetStoresList();
            return stores;
        }

        public List<string> GetProductsInStore(string store)
        {
            List<string> sqlInfo = resource.GetProductsInfo(store);
            return sqlInfo;
        }
    }
}