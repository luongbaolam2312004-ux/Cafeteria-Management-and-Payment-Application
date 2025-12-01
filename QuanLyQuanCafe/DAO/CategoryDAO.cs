using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class CategoryDAO
    {
        private static CategoryDAO instance;

        public static CategoryDAO Instance 
        {
            get { if (instance == null) instance = new CategoryDAO(); return CategoryDAO.instance; }
            private set { CategoryDAO.instance = value; } 
        }

        private CategoryDAO() { }

        public List<Category> GetListCategory()
        {
            List<Category> list = new List<Category>();

            string query = "select * from FoodCategory";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Category category = new Category(item);
                list.Add(category);
            }
            return list;
        }

        public Category GetCatrgoryByID(int id)
        {
            Category category = null;

            string query = "select * from FoodCategory where id = " + id;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                category = new Category(item);
                return category;
            }

            return category;
        }
        public bool InsertCategory(string name)
        {
            string query = string.Format("INSERT INTO FoodCategory (name) VALUES (N'{0}')", name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        public bool UpdateCategory(int id, string name)
        {
            string query = string.Format("UPDATE FoodCategory SET name = N'{0}' WHERE id = {1}", name, id);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        public bool DeleteCategory(int id)
        {
            // Kiểm tra xem có món ăn nào đang sử dụng category này không
            string checkFoodQuery = "SELECT COUNT(*) FROM Food WHERE idCategory = @id";
            int foodCount = (int)DataProvider.Instance.ExecuteScalar(checkFoodQuery, new object[] { id });

            if (foodCount > 0)
            {
                System.Windows.Forms.MessageBox.Show("Không thể xóa danh mục này vì đang có món ăn thuộc danh mục.");
                return false;
            }

            // Nếu không có món nào thuộc category này thì xóa
            string query = "DELETE FROM FoodCategory WHERE id = @id";
            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { id });
            return result > 0;
        }


    }
}
