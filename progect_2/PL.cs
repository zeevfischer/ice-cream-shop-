using BEL;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PL
{
    internal class PL
    {
        public static void existing_sale(int optinal_sale_id, int data_base_type)
        {
            int sale_id = -1;
            if (optinal_sale_id != -1)
            {
                sale_id = optinal_sale_id;
            }
            else
            {
                //get sale id from input
                while (true)
                {
                    try
                    {
                        Console.Clear();
                        Console.Write("enter existing sale id: ");
                        sale_id = Int32.Parse(Console.ReadLine());
                        Console.WriteLine("\n");
                        break;
                    }
                    catch
                    {
                        Console.Clear();
                        Console.WriteLine("enter nubers only");
                    }
                }
                try
                {
                    if (data_base_type == 0)
                    {
                        DAL.DAL.CreateObjectSaleFromSQL(sale_id);
                    }
                    if (data_base_type == 1)
                    {
                        MongoDAL.MongoDAL.creatsalefrommongo(sale_id);
                    }
                }
                catch
                {
                    Console.WriteLine("sale dose not exist");
                    return;
                }
            }
            while (true)
            {
                Console.WriteLine("to viwe sale enter: 1\nto pay enter: 2\nto update sale enter: 3\nto deleat sale enter: 4\n\nto return to main menue enter: -1");
                string ans = Console.ReadLine();
                try
                {
                    if (Int32.Parse(ans) == -1)
                    {
                        Console.Clear();
                        break;
                    }
                    switch (Int32.Parse(ans))
                    {
                        case 1:
                            if (data_base_type == 0)
                            {
                                Console.Clear();
                                Console.WriteLine(DAL.DAL.GetRecit(sale_id));
                                Console.WriteLine("\nto continue press eany key");
                                Console.ReadLine();
                                Console.Clear();
                            }
                            if (data_base_type == 1)
                            {
                                Console.Clear();
                                MongoDAL.MongoDAL.GetRecit(sale_id);
                                Console.WriteLine("\nto continue press eany key");
                                Console.ReadLine();
                                Console.Clear();
                            }
                            break;
                        case 2:
                            if (data_base_type == 0)
                            {
                                DAL.DAL.pay(sale_id);
                            }
                            if (data_base_type == 1)
                            {
                                MongoDAL.MongoDAL.pay(sale_id);
                            }
                            break;
                        case 3:
                            Console.Clear();
                            if (data_base_type == 0)
                            {
                                DAL.DAL.updatesale(sale_id);
                            }
                            if (data_base_type == 1)
                            {
                                MongoDAL.MongoDAL.updatesale(sale_id);
                            }
                            break;
                        case 4:
                            if (data_base_type == 0)
                            {
                                DAL.DAL.DeleatSale(sale_id);
                            }
                            if (data_base_type == 1)
                            {
                                MongoDAL.MongoDAL.DeleatSale(sale_id);
                            }
                            ans = "-1";
                            break;
                    }
                    if (Int32.Parse(ans) == -1)
                    {
                        Console.Clear();
                        break;
                    }
                }
                catch
                {
                    Console.WriteLine("enter valid anser\n");
                }
            }
        }
        public static int admin(int data_base_type)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("to change data base enter: 1\n enter: 2\nto get a dayly report enter: 3\nto get a sale report enter: 4\nto get the most popular ingredient enter: 5\nto get the most popular vlavers enter: 6\n\nto return enter -1");
                try
                {
                    string ans = Console.ReadLine();
                    if (Int32.Parse(ans) == -1)
                    {
                        Console.Clear();
                        break;
                    }
                    switch (Int32.Parse(ans))
                    {
                        case 1:
                            while (true)
                            {
                                try
                                {
                                    Console.WriteLine("choose data bast: " + "\n" +
                                        "for SQL enter: 0" + "\n+" +
                                        "for mongo enter: 1");
                                    int database_type = Int32.Parse(Console.ReadLine());
                                    Console.WriteLine("data base selected");
                                    return data_base_type;
                                }
                                catch
                                {
                                    Console.WriteLine("enter valid anser");
                                }
                            }
                            break;
                        case 2:

                            break;
                        case 3:
                            if (data_base_type == 0)
                            {
                                DAL.DAL.EndOfDay1();
                                Console.WriteLine("\nto continue press eany key");
                                Console.ReadLine();
                            }
                            if (data_base_type == 1)
                            {
                                MongoDAL.MongoDAL.EndOfDay1();
                                Console.WriteLine("\nto continue press eany key");
                                Console.ReadLine();
                            }
                            break;
                        case 4:
                            if (data_base_type == 0)
                            {
                                DAL.DAL.EndOfDay2();
                                Console.WriteLine("\nto continue press eany key");
                                Console.ReadLine();
                            }
                            else
                            {
                                Console.WriteLine("this is an sql fetuer is not created in mongo");
                            }
                            break;
                        case 5:
                            if (data_base_type == 0)
                            {
                                Console.Clear();
                                DAL.DAL.Moste_popular_Ingredient();
                                Console.WriteLine("\nto continue press eany key");
                                Console.ReadLine();
                            }
                            if (data_base_type == 1)
                            {
                                Console.Clear();
                                MongoDAL.MongoDAL.Moste_popular_Ingredient();
                                Console.WriteLine("\nto continue press eany key");
                                Console.ReadLine();
                            }
                            break;
                        case 6:
                            if (data_base_type == 0)
                            {
                                Console.Clear();
                                DAL.DAL.Moste_popular_flaver();
                                Console.WriteLine("\nto continue press eany key");
                                Console.ReadLine();
                            }
                            if (data_base_type == 1)
                            {
                                Console.Clear();
                                MongoDAL.MongoDAL.Moste_popular_flaver();
                                Console.WriteLine("\nto continue press eany key");
                                Console.ReadLine();
                            }
                            break;
                    }
                }
                catch
                {
                    Console.Clear();
                    Console.WriteLine("enter valid anser\n");
                }
            }
            return -1;
        }
        static void Main(string[] args)
        {
            //0 = sql
            //1 = Mongo
            int database_type = 1;

            //create databases regardless 
            DAL.DAL.createDatabase();
            DAL.DAL.createTables();
            DAL.DAL.CreateAndLoadIngredientsTable();
            Thread.Sleep(3000);
            Console.Clear();

            Console.WriteLine("monge database created");
            MongoDAL.MongoDAL.CreateAndLoadIngredientsTable();
            Thread.Sleep(3000);
            Console.Clear();

            //chose data base to use
            while (true)
            {
                try
                {
                    Console.WriteLine("choose data bast: " + "\n" +
                        "for SQL enter: 0" + "\n" +
                        "for mongo enter: 1");
                    database_type = Int32.Parse(Console.ReadLine());
                    Console.WriteLine("data base selected");
                    break;
                }
                catch
                {
                    Console.WriteLine("enter valid anser");
                }
            }
            Exception a = new NullReferenceException("");
            sales newSale = new sales();
            while (true)
            {
                Console.WriteLine("Welcome !!\nTo start a sale press: 1\nfor existing sale press 2\nTo enter admin press: 3\n\nto exit enter -1");
                string ans = Console.ReadLine();
                try
                {
                    if (Int32.Parse(ans) == -1)
                    {
                        break;
                    }
                    switch (Int32.Parse(ans))
                    {
                        case 1:
                            if (database_type == 0)
                            {
                                newSale = DAL.DAL.CreateSale();
                                if (newSale == null)
                                {
                                    throw a;
                                }
                                existing_sale(DAL.DAL.EnterSaleToSQL(newSale), database_type);
                            }
                            if (database_type == 1)
                            {
                                newSale = MongoDAL.MongoDAL.CreateSale();
                                MongoDAL.MongoDAL.EnterSaleToMongo(newSale);
                                if (newSale == null)
                                {
                                    throw a;
                                }
                                existing_sale(newSale.getsale_id(), database_type);
                            }
                            break;
                        case 2:
                            existing_sale(-1, database_type);
                            break;
                        case 3:
                            int temp = admin(database_type);
                            if (temp != -1)
                            {
                                database_type = temp;
                            }
                            break;
                    }
                    if ((Int32.Parse(ans) > 3 || Int32.Parse(ans) < 0) && Int32.Parse(ans) != -1)
                    {
                        Console.Clear();
                        Console.WriteLine("enter valid anser\n");
                    }
                }
                catch (Exception e)
                {
                    Console.Clear();
                    if (e != a)
                    {
                        Console.WriteLine("enter valid anser\n");
                    }
                }
            }

            /*




                        if (database_type == 0)
                        {
                            Exception a = new NullReferenceException("");
                            sales newSale = new sales();
                            while (true)
                            {
                                Console.WriteLine("Welcome !!\nTo start a sale press: 1\nfor existing sale press 2\nTo enter admin press: 3\n\nto exit enter -1");
                                string ans = Console.ReadLine();
                                try
                                {
                                    if (Int32.Parse(ans) == -1)
                                    {
                                        break;
                                    }
                                    switch (Int32.Parse(ans))
                                    {
                                        case 1:
                                            newSale = DAL.DAL.CreateSale();
                                            if (newSale == null)
                                            {
                                                throw a;
                                            }
                                            existing_sale(DAL.DAL.EnterSaleToSQL(newSale));
                                            break;
                                        case 2:
                                            existing_sale(-1);
                                            break;
                                        case 3:
                                            admin();
                                            break;
                                    }
                                    if ((Int32.Parse(ans) > 3 || Int32.Parse(ans) < 0) && Int32.Parse(ans) != -1)
                                    {
                                        Console.Clear();
                                        Console.WriteLine("enter valid anser\n");
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.Clear();
                                    if (e != a)
                                    {
                                        Console.WriteLine("enter valid anser\n");
                                    }
                                }
                            }
                        }
                        if(database_type == 1)
                        {


                            MongoDAL.MongoDAL.GetRecit(0);
                            MongoDAL.MongoDAL.creatsalefrommongo(5);
                            MongoDAL.MongoDAL.DeleatSale(0);
                            MongoDAL.MongoDAL.updatesale(0);
                            MongoDAL.MongoDAL.pay(0);
                            Console.WriteLine(MongoDAL.MongoDAL.UnfinishedSales());
                            MongoDAL.MongoDAL.EndOfDay1();
                            MongoDAL.MongoDAL.Moste_popular_Ingredient();
                            MongoDAL.MongoDAL.Moste_popular_flaver();

                        }*/
        }
    }
}
