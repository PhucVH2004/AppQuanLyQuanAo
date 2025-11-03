using BUS;
using DTO;
using GUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace QLQA
{
    public partial class FrmMain : Form
    {
        private SanPhamBUS bus = new SanPhamBUS();
        private GiaoDichBUS giaoDichBus = new GiaoDichBUS(); // Thêm BUS cho giao dịch
        private string vaitro;
        private readonly SanPhamBUS sanPhamBUS = new SanPhamBUS();
        private LoaiSanPhamBUS loaiSanPhamBUS = new LoaiSanPhamBUS();

        public FrmMain()
        {
            InitializeComponent();            
            ThietLapDataGridView();
            LoadLoaiSanPham();
            Load += FrmMain_Load; // Gán sự kiện Load
            dgvProductCart.CellEndEdit += dgvProductCart_CellEndEdit; // Thêm sự kiện CellEndEdit
        }

        public FrmMain(string role)
        {
            InitializeComponent();
            vaitro = role;
            MessageBox.Show("Đang khởi tạo form với vai trò: " + vaitro);            
            ThietLapDataGridView();
            LoadLoaiSanPham();
            Load += FrmMain_Load; // Gán sự kiện Load
            dgvProductCart.CellEndEdit += dgvProductCart_CellEndEdit; // Thêm sự kiện CellEndEdit
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            TaiDanhSachSanPham();
            LoadLoaiSanPham(); // Load categories into ComboBox
            if (vaitro == "user")
            {
                baocaoToolStripMenuItem.Visible = false;
            }
        }

        // Trong file FrmMain.cs
        private FrmKho frmKho; // Tham chiếu đến FrmKho (cần khai báo nếu chưa có)

        public void CapNhatDanhSachLoaiSanPham()
        {
            // Cập nhật danh sách loại sản phẩm trong FrmKho nếu nó đang mở
            if (frmKho != null && !frmKho.IsDisposed)
            {
                frmKho.LoadLoaiSanPham();  // Gọi phương thức LoadLoaiSanPham của FrmKho
            }
            LoadLoaiSanPham();
        }

        

        private void ThietLapDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.RowTemplate.Height = 80;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false; // Ẩn cột chỉ số hàng mặc định
            dataGridView1.AllowUserToResizeRows = false; // Vô hiệu hóa kéo kích thước hàng
            dataGridView1.AllowUserToResizeColumns = false; // Vô hiệu hóa kéo kích thước cột
            dgvProductCart.RowHeadersVisible = false; // Ẩn cột chỉ số hàng mặc định
            dgvProductCart.AllowUserToAddRows = false; // Vô hiệu hóa thêm hàng trống

            // Thêm cột "Chọn" (nút btnChon) ở đầu
            dataGridView1.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "",
                Name = "btnChon",
                Text = "Chọn",
                UseColumnTextForButtonValue = true,
                Width = 60
            });

            // Các cột dữ liệu
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã Sản Phẩm", Name = "MaSP", Width = 100 });
            dataGridView1.Columns.Add(new DataGridViewImageColumn { HeaderText = "Ảnh", Name = "Anh", ImageLayout = DataGridViewImageCellLayout.Zoom, Width = 100 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên Sản Phẩm", Name = "TenSP", Width = 210 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Đơn Giá", Name = "DonGia", Width = 100 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số Lượng", Name = "SoLuong", Width = 80 });

            TuyChinhDataGridView();
        }

        private void TuyChinhDataGridView()
        {
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.Font = new Font("Arial", 12);
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridView1.GridColor = Color.LightGray;

            // Cấu hình DataGridView tính tiền
            dgvProductCart.AutoGenerateColumns = false;
            dgvProductCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã SP", Name = "MaSP", Width = 60 });
            dgvProductCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên SP", Name = "TenSP", Width = 150 });
            dgvProductCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Giá", Name = "DonGia", Width = 100 });
            dgvProductCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số Lượng", Name = "SoLuong", Width = 60 });
            dgvProductCart.Columns.Add(new DataGridViewButtonColumn { HeaderText = "Xóa", Name = "btnXoa", Text = "Xóa", UseColumnTextForButtonValue = true, Width = 60 });
        }

        //loại danh sách sản phẩm
        private void LoadLoaiSanPham()
        {
            try
            {
                cbLoai.Items.Clear();
                cbLoai.Items.Add("Tất cả"); // Option to show all products
                var danhSachLoai = loaiSanPhamBUS.LayTatCaLoaiSanPham();
                foreach (var loai in danhSachLoai)
                {
                    cbLoai.Items.Add(loai.TenLoai);
                }
                cbLoai.SelectedIndex = 0; // Default to "Tất cả"
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách loại sản phẩm: {ex.Message}");
            }
        }

        public void TaiDanhSachSanPham()
        {
            try
            {
                dataGridView1.Rows.Clear();
                List<SanPhamDTO> danhSachSanPham = bus.LayTatCaSanPham();
                if (danhSachSanPham == null || danhSachSanPham.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu sản phẩm trong cơ sở dữ liệu!");
                    return;
                }

                string selectedLoai = cbLoai.SelectedItem?.ToString();
                foreach (var sanPham in danhSachSanPham)
                {
                    // Filter by category if not "Tất cả"
                    if (selectedLoai != "Tất cả")
                    {
                        var loai = loaiSanPhamBUS.LayTatCaLoaiSanPham().Find(l => l.TenLoai == selectedLoai);
                        if (loai == null || sanPham.MaLoai != loai.MaLoai)
                            continue;
                    }

                    Image anh = null;
                    try
                    {
                        if (sanPham.HinhAnh != null && sanPham.HinhAnh.Length > 0)
                        {
                            using (var ms = new System.IO.MemoryStream(sanPham.HinhAnh))
                            {
                                anh = Image.FromStream(ms);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Không thể tải ảnh cho sản phẩm {sanPham.TenSP}: {ex.Message}");
                        anh = null;
                    }

                    dataGridView1.Rows.Add(null, sanPham.MaSP, anh, sanPham.TenSP, sanPham.DonGia.ToString("N0") + " đ", sanPham.SoLuong);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách sản phẩm: {ex.Message}");
            }
        }

        private void DangXuatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDangNhap dangnhap = new frmDangNhap();
            dangnhap.Show();
            this.Hide();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void themSanPhamToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            FrmThemSP frmThemSP = new FrmThemSP(this);
            frmThemSP.ShowDialog();
            TaiDanhSachSanPham(); // Sử dụng phiên bản đồng bộ
        }

        private void khoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            FrmKho frmKho = new FrmKho(this);
            frmKho.ShowDialog();
            TaiDanhSachSanPham(); // Sử dụng phiên bản đồng bộ
        }

        private void baocaoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmBaoCao frmBaoCao = new FrmBaoCao();
            frmBaoCao.ShowDialog();
        }

        private void txtTim_Enter(object sender, EventArgs e)
        {
            if (txtTim.Text == "Tìm sản phẩm")
            {
                txtTim.Text = "";
                txtTim.ForeColor = SystemColors.ControlText;
            }
        }

        private void txtTim_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTim.Text))
            {
                txtTim.Text = "Tìm sản phẩm";
                txtTim.ForeColor = SystemColors.GrayText;
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            try
            {
                string tuKhoa = txtTim.Text.Trim();
                if (string.IsNullOrEmpty(tuKhoa) || tuKhoa == "Tìm sản phẩm")
                {
                    MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm!");
                    return;
                }

                List<SanPhamDTO> danhSach = bus.TimKiemSanPham(tuKhoa); // Sử dụng phiên bản đồng bộ
                dataGridView1.Rows.Clear();
                foreach (var sanPham in danhSach)
                {
                    Image anh = null;
                    if (sanPham.HinhAnh != null && sanPham.HinhAnh.Length > 0)
                    {
                        using (var ms = new MemoryStream(sanPham.HinhAnh))
                        {
                            anh = Image.FromStream(ms);
                        }
                    }
                    dataGridView1.Rows.Add(null, sanPham.MaSP, anh, sanPham.TenSP, sanPham.DonGia.ToString("N0") + " đ", sanPham.SoLuong);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}");
            }
        }

        private void txtTim_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnTim_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtTim.Text = "";
            txtTim.ForeColor = SystemColors.GrayText;
            TaiDanhSachSanPham(); // Sử dụng phiên bản đồng bộ
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu nhấp vào header
            if (e.RowIndex < 0) return;

            // Lấy thông tin hàng được chọn
            DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
            string maSP = selectedRow.Cells["MaSP"].Value?.ToString();
            string tenSP = selectedRow.Cells["TenSP"].Value?.ToString();
            string donGiaStr = selectedRow.Cells["DonGia"].Value?.ToString().Replace(" đ", "").Trim();
            int soLuongTrongKho = int.Parse(selectedRow.Cells["SoLuong"].Value?.ToString() ?? "0");
            decimal donGia = decimal.Parse(donGiaStr);

            // Kiểm tra nếu sản phẩm đã tồn tại trong giỏ hàng
            foreach (DataGridViewRow row in dgvProductCart.Rows)
            {
                if (row.Cells["MaSP"].Value?.ToString() == maSP)
                {
                    int currentQuantity = Convert.ToInt32(row.Cells["SoLuong"].Value);
                    if (currentQuantity + 1 > soLuongTrongKho)
                    {
                        MessageBox.Show("Không đủ hàng trong kho.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    currentQuantity++;
                    row.Cells["SoLuong"].Value = currentQuantity;
                    lblTotal.Text = GetTotalOrderAmount().ToString("N0") + " đ";
                    return;
                }
            }

            // Thêm sản phẩm mới vào giỏ hàng nếu còn hàng
            if (soLuongTrongKho > 0)
            {
                int quantity = 1;
                dgvProductCart.Rows.Add(maSP, tenSP, donGia.ToString("N0") + " đ", quantity);
                lblTotal.Text = GetTotalOrderAmount().ToString("N0") + " đ";
            }
            else
            {
                MessageBox.Show("Sản phẩm này đã hết hàng.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private decimal GetTotalOrderAmount()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dgvProductCart.Rows)
            {
                if (row.Cells["SoLuong"].Value != null && row.Cells["DonGia"].Value != null)
                {
                    int soLuong = Convert.ToInt32(row.Cells["SoLuong"].Value);
                    string donGiaStr = row.Cells["DonGia"].Value.ToString().Replace(" đ", "").Trim();
                    decimal donGia = decimal.Parse(donGiaStr);
                    total += soLuong * donGia;
                }
            }
            return total;
        }

        private void dgvProductCart_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvProductCart.Columns[e.ColumnIndex].Name == "btnXoa")
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này khỏi giỏ hàng không?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    dgvProductCart.Rows.RemoveAt(e.RowIndex);
                    lblTotal.Text = GetTotalOrderAmount().ToString("N0") + " đ";
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (dgvProductCart.Rows.Count > 0)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa toàn bộ giỏ hàng không?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    dgvProductCart.Rows.Clear();
                    lblTotal.Text = "0 đ";
                    MessageBox.Show("Đã xóa toàn bộ giỏ hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Giỏ hàng đang trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtKhachTra_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TinhTienThua();
                e.SuppressKeyPress = true;
            }
        }

        private void TinhTienThua()
        {
            if (string.IsNullOrEmpty(txtKhachTra.Text))
            {
                lblThua.Text = "0 đ";
                return;
            }
            if (!decimal.TryParse(txtKhachTra.Text, out decimal khachTra))
            {
                MessageBox.Show("Vui lòng nhập số tiền hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtKhachTra.Clear();
                lblThua.Text = "0 đ";
                return;
            }
            decimal tongTien = GetTotalOrderAmount();
            decimal tienThua = khachTra - tongTien;
            lblThua.Text = tienThua >= 0 ? tienThua.ToString("N0") + " đ" : "0 đ";
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            try
            {
                TinhTienThua();
                decimal khachTra = string.IsNullOrEmpty(txtKhachTra.Text) ? 0 : decimal.Parse(txtKhachTra.Text);
                decimal tongTien = GetTotalOrderAmount();

                if (khachTra < tongTien)
                {
                    MessageBox.Show("Số tiền khách trả không đủ để thanh toán.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lưu các sản phẩm trong giỏ hàng vào bảng GiaoDich
                foreach (DataGridViewRow row in dgvProductCart.Rows)
                {
                    string maSP = row.Cells["MaSP"].Value?.ToString();
                    string tenSP = row.Cells["TenSP"].Value?.ToString();
                    string donGiaStr = row.Cells["DonGia"].Value?.ToString().Replace(" đ", "").Trim();
                    int soLuongBan = Convert.ToInt32(row.Cells["SoLuong"].Value);
                    decimal donGia = decimal.Parse(donGiaStr);

                    GiaoDichDTO giaoDich = new GiaoDichDTO
                    {
                        MaSP = maSP,
                        TenSP = tenSP,
                        DonGia = donGia,
                        SoLuong = soLuongBan,
                        NgayGiaoDich = DateTime.Now
                    };

                    giaoDichBus.LuuGiaoDich(giaoDich);
                    UpdateSanPhamQuantity(maSP, soLuongBan);
                }

                // Đồng bộ dữ liệu với FrmKho
                FrmKho frmKho = new FrmKho(this);
                frmKho.TaiDanhSachSanPhamAsync();

                MessageBox.Show("Thanh toán thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // In hóa đơn sau khi thanh toán thành công
                PrintInvoice();

                // Reset giao diện sau khi thanh toán
                dgvProductCart.Rows.Clear();
                txtKhachTra.Clear();
                lblThua.Text = "0 đ";
                lblTotal.Text = "0 đ";
                TaiDanhSachSanPham();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thanh toán: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSanPhamQuantity(string maSP, int soLuongBan)
        {
            SanPhamDTO sanPham = bus.LaySanPhamTheoMa(maSP);
            if (sanPham != null)
            {
                sanPham.SoLuong -= soLuongBan;
                if (sanPham.SoLuong < 0) sanPham.SoLuong = 0;
                bus.CapNhatSanPham(sanPham);
            }
        }

        private void dgvProductCart_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu ô chỉnh sửa nằm trong cột "SoLuong"
            if (e.ColumnIndex == dgvProductCart.Columns["SoLuong"].Index && e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvProductCart.Rows[e.RowIndex];
                string maSP = row.Cells["MaSP"].Value?.ToString();
                int soLuongMoi;

                // Kiểm tra giá trị số lượng mới
                if (!int.TryParse(row.Cells["SoLuong"].Value?.ToString(), out soLuongMoi) || soLuongMoi <= 0)
                {
                    MessageBox.Show("Số lượng phải là số nguyên dương!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    row.Cells["SoLuong"].Value = 1; // Đặt lại giá trị mặc định
                    soLuongMoi = 1;
                }

                // Kiểm tra số lượng trong kho
                foreach (DataGridViewRow productRow in dataGridView1.Rows)
                {
                    if (productRow.Cells["MaSP"].Value?.ToString() == maSP)
                    {
                        int soLuongTrongKho = int.Parse(productRow.Cells["SoLuong"].Value?.ToString() ?? "0");
                        if (soLuongMoi > soLuongTrongKho)
                        {
                            MessageBox.Show("Không đủ hàng trong kho.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            row.Cells["SoLuong"].Value = soLuongTrongKho; // Đặt lại số lượng tối đa
                            soLuongMoi = soLuongTrongKho;
                        }
                        break;
                    }
                }

                // Cập nhật lại tổng hóa đơn
                lblTotal.Text = GetTotalOrderAmount().ToString("N0") + " đ";
            }
        }

        // Phương thức này khởi tạo và hiển thị cửa sổ xem trước hóa đơn
        // - Tạo một PrintDocument để xử lý việc in
        // - Gán sự kiện PrintPage để định nghĩa nội dung hóa đơn
        // - Hiển thị hóa đơn trong PrintPreviewDialog để người dùng xem trước
        private void PrintInvoice()
        {
            try
            {
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                PrintPreviewDialog previewDialog = new PrintPreviewDialog
                {
                    Document = printDoc
                };
                previewDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi in hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Phương thức này định nghĩa nội dung của hóa đơn để in dưới dạng bảng
        // - Hiển thị tiêu đề cửa hàng "XXX" (căn giữa) với font chữ lớn hơn
        // - In thông tin giao dịch: ngày giao dịch
        // - Vẽ bảng với các cột (Mã SP, Tên SP, Đơn Giá, Số Lượng) và các hàng chứa thông tin sản phẩm từ dgvProductCart
        // - In tổng tiền, tiền khách trả, tiền thừa với font chữ lớn hơn
        // - Thêm dòng cảm ơn cuối cùng với font chữ lớn hơn
        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                float leftMargin = e.MarginBounds.Left;
                float topMargin = e.MarginBounds.Top;
                Font printFont = new Font("Arial", 12); // Font chữ thông thường
                Font boldFont = new Font("Arial", 14, FontStyle.Bold); // Font chữ in đậm
                float yPos = topMargin;

                // In tiêu đề cửa hàng (căn giữa)
                string tenCuaHang = "CỬA HÀNG XXX";
                SizeF titleSize = e.Graphics.MeasureString(tenCuaHang, boldFont);
                e.Graphics.DrawString(tenCuaHang, boldFont, Brushes.Black, (e.MarginBounds.Width - titleSize.Width) / 2, yPos);
                yPos += titleSize.Height + 20;

                // In thông tin giao dịch
                e.Graphics.DrawString($"Ngày: {DateTime.Now:dd/MM/yyyy HH:mm}", printFont, Brushes.Black, leftMargin, yPos);
                yPos += 30;

                // Vẽ bảng với các cột
                float columnMaSP = 100;  // Độ rộng cột Mã SP
                float columnTenSP = 200; // Độ rộng cột Tên SP
                float columnDonGia = 100; // Độ rộng cột Đơn Giá
                float columnSoLuong = 80; // Độ rộng cột Số Lượng
                float tableWidth = columnMaSP + columnTenSP + columnDonGia + columnSoLuong;
                float xPos = leftMargin;

                // Vẽ đường ngang đầu bảng
                e.Graphics.DrawLine(Pens.Black, xPos, yPos, xPos + tableWidth, yPos);
                yPos += 2;

                // Vẽ tiêu đề cột
                e.Graphics.DrawString("Mã SP", printFont, Brushes.Black, xPos, yPos);
                e.Graphics.DrawString("Tên SP", printFont, Brushes.Black, xPos + columnMaSP, yPos);
                e.Graphics.DrawString("Đơn Giá", printFont, Brushes.Black, xPos + columnMaSP + columnTenSP, yPos);
                e.Graphics.DrawString("Số Lượng", printFont, Brushes.Black, xPos + columnMaSP + columnTenSP + columnDonGia, yPos);
                yPos += 30;

                // Vẽ đường ngang giữa tiêu đề và dữ liệu
                e.Graphics.DrawLine(Pens.Black, xPos, yPos, xPos + tableWidth, yPos);
                yPos += 2;

                // In danh sách sản phẩm từ dgvProductCart vào bảng
                foreach (DataGridViewRow row in dgvProductCart.Rows)
                {
                    string maSP = row.Cells["MaSP"].Value?.ToString();
                    string tenSP = row.Cells["TenSP"].Value?.ToString();
                    string donGia = row.Cells["DonGia"].Value?.ToString();
                    int soLuong = Convert.ToInt32(row.Cells["SoLuong"].Value);

                    e.Graphics.DrawString(maSP, printFont, Brushes.Black, xPos, yPos);
                    e.Graphics.DrawString(tenSP, printFont, Brushes.Black, xPos + columnMaSP, yPos);
                    e.Graphics.DrawString(donGia, printFont, Brushes.Black, xPos + columnMaSP + columnTenSP, yPos);
                    e.Graphics.DrawString(soLuong.ToString(), printFont, Brushes.Black, xPos + columnMaSP + columnTenSP + columnDonGia, yPos);
                    yPos += 30;

                    // Vẽ đường ngang giữa các hàng
                    e.Graphics.DrawLine(Pens.Black, xPos, yPos, xPos + tableWidth, yPos);
                    yPos += 2;
                }

                // Vẽ đường ngang cuối bảng
                e.Graphics.DrawLine(Pens.Black, xPos, yPos - 2, xPos + tableWidth, yPos - 2);

                // In tổng tiền
                yPos += 20;
                e.Graphics.DrawString($"Tổng tiền: {GetTotalOrderAmount():N0} đ", boldFont, Brushes.Black, leftMargin, yPos);
                yPos += 30;

                // In tiền khách trả và tiền thừa
                decimal khachTra = string.IsNullOrEmpty(txtKhachTra.Text) ? 0 : decimal.Parse(txtKhachTra.Text);
                decimal tienThua = khachTra - GetTotalOrderAmount();
                e.Graphics.DrawString($"Khách trả: {khachTra:N0} đ", printFont, Brushes.Black, leftMargin, yPos);
                yPos += 30;
                e.Graphics.DrawString($"Tiền thừa: {tienThua:N0} đ", printFont, Brushes.Black, leftMargin, yPos);
                yPos += 40;

                // In dòng cảm ơn (căn giữa)
                string camOn = "Cảm ơn quý khách đã ủng hộ!\nChúc quý khách thật nhiều sức khỏe và hẹn gặp lại!";
                SizeF camOnSize = e.Graphics.MeasureString(camOn, printFont);
                e.Graphics.DrawString(camOn, printFont, Brushes.Black, (e.MarginBounds.Width - camOnSize.Width) / 2, yPos);

                // Chỉ định rằng không có trang tiếp theo
                e.HasMorePages = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo nội dung hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbLoai_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaiDanhSachSanPham(); // Refresh product list when category changes
        }
    }
}