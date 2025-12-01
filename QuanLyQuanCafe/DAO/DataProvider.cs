using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace QuanLyQuanCafe.DAO
{
    public class DataProvider
    {
        private static DataProvider instance;

        private string connectionSTR = "Data Source=DESKTOP-CJL68E6;Initial Catalog=QuanLyQuanCafe;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

        public static DataProvider Instance
        {
            get { if (instance == null) instance = new DataProvider(); return instance; }
            private set { instance = value; }
        }

        private DataProvider() { }

        private void AddParameters(SqlCommand command, string query, object[] parameters)
        {
            if (parameters == null) return;

            int i = 0;
            for (int j = 0; j < query.Length; j++)
            {
                if (query[j] == '@')
                {
                    int end = j + 1;
                    while (end < query.Length && (char.IsLetterOrDigit(query[end]) || query[end] == '_'))
                    {
                        end++;
                    }

                    string paramName = query.Substring(j, end - j);
                    if (!command.Parameters.Contains(paramName))
                    {
                        command.Parameters.AddWithValue(paramName, parameters[i]);
                        i++;
                    }

                    j = end;
                }
            }
        }

        public DataTable ExecuteQuery(string query, object[] parameter = null)
        {
            DataTable data = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionSTR))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    AddParameters(command, query, parameter);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(data);
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi ExecuteQuery: " + ex.Message);
                }
            }

            return data;
        }

        public int ExecuteNonQuery(string query, object[] parameter = null)
        {
            int data = 0;

            using (SqlConnection connection = new SqlConnection(connectionSTR))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    AddParameters(command, query, parameter);

                    data = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi ExecuteNonQuery: " + ex.Message);
                }
            }

            return data;
        }

        public object ExecuteScalar(string query, object[] parameter = null)
        {
            object data = null;

            using (SqlConnection connection = new SqlConnection(connectionSTR))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    AddParameters(command, query, parameter);

                    data = command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi ExecuteScalar: " + ex.Message);
                }
            }

            return data;
        }
    }
}
