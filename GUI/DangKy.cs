using System.Linq;
using System;
using System.Windows.Forms;
using BUS;
using DTO;

namespace GUI
{
    public partial class frmDangKy : Form
    {
        private TaiKhoanBUS taiKhoanBUS;

        public frmDangKy()
        {
            InitializeComponent();
            taiKhoanBUS = new TaiKhoanBUS();
        }

        private void btndangky_Click(object sender, EventArgs e)
        {
            // Kiểm tra Email
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Bạn quên nhập Email!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Clear();
                txtEmail.Focus();
                return;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                MessageBox.Show("Bạn nhập Email sai, vui lòng nhập lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Clear();
                txtEmail.Focus();
                return;
            }

            // Kiểm tra Số điện thoại (chỉ số và đủ 10 chữ số)
            if (string.IsNullOrWhiteSpace(txtSDT.Text))
            {
                MessageBox.Show("Bạn quên nhập Số điện thoại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDT.Clear();
                txtSDT.Focus();
                return;
            }
            else if (!txtSDT.Text.All(char.IsDigit) || txtSDT.Text.Length != 10)
            {
                MessageBox.Show("Số điện thoại không hợp lệ. Phải là 10 chữ số.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDT.Clear();
                txtSDT.Focus();
                return;
            }

            // Kiểm tra Tên đăng nhập (ít nhất 5 ký tự, không chứa ký tự đặc biệt)
            if (string.IsNullOrWhiteSpace(txtTenDN.Text))
            {
                MessageBox.Show("Bạn quên nhập Tên đăng nhập!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenDN.Clear();
                txtTenDN.Focus();
                return;
            }
            else if (txtTenDN.Text.Length < 5 || !System.Text.RegularExpressions.Regex.IsMatch(txtTenDN.Text, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("Tên đăng nhập không hợp lệ. Ít nhất 5 ký tự và không chứa ký tự đặc biệt.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenDN.Clear();
                txtTenDN.Focus();
                return;
            }

            // Kiểm tra Mật khẩu (phải có chữ hoa, chữ thường và số)
            if (string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Bạn quên nhập Mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhau.Clear();
                txtMatKhau.Focus();
                return;
            }
            else if (!txtMatKhau.Text.Any(char.IsUpper) || !txtMatKhau.Text.Any(char.IsLower) || !txtMatKhau.Text.Any(char.IsDigit))
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 1 chữ hoa, 1 chữ thường và 1 số.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhau.Clear();
                txtMatKhau.Focus();
                return;
            }

            // Kiểm tra RadioButton giới tính
            if (!rdoNam.Checked && !rdoNu.Checked)
            {
                MessageBox.Show("Bạn chưa chọn giới tính!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra Ngày sinh (tuổi >= 15)
            if (dateTimePicker.Value > DateTime.Now.AddYears(-15))
            {
                MessageBox.Show("Bạn phải ít nhất 15 tuổi để đăng ký.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dateTimePicker.Value = DateTime.Now;
                dateTimePicker.Focus();
                return;
            }

            // Tạo đối tượng TaiKhoanDTO
            TaiKhoanDTO taiKhoan = new TaiKhoanDTO
            {
                Email = txtEmail.Text,
                SDT = txtSDT.Text,
                TenDangNhap = txtTenDN.Text,
                MatKhau = txtMatKhau.Text,
                GioiTinh = rdoNam.Checked ? "Nam" : "Nữ",
                NgaySinh = dateTimePicker.Value,
                VaiTro = "user"
            };

            // Kiểm tra trùng TenDangNhap hoặc Email
            bool isDuplicate = taiKhoanBUS.KiemTraTrungTaiKhoan(taiKhoan.TenDangNhap, taiKhoan.Email);
            Console.WriteLine($"frmDangKy: IsDuplicate={isDuplicate}, Email={taiKhoan.Email}, TenDangNhap={taiKhoan.TenDangNhap}");
            if (isDuplicate)
            {
                MessageBox.Show("Tên đăng nhập hoặc Email đã được sử dụng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTenDN.Clear();
                txtEmail.Clear();
                txtTenDN.Focus();
                return;
            }

            // Lưu vào CSDL
            try
            {
                bool dangKyThanhCong = taiKhoanBUS.DangKy(taiKhoan);
                Console.WriteLine($"frmDangKy: DangKyThanhCong={dangKyThanhCong}");
                if (dangKyThanhCong)
                {
                    MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frmDangNhap dangNhap = new frmDangNhap();
                    this.Hide();
                    dangNhap.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Đăng ký thất bại. Vui lòng thử lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng ký: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnthoat_Click(object sender, EventArgs e)
        {
            frmDangNhap dangnhap = new frmDangNhap();
            dangnhap.Show();
            this.Hide();
        }

        private void frmDangKy_Load(object sender, EventArgs e)
        {
        }
    }
}