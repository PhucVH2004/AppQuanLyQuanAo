using BUS;
using DTO;
using GUI;
using QLQA;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class FrmKho : Form
    {
        private FrmMain frmMain;
        private SanPhamBUS sanPhamBUS = new SanPhamBUS();
        private LoaiSanPhamBUS loaiSanPhamBUS = new LoaiSanPhamBUS();
        private bool isDeleting = false;
        private bool isProcessingClick = false;
        private DateTime lastClickTime = DateTime.MinValue;
        private readonly TimeSpan debounceTime = TimeSpan.FromMilliseconds(500);

        public FrmKho(FrmMain frmMain)
        {
            InitializeComponent();
            this.frmMain = frmMain;
            CaiDatDataGridView();
            LoadLoaiSanPham();
            //TaiDanhSachSanPham();
            TaiDanhSachSanPhamAsync();
            
        }

       

        private void CaiDatDataGridView()
        {
            dataGridViewKho.AutoGenerateColumns = false;
            dataGridViewKho.RowTemplate.Height = 80;
            dataGridViewKho.AllowUserToAddRows = false;

            dataGridViewKho.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã SP", Name = "MaSP", Width = 100 });
            dataGridViewKho.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Loại", Name = "TenLoai", Width = 120 }); // Add Loai column
            dataGridViewKho.Columns.Add(new DataGridViewImageColumn { HeaderText = "Ảnh", Name = "Anh", ImageLayout = DataGridViewImageCellLayout.Zoom, Width = 100 });
            dataGridViewKho.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên SP", Name = "TenSP", Width = 210 });
            dataGridViewKho.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Giá", Name = "DonGia", Width = 100 });
            dataGridViewKho.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số Lượng", Name = "SoLuong", Width = 80 });
            dataGridViewKho.Columns.Add(new DataGridViewButtonColumn { HeaderText = "Xóa", Name = "Xoa", Text = "Xóa", UseColumnTextForButtonValue = true, Width = 60 });

            dataGridViewKho.BorderStyle = BorderStyle.None;
            dataGridViewKho.BackgroundColor = Color.White;
            dataGridViewKho.DefaultCellStyle.BackColor = Color.White;
            dataGridViewKho.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewKho.DefaultCellStyle.Font = new Font("Arial", 12);
            dataGridViewKho.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewKho.GridColor = Color.LightGray;

            dataGridViewKho.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridViewKho.CellDoubleClick -= new DataGridViewCellEventHandler(dataGridViewKho_CellDoubleClick);
            dataGridViewKho.CellDoubleClick += new DataGridViewCellEventHandler(dataGridViewKho_CellDoubleClick);
            dataGridViewKho.CellEndEdit -= new DataGridViewCellEventHandler(dataGridViewKho_CellEndEdit);
            dataGridViewKho.CellEndEdit += new DataGridViewCellEventHandler(dataGridViewKho_CellEndEdit);
            dataGridViewKho.CellContentClick -= new DataGridViewCellEventHandler(dataGridViewKho_CellContentClick);
            dataGridViewKho.CellContentClick += new DataGridViewCellEventHandler(dataGridViewKho_CellContentClick);
        }

        public void LoadLoaiSanPham()
        {
            try
            {
                cbLoai.Items.Clear();
                cbLoai.Items.Add("Tất cả");
                var danhSachLoai = loaiSanPhamBUS.LayTatCaLoaiSanPham();
                foreach (var loai in danhSachLoai)
                {
                    cbLoai.Items.Add(loai.TenLoai);
                }
                cbLoai.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách loại sản phẩm: {ex.Message}");
            }
        }

        public async Task TaiDanhSachSanPhamAsync()
        {
            try
            {
                dataGridViewKho.Rows.Clear();
                var danhSach = await Task.Run(() => sanPhamBUS.LayTatCaSanPham());
                var danhSachLoai = loaiSanPhamBUS.LayTatCaLoaiSanPham();
                if (danhSach == null || danhSach.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu sản phẩm trong cơ sở dữ liệu!");
                    return;
                }

                string selectedLoai = cbLoai.SelectedItem?.ToString();
                foreach (var sanPham in danhSach)
                {
                    // Kiểm tra xem MaSP đã tồn tại trong dataGridViewKho chưa
                    bool daTonTai = false;
                    foreach (DataGridViewRow row in dataGridViewKho.Rows)
                    {
                        if (row.Cells["MaSP"].Value?.ToString() == sanPham.MaSP)
                        {
                            daTonTai = true;
                            break;
                        }
                    }
                    if (daTonTai) continue;

                    // Lọc theo loại
                    if (selectedLoai != "Tất cả")
                    {
                        var loai = danhSachLoai.Find(l => l.TenLoai == selectedLoai);
                        if (loai == null || sanPham.MaLoai != loai.MaLoai)
                            continue;
                    }

                    Image anh = null;
                    if (sanPham.HinhAnh != null && sanPham.HinhAnh.Length > 0)
                    {
                        using (var ms = new System.IO.MemoryStream(sanPham.HinhAnh))
                        {
                            anh = Image.FromStream(ms);
                        }
                    }
                    string tenLoai = danhSachLoai.Find(l => l.MaLoai == sanPham.MaLoai)?.TenLoai ?? "Không xác định";
                    dataGridViewKho.Rows.Add(sanPham.MaSP, tenLoai, anh, sanPham.TenSP, sanPham.DonGia.ToString("N0") + " đ", sanPham.SoLuong);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách sản phẩm: {ex.Message}");
            }
        }

        private void CapNhatHangTrongDataGridView(int rowIndex, SanPhamDTO sanPham)
        {
            var danhSachLoai = loaiSanPhamBUS.LayTatCaLoaiSanPham();
            string tenLoai = danhSachLoai.Find(l => l.MaLoai == sanPham.MaLoai)?.TenLoai ?? "Không xác định";
            dataGridViewKho.Rows[rowIndex].Cells["MaSP"].Value = sanPham.MaSP;
            dataGridViewKho.Rows[rowIndex].Cells["TenLoai"].Value = tenLoai;
            dataGridViewKho.Rows[rowIndex].Cells["TenSP"].Value = sanPham.TenSP;
            dataGridViewKho.Rows[rowIndex].Cells["DonGia"].Value = sanPham.DonGia.ToString("N0") + " đ";
            dataGridViewKho.Rows[rowIndex].Cells["SoLuong"].Value = sanPham.SoLuong;

            Image anh = null;
            if (sanPham.HinhAnh != null && sanPham.HinhAnh.Length > 0)
            {
                using (var ms = new System.IO.MemoryStream(sanPham.HinhAnh))
                {
                    anh = Image.FromStream(ms);
                }
            }
            dataGridViewKho.Rows[rowIndex].Cells["Anh"].Value = anh;
        }

        private void dataGridViewKho_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridViewKho.Rows.Count || e.ColumnIndex < 0) return;

            if (e.ColumnIndex == 2) // Sửa từ 1 thành 2 vì cột "Anh" có index 2
            {
                string maSP = dataGridViewKho.Rows[e.RowIndex].Cells["MaSP"].Value?.ToString();
                if (string.IsNullOrEmpty(maSP))
                {
                    MessageBox.Show("Mã sản phẩm không hợp lệ!");
                    return;
                }
                try
                {
                    using (FrmSuaAnh frmSuaAnh = new FrmSuaAnh(maSP, this))
                    {
                        frmSuaAnh.ShowDialog();
                    }
                    this.BeginInvoke((MethodInvoker)delegate { TaiDanhSachSanPhamAsync(); });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi mở form sửa ảnh: {ex.Message}");
                }
            }
            else
            {
                dataGridViewKho.BeginEdit(true);
            }
        }

        private async void dataGridViewKho_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridViewKho.Rows.Count || e.ColumnIndex < 0) return;

            try
            {
                string maSP = dataGridViewKho.Rows[e.RowIndex].Cells["MaSP"].Value?.ToString();
                if (string.IsNullOrEmpty(maSP))
                {
                    MessageBox.Show("Mã sản phẩm không hợp lệ!");
                    TaiDanhSachSanPhamAsync();
                    return;
                }

                SanPhamDTO sanPham = sanPhamBUS.LaySanPhamTheoMa(maSP);
                if (sanPham == null)
                {
                    MessageBox.Show($"Không tìm thấy sản phẩm với mã {maSP}!");
                    TaiDanhSachSanPhamAsync();
                    return;
                }

                SanPhamDTO sanPhamBanDau = new SanPhamDTO
                {
                    MaSP = sanPham.MaSP,
                    TenSP = sanPham.TenSP,
                    DonGia = sanPham.DonGia,
                    SoLuong = sanPham.SoLuong,
                    HinhAnh = sanPham.HinhAnh,
                    MaLoai = sanPham.MaLoai
                };

                bool hasChanges = false;
                switch (e.ColumnIndex)
                {
                    case 1: // Cột "Loại"
                        string tenLoai = dataGridViewKho.Rows[e.RowIndex].Cells["TenLoai"].Value?.ToString();
                        if (string.IsNullOrEmpty(tenLoai))
                        {
                            MessageBox.Show("Tên loại không được để trống!");
                            TaiDanhSachSanPhamAsync();
                            return;
                        }
                        var loai = loaiSanPhamBUS.LayLoaiSanPhamTheoTen(tenLoai);
                        if (loai == null)
                        {
                            loaiSanPhamBUS.ThemLoaiSanPham(tenLoai);
                            loai = loaiSanPhamBUS.LayLoaiSanPhamTheoTen(tenLoai);
                        }
                        if (loai.MaLoai != sanPham.MaLoai)
                        {
                            sanPham.MaLoai = loai.MaLoai;
                            hasChanges = true;
                        }
                        break;
                    case 3: // Cột "Tên SP"
                        string tenSP = dataGridViewKho.Rows[e.RowIndex].Cells["TenSP"].Value?.ToString();
                        if (string.IsNullOrEmpty(tenSP))
                        {
                            MessageBox.Show("Tên sản phẩm không được để trống!");
                            TaiDanhSachSanPhamAsync();
                            return;
                        }
                        if (tenSP != sanPham.TenSP)
                        {
                            sanPham.TenSP = tenSP;
                            hasChanges = true;
                        }
                        break;
                    case 4: // Cột "Giá"
                        string donGiaStr = dataGridViewKho.Rows[e.RowIndex].Cells["DonGia"].Value?.ToString()?.Replace(" đ", "").Trim();
                        if (string.IsNullOrEmpty(donGiaStr) || !decimal.TryParse(donGiaStr, out decimal donGia) || donGia < 0)
                        {
                            MessageBox.Show("Đơn giá phải là số dương!");
                            TaiDanhSachSanPhamAsync();
                            return;
                        }
                        if (donGia != sanPham.DonGia)
                        {
                            sanPham.DonGia = donGia;
                            hasChanges = true;
                        }
                        break;
                    case 5: // Cột "Số Lượng"
                        string soLuongStr = dataGridViewKho.Rows[e.RowIndex].Cells["SoLuong"].Value?.ToString();
                        if (string.IsNullOrEmpty(soLuongStr) || !int.TryParse(soLuongStr, out int soLuong) || soLuong < 0)
                        {
                            MessageBox.Show("Số lượng phải là số nguyên dương!");
                            TaiDanhSachSanPhamAsync();
                            return;
                        }
                        if (soLuong != sanPham.SoLuong)
                        {
                            sanPham.SoLuong = soLuong;
                            hasChanges = true;
                        }
                        break;
                }

                if (hasChanges)
                {
                    string tenSanPham = sanPham.TenSP;
                    var result = MessageBox.Show($"Bạn có muốn sửa {tenSanPham} không?", "Xác nhận sửa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        await Task.Run(() => sanPhamBUS.CapNhatSanPham(sanPham));
                        CapNhatHangTrongDataGridView(e.RowIndex, sanPham);
                        MessageBox.Show("Sửa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        CapNhatHangTrongDataGridView(e.RowIndex, sanPhamBanDau);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật sản phẩm: {ex.Message}");
                TaiDanhSachSanPhamAsync();
            }
        }

        private async void dataGridViewKho_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (isProcessingClick || e.RowIndex < 0 || e.RowIndex >= dataGridViewKho.Rows.Count || e.ColumnIndex != 6) return; // Sửa từ 5 thành 6

            DateTime currentTime = DateTime.Now;
            if ((currentTime - lastClickTime) < debounceTime) return;

            lastClickTime = currentTime;
            string maSP = dataGridViewKho.Rows[e.RowIndex].Cells["MaSP"].Value?.ToString();
            if (string.IsNullOrEmpty(maSP)) return;

            isProcessingClick = true;
            dataGridViewKho.Enabled = false;

            try
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này?", "Xác nhận xóa", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    isDeleting = true;
                    await Task.Run(() => sanPhamBUS.XoaSanPham(maSP));
                    this.BeginInvoke((MethodInvoker)delegate { TaiDanhSachSanPhamAsync(); });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa sản phẩm: {ex.Message}");
                TaiDanhSachSanPhamAsync();
            }
            finally
            {
                dataGridViewKho.Enabled = true;
                isDeleting = false;
                isProcessingClick = false;
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtTim_Enter(object sender, EventArgs e)
        {
            if (txtTim.Text == "Tìm sản phẩm")
            {
                txtTim.Text = "";
                txtTim.ForeColor = SystemColors.ControlText; // Thay đổi màu chữ khi nhập
            }
        }

        private void txtTim_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTim.Text))
            {
                txtTim.Text = "Tìm sản phẩm";
                txtTim.ForeColor = SystemColors.GrayText; // Màu xám cho placeholder
            }
        }

        private async void btnTim_Click(object sender, EventArgs e)
        {
            try
            {
                string tuKhoa = txtTim.Text.Trim();
                if (string.IsNullOrEmpty(tuKhoa) || tuKhoa == "Tìm sản phẩm")
                {
                    MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm!");
                    return;
                }

                var danhSach = await Task.Run(() => sanPhamBUS.TimKiemSanPham(tuKhoa));
                dataGridViewKho.Rows.Clear();
                var danhSachLoai = loaiSanPhamBUS.LayTatCaLoaiSanPham();
                string selectedLoai = cbLoai.SelectedItem?.ToString();

                foreach (var sanPham in danhSach)
                {
                    // Kiểm tra trùng lặp MaSP
                    bool daTonTai = false;
                    foreach (DataGridViewRow row in dataGridViewKho.Rows)
                    {
                        if (row.Cells["MaSP"].Value?.ToString() == sanPham.MaSP)
                        {
                            daTonTai = true;
                            break;
                        }
                    }
                    if (daTonTai) continue;

                    // Lọc theo loại
                    if (selectedLoai != "Tất cả")
                    {
                        var loai = danhSachLoai.Find(l => l.TenLoai == selectedLoai);
                        if (loai == null || sanPham.MaLoai != loai.MaLoai)
                            continue;
                    }

                    Image anh = sanPham.HinhAnh != null && sanPham.HinhAnh.Length > 0
                        ? Image.FromStream(new System.IO.MemoryStream(sanPham.HinhAnh))
                        : null;
                    string tenLoai = danhSachLoai.Find(l => l.MaLoai == sanPham.MaLoai)?.TenLoai ?? "Không xác định";
                    dataGridViewKho.Rows.Add(sanPham.MaSP, tenLoai, anh, sanPham.TenSP, sanPham.DonGia.ToString("N0") + " đ", sanPham.SoLuong);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}");
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtTim.Text = "Tìm sản phẩm";
            txtTim.ForeColor = SystemColors.GrayText;
            TaiDanhSachSanPhamAsync();
        }

        private void txtTim_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnTim_Click(sender, e); // Gọi sự kiện tìm kiếm của btnTim
                e.SuppressKeyPress = true; // Ngăn tiếng "beep" khi nhấn Enter
            }
        }

        private void cbLoai_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaiDanhSachSanPhamAsync();
        }
    
    }
}