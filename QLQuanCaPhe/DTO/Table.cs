using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLQuanCaPhe.DTO
{
    public class Table
    {
        //nhấn tổ hợp phím Ctrl + R + E => đóng gói nhanh các đối tượng 
        private int iD;
        private string name;
        private string status;
        public int ID
        { 
            get { return iD; }
            set {  iD = value; } 
        }

        public string Name 
        {
            get { return name; }
            set {  name = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; } 
        }
        public Table(int id, string name, string status)
        {
            this.ID = id;
            this.Name = name;
            this.Status = status;
        }

        public Table(DataRow row) 
        {
            //bước trung gian chuyển DataTable sang dạng List
            this.ID = (int)row["id"];
            this.Name = row["tableName"].ToString();
            this.Status = row["status"].ToString();
            //Lưu ý nhỏ để tránh nhầm lẫn các tên "id" , "tableName" và "status" bên trong dấu [""] là tên của cột trong SQL
        }
    }
}
