using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace QuanLyQuanCafe.DTO
{
    public class BestSellingFoodDTO
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public string CategoryName { get; set; }
        public int TotalSold { get; set; }
        public int Price { get; set; }
        public int TotalRevenue { get; set; }

        public BestSellingFoodDTO(DataRow row)
        {
            FoodId = (int)row["FoodId"];
            FoodName = row["FoodName"].ToString();
            CategoryName = row["CategoryName"].ToString();
            TotalSold = (int)row["TotalSold"];
            Price = (int)row["Price"];
            TotalRevenue = (int)row["TotalRevenue"];
        }
    }
}