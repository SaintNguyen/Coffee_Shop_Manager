using Microsoft.Reporting.WinForms;
using QLQuanCaPhe.DAO;
using QLQuanCaPhe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace QLQuanCaPhe
{
    public partial class fAdmin : Form
    {
        BindingSource tableList = new BindingSource();
        BindingSource foodList = new BindingSource();
        BindingSource categoryList = new BindingSource();
        BindingSource accountList = new BindingSource();

        public Account loginAccount;
        public fAdmin()
        {
            InitializeComponent();
            LoadAll();

        }

        #region Method

        List<Food> SearchFoodByName(string name)
        {
            List<Food> listFood = FoodDAO.Instance.SearchFoodByName(name);

            return listFood;
        }

        //hàm load dữ liệu 
        void LoadAll()
        {
            dtgvFood.DataSource = foodList;
            dtgvTable.DataSource = tableList;
            dtgvCategory.DataSource = categoryList;
            dtgvAccount.DataSource = accountList;
            //Load
            LoadDateTimePickerBill();
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value, Convert.ToInt32(txbNumPage.Text));
            LoadListFood();
            LoadCategoryIntoComboBox(cbFoodCategory);
            loadBillByDateRP(dtpkFromDateRP.Value, dtpkToDateRP.Value);

            LoadListTable();
            LoadListCategory();
            LoadListAccount();
            //đổ dữ liệu
            addFoodBinding();
            addTableBinding();
            addCategoryBinding();
            addAccountBinding();
        }
        //hàm load category của food 
        void LoadCategoryIntoComboBox(System.Windows.Forms.ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "Name";
        }

        //hàm add food binding
        void addFoodBinding()
        {
            //sữ dụng kỹ thuật dataBingding
            txbFoodName.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbFoodId.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "id", true, DataSourceUpdateMode.Never));
            nmFoodPrice.DataBindings.Add(new Binding("Value", dtgvFood.DataSource, "Price", true, DataSourceUpdateMode.Never));
        }
        void addTableBinding()
        {
            txbTableID.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "id", true, DataSourceUpdateMode.Never));
            txbTableName.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "tableName", true, DataSourceUpdateMode.Never));
            txbTableStatus.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "status", true, DataSourceUpdateMode.Never));

        }
        void addCategoryBinding()
        {
            txbCategoryFoodID.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "id", true, DataSourceUpdateMode.Never));
            txbCategoryName.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "name", true, DataSourceUpdateMode.Never));
        }

        void addAccountBinding()
        {
            txbAccountUserName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "Username", true, DataSourceUpdateMode.Never));
            txbAccountDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
            nmAccountType.DataBindings.Add(new Binding("Value", dtgvAccount.DataSource, "Type", true, DataSourceUpdateMode.Never));
        }


        //hàm nì dùng để load cái khoản thời gian thống kê các hóa đơn trong 1 tháng
        void LoadDateTimePickerBill()
        {
            DateTime today = DateTime.Now; //tạo 1 cái biến today có giá trị là ngày hôm nay
            dtpkFromDate.Value = new DateTime(today.Year, today.Month, 1); //chỉ định cho cái dtpkFromDate là ngày đầu tiên trogn thán 
            dtpkToDate.Value = dtpkFromDate.Value.AddMonths(1).AddDays(-1);//chỉ định cái dtpkToDate là ngày cuối cùng trong tháng
                                                                           //giải thích cách chỉ định ngày cuối cùng trong tháng:
                                                                           //ta sẽ lấy ngày đầu tiên trong tháng đã chỉ định trước đó công thêm 1 tháng => ngày đầu tiên của tháng sau
                                                                           //sayu đó trừ đi 1 ngày để được ngày cuối cùng của tháng này
            dtpkFromDateRP.Value = new DateTime(today.Year, today.Month, 1); //chỉ định cho cái dtpkFromDate là ngày đầu tiên trogn thán 
            dtpkToDateRP.Value = dtpkFromDateRP.Value.AddMonths(1).AddDays(-1);


        }

        void LoadListBillByDate(DateTime checkIn, DateTime checkOut, int pageNum)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetListBillByDateAndPage(checkIn, checkOut, pageNum);

        }

        void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();
        }

        void LoadListTable()
        {
            tableList.DataSource = TableDAO.Instance.GetTableList();
        }
        void LoadListCategory()
        {
            categoryList.DataSource = CategoryDAO.Instance.GetCategoryList();
        }

        void LoadListAccount()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount();
        }
        void loadBillByDateRP(DateTime checkIn, DateTime checkOut)
        {
            DataTable dataTable = BillDAO.Instance.GetListBillByDate(checkIn, checkOut);

            rpvBillByDate.LocalReport.DataSources.Clear();
            ReportDataSource source = new ReportDataSource("DataSetBillByDate", dataTable);
            rpvBillByDate.LocalReport.ReportPath = @"D:\Workspace\WorkSpace\Winform\QLQuanCaPhe\QLQuanCaPhe\ReportBillByDate.rdlc";
            rpvBillByDate.LocalReport.DataSources.Add(source);

            rpvBillByDate.RefreshReport();
        }
        #endregion


        #region Event
        private void btnViewBill_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value, Convert.ToInt32(txbNumPage.Text));
        }
        private void btnReloadRp_Click(object sender, EventArgs e)
        {
            loadBillByDateRP(dtpkFromDateRP.Value, dtpkToDateRP.Value);
        }
        //-----------------------------------------------
        private void txbFoodId_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtgvFood.SelectedCells.Count > 0)
                {
                    //tọa 1 biến id bằng cách lấy id của nó trogn ô Mã loại có trong dtgvFood
                    int id = (int)dtgvFood.SelectedCells[0].OwningRow.Cells["idCategory"].Value;
                    //tạo 1 biến category có kiểu là category để lấy id của category thông qua CategoryDAO với hàm GetCategoryByID
                    Category category = CategoryDAO.Instance.GetCategoryByID(id);
                    //gán giá trị của cbFoodCategory = category
                    cbFoodCategory.SelectedItem = category;

                    //tạo 1 biến index => vị trí của Category trong cbFoodCategory
                    int index = -1;
                    int i = 0;

                    foreach (Category item in cbFoodCategory.Items)
                    {
                        if (item.ID == category.ID) //so sách id lấy từ category và id lấy từ cbFoodCategory
                        {
                            index = i; //nếu bằng nhau thì gán index =  i (vị trí của nso trong cbFoodCategory)
                            break;
                        }
                        i++;
                    }

                    cbFoodCategory.SelectedIndex = index; //hiện kết quá lấy được thông qua index lên comboBox thông qua vị trí
                }
            }
            catch { }
        }

        //-----------------------------------------
        // Thêm
        //thêm món
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int idCa = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;

            if (FoodDAO.Instance.InsertFood(name, idCa, price))
            {
                MessageBox.Show("Thêm món thành công!!!");
                LoadListFood();
                if (insertFood != null)
                {
                    insertFood(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Có lỗi xẩy ra! Vui lòng thử lại");
            }
        }
        //thêm bàn
        private void btnAddTable_Click(object sender, EventArgs e)
        {
            string name = txbTableName.Text;
            string status = txbTableStatus.Text;


            if (TableDAO.Instance.InsertTable(name, status))
            {
                MessageBox.Show("Thêm bàn thành công!!!");
                LoadListTable();
                if (insertTable != null)
                {
                    insertTable(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Có lỗi xẩy ra! Vui lòng thử lại");
            }
        }
        //thêm loại
        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            string name = txbCategoryName.Text;
            if (CategoryDAO.Instance.InsertCategory(name))
            {
                MessageBox.Show("Thêm Loại thành công!!!");
                LoadListCategory();
                if (insertCategory != null)
                {
                    insertCategory(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Có lỗi xẩy ra! Vui lòng thử lại");
            }
        }

        //Thêm tài khoản
        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            string username = txbAccountUserName.Text;
            string displayname = txbAccountDisplayName.Text;
            int type = (int)nmAccountType.Value;
            if (AccountDAO.Instance.InsertAccount(username, displayname, type))
            {
                MessageBox.Show("Thêm Tài khoản thành công!!!");
                LoadListAccount();
                if (insertAccount != null)
                {
                    insertAccount(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Có lỗi xẩy ra! Vui lòng thử lại");
            }

        }
        //-------------------------------------------------------------------


        //Sửa
        private void btnEditFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int idCa = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;
            int id = Convert.ToInt32(txbFoodId.Text);

            if (FoodDAO.Instance.UpdateFood(id, name, idCa, price))
            {
                MessageBox.Show("Sửa món thành công!!!");
                LoadListFood();
                if (updateFood != null)
                {
                    updateFood(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Có lỗi xẩy ra! Vui lòng thử lại");
            }
        }



        private void btnEditTable_Click(object sender, EventArgs e)
        {
            string name = txbTableName.Text;
            int id = Convert.ToInt32(txbTableID.Text);
            string status = txbTableStatus.Text;
            if (TableDAO.Instance.UpdateTable(id, name, status))
            {
                MessageBox.Show("Sửa bàn thành công!!!");
                LoadListTable();
                if (updateTable != null)
                {
                    updateTable(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Có lỗi xẩy ra! Vui lòng thử lại");
            }
        }
        //sửa loại
        private void btnEditCategory_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbCategoryFoodID.Text);
            string name = txbCategoryName.Text;

            if (CategoryDAO.Instance.UpdateCategory(id, name))
            {
                MessageBox.Show("Sửa Loại thành công!!!");
                LoadListCategory();
                if (updateCategory != null)
                {
                    updateCategory(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Có lỗi xẩy ra! Vui lòng thử lại");
            }
        }

        //------
        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            string username = txbAccountUserName.Text;
            string displayname = txbAccountDisplayName.Text;
            int type = (int)nmAccountType.Value;

            if (AccountDAO.Instance.UpdateAccount(username, displayname, type))
            {
                MessageBox.Show("Sửa tài khoản thành công!!!");
                LoadListAccount();
                if (updateAccount != null)
                {
                    updateAccount(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Có lỗi xẩy ra! Vui lòng thử lại");
            }
        }
        //------------------------------------------
        //xóa
        //xóa món
        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbFoodId.Text);
            if (FoodDAO.Instance.DeleteFood(id))
            {
                MessageBox.Show("Xóa món thành công!!!");
                LoadListFood();
                if (deleteFood != null)
                {
                    deleteFood(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Có lỗi xẩy ra! Vui lòng thử lại");
            }
        }

        //xóa bàn
        private void btnDeleteTable_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbTableID.Text);
            if (TableDAO.Instance.DeleteTable(id))
            {
                MessageBox.Show("Xóa bàn thành công!!!");
                LoadListTable();
                if (deleteTable != null)
                {
                    deleteTable(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Có lỗi xẩy ra! Vui lòng thử lại");
            }
        }

        //xóa Loại
        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbCategoryFoodID.Text);
            if (CategoryDAO.Instance.DeleteCategory(id))
            {
                MessageBox.Show("Xóa Loại thành công!!!");
                LoadListCategory();
                if (deleteCategory != null)
                {
                    deleteCategory(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Có lỗi xẩy ra! Vui lòng thử lại");
            }
        }

        //xóa tìa khoản
        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            string username = txbAccountUserName.Text;

            if (loginAccount.UserName.Equals(username))
            {
                MessageBox.Show("Bạn không thể xóa chính mình!!!!");
                return;
            }

            if (AccountDAO.Instance.DeleteAccount(username))
            {
                MessageBox.Show("Xóa Tài khoản thành công!!!");
                LoadListAccount();
                if (deleteAccount != null)
                {
                    deleteAccount(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("Có lỗi xẩy ra! Vui lòng thử lại");
            }
        }

        //-------------------Reset Password

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            string name = txbAccountUserName.Text;

            if (AccountDAO.Instance.ResetPassword(name))
            {
                MessageBox.Show("cập nhật mật khẩu cho Tài khoản thành công!!!");
            }
            else
            {
                MessageBox.Show("Có lỗi xẩy ra! Vui lòng thử lại");
            }
        }
        //-------------------------------------------------
        //thêm món event
        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add
            {
                insertFood += value;
            }
            remove
            {
                insertFood -= value;
            }
        }
        private event EventHandler insertTable;
        public event EventHandler InsertTable
        {
            add
            {
                insertTable += value;
            }
            remove
            {
                insertTable -= value;
            }
        }

        private event EventHandler insertCategory;
        public event EventHandler InsertCategory
        {
            add { insertCategory += value; }
            remove { insertCategory -= value; }
        }

        private event EventHandler insertAccount;
        public event EventHandler InsertAccount
        {
            add { insertAccount += value; }
            remove { insertAccount -= value; }
        }
        //------------------------------------
        //event delete
        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add
            {
                deleteFood += value;
            }
            remove
            {
                deleteFood -= value;
            }
        }
        private event EventHandler deleteTable;
        public event EventHandler DeleteTable
        {
            add { deleteTable += value; }
            remove
            {
                deleteTable -= value;
            }
        }


        private event EventHandler deleteCategory;
        public event EventHandler DeleteCategory
        {
            add { deleteCategory += value; }
            remove
            {
                deleteCategory -= value;
            }
        }

        private event EventHandler deleteAccount;
        public event EventHandler DeleteAccount
        {
            add { deleteAccount += value; }
            remove { deleteAccount -= value; }
        }
        //-------------------------------------------------------
        //event 

        //sữa món 
        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add
            {
                updateFood += value;
            }
            remove
            {
                updateFood -= value;
            }
        }

        //sửa bàn
        private event EventHandler updateTable;
        public event EventHandler UpdateTable
        {
            add { updateTable += value; }
            remove
            {
                updateTable -= value;
            }
        }

        //sửa Loại
        private event EventHandler updateCategory;
        public event EventHandler UpdateCategory
        {
            add { updateCategory += value; }
            remove
            {
                updateCategory -= value;
            }
        }

        private event EventHandler updateAccount;
        public event EventHandler UpdateAccount
        {
            add { updateAccount += value; }
            remove { updateAccount -= value; }
        }
        //--------------------------
        //tìm kiếm thức ăn 1 cách gần đúng
        private void btnSearchFood_Click(object sender, EventArgs e)
        {
            foodList.DataSource = SearchFoodByName(txbSearchFoodName.Text);
        }


        //--------------------------------------
        //XEM
        private void btnShowTable_Click(object sender, EventArgs e)
        {
            LoadListTable();
        }
        private void btnShowCategory_Click(object sender, EventArgs e)
        {
            LoadListCategory();
        }
        private void btnShowFood_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }

        private void btnShowAccount_Click(object sender, EventArgs e)
        {
            LoadListAccount();
        }

        //-------------Page Bill
        //trang đầu
        private void btnFirstBillPage_Click(object sender, EventArgs e)
        {
            txbNumPage.Text = "1";
        }
        //trang cuối
        private void btnLastBillPage_Click(object sender, EventArgs e)
        {
            int sumRecord = BillDAO.Instance.GetNumBillByDate(dtpkFromDate.Value, dtpkToDate.Value);

            int lastPage = sumRecord / 10;
            if (sumRecord % 10 != 0)
            {
                lastPage++;

                txbNumPage.Text = lastPage.ToString();
            }

        }
        //giá trị của page
        private void txbNumPage_TextChanged(object sender, EventArgs e)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetListBillByDateAndPage(dtpkFromDate.Value, dtpkToDate.Value, Convert.ToInt32(txbNumPage.Text));
        }
        //next và back
        private void btnPrevBillPage_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txbNumPage.Text);

            if (page > 1)
                page--;
            txbNumPage.Text = page.ToString();
        }

        private void btnNextBillPage_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txbNumPage.Text);

            int sumRecord = BillDAO.Instance.GetNumBillByDate(dtpkFromDate.Value, dtpkToDate.Value);

            if (page < sumRecord)
                page++;
            txbNumPage.Text = page.ToString();
        }



        #endregion


    }
}
