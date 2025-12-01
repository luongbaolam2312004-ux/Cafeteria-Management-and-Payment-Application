using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DTO
{
    public class Menu
    {

        public Menu(string foodName, int count, float price, float totalPrice = 0)
        {
            this.Foodname = foodName;
            this.Count = count;
            this.Price = price;
            this.TotalPice = totalPice;
        }

        public Menu(DataRow row)
        {
            this.Foodname = row["name"].ToString();
            this.Count = (int)row["count"];
            this.Price = (float)Convert.ToDouble(row["price"].ToString());
            this.TotalPice = (float)Convert.ToDouble(row["totalPice"].ToString());
        }

        private float totalPice;
        public float TotalPice 
        {
            get { return totalPice; }
            set { totalPice = value; }
        }


        private float price;
        public float Price 
        {
            get { return price; }
            set { price = value; } 
        }

        private int count;

        public int Count
        {
            get { return count; }
            set { count = value; } 
        }


        private string foodname;

        public string Foodname 
        {
            get { return foodname; }
            set { foodname = value; }
        }

        public object FoodName { get; internal set; }
    }
}
