using BEL;
using System;
using System.Threading;

namespace PL
{
    internal class PL
    {
        static void Main(string[] args)
        {
            Exception a = new NullReferenceException("");
            while (true)
            {
                Console.WriteLine("Welcome !!\nTo enter shop press: 1\nTo enter admin press: 2\nto exit press -1");
                string ans = Console.ReadLine();
                try
                {
                    if(Int32.Parse(ans) == 1)
                    {
                        sales newSale = DAL.DAL.CreateSale();
                        if(newSale == null)
                        {
                            throw a;
                        }
                        int sale_id = DAL.DAL.EnterSaleToSQL(newSale);
                        while (true)
                        {
                            Console.WriteLine("to viwe sale enter: 1\nto pay enter: 2\nto update sale enter: 3\nto deleat sale enter: 4\nto return to main menue enter: -1");
                            ans = Console.ReadLine();
                            try
                            {
                                switch (Int32.Parse(ans))
                                {
                                    case 1:
                                        Console.WriteLine(DAL.DAL.GetRecit(sale_id));
                                        break;
                                    case 2:
                                        newSale.setCompleted(1);
                                        DAL.DAL.pay(sale_id);
                                        break;
                                    case 3:
                                        Console.Clear();
                                        DAL.DAL.updatesale(sale_id);
                                        break;
                                    case 4:
                                        break;
                                }
                                if(Int32.Parse(ans) == -1)
                                {
                                    Console.Clear();
                                    break;
                                }

                            }
                            catch
                            {

                                Console.WriteLine("enter valid anser");
                            }
                        }
                    }
                    else if (Int32.Parse(ans) == 2)
                    {
                        while(true)
                        {
                            try
                            {
                                Console.WriteLine("to create data base enter: 1");
                                ans = Console.ReadLine();
                                Console.Clear();
                                if(Int32.Parse(ans)==1)
                                {
                                    DAL.DAL.createDatabase();
                                    DAL.DAL.createTables();
                                    DAL.DAL.CreateAndLoadIngredientsTable();
                                    Thread.Sleep(3000);
                                    Console.Clear();
                                    break;
                                }

                            }
                            catch
                            {
                                Console.WriteLine("enter valid anser");
                            }
                        }
                    }
                    else if(Int32.Parse(ans) == -1)
                    {
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("enter valid anser");
                    }
                }
                catch (Exception e)
                {
                    Console.Clear();
                    if (e != a)
                    {
                        Console.WriteLine("enter valid anser");
                    }
                }
            }
        }
    }
}
