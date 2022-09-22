using BEL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DAL;
using MongoDB.Driver;

namespace BLL
{
    internal class BLL
    {
        /*
            prices reg //
            1 ice cream ball = 7
            2 ice cream ball = 12 // no limin extra
            3 ice cream ball = 18 // no limin extra
            
            prices special
            1 ice cream ball = 9 // no limin extra
            2 ice cream ball = 14 // no limin extra
            3 ice cream ball = 20 // no limin extra

            for box 
            1 ice cream ball = 12 // no limin extra
            2 ice cream ball = 17 // no limin extra
            3 ice cream ball = 23 // no limin extra
            every extra is +5

            Chocolate != HotChocolate 
            vanila != maple
        */
        public static Boolean checK_sale(sales sale)
        {
            if (sale.getCup() == Cupsize.RegularCup || sale.getCup() == Cupsize.SpecialCup)
            {
                if (sale.getFlavors().Count() == 3/* || sale.getFlavors().Count() <= 0*/)
                {
                    Console.Clear();
                    Console.WriteLine("you are limeted to 3 flavers");
                    /*Console.WriteLine("the number of ice cream balls is not valid");*/
                    return false;
                }
                if (sale.getCup() == Cupsize.RegularCup && sale.getFlavors().Count() > 2 && sale.getExtras().Count() > 0)
                {
                    Console.WriteLine("you have a RegularCup and can not add extras");
                    return false;
                }
            }
            return true;
        }
        public static Boolean checkFlaver(sales sale,Flavors F)
        {
            if (F.ToString() == "Chocolate")
            {
                foreach (var Extra in sale.getExtras())
                {
                    if (Extra.ToString() == "HotChocolate")
                    {
                        Console.WriteLine("the flaver Chocolate can not go with HotChocolate");
                        return false;
                    }
                }
            }
            if (F.ToString() == "Vanilla")
            {
                foreach (var Extra in sale.getExtras())
                {
                    if (Extra.ToString() == "maple")
                    {
                        Console.WriteLine("the flaver Vanilla can not go with maple");
                        return false;
                    }
                }
            }
            return true;
        }
        public static Boolean checkextra(sales sale,Extras E)
        {
            if(E.ToString() == "maple")
            {
                foreach(var flavor in sale.getFlavors())
                {
                    if(flavor.ToString() == "Vanilla")
                    {
                        Console.WriteLine("the Extra maple can not go with Vanilla");
                        return false;
                    }
                }
            }
            if(E.ToString() == "HotChocolate")
            {
                foreach (var flavor in sale.getFlavors())
                {
                    if (flavor.ToString() == "Chocolate")
                    {
                        Console.WriteLine("the extra HotChocolate can not go with Chocolate");
                        return false;
                    }
                }
            }
            //only when this happens there is a problem
            if (E != null && sale.getCup() == Cupsize.RegularCup && sale.getFlavors().Count() < 2)
            {
                return false;
            }
            return true;
        }
        /*string new_cup = Enum.GetName(typeof(Cupsize), Int32.Parse(ans)).ToString();*/
        public static Boolean checkcup(sales sale,Cupsize C)
        {
            if (C == Cupsize.RegularCup && sale.getFlavors().Count() > 2 && sale.getExtras().Count() > 0)
            {
                Console.Clear();
                Console.WriteLine("a RegularCup and can not add extras and this order has extras\n");
                return false;
            }
            if(sale.getFlavors().Count() > 3 && C != Cupsize.Box)
            {
                Console.Clear();
                Console.WriteLine("you have to meny flavers to fit in a cup and can only have a box\n");
                return false;
            }
            if(sale.getCup() == C)
            {
                Console.Clear();
                Console.WriteLine("ingreadient already exists\n");
                return false;
            }
            return true;
        }
        public static void total_price(sales sale)
        {
            int total = 0;
            switch(sale.getFlavors().Count())
            {
                case 1:
                    total += 7;
                    break;
                case 2:
                    total += 12;
                    break;
                case 3:
                    total += 18;
                    break;
            }
            switch (sale.getCup())
            {
                case Cupsize.RegularCup:
                    total += 0;
                    break;
                case Cupsize.SpecialCup:
                    total += 2;
                    break;
                case Cupsize.Box:
                    total += 5;
                    if(sale.getFlavors().Count > 3)
                    {
                        total += 18;
                        total += (sale.getFlavors().Count - 3) * 6;
                    }
                    break;
            }
            total += sale.getExtras().Count() * 2;
            sale.setPrice(total);
        }
    }
}

