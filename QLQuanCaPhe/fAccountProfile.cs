using QLQuanCaPhe.DAO;
using QLQuanCaPhe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLQuanCaPhe
{
    public partial class fAccountProfile : Form
    {
        private Account loginAccount; //tạo loginAccout với kiểu là Account

        public Account LoginAccount //định nghĩa
        {
            get { return loginAccount; }
            set { loginAccount = value; ChangeAccount(LoginAccount); }
        }
        public fAccountProfile(Account acc)
        {
            InitializeComponent();

            LoginAccount = acc;
        }
        //Hiển thị tên đăng nhập và tên hiển thị vào trong TextBox
        void ChangeAccount(Account acc)
        {
            txbUserName.Text = LoginAccount.UserName;
            txbDisplayName.Text = LoginAccount.DisplayName;

        }
        //cập nhật thông tin account
        void UpdateAccountInfo()
        {
            string displayName = txbDisplayName.Text;
            string password = txbPassword.Text;
            string newPass = txbNewPassword.Text;
            string reEnterPass = txbReEnterPassword.Text;
            string userName = txbUserName.Text;

            if (!newPass.Equals(reEnterPass))
            {
                MessageBox.Show("Mật khẩu nhập lại và mật khẩu mới không trùng khớp");
            }
            else
            {
                if (AccountDAO.Instance.UpdateAcount(userName, displayName, password, newPass))
                {
                    MessageBox.Show("Cập nhật thông tin thành công");
                    if(updateAcount != null)
                    {
                        updateAcount(this, new AccountEvent(AccountDAO.Instance.GetAccountByUserName(userName)) );
                    }
                }else
                {
                    MessageBox.Show("Không thành công!  Vui lòng thực hiện lại!");
                }
            }
        }
        //tạo event để chuyển thông tin từ thằng con Profile sang thằng cha Management
        private event EventHandler<AccountEvent> updateAcount;
        public event EventHandler<AccountEvent> UpdateAcount
        {
            add { updateAcount += value; }
            remove { updateAcount -= value; }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateAccountInfo();
        }
    }
    public class AccountEvent : EventArgs
    {
        private Account acc;

        public Account Acc
        { 
            get { return acc; }
            set { acc = value; }
        }
        public AccountEvent(Account acc)
        {
            this.Acc = acc;
        }
    }
}
