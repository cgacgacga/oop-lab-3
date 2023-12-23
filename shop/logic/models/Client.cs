using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;

namespace shop
{
    public class Client
    {
        private readonly Service _service;
        public Client(bool useDatabase)
        {
            _service = new Service(useDatabase);
        }


        void show(List<string> lines, string title)
        {
            Console.WriteLine($" ==== {title.ToUpper()} ==== ");
            foreach (var line in lines) Console.WriteLine(line);
        }

        void show(List<Products> lines, string title)
        {
            Console.WriteLine($" ==== {title.ToUpper()} ==== ");
            Console.WriteLine("[name : count]");
            foreach (var line in lines) Console.WriteLine($"{line.Product} : {line.Count}");
        }

        public void addStores()
        {
            _service.createStore("7Я");
            _service.createStore("kek");
            _service.createStore("Spar");
            _service.createStore("Магнит");
            _service.createStore("Окей");
        }

        public void addProducts()
        {
            _service.CreateProduct("7я", "молоко", (float)39.99);
            _service.CreateProduct("Окей", "молоко", (float)42.99);
            _service.CreateProduct("Магнит", "молоко", (float)39.69);
            _service.CreateProduct("Карусель", "молоко", (float)39.99);

            _service.CreateProduct("7я", "Вода", (float)139.99);
            _service.CreateProduct("Окей", "Вода", (float)12.99);
            _service.CreateProduct("Магнит", "Вода", 35);
            _service.CreateProduct("Spar", "Вода", (float)37.23);
        }

        public void DeliverShipment()
        {
            Shipment pr1 = new Shipment("Молоко", 120, 40);
            Shipment pr2 = new Shipment("Вода", 78, 19);
            Shipment pr3 = new Shipment("Мороженное", 23, 140);
            List<Shipment> shipment1 = new List<Shipment>() { pr1, pr2, pr3 };
            _service.DeliverShipment(shipment1, "7я");

            Shipment pr4 = new Shipment("Молоко", 120, 59);
            Shipment pr5 = new Shipment("Вода", 78, (float)27.66);
            List<Shipment> shipment2 = new List<Shipment>() { pr4, pr5 };
            _service.DeliverShipment(shipment2, "Окей");

            Shipment pr6 = new Shipment("Молоко", 120, 92);
            Shipment pr7 = new Shipment("Вода", 78, (float)19.77);
            Shipment pr8 = new Shipment("Мороженное", 23, 133);
            List<Shipment> shipment3 = new List<Shipment>() { pr6, pr7, pr8 };
            _service.DeliverShipment(shipment3, "Лента");
        }

        public void FindCheapestStoreForProduct()
        {
            show(_service.FindCheapestProductStores("молоко"), "Cheapest milk in");
            show(_service.FindCheapestProductStores("вода"), "Cheapest water in");
        }

        public void BuyingOportunity()
        {
            List<Products> canBuy = _service.GetCountProducts((float)272.48, "Лента");
            show(canBuy, "Buying on 272.48 rubles");
        }

        public float buyShipment()
        {
            Products pr1 = new Products("молоко", 2);
            Products pr2 = new Products("вода", 7);
            Products pr3 = new Products("мороженное", 1);
            List<Products> shipment = new List<Products>() { pr1, pr2, pr3 };

            float price = _service.BuyShipment(shipment, "лента");
            return price;
        }

        public float buyShipment(string store, string product, int number)
        {
            Products pr = new Products(product, number);
            List<Products> shipment = new List<Products>() { pr };

            float price = _service.BuyShipment(shipment, store);
            return price;
        }

        public void cheapestStore()
        {
            Products pr1 = new Products("молоко", 2);
            Products pr2 = new Products("вода", 3);
            List<Products> shipment = new List<Products>() { pr1, pr2 };

            List<string> shops = _service.FindCheapestStores(shipment);
            show(shops, "cheapest store for shipment (2 packets of milk + 3 bottles of water)");
        }

        public List<string> GetStoresList()
        {
            List<string> stores = _service.GetStoresList();
            return stores;
        }

        public List<string> GetProducts(string store)
        {
            List<string> products = _service.GetProductsInStore(store);
            return products;
        }

        public List<string> FindCheapestStoreForProduct(string productName)
        {
            return _service.FindCheapestProductStores(productName);
        }
    }
}