using Microsoft.VisualBasic;
using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance;

        public static AccountDAO Instance
        {
            get { if (instance == null) instance = new AccountDAO(); return instance; }
            private set { instance = value; }
        }

        private AccountDAO() { }

        public string EncryptPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); 
            }
        }


        public bool Login(string userName, string passWord)
        {
            string hasPass = EncryptPassword(passWord); 
            string query = "USP_Login @userName , @passWord";
            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] { userName, hasPass });
            return result.Rows.Count > 0;
        }

        public bool ChangePassAccount(string userName, string displayName, string pass, string newPass)
        {
            string passHash = EncryptPassword(pass);
            string newPassHash = EncryptPassword(newPass);

            int result = DataProvider.Instance.ExecuteNonQuery("exec USP_UpdateAccount @userName, @displayName, @password, @newPassword", new object[] { userName, displayName, passHash, newPassHash });

            return result > 0;
        }

        public DataTable GetListAccount()
        {
            return DataProvider.Instance.ExecuteQuery("SELECT UserName, DisplayName, Type FROM dbo.Account");
        }

        public Account GetAccountByUserName(string userName)
        {
            string query = "SELECT * FROM Account WHERE UserName = @userName";
            DataTable data = DataProvider.Instance.ExecuteQuery(query, new object[] { userName });

            foreach (DataRow item in data.Rows)
            {
                return new Account(item);
            }

            return null;
        }

        public bool InsertAccount(string name, string displayName, int type)
        {
            string query = string.Format("INSERT dbo.Account (UserName, DisplayName, Type) VALUES (N'{0}', N'{1}', {2})", name, displayName, type);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }


        public bool UpdateAccount(string userName, string newDisplayName, int newType)
        {
            string query = string.Format("UPDATE Account SET DisplayName = N'{0}', Type = {1} WHERE UserName = N'{2}'", newDisplayName, newType, userName);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }


        public bool DeleteAccount(string name)
        {
            string query = string.Format("Delete Account where UserName = N'{0}'", name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }


        public bool ResetPassword(string name)
        {
            string query = string.Format(
                "UPDATE Account SET Password = N'c4ca4238a0b923820dcc509a6f75849b' WHERE UserName = N'{0}'",name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

    }
}
  