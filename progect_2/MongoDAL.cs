using BEL;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace MongoDAL
{
    internal class MongoDAL
    {

        public static IMongoDatabase connect()
        {
            MongoClient client = new MongoClient("mongodb+srv://zeev:zeev@cluster0.e01q4gc.mongodb.net/test");
            IMongoDatabase database = client.GetDatabase("project_2");
            return database;
        }
        public static void CreateAndLoadIngredientsTable()
        {
            IMongoDatabase datebase = connect();
            var IngredientsCollection = datebase.GetCollection<BsonDocument>("Ingredients"); 
            //this will enter the Ingredients into the mongo 
            //first test that what we whant to enter dose not exist
            int i = 0;
            var values = Enum.GetValues(typeof(Flavors));
            foreach (var flaver in values)
            {
                var filter = Builders<BsonDocument>.Filter.Eq("Name", flaver.ToString());
                var filter_list = IngredientsCollection.Find(filter).ToList();
                if(filter_list.Count() == 0)
                {
                    string json = "{'Name': '" + flaver + "', 'Iid': " + i + "}";
                    IngredientsCollection.InsertOne(BsonDocument.Parse(json));
                    i++;
                }
            }
            values = Enum.GetValues(typeof(Extras));
            foreach (var extra in values)
            {
                var filter = Builders<BsonDocument>.Filter.Eq("Name", extra.ToString());
                var filter_list = IngredientsCollection.Find(filter).ToList();
                if (filter_list.Count() == 0)
                {
                    string json = "{'Name': '" + extra + "', 'Iid': " + i + "}";
                    IngredientsCollection.InsertOne(BsonDocument.Parse(json));
                    i++;
                }
            }
            values = Enum.GetValues(typeof(Cupsize));
            foreach (var cup in values)
            {
                var filter = Builders<BsonDocument>.Filter.Eq("Name", cup.ToString());
                var filter_list = IngredientsCollection.Find(filter).ToList();
                if (filter_list.Count() == 0)
                {
                    string json = "{'Name': '" + cup + "', 'Iid': " + i + "}";
                    IngredientsCollection.InsertOne(BsonDocument.Parse(json));
                    i++;
                }
            }
        }
        public static sales CreateSale()
        {
            //this is exactly the same thing regardless of the data base 
            sales newsale = DAL.DAL.CreateSale();
            // the only difference is the sale id 
            IMongoDatabase datebase = connect();
            var saleCollection = datebase.GetCollection<BsonDocument>("sales");
            int i = 0;
            var filter = Builders<BsonDocument>.Filter.Eq("sale id", i);
            var filter_list = saleCollection.Find(filter).ToList();
            while (filter_list.Count() != 0)
            {
                i++;
                filter = Builders<BsonDocument>.Filter.Eq("sale id", i);
                filter_list = saleCollection.Find(filter).ToList();
            }
            //now i is a non used sale id
            newsale.setsale_id(i);
            return (newsale);
        }
        public static void EnterSaleToMongo(sales sale)
        {
            string Flavers = "{";
            foreach (var flaver in sale.getFlavors())
            {
                Flavers += "" + flaver.ToString() + ": " + "'flaver id " + flaver.GetHashCode().ToString() + "'" + "\n";
            }
            Flavers += "}";
            BsonDocument F = BsonDocument.Parse(Flavers);

            string Extras = "{";
            foreach (var EX in sale.getExtras())
            {
                Extras += "" + EX.ToString() + ": " + "'flaver id " + EX.GetHashCode().ToString() + "'" +"\n";
            }
            Extras += "}";
            BsonDocument E = BsonDocument.Parse(Extras);
            var doc = new BsonDocument
            {
                {"sale id",(int)sale.getsale_id()},
                {"total price",(int)sale.getPrice()},
                {"compleated",(int)sale.getCompleted()},
                {"date",sale.getDate()},
                {"cup",sale.getCup().ToString()},
                {"Flavers",F},
                {"Extras",E}
            };
            IMongoDatabase datebase = connect();
            var salesCollection = datebase.GetCollection<BsonDocument>("sales");
            salesCollection.InsertOne(doc);
        }
        public static void GetRecit(int sale_id)
        {
            // connection
            IMongoDatabase datebase = connect();
            var salesCollection = datebase.GetCollection<BsonDocument>("sales");
            // get object
            var filter = Builders<BsonDocument>.Filter.Eq("sale id", sale_id);
            var sale = salesCollection.Find(filter).ToList()[0];
            //print
            string compleated = "";
            if (sale["compleated"] == 1)
            {
                compleated = "yes";
            }
            else
            {
                compleated = "no";
            }
            string contence = "";
            contence += sale["cup"] + "\n";
            var Flavercollection = sale["Flavers"].ToBsonDocument().ToList();
            var Extrascollection = sale["Extras"].ToBsonDocument().ToList();
            foreach (var f in Flavercollection)
            {
                string[] sub = f.ToString().Split('=');
                contence += sub[0] + "\n";
            }
            foreach (var f in Extrascollection)
            {
                string[] sub = f.ToString().Split('=');
                contence += sub[0] + "\n";
            }
            string recit = "";
            recit += "recit for sale id: " + sale_id + "\n" +
                "date: " + sale["date"] + "\n" +
                "total price is: " + sale["total price"] + "\n" +
                "payment compleated: " + compleated + "\n" +
                "ingridients: \n" + contence;
            Console.WriteLine(recit);
        }
        public static sales creatsalefrommongo(int sale_id)
        {
            sales newsale = new sales();
            // connection
            IMongoDatabase datebase = connect();
            var salesCollection = datebase.GetCollection<BsonDocument>("sales");
            // get object
            var filter = Builders<BsonDocument>.Filter.Eq("sale id", sale_id);
            var sale = salesCollection.Find(filter).ToList()[0];
            //fill data
            newsale.set_ObjectId((ObjectId)sale["_id"]);
            newsale.setsale_id(sale_id);
            newsale.setPrice((int)sale["total price"]);
            newsale.setDate((string)sale["date"]);

            var IngredientsCollection = datebase.GetCollection<BsonDocument>("Ingredients");
            var filter2 = Builders<BsonDocument>.Filter.Eq("Name", sale["cup"]);
            var filter_list = IngredientsCollection.Find(filter2).ToList()[0];

            newsale.setCup((Cupsize)((int)filter_list["Iid"])-13);
            List<Flavors> flavors = new List<Flavors>();
            List<Extras> Extras = new List<Extras>();

            var Flavercollection = sale["Flavers"].ToBsonDocument().ToList();
            var Extrascollection = sale["Extras"].ToBsonDocument().ToList();
            foreach (var f in Flavercollection)
            {
                string[] sub = f.ToString().Split('=');
                filter2 = Builders<BsonDocument>.Filter.Eq("Name", sub[0]);
                filter_list = IngredientsCollection.Find(filter2).ToList()[0];
                flavors.Add((Flavors)((int)filter_list["Iid"]));
            }
            foreach (var f in Extrascollection)
            {
                string[] sub = f.ToString().Split('=');
                filter2 = Builders<BsonDocument>.Filter.Eq("Name", sub[0]);
                filter_list = IngredientsCollection.Find(filter2).ToList()[0];
                Extras.Add((Extras)((int)filter_list["Iid"])-10);
            }
            newsale.setExtras(Extras);
            newsale.setFlavors(flavors);
            return newsale;
        }
        public static void updatesale(int sale_id)
        {
            sales sale = creatsalefrommongo(sale_id);
            while(true)
            {
                Console.Clear();
                Console.WriteLine("to change cup size enter 1" + "\n" +
                "to change Flavers enter 2" + "\n" +
                "to change extras enter 3" + "\n\n" +
                "to go back enter -1");
                string ans = Console.ReadLine();
                try
                {
                    switch(Int32.Parse(ans))
                    {
                        case 1:
                            do
                            {
                                /*Console.Clear();*/
                                var values = Enum.GetValues(typeof(Cupsize));
                                foreach (var newcup in values)
                                {
                                    Console.WriteLine("for " + newcup.ToString() + " press " + newcup.GetHashCode());
                                }
                                Console.WriteLine("\nto go back enter -1");
                                ans = Console.ReadLine();
                                try
                                {
                                    if (Int32.Parse(ans) == -1)
                                    {
                                        ans = "0";
                                        break;
                                    }
                                    if (BLL.BLL.checkcup(sale, (Cupsize)Int32.Parse(ans)))
                                    {
                                        Console.WriteLine("\ncup changed\n");
                                        sale.setCup((Cupsize)Int32.Parse(ans));
                                    }
                                }
                                catch
                                {
                                    Console.WriteLine("enter vaild anser");
                                }
                            }
                            while (true);
                            break;
                        case 2:
                            while (true)
                            {
                                // main try catch for ans 
                                try
                                {
                                    Console.WriteLine("to add a flaver enter: 1 to remove a flaver enter: 2\nto return enter: -1");
                                    ans = Console.ReadLine();
                                    switch (Int32.Parse(ans))
                                    {
                                        //add
                                        case 1:
                                            var values = Enum.GetValues(typeof(Flavors));
                                            while (true)
                                            {
                                                try
                                                {
                                                    do
                                                    {
                                                        Console.WriteLine("select a Flavor from the folloing and -1 to stop");
                                                        foreach (var v in values)
                                                        {
                                                            Console.WriteLine("for " + v.ToString() + " press " + v.GetHashCode());
                                                        }
                                                        ans = Console.ReadLine();
                                                        if (Int32.Parse(ans) == -1)
                                                        {
                                                            break;
                                                        }
                                                        if (Int32.Parse(ans) > 9 || Int32.Parse(ans) < 0)
                                                        {
                                                            Console.WriteLine("enter valid anser");
                                                        }
                                                        else if (BLL.BLL.checkFlaver(sale, (Flavors)Int32.Parse(ans)) && BLL.BLL.checK_sale(sale))
                                                        {
                                                            // for the while to continue
                                                            ans = "10";
                                                            Console.WriteLine("flaver added");
                                                            sale.getFlavors().Add((Flavors)Int32.Parse(ans));
                                                        }
                                                    }
                                                    while (Int32.Parse(ans) > 9 || Int32.Parse(ans) < 0);
                                                    if (Int32.Parse(ans) == -1)
                                                    {
                                                        break;
                                                    }
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("enter valid anser");
                                                }
                                            }
                                            break;
                                        //remove
                                        case 2:
                                            if (sale.getCompleted() == 1)
                                            {
                                                Console.WriteLine("you have paid the order was made and you can not change this item\n");
                                                break;
                                            }
                                            foreach (int F in sale.getFlavors())
                                            {
                                                Console.WriteLine("to remove flaver: " +(Flavors)F + " enter: " + F.GetHashCode());
                                            }
                                            ans = "0";
                                            Console.WriteLine("to stop enter -1");
                                            while (true)
                                            {
                                                ans = Console.ReadLine();
                                                try
                                                {
                                                    if (Int32.Parse(ans) == -1)
                                                    {
                                                        break;
                                                    }
                                                    Console.WriteLine("\nflaver removed\n");
                                                    sale.getFlavors().Remove((Flavors)Int32.Parse(ans));
                                                    break;
                                                }
                                                catch (SqlException e)
                                                {
                                                    Console.WriteLine(e);
                                                }
                                            }
                                            break;
                                    }
                                    if (Int32.Parse(ans) == -1)
                                    {
                                        ans = "0";
                                        break;
                                    }
                                }
                                catch
                                {
                                    Console.WriteLine("pleas enter a valid input");
                                }
                            }
                            break;
                        case 3:
                            while (true)
                            {
                                // main try catch for ans 
                                try
                                {
                                    Console.WriteLine("to add a Extra enter: 1 to remove a Extra enter: 2\nto return enter: -1");
                                    ans = Console.ReadLine();
                                    switch (Int32.Parse(ans))
                                    {
                                        //add
                                        case 1:
                                            var values = Enum.GetValues(typeof(Extras));
                                            while (true)
                                            {
                                                try
                                                {
                                                    do
                                                    {
                                                        Console.WriteLine("select a Extra from the folloing and -1 to stop");
                                                        foreach (var v in values)
                                                        {
                                                            Console.WriteLine("for " + v.ToString() + " press " + v.GetHashCode());
                                                        }
                                                        ans = Console.ReadLine();
                                                        if (Int32.Parse(ans) == -1)
                                                        {
                                                            break;
                                                        }
                                                        if (Int32.Parse(ans) > 2 || Int32.Parse(ans) < 0)
                                                        {
                                                            Console.WriteLine("enter valid anser");
                                                        }
                                                        else if (BLL.BLL.checkextra(sale, (Extras)Int32.Parse(ans)) && BLL.BLL.checK_sale(sale))
                                                        {
                                                            // for the while to continue
                                                            ans = "3";
                                                            Console.WriteLine("\nextra added\n");
                                                            sale.getExtras().Add((Extras)Int32.Parse(ans));
                                                        }
                                                    }
                                                    while (Int32.Parse(ans) > 2 || Int32.Parse(ans) < 0);
                                                    if (Int32.Parse(ans) == -1)
                                                    {
                                                        break;
                                                    }

                                                }
                                                catch
                                                {
                                                    Console.WriteLine("enter valid anser");
                                                }
                                            }

                                            break;
                                        //remove
                                        case 2:
                                            if (sale.getCompleted() == 1)
                                            {
                                                Console.WriteLine("you have paid the order was made and you can not change this item\n");
                                                break;
                                            }
                                            foreach (int F in sale.getExtras())
                                            {
                                                Console.WriteLine("to remove Extra: " + (Extras)F + " enter: " + F.GetHashCode());
                                            }
                                            ans = "0";
                                            Console.WriteLine("to stop enter -1");
                                            while (true)
                                            {
                                                ans = Console.ReadLine();
                                                try
                                                {
                                                    if (Int32.Parse(ans) == -1)
                                                    {
                                                        break;
                                                    }
                                                    Console.WriteLine("\nextra removed\n");
                                                    sale.getExtras().Remove((Extras)Int32.Parse(ans));
                                                    break;
                                                }
                                                catch (SqlException e)
                                                {
                                                    Console.WriteLine(e);
                                                }
                                            }
                                            break;
                                    }
                                    if (Int32.Parse(ans) == -1)
                                    {
                                        ans = "0";
                                        break;
                                    }
                                }
                                catch
                                {
                                    Console.WriteLine("pleas enter a valid input");
                                }
                            }
                            break;
                    }
                    if(Int32.Parse(ans)== -1)
                    {
                        break;
                    }
                }
                catch
                {
                    if (Int32.Parse(ans) != -1)
                    {
                        Console.WriteLine("enter vaild anser");
                    }
                }
            }
            DeleatSale(sale_id);
            EnterSaleToMongo(sale);

        }
        public static void DeleatSale(int sale_id)
        {
            //connect
            IMongoDatabase datebase = connect();
            var saleCollection = datebase.GetCollection<BsonDocument>("sales");
            //get object
            var filter = Builders<BsonDocument>.Filter.Eq("sale id", sale_id);
            var sale = saleCollection.Find(filter).ToList()[0];
            saleCollection.DeleteOne(sale);
        }
        public static string UnfinishedSales()
        {
            string ans = "UNFINISHED SALES:\n";
            IMongoDatabase datebase = connect();
            var salesCollection = datebase.GetCollection<BsonDocument>("sales");
            var filter = Builders<BsonDocument>.Filter.Eq("compleated", -1);
            var saleList = salesCollection.Find(filter).ToList();
            foreach (var sale in saleList)
                ans += "sale id: " + sale["sale id"] + "\n";

            return ans;
        }
        public static void pay(int sale_id)
        {
            IMongoDatabase datebase = connect();
            var salesCollection = datebase.GetCollection<BsonDocument>("sales");
            var filter = Builders<BsonDocument>.Filter.Eq("sale id", sale_id);
            var update = Builders<BsonDocument>.Update.Set("compleated",1);
            salesCollection.UpdateOne(filter, update);
        }
        public static void Moste_popular_Ingredient()
        {
            string ans = "most popula Ingredients are:\n";
            IMongoDatabase datebase = connect();
            var salesCollection = datebase.GetCollection<BsonDocument>("sales");
            var filter = Builders<BsonDocument>.Filter.Gt("sale id", 0); // any
            var saleList = salesCollection.Find(filter).ToList();
            int[] ing = new int[16];

            foreach (var sale in saleList)
            {
                var Flavercollection = sale["Flavers"].ToBsonDocument().ToList();
                var Extrascollection = sale["Extras"].ToBsonDocument().ToList();
                foreach (var F in Flavercollection)
                {
                    string[] sub = F.ToString().Split('=');
                    string[] sub2 = sub[1].Split(' ');
                    ing[Int32.Parse(sub2[2])]++;
                }
                foreach (var E in Extrascollection)
                {
                    string[] sub = E.ToString().Split('=');
                    string[] sub2 = sub[1].Split(' ');
                    ing[(Int32.Parse(sub2[2])) + 10]++;

                }
                //cup
                var IngredientsCollection = datebase.GetCollection<BsonDocument>("Ingredients");
                var filter2 = Builders<BsonDocument>.Filter.Eq("Name", sale["cup"]);
                var filter_list = IngredientsCollection.Find(filter2).ToList()[0];
                ing[(int)filter_list["Iid"]]++;
            }
            //the Moste_popular_Ingredient is the id of the biggest value in ing
            int max = -1;
            int id_max = -1;
            List<int> id_list = new List<int>();
            for (int i = 0; i < 16; i++)
            {
                if (ing[i] > max && ing[i] != 0)
                {
                    id_list.Clear();
                    id_list.Add(i);
                    max = ing[i];
                }
                if (ing[i] == max && ing[i] != 0)
                {
                    id_list.Add(i);
                }

            }
            foreach (var id in id_list)
            {
                var finalCollection = datebase.GetCollection<BsonDocument>("Ingredients");
                var finalfilter = Builders<BsonDocument>.Filter.Eq("Iid", id);
                var finalfilter_list = finalCollection.Find(finalfilter).ToList()[0];
                ans += finalfilter_list["Name"]+"  ";
            }
            Console.WriteLine(ans);
        }
        public static void Moste_popular_flaver()
        {
            string ans = "most popula flavers are:\n";
            IMongoDatabase datebase = connect();
            var salesCollection = datebase.GetCollection<BsonDocument>("sales");
            var filter = Builders<BsonDocument>.Filter.Gt("sale id", 0); // any
            var saleList = salesCollection.Find(filter).ToList();
            int[] ing = new int[10];

            foreach (var sale in saleList)
            {
                var Flavercollection = sale["Flavers"].ToBsonDocument().ToList();
                var Extrascollection = sale["Extras"].ToBsonDocument().ToList();
                foreach (var F in Flavercollection)
                {
                    string[] sub = F.ToString().Split('=');
                    string[] sub2 = sub[1].Split(' ');
                    ing[Int32.Parse(sub2[2])]++;
                }
            }
            //the Moste_popular_Ingredient is the id of the biggest value in ing
            int max = -1;
            int id_max = -1;
            List<int> id_list = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                if (ing[i] > max && ing[i] != 0)
                {
                    id_list.Clear();
                    id_list.Add(i);
                    max = ing[i];
                }
                if (ing[i] == max && ing[i] != 0)
                {
                    id_list.Add(i);
                }

            }
            foreach (var id in id_list)
            {
                var finalCollection = datebase.GetCollection<BsonDocument>("Ingredients");
                var finalfilter = Builders<BsonDocument>.Filter.Eq("Iid", id);
                var finalfilter_list = finalCollection.Find(finalfilter).ToList()[0];
                ans += finalfilter_list["Name"] + "  ";
            }
            Console.WriteLine(ans);
        }
        public static void EndOfDay1()
        {
            DateTime localDate = DateTime.Now;
            string today = localDate.ToString("dd/MM/yyyy");

            string ans = "";
            int count = 0;
            int sum = 0;
            int avg = 0;
            IMongoDatabase datebase = connect();
            var salesCollection = datebase.GetCollection<BsonDocument>("sales");
            var filter = Builders<BsonDocument>.Filter.Eq("compleated",1);
            var saleList = salesCollection.Find(filter).ToList();
            foreach(var sale in saleList)
            {
                string date = (string)sale["date"];
                string[] sub = date.Split(' ');
                if (sub[0] == today)
                {
                    count++;
                    sum += (int)sale["total price"];
                }
            }
            if(count >0)
            {
                avg = sum/count;
            }
            ans = "amount of sales: " + count + "\nsum of sales: " + sum + "\navarage: " + avg;
            Console.WriteLine(ans);

        }
    }

    
}
