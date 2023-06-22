using QLQuanCaPhe.DTO;
using System.Collections.Generic;
using System.Data;

namespace QLQuanCaPhe.DAO
{
    public class FoodDAO
    {
        //Vẫn như thường lệ thì đây lại là tạo cấu trúc theo kiểu SingleTon cho FoodDAO~~~
        private static FoodDAO instance;

        public static FoodDAO Instance
        {
            get { if (instance == null) instance = new FoodDAO(); return instance; }
            private set { instance = value; }
        }
        private FoodDAO() { }

        //lấy danh sách thức ăn theo id của categoryFood từ cái comboBox category á 
        public List<Food> GetListFoodByCategoryID(int id)
        {
            List<Food> listFood = new List<Food>();
            //tạo câu truy vấn nhét nó zô cái query để khúc dưới chạy data kiểu DataTable => lấy dc danh sách thức ăn
            string query = "SELECT * FROM dbo.Food WHERE idCategory =" + id;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                //lấy từng cái food và nhét nó zô danh sách (listFood)
                Food food = new Food(item);
                listFood.Add(food);
            }


            return listFood;
        }

        //tạo hàm lấy danh sách đồ ăn
        public DataTable GetListFood()
        {
            return DataProvider.Instance.ExecuteQuery("exec USP_GetListFood");
        }
        //Thêm món ăn
        public bool InsertFood(string name, int idCa, float price)
        {
            string query = string.Format("insert dbo.Food ( name , idCategory , price ) values( N'{0}' , {1} , {2})", name, idCa, price);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        //Sửa món ăn
        public bool UpdateFood(int idF, string name, int idCa, float price)
        {
            string query = string.Format("update dbo.Food set name = N'{0}' , idCategory = {1} , price = {2} where id = {3}", name, idCa, price, idF);

            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
        //Xóa món
        public bool DeleteFood(int id)
        {
            BillInfoDAO.Instance.DeleteBillInfoByFoodID(id);

            string query = string.Format("delete dbo.Food where id = {0} ", id);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        //tìm kiếm món
        public List<Food> SearchFoodByName(string name)
        {
            List<Food> listFood = new List<Food>();
            //tạo câu truy vấn nhét nó zô cái query để khúc dưới chạy data kiểu DataTable => lấy dc danh sách thức ăn
            string query = string.Format("select * from dbo.Food where dbo.fuConvertToUnsign1(name) like N'%' + dbo.fuConvertToUnsign1(N'{0}') + '%'\r\n", name);

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                //lấy từng cái food và nhét nó zô danh sách (listFood)
                Food food = new Food(item);
                listFood.Add(food);
            }
            return listFood;
        }
    }
}
