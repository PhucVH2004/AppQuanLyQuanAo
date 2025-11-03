using BUS;
using DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace GUI
{
    public partial class FrmBaoCao : Form
    {
        private GiaoDichBUS giaoDichBUS = new GiaoDichBUS();
        private List<GiaoDichDTO> danhSachGiaoDichToanBo; // Lưu trữ toàn bộ giao dịch để reset

        public FrmBaoCao()
        {
            InitializeComponent();
        }

        private void FrmBaoCao_Load(object sender, EventArgs e)
        {
            LoadReport(); // Gọi LoadReport để tải dữ liệu và hiển thị báo cáo
        }

        private void LoadReport()
        {
            try
            {
                // Kiểm tra reportViewer1
                if (reportViewer1 == null)
                {
                    throw new Exception("ReportViewer không được khởi tạo. Vui lòng kiểm tra thiết kế form.");
                }

                // Lấy dữ liệu giao dịch từ BUS
                danhSachGiaoDichToanBo = giaoDichBUS.LayTatCaGiaoDich();

                // Tạo DataTable để chứa dữ liệu báo cáo
                DataTable dt = new DataTable();
                dt.Columns.Add("GiaoDichID", typeof(int));
                dt.Columns.Add("MaSP", typeof(string));
                dt.Columns.Add("TenSP", typeof(string));
                dt.Columns.Add("DonGia", typeof(decimal));
                dt.Columns.Add("SoLuong", typeof(int));
                dt.Columns.Add("NgayGiaoDich", typeof(DateTime));

                // Nếu có dữ liệu, gộp và thêm vào DataTable
                if (danhSachGiaoDichToanBo != null && danhSachGiaoDichToanBo.Count > 0)
                {
                    var danhSachGiaoDichGop = danhSachGiaoDichToanBo
                        .GroupBy(gd => new { gd.MaSP, gd.DonGia })
                        .Select(g => new GiaoDichDTO
                        {
                            GiaoDichID = 0,
                            MaSP = g.Key.MaSP,
                            TenSP = g.First().TenSP,
                            DonGia = g.Key.DonGia,
                            SoLuong = g.Sum(x => x.SoLuong),
                            NgayGiaoDich = g.Max(x => x.NgayGiaoDich),
                            DaXoa = false
                        })
                        .OrderBy(g => g.MaSP)
                        .ToList();

                    foreach (var giaoDich in danhSachGiaoDichGop)
                    {
                        dt.Rows.Add(giaoDich.GiaoDichID, giaoDich.MaSP, giaoDich.TenSP, giaoDich.DonGia, giaoDich.SoLuong, giaoDich.NgayGiaoDich);
                    }
                }
                else
                {
                    // Nếu không có dữ liệu, hiển thị thông báo nhưng vẫn giữ DataTable với các cột
                    MessageBox.Show("Không có dữ liệu giao dịch để hiển thị.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Thiết lập nguồn dữ liệu cho báo cáo (DataTable có cột nhưng có thể không có hàng)
                ReportDataSource rds = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(rds);
                reportViewer1.LocalReport.ReportEmbeddedResource = "GUI.BaoCao.rdlc";
                reportViewer1.RefreshReport();
                reportViewer1.Visible = true; // Đảm bảo reportViewer luôn hiển thị
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải báo cáo: {ex.Message}\nStackTrace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Vẫn hiển thị báo cáo với bảng rỗng trong trường hợp lỗi
                DataTable dt = new DataTable();
                dt.Columns.Add("GiaoDichID", typeof(int));
                dt.Columns.Add("MaSP", typeof(string));
                dt.Columns.Add("TenSP", typeof(string));
                dt.Columns.Add("DonGia", typeof(decimal));
                dt.Columns.Add("SoLuong", typeof(int));
                dt.Columns.Add("NgayGiaoDich", typeof(DateTime));
                ReportDataSource rds = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(rds);
                reportViewer1.LocalReport.ReportEmbeddedResource = "GUI.BaoCao.rdlc";
                reportViewer1.RefreshReport();
                reportViewer1.Visible = true;
            }
        }

        private void btnBaoCao_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra ngày bắt đầu và ngày kết thúc
                DateTime ngayBatDau = dateTimePicker1.Value.Date;
                DateTime ngayKetThuc = dateTimePicker2.Value.Date.AddDays(1).AddTicks(-1);

                if (ngayBatDau > ngayKetThuc)
                {
                    MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lọc danh sách giao dịch theo khoảng thời gian
                var danhSachGiaoDichLoc = danhSachGiaoDichToanBo
                    .Where(gd => gd.NgayGiaoDich >= ngayBatDau && gd.NgayGiaoDich <= ngayKetThuc)
                    .ToList();

                // Tạo DataTable để chứa dữ liệu báo cáo
                DataTable dt = new DataTable();
                dt.Columns.Add("GiaoDichID", typeof(int));
                dt.Columns.Add("MaSP", typeof(string));
                dt.Columns.Add("TenSP", typeof(string));
                dt.Columns.Add("DonGia", typeof(decimal));
                dt.Columns.Add("SoLuong", typeof(int));
                dt.Columns.Add("NgayGiaoDich", typeof(DateTime));

                decimal tongDoanhThu = 0;

                if (danhSachGiaoDichLoc != null && danhSachGiaoDichLoc.Count > 0)
                {
                    // Gộp các giao dịch, tách riêng nếu giá khác nhau
                    var danhSachGiaoDichGop = danhSachGiaoDichLoc
                        .GroupBy(gd => new { gd.MaSP, gd.DonGia })
                        .Select(g => new GiaoDichDTO
                        {
                            GiaoDichID = 0,
                            MaSP = g.Key.MaSP,
                            TenSP = g.First().TenSP,
                            DonGia = g.Key.DonGia,
                            SoLuong = g.Sum(x => x.SoLuong),
                            NgayGiaoDich = g.Max(x => x.NgayGiaoDich),
                            DaXoa = false
                        })
                        .OrderBy(g => g.MaSP)
                        .ToList();

                    foreach (var giaoDich in danhSachGiaoDichGop)
                    {
                        dt.Rows.Add(giaoDich.GiaoDichID, giaoDich.MaSP, giaoDich.TenSP, giaoDich.DonGia, giaoDich.SoLuong, giaoDich.NgayGiaoDich);
                        tongDoanhThu += giaoDich.DonGia * giaoDich.SoLuong;
                    }

                    lblDoanhThu.Text = $"Tổng doanh thu: {tongDoanhThu:N0} đ";
                    lblDoanhThu.Visible = true;
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu giao dịch trong khoảng thời gian này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblDoanhThu.Visible = false;
                }

                // Thiết lập nguồn dữ liệu cho báo cáo
                ReportDataSource rds = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(rds);
                reportViewer1.LocalReport.ReportEmbeddedResource = "GUI.BaoCao.rdlc";
                reportViewer1.RefreshReport();
                reportViewer1.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc báo cáo: {ex.Message}\nStackTrace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Hiển thị bảng rỗng trong trường hợp lỗi
                DataTable dt = new DataTable();
                dt.Columns.Add("GiaoDichID", typeof(int));
                dt.Columns.Add("MaSP", typeof(string));
                dt.Columns.Add("TenSP", typeof(string));
                dt.Columns.Add("DonGia", typeof(decimal));
                dt.Columns.Add("SoLuong", typeof(int));
                dt.Columns.Add("NgayGiaoDich", typeof(DateTime));
                ReportDataSource rds = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(rds);
                reportViewer1.LocalReport.ReportEmbeddedResource = "GUI.BaoCao.rdlc";
                reportViewer1.RefreshReport();
                reportViewer1.Visible = true;
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime ngayBatDau = dateTimePicker1.Value.Date;
                DateTime ngayKetThuc = dateTimePicker2.Value.Date.AddDays(1).AddTicks(-1);

                if (ngayBatDau > ngayKetThuc)
                {
                    MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var danhSachGiaoDichLoc = danhSachGiaoDichToanBo
                    .Where(gd => gd.NgayGiaoDich >= ngayBatDau && gd.NgayGiaoDich <= ngayKetThuc)
                    .ToList();

                if (danhSachGiaoDichLoc == null || danhSachGiaoDichLoc.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu giao dịch trong khoảng thời gian này để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var result = MessageBox.Show($"Bạn có muốn xóa các giao dịch từ {ngayBatDau:dd/MM/yyyy} đến {ngayKetThuc:dd/MM/yyyy} không?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    giaoDichBUS.XoaGiaoDichTheoKhoangThoiGian(ngayBatDau, ngayKetThuc);
                    LoadReport(); // Tải lại báo cáo, sẽ hiển thị bảng rỗng nếu không còn dữ liệu
                    MessageBox.Show("Xóa giao dịch thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa giao dịch: {ex.Message}\nStackTrace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Hiển thị bảng rỗng trong trường hợp lỗi
                DataTable dt = new DataTable();
                dt.Columns.Add("GiaoDichID", typeof(int));
                dt.Columns.Add("MaSP", typeof(string));
                dt.Columns.Add("TenSP", typeof(string));
                dt.Columns.Add("DonGia", typeof(decimal));
                dt.Columns.Add("SoLuong", typeof(int));
                dt.Columns.Add("NgayGiaoDich", typeof(DateTime));
                ReportDataSource rds = new ReportDataSource("DataSet1", dt);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(rds);
                reportViewer1.LocalReport.ReportEmbeddedResource = "GUI.BaoCao.rdlc";
                reportViewer1.RefreshReport();
                reportViewer1.Visible = true;
            }
        }

        private void btnKhoiPhuc_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime ngayBatDau = dateTimePicker1.Value.Date;
                DateTime ngayKetThuc = dateTimePicker2.Value.Date.AddDays(1).AddTicks(-1);

                if (ngayBatDau > ngayKetThuc)
                {
                    MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show($"Bạn có muốn khôi phục các giao dịch từ {ngayBatDau:dd/MM/yyyy} đến {ngayKetThuc:dd/MM/yyyy} không?",
                    "Xác nhận khôi phục", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    giaoDichBUS.KhoiPhucGiaoDichTheoKhoangThoiGian(ngayBatDau, ngayKetThuc);
                    LoadReport();
                    MessageBox.Show("Khôi phục giao dịch thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khôi phục giao dịch: {ex.Message}\nStackTrace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                dateTimePicker1.Value = DateTime.Now;
                dateTimePicker2.Value = DateTime.Now;
                lblDoanhThu.Text = "đ";
                lblDoanhThu.Visible = true;
                LoadReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi reset: {ex.Message}\nStackTrace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}