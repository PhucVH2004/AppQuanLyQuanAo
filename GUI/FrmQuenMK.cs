using BUS;
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
    public partial class FrmQuenMK : Form
    {
        private TaiKhoanBUS taiKhoanBUS;

        public FrmQuenMK()
        {
            InitializeComponent();
            taiKhoanBUS = new TaiKhoanBUS();
            // Đặt mặc định PasswordChar là '*' cho các trường mật khẩu
            txtMK.PasswordChar = '*';
            txtXNMK.PasswordChar = '*';
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            frmDangNhap frmDangNhap = new frmDangNhap();
            this.Hide();
            frmDangNhap.ShowDialog();
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy dữ liệu từ các trường nhập
                string tenDangNhap = txtTenDN.Text.Trim();
                string email = txtEmail.Text.Trim();
                string matKhauMoi = txtMK.Text.Trim();
                string xacNhanMatKhau = txtXNMK.Text.Trim();

                // Kiểm tra đầu vào
                if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(email) ||
                    string.IsNullOrEmpty(matKhauMoi) || string.IsNullOrEmpty(xacNhanMatKhau))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra mật khẩu mới và xác nhận mật khẩu có khớp không
                if (matKhauMoi != xacNhanMatKhau)
                {
                    MessageBox.Show("Mật khẩu mới và xác nhận mật khẩu không khớp.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra độ dài mật khẩu (tối thiểu 6 ký tự)
                if (matKhauMoi.Length < 6)
                {
                    MessageBox.Show("Mật khẩu mới phải có ít nhất 6 ký tự.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra tài khoản có tồn tại không
                bool taiKhoanHopLe = taiKhoanBUS.KiemTraTaiKhoan(tenDangNhap, email);
                Console.WriteLine($"KiemTraTaiKhoan: TenDangNhap={tenDangNhap}, Email={email}, Result={taiKhoanHopLe}");
                if (!taiKhoanHopLe)
                {
                    MessageBox.Show("Tên đăng nhập hoặc email không đúng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Cập nhật mật khẩu mới
                bool capNhatThanhCong = taiKhoanBUS.CapNhatMatKhau(tenDangNhap, matKhauMoi);
                Console.WriteLine($"CapNhatMatKhau: TenDangNhap={tenDangNhap}, Result={capNhatThanhCong}");
                if (capNhatThanhCong)
                {
                    MessageBox.Show("Đặt lại mật khẩu thành công! Vui lòng đăng nhập lại.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frmDangNhap frmDangNhap = new frmDangNhap();
                    this.Hide();
                    frmDangNhap.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Đã có lỗi xảy ra. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đặt lại mật khẩu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkMK_CheckedChanged(object sender, EventArgs e)
        {
            // Hiển thị/ẩn mật khẩu
            if (checkMK.Checked)
            {
                txtMK.PasswordChar = '\0';
                txtXNMK.PasswordChar = '\0';
            }
            else
            {
                txtMK.PasswordChar = '*';
                txtXNMK.PasswordChar = '*';
            }
        }
    }
}