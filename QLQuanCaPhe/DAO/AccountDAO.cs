using QLQuanCaPhe.DTO;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace QLQuanCaPhe.DAO
{
    internal class AccountDAO
    {
        //thiết kế theo cấu trúc singleTon -- tương tự với DataProvider
        private static AccountDAO instance;



        public static AccountDAO Instance
        {
            get { if (instance == null) instance = new AccountDAO(); return instance; }
            private set { instance = value; }
        }
        private AccountDAO() { }

        public bool Login(string username, string password)
        {
            byte[] temp = ASCIIEncoding.ASCII.GetBytes(password);

            byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp);
            string hasPass = "";

            foreach (byte item in hasData)
            {
                hasPass += item;
            }
            password = hasPass;
            /*var List = hasData.ToString();
            List.Reverse();*/

            string query = "USP_Login @username , @password";
            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] { username, password });
            return result.Rows.Count > 0;
            //chú ý ở đoạn mã này bên phía SQL sử dụng Store Proc , kết hợp bên này sữ dụng parameter để hạn chế lỗi SQL Injection có thể xảy ra
        }
        //
        public bool UpdateAcount(string userName, string displayName, string pass, string newPass)
        {
            byte[] temp = ASCIIEncoding.ASCII.GetBytes(pass);
            byte[] temp1 = ASCIIEncoding.ASCII.GetBytes(newPass);

            byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp);
            byte[] hasData1 = new MD5CryptoServiceProvider().ComputeHash(temp1);

            string hasPass = "";
            string hasPass1 = "";

            foreach (byte item in hasData)
            {
                hasPass += item;
            }
            foreach (byte item in hasData1)
            {
                hasPass1 += item;
            }
            pass = hasPass;
            newPass = hasPass1;
            int result = DataProvider.Instance.ExecuteNonQuery("exec USP_UpdateAccount @userName , @displayName , @password , @newPassword ", new object[] { userName, displayName, pass, newPass });
            return result > 0;
        }


        //khỏi tạo hàm get tài khoản với username
        public Account GetAccountByUserName(string username)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("select * from dbo.Account where UserName = '" + username + "'");

            foreach (DataRow item in data.Rows)
            {
                return new Account(item);
            }

            return null;
        }
        //lấy ra danh sách accout ko lấy password
        public DataTable GetListAccount()
        {
            return DataProvider.Instance.ExecuteQuery("select Username, DisplayName, Type from dbo.Account");
        }

        //--------------
        //---------------thêm xóa sửa
        //thêm Account
        public bool InsertAccount(string username, string displayname, int type)
        {
            string query = string.Format("insert dbo.Account ( Username , Displayname , Type , Password) values( N'{0}' , N'{1}' , {2} , N'{3}')", username, displayname, type, "2251022057731868917119086224872421513662");
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        //xóa tài khoản
        public bool DeleteAccount(string username)
        {
            string query = string.Format("delete dbo.Account where username = N'{0}' ", username);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
        //sửa tài khoản
        public bool UpdateAccount(string username, string displayname, int type)
        {
            string query = string.Format("update dbo.Account set Displayname = N'{0}' , Type = {1}  where  Username = N'{2}'", displayname, type, username);

            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        //đặt lại mật khẩu
        public bool ResetPassword(string username)
        {
            string query = string.Format("update dbo.Account set password = N'2251022057731868917119086224872421513662' where Username = N'{0}' ", username);

            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
    }
}
