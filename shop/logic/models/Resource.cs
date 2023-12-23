using System;
using System.Collections.Generic;

namespace shop
{
    public abstract class Resource
    {
        public abstract void CreateStore(string name);

        public abstract void CreateProduct(string productName, string store, float price);

        public abstract void DeliverShipment(List<Shipment> shipments, string store);

        public abstract List<string> FindCheapestProductStore(string productName);

        public abstract List<string> GetProductsInfo(string store);

        public abstract void DecreaseCount(string product, string store, int count);

        public abstract List<string> GetStoresList();
    }

    public abstract class Connector
    {
        public abstract Resource Create();
    }

    public class DBConnector : Connector
    {
        public override Resource Create()
        {
            return new MySQLDAO("localhost", 3306, "StoreInfo", "root", "qoe74859");
        }
    }

    public class FileConnector : Connector
    {
        public override Resource Create()
        {
            return new FileDAO(@"..\stores.txt", @"..\products.txt");
        }
    }
}