using BEL;
using System;
using System.Threading;

namespace PL
{
    internal class PL
    {

        public static void existing_sale(int optinal_sale_id)
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
                    DAL.DAL.CreateObjectSaleFromSQL(sale_id);
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
                    if(Int32.Parse(ans)== -1)
                    {
                        Console.Clear();
                        break;
                    }
                    switch (Int32.Parse(ans))
                    {
                        case 1:
                            Console.Clear();
                            Console.WriteLine(DAL.DAL.GetRecit(sale_id));
                            Console.WriteLine("\nto continue press eany key");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        case 2:
                            DAL.DAL.pay(sale_id);
                            break;
                        case 3:
                            Console.Clear();
                            DAL.DAL.updatesale(sale_id);
                            break;
                        case 4:
                            DAL.DAL.DeleatSale(sale_id);
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
        public static void admin()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("to create sql data base enter: 1\nto create mongo data base enter: 2\nto get a dayly report enter: 3\nto get a sale report enter: 4\nto get the most popular ingredient enter: 5\nto get the most popular vlavers enter: 6\n\nto return enter -1");
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
                            try
                            {
                                DAL.DAL.createDatabase();
                                DAL.DAL.createTables();
                                DAL.DAL.CreateAndLoadIngredientsTable();
                                Thread.Sleep(3000);
                                Console.Clear();
                            }
                            catch
                            {
                                Console.WriteLine("enter valid anser\n");
                            }
                            break;
                        case 2:
                            
                            break;
                        case 3:
                            DAL.DAL.EndOfDay1();
                            Console.WriteLine("\nto continue press eany key");
                            Console.ReadLine();
                            break;
                        case 4:
                            DAL.DAL.EndOfDay2();
                            Console.WriteLine("\nto continue press eany key");
                            Console.ReadLine();
                            break;
                        case 5:
                            Console.Clear();
                            DAL.DAL.Moste_popular_Ingredient();
                            Console.WriteLine("\nto continue press eany key");
                            Console.ReadLine();
                            break;
                        case 6:
                            Console.Clear();
                            DAL.DAL.Moste_popular_flaver();
                            Console.WriteLine("\nto continue press eany key");
                            Console.ReadLine();
                            break;
                    }
                }
                catch
                {
                    Console.Clear();
                    Console.WriteLine("enter valid anser\n");
                }
            }
        }
        static void Main(string[] args)
        {
            int database_type = 1;
            //0 = sql
            //1 = Mongo
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

            MongoDAL.MongoDAL.CreateAndLoadIngredientsTable();
            MongoDAL.MongoDAL.EnterSaleToMongo(MongoDAL.MongoDAL.CreateSale());
            MongoDAL.MongoDAL.GetRecit(0);
            MongoDAL.MongoDAL.creatsalefrommongo(5);
            MongoDAL.MongoDAL.DeleatSale(0);
            MongoDAL.MongoDAL.updatesale(0);
            MongoDAL.MongoDAL.pay(0);
            Console.WriteLine(MongoDAL.MongoDAL.UnfinishedSales());
            MongoDAL.MongoDAL.EndOfDay1();
            MongoDAL.MongoDAL.Moste_popular_Ingredient();
            MongoDAL.MongoDAL.Moste_popular_flaver();
        }
    }
}
