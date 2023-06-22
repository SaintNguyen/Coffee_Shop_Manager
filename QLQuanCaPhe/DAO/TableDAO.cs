using QLQuanCaPhe.DTO;
using System.Collections.Generic;
using System.Data;

namespace QLQuanCaPhe.DAO
{
    //tạo cấu trúc SingleTon cho Table DAO
    public class TableDAO
    {
        private static TableDAO instance;

        public static TableDAO Instance
        {
            get { if (instance == null) instance = new TableDAO(); return TableDAO.instance; }
            private set { instance = value; }
        }
        private TableDAO() { }

        //tạo và lưu giá trị kích thước của từng bàn
        public static int tableWidth = 90;
        public static int tableHeight = 90;

        //Chuyển bàn
        public void SwitchTable(int id1, int id2)
        {
            DataProvider.Instance.ExecuteQuery("USP_SwitchTable @idTable1 , @idTable2", new object[] { id1, id2 });
        }

        public List<Table> LoadTableList()
        {
            List<Table> tableList = new List<Table>();

            DataTable data = DataProvider.Instance.ExecuteQuery("USP_GetTableList");

            foreach (DataRow item in data.Rows)
            {
                //tạo table
                Table table = new Table(item);
                //add table vào tableList
                tableList.Add(table);
            }

            return tableList;
        }

        //lấy danh sách bàn
        public DataTable GetTableList()
        {
            return DataProvider.Instance.ExecuteQuery("exec USP_GetTableList");
        }
        //thêm bàn
        public bool InsertTable(string name, string status)
        {
            string query = string.Format("insert dbo.TableFood ( tableName , status ) values( N'{0}' , N'{1}')", name, status);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        //xóa bàn
        public bool DeleteTable(int id)
        {
            string query = string.Format("delete dbo.TableFood where id = {0} ", id);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
        //sửa bàn
        public bool UpdateTable(int id, string name, string status)
        {
            string query = string.Format("update dbo.TableFood set tableName = N'{0}' , status = N'{1}'  where  id = {2}", name, status, id);

            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
    }
}
