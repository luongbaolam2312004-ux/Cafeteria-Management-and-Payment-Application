using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class TableDAO
    {
        private static TableDAO instance;

        public static TableDAO Instance
        {
            get { if (instance == null) instance = new TableDAO(); return TableDAO.instance; }
            private set { TableDAO.instance = value; }
        }

        public static int TableWidth = 125;
        public static int TableHeight = 125;

        private TableDAO() { }

        public void SwitchTable ( int id1 , int id2 )
        {
            DataProvider.Instance.ExecuteQuery("USP_SwitchTable @idTable1 , @idTable2", new object[] {id1 , id2});
        }

        public List<Table> LoadTableList()
        {
            List<Table> tableList = new List<Table>();

            DataTable data = DataProvider.Instance.ExecuteQuery("USP_GetTableList");

            foreach (DataRow item in data.Rows)
            {
                Table table = new Table(item);
                tableList.Add(table);
            }    

            return tableList;
        }
        public bool InsertTable(string name, string status)
        {
            string query = string.Format("INSERT INTO dbo.TableFood (name, status) VALUES (N'{0}', N'{1}')", name, status);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool UpdateTable(int id, string name, string status)
        {
            string query = string.Format("UPDATE dbo.TableFood SET name = N'{0}', status = N'{1}' WHERE id = {2}", name, status, id);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool DeleteTable(int id)
        {
         
            string checkStatusQuery = "SELECT status FROM TableFood WHERE id = @id";
            string status = DataProvider.Instance.ExecuteScalar(checkStatusQuery, new object[] { id })?.ToString();

            if (status == "Có Người")
            {
                System.Windows.Forms.MessageBox.Show("Không thể xóa bàn này vì bàn đang có người sử dụng.");
                return false;
            }

            string checkBillQuery = "SELECT COUNT(*) FROM Bill WHERE idTable = @id";
            int billCount = (int)DataProvider.Instance.ExecuteScalar(checkBillQuery, new object[] { id });

            if (billCount > 0)
            {
                System.Windows.Forms.MessageBox.Show("Không thể xóa bàn này vì đã từng có hóa đơn liên kết.");
                return false;
            }

           
            string deleteTableQuery = "DELETE FROM dbo.TableFood WHERE id = @id";
            int result = DataProvider.Instance.ExecuteNonQuery(deleteTableQuery, new object[] { id });
            return result > 0;
        }


    }
}
