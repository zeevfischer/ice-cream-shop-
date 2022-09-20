using BEL;
using BLL;
using MongoDB.Driver.Core.WireProtocol.Messages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MongoDB.Driver.WriteConcern;


namespace DAL
{
    internal class DAL
    {
        public static SqlConnection ConnectSQLserver(string database)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.ConnectionString = @"Server = (localdb)\MSSQLLocalDB;"+database+"Trusted_Connection=True;";
                SqlConnection connection = new SqlConnection(builder.ConnectionString);
                return connection;
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            throw new ArgumentException("Couldn't establish connection with SQL server");
        }
        public static void createDatabase()
        {
            SqlConnection connection = ConnectSQLserver("");
            connection.Open();
            string sql;
            SqlCommand command;

            sql =
                "DROP DATABASE IF EXISTS project2;"+
                "CREATE DATABASE project2;";
            command = new SqlCommand(sql, connection);
            command.ExecuteNonQuery();
            Console.WriteLine("created database project2");
            connection.Close();
        }
        public static void createTables()
        {
            SqlConnection connection = ConnectSQLserver("Database=project2;");
            connection.Open();
            string sql;
            SqlCommand command;
            try
            {
                sql =
                    "DROP TABLE IF EXISTS [dbo].[sales]" +
                    "CREATE TABLE [dbo].[sales]" +
                    "(" +
                        "[sale id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY," +
                        "[total price] INT NULL," +
                        "[payment left] INT NULL" +
                        "[compleated] INT NULL, "+
                        "[date] varchar(45) NOT NULL" +
                    ")";
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                Console.WriteLine("created table sales");    

                 sql =
                    "DROP TABLE IF EXISTS [dbo].[sale contence]" +
                    "CREATE TABLE [dbo].[sale contence]" +
                    "(" +
                        "[id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY," +
                        "[sale id] INT NOT NULL," +
                        "[pruduct id] INT NOT NULL," +
                    ")";
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                Console.WriteLine("created table sale contence");
            }
            catch (SqlException exp)
            {
                Console.WriteLine(exp.ToString());
            }
            connection.Close();
        }
        public static void CreateAndLoadIngredientsTable()
        {
            int ingredient_count = 0;
            SqlConnection connection = ConnectSQLserver("Database=project2;");
            connection.Open();
            string sql;
            SqlCommand command;

            try
            {
                sql =
                    "DROP TABLE IF EXISTS [dbo].[Ingredients]" +
                    "CREATE TABLE [dbo].[Ingredients]" +
                    "(" +
                        "[IngredientId] INT NOT NULL PRIMARY KEY," +
                        "[Name] VARCHAR(45) NOT NULL," +
                    ")";
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                Console.WriteLine("created table Ingredients");

                var values = Enum.GetValues(typeof(Flavors));
                foreach (var ingredientName in values)
                {
                    sql = "INSERT INTO [dbo].[Ingredients] ([IngredientId], [Name]) VALUES (" + ingredient_count + ", N'" + ingredientName.ToString() + "')";
                    ingredient_count++;
                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
                values = Enum.GetValues(typeof(Extras));
                foreach(var extras in values)
                {
                    sql = "INSERT INTO [dbo].[Ingredients] ([IngredientId], [Name]) VALUES (" + ingredient_count + ", N'" + extras.ToString() + "')";
                    ingredient_count++;
                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
                values = Enum.GetValues(typeof(Cupsize));
                foreach (var Cupsize in values)
                {
                    sql = "INSERT INTO [dbo].[Ingredients] ([IngredientId], [Name]) VALUES (" + ingredient_count + ", N'" + Cupsize.ToString() + "')";
                    ingredient_count++;
                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                }

            }
            catch (SqlException exp)
            {
                Console.WriteLine(exp.ToString());
            }
            connection.Close();
        }
        //this function will get a object sale and devide its data to the apropriet table
        public static int EnterSaleToSQL(sales sale)
        {
            string sql;
            SqlConnection connection = ConnectSQLserver("Database=project2;");
            connection.Open();
            SqlCommand command;
            try
            {
                /*Console.WriteLine(sale.getDate());*/
                sql = "INSERT INTO [dbo].[sales] ([total price], [compleated], [date]) VALUES ("+ sale.getPrice() + ", " + sale.getCompleted() + ", '" + sale.getDate() + "')";
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                Console.WriteLine("enterd new sale\n");
            }
            catch (SqlException exp)
            {
                Console.WriteLine(exp.ToString());
            }
            // now we need to get the sale id jenerated by the sql 
            /*
                Noat to get the sale id i whant i will go to the last enterd sale in this table 
                this needs to be thread safe 
                because if 2 people are entering therre order together thing may get complecated 
                but for this project it will worck without 
            */
            sql = $"select * FROM sales [sale id];";
            command = new SqlCommand(sql, connection);
            SqlDataReader res = command.ExecuteReader();
            int sale_id = -1;
            while (res.Read())
            {
                sale_id = res.GetInt32(0); // at the end we get the sale id we want
            }
            res.Close();
            

            // now to enter the sale contence to the second table 
            sql = "INSERT INTO [dbo].[sale contence] ([sale id], [pruduct id]) VALUES (" + sale_id + ", " + ((int)sale.getCup() + 13) + ")";
            command = new SqlCommand(sql, connection);
            command.ExecuteNonQuery();
            foreach (var F in sale.getFlavors())
            {
                sql = "INSERT INTO [dbo].[sale contence] ([sale id], [pruduct id]) VALUES (" + sale_id + ", " + (int)F+")";
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
            foreach(var E in sale.getExtras())
            {
                sql = "INSERT INTO [dbo].[sale contence] ([sale id], [pruduct id]) VALUES (" + sale_id + ", " + ((int)E+10) + ")";
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
            connection.Close();
            return sale_id;
        }
        public static void DeleatSale(int sale_id)
        {
            string sql;
            SqlConnection connection = ConnectSQLserver("Database=project2;");
            connection.Open();
            SqlCommand command;
            try
            {
                sql = "DELETE dbo.sales where [sale id] = ("+sale_id+")";
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                Console.WriteLine("deleted sale id "+ sale_id);
            }
            catch (SqlException exp)
            {
                Console.WriteLine(exp.ToString());
            }
            try
            {
                sql = "DELETE dbo.[sale contence] where [sale id] = (" + sale_id + ")";
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                Console.WriteLine("deleted sale id " + sale_id);
            }
            catch (SqlException exp)
            {
                Console.WriteLine(exp.ToString());
            }
            connection.Close();
        }
        public static string GetRecit(int sale_id)
        {
            //this is a later function and i can use it to change the entier function GetRecit but never mind
            //we will use this to alsow add a money change return or new deat that will mainly be after updats to the order

            string recit = "";
            // data from salse table 
            string sql = $"select * FROM sales WHERE [sale id] = (" + sale_id + ");";
            SqlConnection connection = ConnectSQLserver("Database=project2;");
            connection.Open();
            SqlCommand command = new SqlCommand(sql, connection);
            SqlDataReader res = command.ExecuteReader();
            res.Read();

            int id = res.GetInt32(0);
            float total_price = res.GetInt32(1);
            string compleated = "";
            if (res.GetInt32(2) == 1)
            {
                compleated = "yes";
            }
            else
            {
                compleated = "no";
            }
            string date = res.GetString(3);
            res.Close();

            // data from contence
            sql = $"select * FROM [sale contence] WHERE [sale id] = (" + sale_id + ");";
            command = new SqlCommand(sql, connection);
            res = command.ExecuteReader();

            string contence = "";
            List<int> temp = new List<int>();
            while (res.Read())
            {
                temp.Add(res.GetInt32(2));
            }
            res.Close();

            foreach (var T in temp)
            {
                sql = $"select * FROM [Ingredients] WHERE [IngredientId] = (" + T + ");";
                command = new SqlCommand(sql, connection);
                res = command.ExecuteReader();
                res.Read();
                contence += "" + res.GetString(1) + ",\n";
                res.Close();
            }

            recit = "recit for sale id: " + sale_id + "\n" +
                "date: " + date + "\n" +
                "total price is: " + total_price + "\n" +
                "payment compleated: " + compleated + "\n" +
                "ingridients: " + "\n" + contence;

            return recit;
        }
        public static sales CreateSale()
        {
            sales newsale = new sales();
            Console.Clear();
            Console.WriteLine("select a cup size from the folloing");
            var values = Enum.GetValues(typeof(Cupsize));
            string ans = "";
            while(true)
            {
                try
                {
                    do
                    {
                        foreach (var v in values)
                        {
                            Console.WriteLine("for " + v.ToString() + " press " + v.GetHashCode());
                        }
                        Console.WriteLine("enter -1 to exit");
                        ans = Console.ReadLine();
                        if(Int32.Parse(ans) == -1)
                        {
                            return null;
                        }
                        if (Int32.Parse(ans) > 2 || Int32.Parse(ans) < 0)
                        {
                            Console.WriteLine("enter valid anser");
                        }
                    }
                    while (Int32.Parse(ans) > 2 || Int32.Parse(ans) < 0);// while it is not what i want
                    newsale.setCup((Cupsize)Int32.Parse(ans));
                    break;
                }
                catch
                {
                    Console.WriteLine("enter valid anser");
                }
            }
            Console.Clear();          
            values = Enum.GetValues(typeof(Flavors));
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
                        else if (BLL.BLL.checkFlaver(newsale, (Flavors)Int32.Parse(ans)) == true && BLL.BLL.checK_sale(newsale) == true)
                        {
                            newsale.getFlavors().Add((Flavors)Int32.Parse(ans));
                        }
                        else
                        {
                            ans = "-1";
                            break;
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
            if(ans != "-1")
            {
                Thread.Sleep(3000);
            }
            Console.Clear();
            values = Enum.GetValues(typeof(Extras));
            ans = "";
            while (1 > 0)
            {
                try
                {
                    do
                    {
                        Console.WriteLine("select a Extras from the folloing and -1 to stop");
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
                        else if (BLL.BLL.checkextra(newsale, (Extras)Int32.Parse(ans)) == true)
                        {
                            newsale.getExtras().Add((Extras)Int32.Parse(ans));
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
            Console.Clear();

            BLL.BLL.total_price(newsale);
            DateTime localDate = DateTime.Now;
            newsale.setDate(localDate.ToString("dd/MM/yyyy HH:mm:ss"));
            return newsale;
        }
        public static void updatesale(int sale_id)
        {
            // we will let you add to the order as mutch as you whant but if you paied you can not return and remove
            // so lets get the compleated status 
            int compleated = -1;
            SqlConnection connection = ConnectSQLserver("Database=project2;");
            connection.Open();
            string sql = $"select * FROM [sales] WHERE [sale id] = (" + sale_id + ");";
            SqlCommand command = new SqlCommand(sql, connection); ;
            SqlDataReader res = command.ExecuteReader();
            if (res.Read())
            {
                compleated = res.GetInt32(2);
            }
            res.Close();
            connection.Close();

            // if i whant to change the cup size
            while (true)
            {
                Console.WriteLine("to change cup size enter 1" + "\n" +
                "to change Flavers enter 2" + "\n" +
                "to change extras enter 3" + "\n" +
                "to go back enter -1");
                string ans = Console.ReadLine();
                Console.Clear();
                // try for what to change
                try
                {
                    if (Int32.Parse(ans) == 1)
                    {
                        if(compleated == 1)
                        {
                            Console.WriteLine("you have paid the order was made and you can not change this item\n");
                            break;
                        }
                        string s = "";
                        var values = Enum.GetValues(typeof(Cupsize));
                        int i = 0;
                        foreach (var newcup in values)
                        {
                            s += "for: " + newcup.ToString() + " enter " + i+"\n";
                            i++;
                        }
                        bool breaker = true;
                        while (breaker)
                        {
                            // try for new cup
                            try
                            {
                                do
                                {
                                    Console.WriteLine(s);
                                    ans = Console.ReadLine();
                                }
                                while (Int32.Parse(ans) <= 0 && Int32.Parse(ans) >= 2);

                                connection = ConnectSQLserver("Database=project2;");
                                connection.Open();

                                // GET NEW CUP INGREDIENT ID
                                string new_cup = Enum.GetName(typeof(Cupsize), Int32.Parse(ans)).ToString();
                                sql = $"select * FROM [Ingredients] WHERE [Name] = ('" + new_cup  + "');";
                                command = new SqlCommand(sql, connection); ;
                                res = command.ExecuteReader();
                                res.Read();
                                int new_cup_id = res.GetInt32(0);
                                res.Close();

                                //chek that new ingreadient dose not exist in old order
                                sql = $"select * FROM [sale contence] WHERE [sale id] = (" + sale_id + ");";
                                command = new SqlCommand(sql, connection); ;
                                res = command.ExecuteReader();//
                                int line_id = -1;
                                while (res.Read())
                                {
                                    if(res.GetInt32(2) == new_cup_id)
                                    {
                                        Console.WriteLine("ingreadient already exists");
                                        Thread.Sleep(3000);
                                        Console.Clear();
                                        break;
                                    }
                                    //yes this is not elegent and very not safe when code will need update but fouck it 
                                    if(res.GetInt32(2) >= 13 && res.GetInt32(2) <= 15)
                                    {
                                        line_id = res.GetInt32(0);
                                    }
                                }
                                res.Close();
                                // UPDATE
                                sql = "UPDATE dbo.[sale contence] SET [pruduct id] = (" + new_cup_id + ") WHERE [id] = (" + line_id + ")";
                                command = new SqlCommand(sql, connection);
                                command.ExecuteNonQuery();
                                connection.Close();
                                break;
                            }
                            catch
                            {
                                Console.WriteLine("pleas enter a valid input");
                            }
                        }
                        if(!breaker)
                        {
                            break;
                        }
                    }
                    //if i whant to change the ice cream flavers
                    else if (Int32.Parse(ans) == 2)
                    {
                        while (true)
                        {
                            // main try catch for ans 
                            try
                            {
                                Console.WriteLine("to add a flaver enter: 1 to remove a flaver enter: 2\nto return enter: -1");
                                ans = Console.ReadLine();

                                connection = ConnectSQLserver("Database=project2;");
                                connection.Open();
                                switch (Int32.Parse(ans))
                                {

                                    //add
                                    case 1:
                                        //choose a flaver to add 
                                        sales new_sale = CreateObjectSaleFromSQL(sale_id);
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
                                                    else if (BLL.BLL.checkFlaver(new_sale, (Flavors)Int32.Parse(ans)) && BLL.BLL.checK_sale(new_sale))
                                                    {
                                                        new_sale.getFlavors().Add((Flavors)Int32.Parse(ans));
                                                        // enter the flaver to the sql table
                                                        sql = "INSERT INTO [dbo].[sale contence] ([sale id], [pruduct id]) VALUES (" + sale_id + ", " + Int32.Parse(ans) + ")";
                                                        command = new SqlCommand(sql, connection);
                                                        command.ExecuteNonQuery();
                                                    }
                                                    else
                                                    {
                                                        ans = "-1";
                                                        break;
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
                                        if (compleated == 1)
                                        {
                                            Console.WriteLine("you have paid the order was made and you can not change this item\n");
                                            break;
                                        }
                                        //get the sale contence
                                        sql = $"select * FROM [sale contence] WHERE [sale id] = ('" + sale_id + "');";
                                        command = new SqlCommand(sql, connection);
                                        res = command.ExecuteReader();
                                        //get all flavers from order
                                        // has map key = id (sql key) value = IngredientId
                                        Dictionary<int, int> flavers_id = new Dictionary<int, int>();
                                        while (res.Read())
                                        {
                                            //again this is not safe if things change but my code so ...
                                            // if it is a flaver get its id
                                            if (res.GetInt32(2) <= 10 && res.GetInt32(2) >= 0)
                                            {
                                                flavers_id[res.GetInt32(0)] = res.GetInt32(2);
                                            }
                                        }
                                        res.Close();
                                        foreach (int F in flavers_id.Keys)
                                        {
                                            // get the flaver by key of hash
                                            sql = $"select * FROM [Ingredients] WHERE [IngredientId] = ('" + flavers_id[F] + "');";
                                            command = new SqlCommand(sql, connection);
                                            res = command.ExecuteReader();
                                            res.Read();
                                            // enter key to remove
                                            Console.WriteLine("to remove flaver: " + res.GetString(1) + " enter: " + F);
                                            res.Close();
                                        }
                                        // try for removing ans
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
                                                sql = $"DELETE [sale contence] WHERE [id] = (" + Int32.Parse(ans) + ");";
                                                command = new SqlCommand(sql, connection);
                                                command.ExecuteNonQuery();
                                                break;
                                            }
                                            catch (SqlException e)
                                            {
                                                Console.WriteLine(e);
                                            }
                                        }
                                        break;
                                }
                                connection.Close ();
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

                    }
                    //if i whant to change the extras
                    else if (Int32.Parse(ans) == 3)
                    {
                        while (true)
                        {
                            // main try catch for ans 
                            try
                            {
                                Console.WriteLine("to add a extras enter: 1 to remove a extras enter:2\nto return enter -1");
                                ans = Console.ReadLine();

                                connection = ConnectSQLserver("Database=project2;");
                                connection.Open();
                                switch (Int32.Parse(ans))
                                {

                                    //add
                                    case 1:
                                        //choose an extra to add 
                                        sales new_sale = CreateObjectSaleFromSQL(sale_id);
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
                                                    else if (BLL.BLL.checkextra(new_sale, (Extras)Int32.Parse(ans)) && BLL.BLL.checK_sale(new_sale))
                                                    {
                                                        new_sale.getExtras().Add((Extras)Int32.Parse(ans));
                                                        // enter the extra to the sql table
                                                        sql = "INSERT INTO [dbo].[sale contence] ([sale id], [pruduct id]) VALUES (" + sale_id + ", " + Int32.Parse(ans)+10 + ")";
                                                        command = new SqlCommand(sql, connection);
                                                        command.ExecuteNonQuery();
                                                    }
                                                    else
                                                    {
                                                        ans = "-1";
                                                        break;
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
                                        if (compleated == 1)
                                        {
                                            Console.WriteLine("you have paid the order was made and you can not change this item\n");
                                            break;
                                        }
                                        //get the sale contence
                                        sql = $"select * FROM [sale contence] WHERE [sale id] = ('" + sale_id + "');";
                                        command = new SqlCommand(sql, connection);
                                        res = command.ExecuteReader();
                                        //get all Extras from order
                                        // has map key = id (sql key) value = IngredientId
                                        Dictionary<int, int> extras_id = new Dictionary<int, int>();
                                        while (res.Read())
                                        {
                                            //again this is not safe if things change but my code so ...
                                            // if it is a flaver get its id
                                            if (res.GetInt32(2) <= 12 && res.GetInt32(2) >= 10)
                                            {
                                                extras_id[res.GetInt32(0)] = res.GetInt32(2);
                                            }
                                        }
                                        res.Close();
                                        foreach (int key in extras_id.Keys)
                                        {
                                            // get the extras by key of hash
                                            sql = $"select * FROM [Ingredients] WHERE [IngredientId] = ('" + extras_id[key] + "');";
                                            command = new SqlCommand(sql, connection);
                                            res = command.ExecuteReader();
                                            res.Read();
                                            // enter key to remove
                                            Console.WriteLine("to remove flaver: " + res.GetString(1) + " enter: " + key);
                                            res.Close();
                                        }
                                        // try for removing ans
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
                                                sql = $"DELETE [sale contence] WHERE [id] = (" + Int32.Parse(ans) + ");";
                                                command = new SqlCommand(sql, connection);
                                                command.ExecuteNonQuery();
                                                break;
                                            }
                                            catch (SqlException e)
                                            {
                                                Console.WriteLine(e);
                                            }
                                        }
                                        break;
                                }
                                connection.Close();
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
                    }
                    else if(Int32.Parse(ans) == -1)
                    {
                        break;
                    }
                    //now after updating everything we need to update the price and the payment compleated 
                    //set payment to not compleated
                    connection = ConnectSQLserver("Database=project2;");
                    connection.Open();
                    sql = "UPDATE dbo.[sales] SET [compleated] = (" + 0 + ") WHERE [sale id] = (" + sale_id + ")";
                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                    // set total
                    sales updated_sale = CreateObjectSaleFromSQL(sale_id);
                    sql = "UPDATE dbo.[sales] SET [total price] = (" + updated_sale.getPrice() + ") WHERE [sale id] = (" + sale_id + ")";
                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                    connection.Close();

                }
                //un valid response
                catch
                {
                    Console.WriteLine("pleas enter a valid input");
                }
            }
        }
        public static string UnfinishedSales()
        {
            string unfinishedSales = "un finished sales are: ";
            string sql = $"select * FROM sales WHERE [compleated] = 0;";
            SqlConnection connection = ConnectSQLserver("Database=project2;");
            connection.Open();
            SqlCommand command = new SqlCommand(sql, connection);
            SqlDataReader res = command.ExecuteReader();
            while(res.Read())
            {
                int temp = res.GetInt32(0);
                unfinishedSales += "" + temp + ",";
            }
            return unfinishedSales;
        }
        public static void pay(int sale_id)
        {
            SqlConnection connection = ConnectSQLserver("Database=project2;");
            connection.Open();
            string sql = "UPDATE dbo.[sales] SET [compleated] = (" + 1 + ") WHERE [sale id] = (" + sale_id + ")";
            SqlCommand command = new SqlCommand(sql, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }
        public static sales CreateObjectSaleFromSQL(int sale_id)
        {
            sales new_sale = new sales();
            SqlConnection connection = ConnectSQLserver("Database=project2;");
            connection.Open();
            SqlCommand command;
            SqlDataReader res;
            string sql;

            sql = $"select * FROM [sales] WHERE [sale id] = (" + sale_id + ");";
            command = new SqlCommand(sql, connection);
            res = command.ExecuteReader();
            res.Read();
            // data fore object from sales
            new_sale.setPrice(res.GetInt32(1));
            new_sale.setCompleted(res.GetInt32(2));
            new_sale.setDate(res.GetString(3));
            res.Close();
            // data from contence
            sql = $"select * FROM [sale contence] WHERE [sale id] = (" + sale_id + ");";
            command = new SqlCommand(sql, connection);
            res = command.ExecuteReader();
            Cupsize cup = Cupsize.Box;
            List<Flavors> flavors = new List<Flavors>();
            List<Extras> Extras = new List<Extras>();
            while (res.Read())
            {
                //this is a flaver
                if (res.GetInt32(2) >= 0 && res.GetInt32(2) <= 9)
                {
                    new_sale.getFlavors().Add((Flavors)res.GetInt32(2));
                }
                //this is a Extra
                if (res.GetInt32(2) >= 10 && res.GetInt32(2) <= 12)
                {
                    int temp = res.GetInt32(2) - 10;
                    new_sale.getExtras().Add((Extras)temp);
                }
                //this is a cup
                if (res.GetInt32(2) >= 13 && res.GetInt32(2) <= 15)
                {
                    int temp = (res.GetInt32(2) - 13);
                    new_sale.setCup((Cupsize)temp);
                }
            }
            res.Close();
            return (new_sale);
        }


        /*        static void Main(string[] args)
                {
                    createDatabase();
                    createTables();
                    CreateAndLoadIngredientsTable();
                    EnterSaleToSQL(CreateSale());
                    string temp = GetRecit(1);
                    Console.WriteLine();
                    Console.WriteLine(temp);
                    string temp2 = UnfinishedSales();
                    Console.WriteLine();
                    Console.WriteLine(temp2);
                    *//*DeleatSale(888);*//*
                }*/
        /*public static sales demo()
        {

            *//*int sale_id = 888;*//*
            int compleated = 0;
            DateTime localDate = DateTime.Now;
            string date = localDate.ToString("dd/MM/yyyy HH:mm:ss");
            Cupsize cup = Cupsize.RegularCup;
            List<Flavors> F = new List<Flavors>();
            F.Add(Flavors.Coconut);
            F.Add(Flavors.Pecan);
            F.Add(Flavors.Vanilla);
            List<Extras> E = new List<Extras>();
            E.Add(Extras.Peanuts);
            E.Add(Extras.maple);
            sales S = new sales(*//*sale_id,*/ /*price,*//* compleated, date, cup, F, E);
            BLL.BLL.total_price(S);
            foreach (var fla in F)
            {
                Console.WriteLine(fla);
            }
            Console.WriteLine(S.getPrice());
            *//*Console.Clear();*//*
            return S;
        }*/
    }
}
