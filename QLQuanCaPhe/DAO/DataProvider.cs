using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLQuanCaPhe.DAO
{
    public class DataProvider
    {
        private static DataProvider instance; //Ctrl + R + E


        //singleton
        public static DataProvider Instance
        {
            get { if (instance == null) instance = new DataProvider(); return DataProvider.instance; }
            private set { DataProvider.Instance = value; }
        }
        private DataProvider() { }

        //chuỗi kết nối để kết nối với csdl
        private string connectionStr = "Data Source=SATOMIKOUTAROU\\SATOMI;Initial Catalog=QuanLyQuanCafe;Integrated Security=True";


        public DataTable /*int*/ ExecuteQuery(string query, object[] parameter = null)
        {
            //nơi để đổ dữ liệu từ sql vào
            DataTable data = new DataTable();
            //trả về số dòng thành công
            /*int data = 0;*/

            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                //mở connection
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

               if(parameter != null)
                {
                    string[]  listPara = query.Split(' ');

                    int i = 0;
                    foreach (string para in listPara)
                    {
                        if(para.Contains("@")) 
                        { 
                            command.Parameters.AddWithValue(para, parameter[i]);
                            i++;
                        }
                    }
                }

                //sql Adapter sẽ làm trung gian lấy dữ liệu và đổ vào DataTable
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                //đổ dữ liệu đã lấy ra vào biến data
               adapter.Fill(data);
                /* data = command.ExecuteScalar();*/

                //đóng connection
                connection.Close();

            }
            return data;
        }
        public /*string*/ int ExecuteNonQuery(string query, object[] parameter = null)
        {
            /*//nơi để đổ dữ liệu từ sql vào
            DataTable data = new DataTable();*/
            //trả về số dòng thành công
            int data = 0;

            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                //mở connection
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                if (parameter != null)
                {
                    string[] listPara = query.Split(' ');

                    int i = 0;
                    foreach (string para in listPara)
                    {
                        if (para.Contains("@"))
                        {
                            command.Parameters.AddWithValue(para, parameter[i]);
                            i++;
                        }
                    }
                }

               /* //sql Adapter sẽ làm trung gian lấy dữ liệu và đổ vào DataTable
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                //đổ dữ liệu đã lấy ra vào biến data
                adapter.Fill(data);*/
               data = command.ExecuteNonQuery();
                /*data = command.ExecuteScalar();*/

                //đóng connection
                connection.Close();
            }
            return data;
        }
        public object /*string*/ /*int*/ ExecuteScalar(string query, object[] parameter = null)
        {
            /*//nơi để đổ dữ liệu từ sql vào
            DataTable data = new DataTable();*/
            //trả về số dòng thành công
            /*int data = 0;*/
            //trả về tổng số dòng thành công
            object data = 0;

            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                //mở connection
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                if (parameter != null)
                {
                    string[] listPara = query.Split(' ');

                    int i = 0;
                    foreach (string para in listPara)
                    {
                        if (para.Contains("@"))
                        {
                            command.Parameters.AddWithValue(para, parameter[i]);
                            i++;
                        }
                    }
                }

                /* //sql Adapter sẽ làm trung gian lấy dữ liệu và đổ vào DataTable
                 SqlDataAdapter adapter = new SqlDataAdapter(command);

                 //đổ dữ liệu đã lấy ra vào biến data
                 adapter.Fill(data);*/

                /*data = command.ExecuteNonQuery();*/
                data = command.ExecuteScalar();

                //đóng connection
                connection.Close();
            }
            return data;
        }
    }
}
