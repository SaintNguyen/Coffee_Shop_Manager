using QLQuanCaPhe.DAO;
using QLQuanCaPhe.DTO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace QLQuanCaPhe
{
    public partial class fTableManager : Form
    {
        private Account loginAccount; //tạo loginAccout với kiểu là Account

        public Account LoginAccount //định nghĩa
        {
            get { return loginAccount; }
            set { loginAccount = value; ChangeAccount(loginAccount.Type); }
        }

        public fTableManager(Account acc) //truyền account vào TableManager
        {
            InitializeComponent();
            this.LoginAccount = acc;

            LoadAll();

        }

        #region Method
        //
        void LoadAll()
        {
            LoadTable();
            LoadCategory();
            LoadComboBoxTable(cbSwitchTable);
        }
        void ChangeAccount(int type) //tạo 1 hàm dùng để kiểm tra xem tài khoản là admin hay nhân viên
                                     //nếu tài khoản là admin thì type là 1 , ngược lại là nhân viện type là 0
                                     //type mặc định là 0
        {
            adminToolStripMenuItem.Enabled = type == 1;
            thôngTinTàiKhoảnToolStripMenuItem.Text += " (" + LoginAccount.DisplayName + ")";

        }


        //tạo hàm load category của food lên từ SQL

        void LoadCategory()
        {
            //tạo 1 cái list tên là listCategory rồi lưu mấy cái giá trị trong cái CategoryDAO zô cái list
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();

            //load nó zô cái comboBox 
            cbCategory.DataSource = listCategory;
            //chỉ cho  ComboBox pít là mình mún hiển thị cái trường (Field) name , tức là tên của Category
            cbCategory.DisplayMember = "name";
        }
        //tạo hàm load tên thức ăn lên theo biến ID của Category trong SQL
        void LoadFoodListByCategoryID(int id)
        {
            //lấy danh sách Food theo id của Category OK ?
            List<Food> listFood = FoodDAO.Instance.GetListFoodByCategoryID(id);
            //như cái trên cái này củm load nó zô cái comboBox
            cbFood.DataSource = listFood;
            //chỉ cho  ComboBox pít là mình mún hiển thị cái trường (Field) name , tức là tên của Food
            cbFood.DisplayMember = "name";

        }
        //hàm load table lên từ danh sách table trong SQL
        void LoadTable()
        {
            flpTable.Controls.Clear();
            //thực hiện lấy danh sách table
            List<Table> tableList = TableDAO.Instance.LoadTableList();

            foreach (Table item in tableList)  //Với mỗi table nằm bên trong tableList ta sẽ tiến hành tạo 1 button tương ứng
            {
                Button btn = new Button() { Width = TableDAO.tableWidth, Height = TableDAO.tableHeight };
                btn.Text = item.Name + Environment.NewLine + item.Status; //Environment.NewLine = /n (xuống đòng)
                //tạo click cho btn
                btn.Click += btn_Click;
                //gắn id cho btn
                btn.Tag = item;


                switch (item.Status)
                {
                    case "Bàn trống":
                        btn.BackColor = Color.MediumPurple;
                        break;
                    default:
                        btn.BackColor = Color.DeepPink;
                        break;
                }

                flpTable.Controls.Add(btn);
            }
        }

        void showBill(int id)
        {
            lsvBill.Items.Clear();
            //tạo list Bill Info bằng cách từ DAO của BillInfo dùng hàm GetListBillInfo từ đó trỏ tới DAO của Bill lấy hàm GetUnCheckBillIDByTableID
            //GetUnCheckBillIDByTableID tức là lấy cái billID dựa vào cái ID của cái bàn (Table)
            //quá là mệt mỏi 
            //tạo biến để lưu giá trị của tổng thành tiền
            float totalPrice = 0;
            List<QLQuanCaPhe.DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);


            foreach (QLQuanCaPhe.DTO.Menu item in listBillInfo)
            {
                //tạo List View sẽ hiển thị 2 thông số là ID Food và Count
                //Trong đó thì Count là SubItem của ID food


                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                //tính giá trị của totalPrice bằng việc cộng dồn thành tiền của item
                totalPrice += item.TotalPrice;

                lsvBill.Items.Add(lsvItem);
            }
            //Điều chỉnh format 
            CultureInfo culture = new CultureInfo("vi-VN");

            //Thread.CurrentThread.CurrentCulture = culture;
            //hiển thị giá trị của totalPrice vòa text box tên là txbTotalPrice
            txbTotalPrice.Text = totalPrice.ToString("c", culture);
            //mỗi lần showBill dc thực hiện thì củm load lại table

        }
        //Hàm này dùng để load danh sách table
        void LoadComboBoxTable(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableList(); //lấy danh sách table từ LoadTableList trong TableDAO
            cb.DisplayMember = "Name"; //Hiện tên của mấy cái bàn đóa ra OK?
        }
        #endregion

        #region Event

        //Phím tắt
        private void thanhToánToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnCheckOut_Click(this, new EventArgs());
        }

        private void thêmMónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnAdd_Click(this, new EventArgs());
        }

        //sự kiện click khi người dùng click vào 1 bàn
        private void btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            //lưu lại cái table để sử dựng ở hàm thêm món
            lsvBill.Tag = (sender as Button).Tag;

            showBill(tableID);

        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAccountProfile f = new fAccountProfile(LoginAccount);
            f.UpdateAcount += f_UpdateAccount;
            f.ShowDialog();
        }

        private void f_UpdateAccount(object sender, AccountEvent e)
        {
            thôngTinTàiKhoảnToolStripMenuItem.Text = "Thông tin tài khoản (" + e.Acc.DisplayName + ")";
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin f = new fAdmin();
            f.loginAccount = LoginAccount;

            f.InsertFood += f_InsertFood;
            f.DeleteFood += f_DeleteFood;
            f.UpdateFood += f_UpdateFood;

            f.InsertTable += f_InsertTable;
            f.UpdateTable += f_UpdateTable;
            f.DeleteTable += f_DeleteTable;

            f.InsertCategory += f_InsertCategory;
            f.UpdateCategory += f_UpdateCategory;
            f.DeleteCategory += f_DeleteCategory;
            f.ShowDialog();
        }

        #region RenderData
        private void f_DeleteCategory(object sender, EventArgs e)
        {
            LoadCategory();
        }

        private void f_UpdateCategory(object sender, EventArgs e)
        {
            LoadCategory();
        }

        private void f_InsertCategory(object sender, EventArgs e)
        {
            LoadCategory();
        }

        private void f_DeleteTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void f_UpdateTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void f_InsertTable(object sender, EventArgs e)
        {
            LoadTable();
        }
        #endregion
        //khi thực hiện thao tác sủa món ăn sự kiện này sẽ đucợ gọi đến và tiến hành load loại bill + bill info củm như status của bàn 

        private void f_UpdateFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
            {
                showBill((lsvBill.Tag as Table).ID);
            }
        }
        //khi thực hiện thao tác xóa món ăn thì sự kiên jnayf sẽ đucợ gọi để nhắm load lại dữ liệu
        private void f_DeleteFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            showBill((lsvBill.Tag as Table).ID);
            LoadTable();
        }
        //khi thực hiện thao tác thêm món mới dữ liệu về món ăn sẽ được cập nhật lại
        private void f_InsertFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
            {
                showBill((lsvBill.Tag as Table).ID);
            }
        }

        private void fTableManager_Load(object sender, EventArgs e)
        {

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;

            ComboBox cb = sender as ComboBox;

            //nôm na thì nếu mà trong cái DataSource của cái ComboBox nó ch có j thì nó return luôn chứ ko chạy tiếp nữa 
            if (cb.SelectedItem == null) return;
            Category selected = cb.SelectedItem as Category;
            //gán cái id được tạo ở trên = cái ID của Category
            id = selected.ID;
            LoadFoodListByCategoryID(id);
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;

            if (table == null)
            {
                MessageBox.Show("Hãy chọn bàn để thêm món"); return;
            }

            //lấy id Bill
            int idBill = BillDAO.Instance.GetUnCheckBillIDByTableID(table.ID);
            //lấy id food
            int foodID = (cbFood.SelectedItem as Food).ID;
            //số lượng food
            int count = (int)nmFoodCount.Value;

            //tiến hành kiểm tra  1 số điều kiện để tạo bill blah blah

            //trường hợp 1 => khi table chưa tồn tại bill
            if (idBill == -1) //ở đây tại sao lại là trừ 1 ?? vì ... coi cái hàm GetBillDByTableID  ở class BillDAO trong thư mục DAO
            {
                BillDAO.Instance.insertBill(table.ID);
                BillInfoDAO.Instance.insertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
            }
            else //trường hợp 2 => khi mà table dã có bill rồi
            {
                //trong trường hợp này sẽ cóa thêm 2 trường hợp nhỏ khác
                BillInfoDAO.Instance.insertBillInfo(idBill, foodID, count);
            }
            showBill(table.ID); //ko cần để ý dòng này nó dùng đẻ load lại cái table thôi chứ hông j âu
            LoadTable();
        }
        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            //lấy ra id của bill chưa thành toán tức là status = 0
            int idBill = BillDAO.Instance.GetUnCheckBillIDByTableID(table.ID);
            int discount = (int)nmDiscount.Value;

            double totalPrice = Convert.ToDouble(txbTotalPrice.Text.Split(',')[0]);
            double finalTotalPrice = totalPrice - (totalPrice / 100) * discount;
            if (idBill != -1)
            {
                //khi mà bấm thanh toán sẽ hiện ra của sổ thông báo và khách hàng bấm vào OK thì thực hiện
                if (MessageBox.Show(string.Format("Bạn có muốn thanh toán hóa đơn cho bàn {0}\n Tạm thu : {1} VNĐ \n Giảm giá :{2}% \n Thành tiền: {3} VNĐ", table.Name, totalPrice, discount, finalTotalPrice), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    BillDAO.Instance.checkOut(idBill, discount, (float)finalTotalPrice);
                    showBill(table.ID);
                    LoadTable();
                }
            }
        }
        //sự kiện khi nhấn vào button chuyển bàn nà 
        private void btnSwitchTable_Click(object sender, EventArgs e)
        {

            int id1 = (lsvBill.Tag as Table).ID; //id1 là id của cái bàn hiện tại
            int id2 = (cbSwitchTable.SelectedItem as Table).ID; //id2 là id của cái bàn ta sẽ chuyển tói và lấy dữ liệu từ comboBox

            if (MessageBox.Show(string.Format("Bạn có muốn chuyển bàn {0} sang bàn {1} không?", (lsvBill.Tag as Table).Name, (cbSwitchTable.SelectedItem as Table).Name), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                //gọi tới hàm chuyển bàn trong TableDAO
                //TableDAO được thiết kế theo cấu trúc SingleTon nên sẽ gọi thông qua Instance nên lệnh nó như này
                //lâu lâu nhắc lại thui
                TableDAO.Instance.SwitchTable(id1, id2);

                LoadTable();
            }
        }
        #endregion


    }
}
