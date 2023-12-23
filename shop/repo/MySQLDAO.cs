using System;
using System.Collections.Generic;
using System.Globalization;
using MySql.Data.MySqlClient;

namespace shop
{
    public class MySQLDAO : Resource, IDisposable
    {
        private MySqlConnection connection;
        //qoe74859
        public MySQLDAO(string host, int port, string database, string username, string password)
        {
            connection =
                new MySqlConnection(
                    $"Server={host}; Database={database}; Port={port}; User Id={username}; Password={password}");

            connection.Open();
        }

        public void Dispose()
        {
            connection.Close();
        }

        bool haveSomething(string command)
        {
            //            command = "SELECT * FROM shop WHERE name = @param";
            MySqlCommand sqlCommand = new MySqlCommand(command, connection);
            //            sqlCommand.Parameters.AddWithValue("@param", "hello world");
            int count = int.Parse(sqlCommand.ExecuteScalar().ToString());

            return count != 0;
        }

        int getStoreId(string store)
        {
            string command = $"SELECT id FROM storeinfo.stores WHERE name = '{store}'";
            MySqlCommand sqlCommand = new MySqlCommand(command, connection);

            return int.Parse(sqlCommand.ExecuteScalar().ToString());
        }

        bool haveStore(string store)
        {
            return haveSomething($"SELECT COUNT(name) FROM storeinfo.stores WHERE name = '{store}'");
        }

        bool haveProduct(string product, string store)
        {
            if (!haveStore(store)) return false;

            int id = getStoreId(store);
            return haveSomething(
                "SELECT COUNT(name) FROM storeinfo.products" +
                $" WHERE name = '{product}' AND store_id = {id}");
        }

        public override void CreateStore(string name)
        {
            if (!haveSomething($"SELECT COUNT(name) FROM storeinfo.stores WHERE name = '{name}'"))
            {
                string command = $"INSERT INTO storeinfo.stores (name) VALUE('{name}') ";
                MySqlCommand sqlCommand = new MySqlCommand(command, connection);
                sqlCommand.ExecuteNonQuery();
            }
        }

        public override void CreateProduct(string productName, string store, float price)
        {
            if (!haveStore(store))
            {
                CreateStore(store);
                CreateProduct(productName, store, price);
            }
            else
            {
                if (haveProduct(productName, store)) return;

                int id = getStoreId(store);
                string command =
                    "INSERT INTO storeinfo.products (name, store_id, number, price)" +
                $"VALUES ('{productName}', {id}, 0, {price.ToString("F2", CultureInfo.InvariantCulture)})";

                MySqlCommand sqlCommand = new MySqlCommand(command, connection);
                sqlCommand.ExecuteNonQuery();
            }
        }

        public override void DeliverShipment(List<Shipment> shipments, string store)
        {
            foreach (var shipment in shipments)
            {
                if (!haveStore(store) || !haveProduct(shipment.product, store))
                {
                    CreateProduct(shipment.product, store, shipment.price);
                    DeliverShipment(shipments, store);
                }
                else
                {
                    int id = getStoreId(store);
                    string command =
                        $"UPDATE storeinfo.products SET number = {shipment.count} + number, price = {shipment.price.ToString("F2", CultureInfo.InvariantCulture)} WHERE store_id = {id} AND name = '{shipment.product}'";

                    MySqlCommand sqlCommand = new MySqlCommand(command, connection);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public override List<string> FindCheapestProductStore(string productName)
        {
            string command =
                $"SELECT store.name, products.price FROM storeinfo.stores AS store INNER JOIN storeinfo.products AS products ON store.id = products.store_id WHERE products.name = '{productName}'";
            MySqlCommand sqlCommand = new MySqlCommand(command, connection);
            MySqlDataReader reader = sqlCommand.ExecuteReader();

            List<string> data = new List<string>();
            while (reader.Read())
                data.Add(reader[0].ToString() + "$" + reader[1].ToString());

            reader.Close();
            return data;
        }

        public override List<string> GetProductsInfo(string store)
        {
            int id = getStoreId(store);
            string command =
                $"SELECT name, price, number FROM storeinfo.products WHERE store_id = {id}";
            MySqlCommand sqlCommand = new MySqlCommand(command, connection);
            MySqlDataReader reader = sqlCommand.ExecuteReader();

            List<string> data = new List<string>();
            while (reader.Read())
                data.Add(reader[0].ToString() + "$" + reader[1].ToString() + "$" + reader[2].ToString());

            reader.Close();
            return data;
        }

        public override void DecreaseCount(string product, string store, int count)
        {
            int id = getStoreId(store);
            string command =
                $"UPDATE storeinfo.products SET number = number - {count} WHERE store_id = {id} AND name = '{product}'";
            MySqlCommand sqlCommand = new MySqlCommand(command, connection);
            sqlCommand.ExecuteNonQuery();
        }

        public override List<string> GetStoresList()
        {
            string command = "SELECT name FROM storeinfo.stores";
            MySqlCommand sqlCommand = new MySqlCommand(command, connection);
            MySqlDataReader reader = sqlCommand.ExecuteReader();

            List<string> stores = new List<string>();
            while (reader.Read()) stores.Add(reader[0].ToString());

            reader.Close();
            return stores;
        }
    }
}