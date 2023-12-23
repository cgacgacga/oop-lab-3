using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using Org.BouncyCastle.Bcpg;

namespace shop
{
    public class FileDAO : Resource
    {
        private StreamWriter writer;
        private StreamReader reader;
        private string pathStores;
        private string pathProducts;

        private int id;

        public FileDAO(string fileNameStores, string fileNamePathProducts)
        {
            pathStores = $"..\\{fileNameStores}";
            pathProducts = $"..\\{fileNamePathProducts}";
            id = getStartId();
        }

        int getStartId()
        {
            int id = 0;
            reader = File.OpenText(pathStores);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                string[] lines = line.Split(':');
                id = int.Parse(lines[0]) + 1;
            }
            reader.Close();
            return id;
        }

        private int getStoreId(string name)
        {
            int id = -1;
            reader = File.OpenText(pathStores);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                string[] lines = line.Split(':');
                if (name.ToLower().Equals(lines[1].Trim().ToLower()))
                {
                    id = int.Parse(lines[0]);
                    break;
                }
            }

            reader.Close();
            return id;
        }

        public override void CreateStore(string name)
        {
            MakeStore(name);
        }

        public int MakeStore(string name)
        {
            int sId = getStoreId(name);
            writer = File.AppendText(pathStores);
            if (sId == -1)
            {
                writer.WriteLine($"{id} : {name.ToLower()}");
                sId = id;
                id++;
            }

            writer.Close();
            return sId;
        }

        private bool haveProduct(string product, int store_id)
        {
            reader = File.OpenText(pathProducts);
            string str;
            bool result = false;
            while ((str = reader.ReadLine()) != null)
            {
                string[] lines = str.Split(':');
                if (int.Parse(lines[0]) == store_id && lines[1].Equals(product.ToLower()))
                {
                    result = true;
                    break;
                }
            }

            reader.Close();
            return result;
        }

        public override void CreateProduct(string productName, string store, float price)
        {
            CreateProduct(productName, store, 0, price);
        }

        private void CreateProduct(string productName, string store, int count, float price)
        {
            int storeId = MakeStore(store);
            if (haveProduct(productName, storeId)) return;

            writer = File.AppendText(pathProducts);
            writer.WriteLine($"{storeId} : {productName.ToLower()} : {count} : {price}");
            writer.Close();
        }

        public override void DeliverShipment(List<Shipment> shipments, string store)
        {
            int storeID = getStoreId(store);
            foreach (var shipment in shipments)
            {
                if (storeID == -1 || !haveProduct(shipment.product, getStoreId(store)))
                    CreateProduct(shipment.product, store, shipment.count, shipment.price);
                else
                {
                    reader = File.OpenText(pathStores);
                    List<string> fileData = new List<string>();

                    string str;
                    while ((str = reader.ReadLine()) != null)
                    {
                        string[] lines = str.Split(':');
                        if (int.Parse(lines[1].Trim()) == storeID &&
                            lines[1].Trim().Equals(shipment.product.ToLower().Trim()))
                        {
                            int number = int.Parse(lines[2].Trim()) + shipment.count;
                            float price = float.Parse(lines[3].Trim());
                            fileData.Add($"{storeID} : {lines[1]} : {number} : {price}");
                        }
                        else fileData.Add(str);
                    }
                    reader.Close();

                    writer = new StreamWriter(pathProducts, false);
                    foreach (var line in fileData)
                    {
                        writer.WriteLine(line);
                    }
                    writer.Close();
                }
            }
        }

        private string GetStoreById(int id)
        {
            reader = File.OpenText(pathStores);
            string str;
            while ((str = reader.ReadLine()) != null)
            {
                string[] lines = str.Split(':');
                lines[0].Trim();

                if (int.Parse(lines[0]) == id) return lines[1].Trim();
            }

            return null;
        }

        private List<string> GetStoresNamesById(List<int> id)
        {
            List<string> result = new List<string>();
            foreach (var useID in id)
            {
                result.Add(GetStoreById(useID));
            }

            return result;
        }

        public override List<string> FindCheapestProductStore(string productName)
        {
            List<string> data = new List<string>();
            reader = File.OpenText(pathProducts);

            string str;
            while ((str = reader.ReadLine()) != null)
            {
                string[] lines = str.Split(':');
                lines[1].Trim();
                if (lines[1].Equals(productName.ToLower()))
                    data.Add($"{GetStoreById(int.Parse(lines[0].Trim()))}${lines[1]}");
            }

            reader.Close();

            return data;
        }

        public override List<string> GetProductsInfo(string store)
        {
            int storeId = getStoreId(store);
            reader = File.OpenText(pathProducts);
            string str;

            List<string> info = new List<string>();
            while ((str = reader.ReadLine()) != null)
            {
                string[] lines = str.Split(':');
                if (storeId == int.Parse(lines[0].Trim()))
                    info.Add($"{lines[1].Trim()}${lines[3].Trim()}${lines[2].Trim()}");
            }

            reader.Close();
            return info;
        }

        public override void DecreaseCount(string product, string store, int count)
        {
            int id = getStoreId(store);
            reader = File.OpenText(pathProducts);

            List<string> fileData = new List<string>();

            string str;
            while ((str = reader.ReadLine()) != null)
            {
                string[] lines = str.Split(':');
                lines[0].Trim();
                if (int.Parse(lines[0]) == id && lines[1].Trim().Equals(product.ToLower()))
                    fileData.Add(
                        $"{id} : {lines[1].Trim()} : {int.Parse(lines[2].Trim()) - count} : {lines[3].Trim()}");
                else fileData.Add(str);
            }

            reader.Close();

            writer = new StreamWriter(pathProducts, false);
            foreach (var line in fileData)
            {
                writer.WriteLine(line);
            }

            writer.Close();
        }

        public override List<string> GetStoresList()
        {
            List<string> result = new List<string>();
            reader = File.OpenText(pathStores);

            string str;
            while ((str = reader.ReadLine()) != null)
            {
                string[] lines = str.Split(':');
                result.Add(lines[1].Trim());
            }

            reader.Close();
            return result;
        }
    }
}