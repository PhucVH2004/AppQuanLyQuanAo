using DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class GiaoDichDAO
    {
        private string connectionString = "Data Source=LAPTOP-H0J8DGVP\\SQLEXPRESS;Initial Catalog=QLQA;Integrated Security=True;Encrypt=False";

        public void LuuGiaoDich(GiaoDichDTO giaoDich)
        {
            try
            {
                using (SqlConnection KetNoi = new SqlConnection(connectionString))
                {
                    KetNoi.Open();

                    // Kiểm tra xem bảng GiaoDich có tồn tại không
                    string checkTableQuery = "SELECT COUNT(*) FROM sys.tables WHERE name = 'GiaoDich'";
                    SqlCommand checkCmd = new SqlCommand(checkTableQuery, KetNoi);
                    int tableExists = (int)checkCmd.ExecuteScalar();
                    if (tableExists == 0)
                    {
                        throw new Exception("Bảng GiaoDich không tồn tại trong cơ sở dữ liệu!");
                    }

                    string CauTruyVan = "INSERT INTO GiaoDich (MaSP, TenSP, DonGia, SoLuong, NgayGiaoDich) VALUES (@MaSP, @TenSP, @DonGia, @SoLuong, @NgayGiaoDich)";
                    SqlCommand Lenh = new SqlCommand(CauTruyVan, KetNoi);

                    Lenh.Parameters.AddWithValue("@MaSP", giaoDich.MaSP ?? (object)DBNull.Value);
                    Lenh.Parameters.AddWithValue("@TenSP", giaoDich.TenSP ?? (object)DBNull.Value);
                    Lenh.Parameters.AddWithValue("@DonGia", giaoDich.DonGia);
                    Lenh.Parameters.AddWithValue("@SoLuong", giaoDich.SoLuong);
                    Lenh.Parameters.AddWithValue("@NgayGiaoDich", giaoDich.NgayGiaoDich);

                    Lenh.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi SQL khi lưu giao dịch: {ex.Message}\nError Number: {ex.Number}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu giao dịch: {ex.Message}");
            }
        }

        public List<GiaoDichDTO> LayTatCaGiaoDich()
        {
            List<GiaoDichDTO> DanhSachGiaoDich = new List<GiaoDichDTO>();
            try
            {
                using (SqlConnection KetNoi = new SqlConnection(connectionString))
                {
                    KetNoi.Open();

                    // Kiểm tra xem bảng GiaoDich có tồn tại không
                    string checkTableQuery = "SELECT COUNT(*) FROM sys.tables WHERE name = 'GiaoDich'";
                    SqlCommand checkCmd = new SqlCommand(checkTableQuery, KetNoi);
                    int tableExists = (int)checkCmd.ExecuteScalar();
                    if (tableExists == 0)
                    {
                        throw new Exception("Bảng GiaoDich không tồn tại trong cơ sở dữ liệu!");
                    }

                    string CauTruyVan = "SELECT GiaoDichID, MaSP, TenSP, DonGia, SoLuong, NgayGiaoDich FROM GiaoDich WHERE DaXoa = 0"; // Chỉ lấy giao dịch chưa xóa
                    SqlCommand Lenh = new SqlCommand(CauTruyVan, KetNoi);

                    SqlDataReader DocDuLieu = Lenh.ExecuteReader();
                    while (DocDuLieu.Read())
                    {
                        DanhSachGiaoDich.Add(new GiaoDichDTO
                        {
                            GiaoDichID = Convert.ToInt32(DocDuLieu["GiaoDichID"]),
                            MaSP = DocDuLieu["MaSP"].ToString(),
                            TenSP = DocDuLieu["TenSP"].ToString(),
                            DonGia = Convert.ToDecimal(DocDuLieu["DonGia"]),
                            SoLuong = Convert.ToInt32(DocDuLieu["SoLuong"]),
                            NgayGiaoDich = Convert.ToDateTime(DocDuLieu["NgayGiaoDich"]),
                            DaXoa = false // Giả định các giao dịch chưa xóa
                        });
                    }
                    DocDuLieu.Close();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi SQL khi lấy danh sách giao dịch: {ex.Message}\nError Number: {ex.Number}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách giao dịch: {ex.Message}");
            }
            return DanhSachGiaoDich;
        }

        public void LuuDanhSachGiaoDich(List<GiaoDichDTO> danhSachGiaoDich)
        {
            try
            {
                using (SqlConnection KetNoi = new SqlConnection(connectionString))
                {
                    KetNoi.Open();

                    // Kiểm tra xem bảng GiaoDich có tồn tại không
                    string checkTableQuery = "SELECT COUNT(*) FROM sys.tables WHERE name = 'GiaoDich'";
                    SqlCommand checkCmd = new SqlCommand(checkTableQuery, KetNoi);
                    int tableExists = (int)checkCmd.ExecuteScalar();
                    if (tableExists == 0)
                    {
                        throw new Exception("Bảng GiaoDich không tồn tại trong cơ sở dữ liệu!");
                    }

                    string CauTruyVan = "INSERT INTO GiaoDich (MaSP, TenSP, DonGia, SoLuong, NgayGiaoDich) VALUES (@MaSP, @TenSP, @DonGia, @SoLuong, @NgayGiaoDich)";
                    foreach (var giaoDich in danhSachGiaoDich)
                    {
                        SqlCommand Lenh = new SqlCommand(CauTruyVan, KetNoi);
                        Lenh.Parameters.AddWithValue("@MaSP", giaoDich.MaSP ?? (object)DBNull.Value);
                        Lenh.Parameters.AddWithValue("@TenSP", giaoDich.TenSP ?? (object)DBNull.Value);
                        Lenh.Parameters.AddWithValue("@DonGia", giaoDich.DonGia);
                        Lenh.Parameters.AddWithValue("@SoLuong", giaoDich.SoLuong);
                        Lenh.Parameters.AddWithValue("@NgayGiaoDich", giaoDich.NgayGiaoDich);
                        Lenh.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi SQL khi lưu danh sách giao dịch: {ex.Message}\nError Number: {ex.Number}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu danh sách giao dịch: {ex.Message}");
            }
        }

        // Xóa giao dịch theo khoảng thời gian (soft delete)
        public void XoaGiaoDichTheoKhoangThoiGian(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            try
            {
                using (SqlConnection KetNoi = new SqlConnection(connectionString))
                {
                    KetNoi.Open();

                    string CauTruyVan = "UPDATE GiaoDich SET DaXoa = 1 WHERE NgayGiaoDich BETWEEN @NgayBatDau AND @NgayKetThuc AND DaXoa = 0";
                    SqlCommand Lenh = new SqlCommand(CauTruyVan, KetNoi);

                    Lenh.Parameters.AddWithValue("@NgayBatDau", ngayBatDau);
                    Lenh.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc);

                    Lenh.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi SQL khi xóa giao dịch: {ex.Message}\nError Number: {ex.Number}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa giao dịch: {ex.Message}");
            }
        }

        // Khôi phục giao dịch theo khoảng thời gian
        public void KhoiPhucGiaoDichTheoKhoangThoiGian(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            try
            {
                using (SqlConnection KetNoi = new SqlConnection(connectionString))
                {
                    KetNoi.Open();

                    string CauTruyVan = "UPDATE GiaoDich SET DaXoa = 0 WHERE NgayGiaoDich BETWEEN @NgayBatDau AND @NgayKetThuc AND DaXoa = 1";
                    SqlCommand Lenh = new SqlCommand(CauTruyVan, KetNoi);

                    Lenh.Parameters.AddWithValue("@NgayBatDau", ngayBatDau);
                    Lenh.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc);

                    Lenh.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi SQL khi khôi phục giao dịch: {ex.Message}\nError Number: {ex.Number}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi khôi phục giao dịch: {ex.Message}");
            }
        }
    }
}