using BUS;
using QLQA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class frmDangNhap : Form
    {
        public frmDangNhap()
        {
            InitializeComponent();
        }

        private void DangNhap_Load(object sender, EventArgs e)
        {
            // Đặt dấu sao (*) cho TextBox mật khẩu
            txtMatKhau.PasswordChar = '*'; // Dấu sao khi nhập mật khẩu
        }

        private void linkLabelDangKy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmDangKy dangky = new frmDangKy();
            dangky.Show();
            this.Hide();
        }

        private void chkHienThiMatKhau_CheckedChanged(object sender, EventArgs e)
        {
            // Nếu Checkbox được tick, hiển thị mật khẩu
            if (chkHienThiMatKhau.Checked)
            {
                txtMatKhau.PasswordChar = '\0';
            }
            else
            {
                txtMatKhau.PasswordChar = '*';
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            TaiKhoanBUS bus = new TaiKhoanBUS();
            var tk = bus.DangNhap(txtTenDN.Text, txtMatKhau.Text);

            if (tk != null)
            {
                FrmMain main = new FrmMain(tk.VaiTro);
                this.Hide();
                main.ShowDialog();
                

            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu sai");
            }
        }

        private void lblQuenMatKhau_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmQuenMK frmQuenMK = new FrmQuenMK();
            this.Hide();
            frmQuenMK.ShowDialog();
        }
    }
}

