using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;

namespace BEL
{

    ///////////////////////////// Flavors ///////////////////////////
    public enum Flavors
    {
        Vanilla, //0
        Chocolate,
        Cookies,
        Cream,
        Mint,
        Pecan,
        Strawberry,
        Coconut,
        Mango,
        Oreo,//9
    }
    public class Flavor
    {
        public Flavor(Flavors F)
        {
            this.Flavors = F;
        }
        public Flavors Flavors { get; set; }
    }
    ///////////////////////////// Flavors ///////////////////////////

    ///////////////////////////// Extras ////////////////////////////
    public enum Extras 
    {
        // + 10
        HotChocolate,//10
        Peanuts,
        maple,//12
    }
    public class Extra
    {
        public Extra(Extras E)
        {
            this.Extras = E;
        }
        public Extras Extras { get; set; }
    }
    ///////////////////////////// Extras ////////////////////////////

    ///////////////////////////// Cupsize ///////////////////////////
    public enum Cupsize
    {
        // + 13
        RegularCup,//13
        SpecialCup,
        Box,//15
    }
    public class cupsize
    {
        public cupsize(Cupsize s)
        {
            this.Cupsize = s;
        }
        public Cupsize Cupsize { get; set; }
    }
    ///////////////////////////// Cupsize ///////////////////////////

    ///////////////////////////// sales /////////////////////////////
    public class sales
    {
        ObjectId id { get; set; } //for mongo
        int sale_id { get; set; } //for mongo
        int price { get; set; }
        int completed { get; set; }
        string date { get; set; }


        Cupsize cup { get; set; }
        List<Flavors> flavors { get; set; }
        List<Extras> Extras { get; set; }
        // constructors
        public sales(int price, int completed, string date, Cupsize cup, List<Flavors> flavors, List<Extras> Extras)
        {
            this.price = price;
            this.completed = completed;
            this.date = date;


            //sale contence
            this.cup = (Cupsize)cup;
            this.flavors = flavors;
            this.Extras = Extras;
        }
        public sales()
        {
            this.id = new ObjectId();//for mongo
            this.sale_id = -1;//for mongo
            this.price = -1;
            this.completed = -1;
            this.date = "";
            this.cup = Cupsize.RegularCup;
            this.flavors = new List<Flavors>();
            this.Extras = new List<Extras>();
        }
        //getters and setters
        public ObjectId get_ObjectId()  { return this.id; }
        public void set_ObjectId(ObjectId id) { this.id = id; }
        public int getsale_id() { return this.sale_id; }
        public void setsale_id(int sale_id) { this.sale_id = sale_id; }
        public float getPrice() { return this.price; }
        public void setPrice(int price) { this.price = price; }
        public int getCompleted() { return this.completed; }
        public void setCompleted(int completed) { this.completed = completed; }
        public string getDate() { return this.date; }
        public void setDate(string date) { this.date = date; }


        public Cupsize getCup() { return this.cup; }
        public void setCup(Cupsize cup) { this.cup = cup; }
        public List<Flavors> getFlavors() { return this.flavors; }
        public void setFlavors(List<Flavors> flavors) { this.flavors = flavors; }
        public List<Extras> getExtras() { return this.Extras; }
        public void setExtras(List<Extras> extras) { this.Extras = extras; }

    }
    ///////////////////////////// sales /////////////////////////////
}
