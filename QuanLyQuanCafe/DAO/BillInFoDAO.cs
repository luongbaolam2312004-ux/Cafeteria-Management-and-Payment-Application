using QuanLyQuanCafe.DTO;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace QuanLyQuanCafe.DAO
{
    public class BillInFoDAO
    {
        private static BillInFoDAO instance;

        public static BillInFoDAO Instance
        {
            get { if (instance == null) instance = new BillInFoDAO(); return instance; }
            private set { instance = value; }
        }

        private BillInFoDAO() { }

        public void DeleteBillInfoByFoodID(int id)
        {
            DataProvider.Instance.ExecuteQuery("DELETE dbo.BillInfo WHERE idFood = " + id);
        }

        public List<BillInfo> GetListBillInfo(int id)
        {
            List<BillInfo> listBillInFo = new List<BillInfo>();

            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM dbo.BillInfo WHERE idBill = " + id);

            foreach (DataRow item in data.Rows)
            {
                BillInfo info = new BillInfo(item);
                listBillInFo.Add(info);
            }
            return listBillInFo;
        }
        public void InsertBillInfo(int idBill, int idFood, int count)
        {
            DataProvider.Instance.ExecuteQuery("exec USP_InsertBillInfo @idBill , @idFood , @count", new object[] { idBill, idFood, count });
        }

    }
}
