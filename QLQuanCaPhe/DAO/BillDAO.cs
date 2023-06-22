using QLQuanCaPhe.DTO;
using System;
using System.Data;

namespace QLQuanCaPhe.DAO
{
    //tạo cấu trúc singleTon cho Bill
    public class BillDAO
    {
        private static BillDAO instance; //=> nhấn Ctrl + R + E

        public static BillDAO Instance
        {
            get { if (instance == null) instance = new BillDAO(); return instance; }
            private set { BillDAO.instance = value; }
        }
        private BillDAO() { }

        public int GetUnCheckBillIDByTableID(int id)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM dbo.Bill WHERE idTable = " + id + "AND status = 0");

            if (data.Rows.Count > 0)
            {
                Bill bill = new Bill(data.Rows[0]);
                return bill.ID;
            }
            return -1;
        }
        //nếu thành công : Bill ID
        //Nếu thất bại : -1

        //hàm này dùng để set giá trị stutus của Bill từ 0 sang 1 khi thanh toán
        public void checkOut(int id, int discount, float totalPrice)
        {
            string query = "update dbo.Bill set dateCheckOut = GETDATE() , status = 1 , " + "discount = " + discount + ", totalPrice =" + totalPrice + " where id = " + id;
            DataProvider.Instance.ExecuteNonQuery(query);
        }


        //them bill
        public void insertBill(int id)
        {
            DataProvider.Instance.ExecuteQuery("exec USP_InsertBill @idTable", new object[] { id });
        }

        //ListBill
        public DataTable GetListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            return DataProvider.Instance.ExecuteQuery("exec USP_GetListBillByDate @checkIn , @checkOut", new object[] { checkIn, checkOut });
        }

        //-----------------
        //phân trang bill theo ngày
        public DataTable GetListBillByDateAndPage(DateTime checkIn, DateTime checkOut, int pageNum)
        {
            return DataProvider.Instance.ExecuteQuery("exec USP_GetListBillByDateAndPage @checkIn , @checkOut , @Page", new object[] { checkIn, checkOut, pageNum });
        }
        //lấy số lượng bill
        public int GetNumBillByDate(DateTime checkIn, DateTime checkOut)
        {
            return (int)DataProvider.Instance.ExecuteScalar("exec USP_GetNumBillByDate @checkIn , @checkOut", new object[] { checkIn, checkOut });
        }


        //lấy cái ID bill lớn nhất hiện tại
        public int GetMaxIDBill()
        {
            try
            {
                return (int)DataProvider.Instance.ExecuteScalar(" select max(id) from dbo.Bill");
            }
            catch
            {
                return 1;
            }
            //làm xog mới nhận ra cái này nó hơi thừa mà kệ nó đi
            //nó thừa zì theo cái dòng chạy của code thì đường nào nó cx phải tạo 1 cái bill trước
            //rồi nó mới chạy vào cái hàm tạo billInfo
            //mà cái get max này thì dùng cho chỗ đó nên duognwf nào thì cx sẽ có 
            //1 cái bill đc tạo ròi nên ko có chuyện ch có bill nào dc tạo 
            //mà lỡ rồi nên kệ nó đi
        }

    }
}
