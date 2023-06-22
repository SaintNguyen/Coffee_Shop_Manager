using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLQuanCaPhe.DTO
{
    public class BillInfo
    {
        public BillInfo(int id, int billID, int foodID, int count)
        {
            this.ID = id;
            this.BillID = billID;
            this.FoodID = foodID;
            this.Count = count;
        }


        private int count;
        private int foodID;
        private int billID; 
        private int iD;

        public BillInfo(DataRow row)
        {
            //lấy ra từng cột trong table dbo.billInfo
            //lưu ý : kiểu mặc định được trả về của cách này là kiểu object nên cần ép kiểu cho phù hợp
            this.ID = (int)row["id"];
            this.BillID = (int)row["idBill"];
            this.FoodID = (int)row["idFood"];
            this.Count = (int)row["count"];
        } 

        public int ID
        {
            get { return iD; }
            set { iD = value; }
        }

        public int BillID
        {
            get { return billID; }
            set { billID = value; }
        }

        public int FoodID
        {
            get { return foodID; }
            set { foodID = value; }
        }

        public int Count
        {
            get { return count; }
            set { count = value; }
        }
    }
}
