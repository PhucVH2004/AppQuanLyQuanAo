using BUS;
using DTO;
using QLQA;
using GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class FrmThemSP : Form
    {
        private SanPhamBUS sanPhamBUS = new SanPhamBUS();
        private LoaiSanPhamBUS loaiSanPhamBUS = new LoaiSanPhamBUS();
        private FrmMain frmMain; // Tham chiếu đến form chính để cập nhật DataGridView
        private bool maSPTonTai = false; // Biến để theo dõi trạng thái tồn tại của MaSP

        public FrmThemSP(FrmMain mainForm)
        {
            InitializeComponent();
            frmMain = mainForm;
            this.txtMaSP.TextChanged += new System.EventHandler(this.txtMaSP_TextChanged);
            LoadLoaiSanPham(); // Load categories when form initializes
        }

        private void FrmThemSP_Load(object sender, EventArgs e)
        {
            // Mặc định các control hoạt động khi form tải
            txtTenSP.Enabled = true;
            txtDonGia.Enabled = true;
            btnThemAnh.Enabled = true;
            pictureBoxAnh.Enabled = true;
            txtSL.Text = ""; // Đảm bảo txtSL trống khi tải form
        }

        private void LoadLoaiSanPham()
        {
            try
            {
                cbLoai.Items.Clear();
                var danhSachLoai = loaiSanPhamBUS.LayTatCaLoaiSanPham();
                foreach (var loai in danhSachLoai)
                {
                    cbLoai.Items.Add(loai.TenLoai);
                }
                if (cbLoai.Items.Count > 0)
                {
                    cbLoai.SelectedIndex = 0; // Chọn loại đầu tiên mặc định
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách loại sản phẩm: {ex.Message}");
            }
        }

        private void txtMaSP_TextChanged(object sender, EventArgs e)
        {
            string maSP = txtMaSP.Text.Trim();
            if (string.IsNullOrEmpty(maSP))
            {
                txtTenSP.Text = "";
                txtDonGia.Text = "";
                txtSL.Text = "";
                pictureBoxAnh.Image = null;
                txtTenSP.Enabled = true;
                txtDonGia.Enabled = true;
                btnThemAnh.Enabled = true;
                pictureBoxAnh.Enabled = true;
                cbLoai.Enabled = true;
                txtLoai.Visible = true; // Luôn hiển thị txtLoai
                txtLoai.Text = cbLoai.SelectedItem?.ToString() ?? "";
                maSPTonTai = false;
                return;
            }

            try
            {
                SanPhamDTO sanPhamTonTai = sanPhamBUS.LayTatCaSanPham().Find(sp => sp.MaSP == maSP);
                if (sanPhamTonTai != null)
                {
                    maSPTonTai = true;
                    txtTenSP.Text = sanPhamTonTai.TenSP;
                    txtDonGia.Text = sanPhamTonTai.DonGia.ToString("N0");
                    txtSL.Text = "";
                    if (sanPhamTonTai.HinhAnh != null && sanPhamTonTai.HinhAnh.Length > 0)
                    {
                        using (var ms = new MemoryStream(sanPhamTonTai.HinhAnh))
                        {
                            pictureBoxAnh.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        pictureBoxAnh.Image = null;
                    }
                    pictureBoxAnh.SizeMode = PictureBoxSizeMode.StretchImage;

                    // Display category
                    var loai = loaiSanPhamBUS.LayTatCaLoaiSanPham().Find(l => l.MaLoai == sanPhamTonTai.MaLoai);
                    cbLoai.SelectedItem = loai?.TenLoai;
                    txtLoai.Text = loai?.TenLoai;
                    txtLoai.Visible = true; // Luôn hiển thị txtLoai
                    cbLoai.Enabled = false; // Disable category selection for existing products

                    txtTenSP.Enabled = false;
                    txtDonGia.Enabled = false;
                    btnThemAnh.Enabled = false;
                    pictureBoxAnh.Enabled = false;
                }
                else
                {
                    maSPTonTai = false;
                    txtTenSP.Text = "";
                    txtDonGia.Text = "";
                    txtSL.Text = "";
                    pictureBoxAnh.Image = null;
                    txtTenSP.Enabled = true;
                    txtDonGia.Enabled = true;
                    btnThemAnh.Enabled = true;
                    pictureBoxAnh.Enabled = true;
                    cbLoai.Enabled = true;
                    txtLoai.Visible = true; // Luôn hiển thị txtLoai
                    txtLoai.Text = cbLoai.SelectedItem?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kiểm tra mã sản phẩm: {ex.Message}");
            }
        }

        private void btnThemAnh_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Chọn ảnh sản phẩm"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Hiển thị ảnh trong PictureBox
                pictureBoxAnh.Image = Image.FromFile(openFileDialog.FileName);
                pictureBoxAnh.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                string maSP = txtMaSP.Text.Trim();
                if (string.IsNullOrEmpty(maSP))
                {
                    MessageBox.Show("Mã sản phẩm không được để trống!");
                    return;
                }

                if (!int.TryParse(txtSL.Text.Trim(), out int soLuong) || soLuong < 0)
                {
                    MessageBox.Show("Số lượng phải là số nguyên dương!");
                    return;
                }

                string tenLoai = txtLoai.Text.Trim();
                if (string.IsNullOrEmpty(tenLoai))
                {
                    MessageBox.Show("Vui lòng nhập loại sản phẩm!");
                    return;
                }

                
                

                var loai = loaiSanPhamBUS.LayLoaiSanPhamTheoTen(tenLoai);
                if (loai == null)
                {
                    loaiSanPhamBUS.ThemLoaiSanPham(tenLoai);
                    loai = loaiSanPhamBUS.LayLoaiSanPhamTheoTen(tenLoai);

                    // Cập nhật cbLoai trong form hiện tại
                    cbLoai.Items.Add(tenLoai);
                    cbLoai.SelectedItem = tenLoai;

                    // Đồng bộ với FrmMain (và các form khác nếu cần)
                    if (frmMain != null)
                    {
                        frmMain.CapNhatDanhSachLoaiSanPham();
                    }
                }

                if (maSPTonTai)
                {
                    SanPhamDTO sanPhamTonTai = sanPhamBUS.LayTatCaSanPham().Find(sp => sp.MaSP == maSP);
                    if (sanPhamTonTai != null)
                    {
                        SanPhamDTO sanPhamCapNhat = new SanPhamDTO
                        {
                            MaSP = sanPhamTonTai.MaSP,
                            TenSP = sanPhamTonTai.TenSP,
                            DonGia = sanPhamTonTai.DonGia,
                            SoLuong = sanPhamTonTai.SoLuong + soLuong,
                            HinhAnh = sanPhamTonTai.HinhAnh,
                            MaLoai = sanPhamTonTai.MaLoai
                        };
                        sanPhamBUS.CapNhatSanPham(sanPhamCapNhat);
                        MessageBox.Show($"Đã cập nhật số lượng sản phẩm {sanPhamTonTai.TenSP} lên {sanPhamCapNhat.SoLuong}.");
                    }
                    else
                    {
                        MessageBox.Show("Lỗi: Không tìm thấy sản phẩm để cập nhật!");
                        return;
                    }
                }
                else
                {
                    string tenSP = txtTenSP.Text.Trim();
                    if (string.IsNullOrEmpty(tenSP))
                    {
                        MessageBox.Show("Tên sản phẩm không được để trống!");
                        return;
                    }

                    if (!decimal.TryParse(txtDonGia.Text.Trim(), out decimal donGia) || donGia < 0)
                    {
                        MessageBox.Show("Đơn giá phải là số dương!");
                        return;
                    }

                    byte[] hinhAnh = null;
                    if (pictureBoxAnh.Image != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            pictureBoxAnh.Image.Save(ms, pictureBoxAnh.Image.RawFormat);
                            hinhAnh = ms.ToArray();
                        }
                    }

                    SanPhamDTO sanPhamMoi = new SanPhamDTO
                    {
                        MaSP = maSP,
                        TenSP = tenSP,
                        DonGia = donGia,
                        SoLuong = soLuong,
                        HinhAnh = hinhAnh,
                        MaLoai = loai.MaLoai
                    };
                    sanPhamBUS.ThemSanPham(sanPhamMoi);
                    MessageBox.Show($"Đã thêm sản phẩm {tenSP} thành công.");
                }

                frmMain.TaiDanhSachSanPham();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm hoặc cập nhật sản phẩm: {ex.Message}");
            }
            finally
            {
                if (frmMain != null)
                {
                    if (!frmMain.Visible)
                    {
                        frmMain.Show();
                    }
                    this.Close();
                }
            }
        }


        private void btnReset_Click(object sender, EventArgs e)
        {
            // Xóa tất cả thông tin và bật lại các control
            txtMaSP.Text = "";
            txtTenSP.Text = "";
            txtDonGia.Text = "";
            txtSL.Text = "";
            pictureBoxAnh.Image = null;
            txtTenSP.Enabled = true;
            txtDonGia.Enabled = true;
            btnThemAnh.Enabled = true;
            pictureBoxAnh.Enabled = true;
            maSPTonTai = false; // Đặt lại trạng thái
        }

        private void btnTrangChu_Click(object sender, EventArgs e)
        {
            // Nút "Trở về trang chủ"
            if (frmMain != null)
            {
                frmMain.TaiDanhSachSanPham(); // Cập nhật danh sách trước khi hiển thị
                frmMain.Show();
            }
            this.Close();

        }

        private void cbLoai_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Luôn hiển thị txtLoai để người dùng có thể nhập loại mới nếu cần
            txtLoai.Visible = true;
            txtLoai.Text = cbLoai.SelectedItem?.ToString() ?? "";
        }
    }
}