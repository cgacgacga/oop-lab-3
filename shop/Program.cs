using System;
using System.Configuration;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cmp;

/*
 * сделать UI
 * Разбить по папочкам
*/
namespace shop
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var storage = ConfigurationManager.AppSettings.Get("storage");
            Client client = new Client(storage == "database");
            client.addStores();
            client.addProducts();
            client.DeliverShipment();

            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1 - shop at the store");
            Console.WriteLine("2 - shop for product");
            int mode = int.Parse(Console.ReadLine());

            if (mode == 1)
            {
                List<string> stores = client.GetStoresList();
                for (int i = 0; i < stores.Count; i++)
                {
                    Console.WriteLine((i + 1) + " " + stores[i].ToString());
                }
                int shop = int.Parse(Console.ReadLine());
                List<string> products = client.GetProducts(stores[shop - 1]);
                for (int j = 0; j < products.Count; j++)
                {
                    Console.WriteLine((j + 1) + " " + products[j].ToString());
                }
                Console.WriteLine("Which product would you like to buy?");
                int productNumber = int.Parse(Console.ReadLine());
                Console.WriteLine("And how much");
                int number = int.Parse(Console.ReadLine());
                try
                {
                    client.buyShipment(stores[shop - 1], products[productNumber - 1].Split("$")[0], number);
                }
                catch (NotEnoughExceptions e)
                {
                    Console.WriteLine("Not enough");
                }
            }
            if (mode == 2)
            {
                Console.WriteLine("Enter product to find the store with the best price for product");
                string productName = Console.ReadLine();
                List<string> stores = client.FindCheapestStoreForProduct(productName);
                foreach (var store in stores)
                {
                    Console.WriteLine(store);
                }
            }
        }
    }
}