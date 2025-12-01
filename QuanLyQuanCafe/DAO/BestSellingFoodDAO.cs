using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLyQuanCafe.DAO;
using QuanLyQuanCafe.DTO;
using System.Data;

namespace QuanLyQuanCafe.DAO
{
    public class BestSellingFoodDAO
    {
        private static BestSellingFoodDAO instance;

        public static BestSellingFoodDAO Instance
        {
            get { if (instance == null) instance = new BestSellingFoodDAO(); return instance; }
            private set { instance = value; }
        }

        private BestSellingFoodDAO() { }

        public List<BestSellingFoodDTO> GetBestSellingFoods(DateTime fromDate, DateTime toDate, int topN = 10)
        {
            List<BestSellingFoodDTO> list = new List<BestSellingFoodDTO>();

            string query = "USP_GetBestSellingFoods @fromDate , @toDate , @topN";
            object[] parameters = new object[] { fromDate, toDate, topN };

            DataTable data = DataProvider.Instance.ExecuteQuery(query, parameters);

            foreach (DataRow row in data.Rows)
            {
                BestSellingFoodDTO food = new BestSellingFoodDTO(row);
                list.Add(food);
            }

            return list;
        }
    }
}