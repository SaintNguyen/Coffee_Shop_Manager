using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QLQuanCaPhe.DTO
{
    public class Bill
    {
        public Bill(int id, DateTime? dateCheckIn, DateTime? dateCheckOut, int status, int discount =0)
        {
            this.ID = id;
            this.dateCheckIn = dateCheckIn;  
            this.DateCheckOut = dateCheckOut;
            this.Status = status;
            this.Discount = discount;
        }
        public Bill(DataRow row)
        {
            //lấy ra từng cột trong table dbo.bill
            //lưu ý : kiểu mặc định được trả về của cách này là kiểu object nên cần ép kiểu cho phù hợp
            this.ID = (int)row["ID"];
            this.DateCheckIn = (DateTime?)row["dateCheckIn"];

            var dateCheckOutTemp = row["dateCheckOut"];
            if(dateCheckOutTemp.ToString() != "") //cần kiểm tra xem dữ liệu trả về có là null hay ko , nhưng nếu kiểm tra != null sẽ lỗi
                                                                           // vì vậy cần dùng ToString và kiểm tra khác rỗng != ""
            {
                this.DateCheckOut = (DateTime?)dateCheckOutTemp;
            }
            this.Status = (int)row["status"];
            if(row["discount"].ToString() != "")
                this.Discount = (int)row["discount"];
        }
        private int discount;
        private int status;
        private DateTime? dateCheckOut;
        private DateTime? dateCheckIn;
        private int iD;

        public int ID 
        {
            get { return iD; }
            set { iD = value; }
        }

        public DateTime? DateCheckIn
        {
            get { return dateCheckIn; }
            set { dateCheckIn = value; } 
        }

        public DateTime? DateCheckOut 
        { 
            get { return dateCheckOut; }
            set { dateCheckOut = value; }
        }

        public int Status 
        { 
            get { return status; }
            set { status = value; }
        }

        public int Discount {
            get { return discount; }
            set { discount = value; }
        }
    }
}
