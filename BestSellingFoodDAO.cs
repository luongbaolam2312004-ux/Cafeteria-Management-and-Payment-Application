using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyQuanCafe
{
    public class BestSellingFoodDAO
    {
        private static BestSellingFoodDAO instance;

        public static BestSellingFoodDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new BestSellingFoodDAO();
                return instance;
            }
        }

        private BestSellingFoodDAO() { }

        public DataTable GetBestSellingFoods(DateTime fromDate, DateTime toDate, int topN)
        {
            string query = @"SELECT TOP (@TopN)  
                               FoodId,  
                               FoodName,  
                               SUM(Quantity) AS TotalSold,  
                               Price,  
                               SUM(Quantity * Price) AS TotalRevenue  
                            FROM BillInfo  
                            INNER JOIN Food ON BillInfo.FoodId = Food.Id  
                            INNER JOIN Bill ON BillInfo.BillId = Bill.Id  
                            WHERE Bill.DateCheckIn >= @FromDate AND Bill.DateCheckOut <= @ToDate  
                            GROUP BY FoodId, FoodName, Price  
                            ORDER BY TotalSold DESC";

            DataTable data = new DataTable();
            using (SqlConnection connection = new SqlConnection(/* Your connection string here */))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TopN", topN);
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toDate);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(data);
                }
            }
            return data;
        }
    }
}
