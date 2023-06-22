using QLQuanCaPhe.DTO;
using System.Collections.Generic;
using System.Data;

namespace QLQuanCaPhe.DAO
{
    public class CategoryDAO
    {
        //tạo cấu trúc singleTon cho Category
        private static CategoryDAO instance;

        public static CategoryDAO Instance
        {
            get { if (instance == null) instance = new CategoryDAO(); return instance; }
            private set { instance = value; }
        }
        private CategoryDAO() { }

        //tạo danh sách Category
        public DataTable GetCategoryList()
        {
            return DataProvider.Instance.ExecuteQuery("USP_GetListCattegory");
        }
        public List<Category> GetListCategory()
        {
            List<Category> listCategory = new List<Category>();

            string query = "SELECT * FROM dbo.FoodCategory";

            //chạy data bằng câu query(câu truy vấn SQL )
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Category category = new Category(item);
                //thêm category vào list
                listCategory.Add(category);
            }

            return listCategory;
        }
        //lấy ra teen category theo id của nó 
        public Category GetCategoryByID(int id)
        {
            Category category = null;

            string query = "SELECT * FROM dbo.FoodCategory where id = " + id;

            //chạy data bằng câu query(câu truy vấn SQL )
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                category = new Category(item);
                return category;

            }

            return category;
        }

        //insert Loại
        public bool InsertCategory(string name)
        {
            string query = string.Format("insert dbo.FoodCategory ( name  ) values( N'{0}' )", name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        //xóa loại
        public bool DeleteCategory(int id)
        {
            string query = "delete dbo.FoodCategory where id = " + id;
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
        //sửa loại
        public bool UpdateCategory(int id, string name)
        {
            string query = string.Format("update dbo.FoodCategory set name = N'{0}' where  id = {1}", name, id);

            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
    }
}
