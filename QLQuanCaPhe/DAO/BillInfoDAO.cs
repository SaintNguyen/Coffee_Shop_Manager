using QLQuanCaPhe.DTO;
using System.Collections.Generic;
using System.Data;

namespace QLQuanCaPhe.DAO
{
    public class BillInfoDAO
    {
        //xây dựng cấu trúc SingleTon Bill Info
        private static BillInfoDAO instance;

        public static BillInfoDAO Instance
        {
            get { if (instance == null) instance = new BillInfoDAO(); return instance; }
            private set { instance = value; }
        }
        private BillInfoDAO() { }

        public List<BillInfo> GetListBillInfo(int id) //id trong này là id của Bill
        {
            List<BillInfo> listBillInfo = new List<BillInfo>();

            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM dbo.BillInfo WHERE idBill =" + id);

            foreach (DataRow item in data.Rows)
            {
                //tạo billInfo
                BillInfo info = new BillInfo(item);
                //thực hiện add bill info vào listBillInfo
                listBillInfo.Add(info);
            }

            return listBillInfo;
        }


        //thêm billInfo

        public void insertBillInfo(int idBill, int idFood, int count)
        {
            DataProvider.Instance.ExecuteQuery("exec USP_InsertBillInfo @idBill , @idFood , @count", new object[] { idBill, idFood, count });
        }
        //Xóa Bill Info
        public void DeleteBillInfoByFoodID(int id)
        {
            DataProvider.Instance.ExecuteQuery(string.Format("delete dbo.BillInfo where idFood = {0}", id));
        }
    }
}
