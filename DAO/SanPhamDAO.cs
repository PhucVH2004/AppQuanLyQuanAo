using DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAO
{
    public class SanPhamDAO
    {
        private string connectionString = "Data Source=LAPTOP-H0J8DGVP\\SQLEXPRESS;Initial Catalog=QLQA;Integrated Security=True;Encrypt=False";

        public List<SanPhamDTO> LayTatCaSanPham()
        {
            List<SanPhamDTO> DanhSachSanPham = new List<SanPhamDTO>();
            try
            {
                using (SqlConnection KetNoi = new SqlConnection(connectionString))
                {
                    Console.WriteLine("Đang mở kết nối đến cơ sở dữ liệu...");
                    string CauTruyVan = @"SELECT sp.MaSP, sp.TenSP, sp.DonGia, sp.SoLuong, sp.HinhAnh, sp.MaLoai 
                                 FROM SanPham sp 
                                 LEFT JOIN LoaiSanPham lsp ON sp.MaLoai = lsp.MaLoai";
                    SqlCommand Lenh = new SqlCommand(CauTruyVan, KetNoi);

                    KetNoi.Open();
                    Console.WriteLine("Kết nối thành công, đang thực thi truy vấn...");
                    SqlDataReader DocDuLieu = Lenh.ExecuteReader();
                    while (DocDuLieu.Read())
                    {
                        SanPhamDTO SanPham = new SanPhamDTO
                        {
                            MaSP = DocDuLieu.IsDBNull(0) ? null : DocDuLieu[0].ToString(),
                            TenSP = DocDuLieu.IsDBNull(1) ? null : DocDuLieu.GetString(1),
                            DonGia = DocDuLieu.IsDBNull(2) ? 0 : DocDuLieu.GetDecimal(2),
                            SoLuong = DocDuLieu.IsDBNull(3) ? 0 : DocDuLieu.GetInt32(3),
                            HinhAnh = DocDuLieu.IsDBNull(4) ? null : (byte[])DocDuLieu[4],
                            MaLoai = DocDuLieu.IsDBNull(5) ? 0 : DocDuLieu.GetInt32(5) // Add MaLoai
                        };
                        DanhSachSanPham.Add(SanPham);
                    }
                    DocDuLieu.Close();
                    KetNoi.Close();
                    Console.WriteLine($"Đã đọc được {DanhSachSanPham.Count} sản phẩm từ CSDL.");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Lỗi SQL trong LayTatCaSanPham: {ex.Message}\nChi tiết: {ex.StackTrace}");
                throw new Exception($"Lỗi SQL khi lấy danh sách sản phẩm:\n" +
                                    $"Thông báo: {ex.Message}\n" +
                                    $"Chi tiết: {ex.StackTrace}\n" +
                                    $"Nguồn: {ex.Source}");
            }
            return DanhSachSanPham;
        }

        public void ThemSanPham(SanPhamDTO sanPham)
        {
            try
            {
                if (sanPham == null)
                {
                    throw new ArgumentNullException(nameof(sanPham), "Đối tượng SanPhamDTO không được null!");
                }
                if (string.IsNullOrEmpty(sanPham.MaSP))
                {
                    throw new ArgumentException("Mã sản phẩm không được để trống!");
                }
                if (string.IsNullOrEmpty(sanPham.TenSP))
                {
                    throw new ArgumentException("Tên sản phẩm không được để trống!");
                }

                using (SqlConnection KetNoi = new SqlConnection(connectionString))
                {
                    KetNoi.Open();
                    string CauTruyVan = "INSERT INTO SanPham (MaSP, TenSP, DonGia, SoLuong, HinhAnh, MaLoai) VALUES (@MaSP, @TenSP, @DonGia, @SoLuong, @HinhAnh, @MaLoai)";
                    SqlCommand Lenh = new SqlCommand(CauTruyVan, KetNoi);

                    Lenh.Parameters.AddWithValue("@MaSP", sanPham.MaSP);
                    Lenh.Parameters.AddWithValue("@TenSP", sanPham.TenSP);
                    Lenh.Parameters.AddWithValue("@DonGia", sanPham.DonGia);
                    Lenh.Parameters.AddWithValue("@SoLuong", sanPham.SoLuong);
                    Lenh.Parameters.AddWithValue("@HinhAnh", sanPham.HinhAnh ?? (object)DBNull.Value);
                    Lenh.Parameters.AddWithValue("@MaLoai", sanPham.MaLoai);

                    int rowsAffected = Lenh.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Không thể thêm sản phẩm với mã {sanPham.MaSP}. Có thể mã sản phẩm đã tồn tại.");
                    }
                    Console.WriteLine("Đã thêm sản phẩm thành công.");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Lỗi SQL trong ThemSanPham: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khác trong ThemSanPham: {ex.Message}");
                throw;
            }
        }

        public void CapNhatSanPham(SanPhamDTO sanPham)
        {
            try
            {
                if (sanPham == null)
                {
                    throw new ArgumentNullException(nameof(sanPham), "Đối tượng SanPhamDTO không được null!");
                }
                if (string.IsNullOrEmpty(sanPham.MaSP))
                {
                    throw new ArgumentException("Mã sản phẩm không được để trống!");
                }
                if (string.IsNullOrEmpty(sanPham.TenSP))
                {
                    throw new ArgumentException("Tên sản phẩm không được để trống!");
                }

                using (SqlConnection KetNoi = new SqlConnection(connectionString))
                {
                    KetNoi.Open();
                    string CauTruyVan = "UPDATE SanPham SET TenSP = @TenSP, DonGia = @DonGia, SoLuong = @SoLuong, HinhAnh = @HinhAnh, MaLoai = @MaLoai WHERE MaSP = @MaSP";
                    SqlCommand Lenh = new SqlCommand(CauTruyVan, KetNoi);

                    Lenh.Parameters.AddWithValue("@MaSP", sanPham.MaSP);
                    Lenh.Parameters.AddWithValue("@TenSP", sanPham.TenSP);
                    Lenh.Parameters.AddWithValue("@DonGia", sanPham.DonGia);
                    Lenh.Parameters.AddWithValue("@SoLuong", sanPham.SoLuong);
                    Lenh.Parameters.AddWithValue("@HinhAnh", sanPham.HinhAnh ?? (object)DBNull.Value);
                    Lenh.Parameters.AddWithValue("@MaLoai", sanPham.MaLoai);

                    int rowsAffected = Lenh.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        Console.WriteLine($"Không có bản ghi nào được cập nhật cho MaSP: {sanPham.MaSP}");
                        throw new Exception($"Không tìm thấy sản phẩm với mã {sanPham.MaSP} để cập nhật!");
                    }
                    Console.WriteLine("Đã cập nhật sản phẩm thành công.");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Lỗi SQL trong CapNhatSanPham: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khác trong CapNhatSanPham: {ex.Message}");
                throw;
            }
        }

        public void XoaSanPham(string maSP)
        {
            try
            {
                if (string.IsNullOrEmpty(maSP))
                {
                    throw new ArgumentException("Mã sản phẩm không được để trống!");
                }

                using (SqlConnection KetNoi = new SqlConnection(connectionString))
                {
                    KetNoi.Open();
                    string query = "DELETE FROM SanPham WHERE MaSP = @MaSP";
                    using (SqlCommand Lenh = new SqlCommand(query, KetNoi))
                    {
                        Lenh.Parameters.AddWithValue("@MaSP", maSP);
                        int rowsAffected = Lenh.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            Console.WriteLine($"Không có bản ghi nào được xóa cho MaSP: {maSP}");
                            throw new Exception($"Không tìm thấy sản phẩm với mã {maSP} để xóa!");
                        }
                        Console.WriteLine($"Đã xóa sản phẩm {maSP} thành công.");
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Lỗi SQL trong XoaSanPham: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khác trong XoaSanPham: {ex.Message}");
                throw;
            }
        }

        public SanPhamDTO LaySanPhamTheoMa(string maSP)
        {
            try
            {
                if (string.IsNullOrEmpty(maSP))
                {
                    throw new ArgumentException("Mã sản phẩm không được để trống!");
                }

                using (SqlConnection KetNoi = new SqlConnection(connectionString))
                {
                    KetNoi.Open();
                    string query = "SELECT * FROM SanPham WHERE MaSP = @MaSP";
                    using (SqlCommand Lenh = new SqlCommand(query, KetNoi))
                    {
                        Lenh.Parameters.AddWithValue("@MaSP", maSP);
                        using (SqlDataReader reader = Lenh.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new SanPhamDTO
                                {
                                    MaSP = reader["MaSP"].ToString(),
                                    TenSP = reader["TenSP"].ToString(),
                                    HinhAnh = reader["HinhAnh"] != DBNull.Value ? (byte[])reader["HinhAnh"] : null,
                                    DonGia = Convert.ToDecimal(reader["DonGia"]),
                                    SoLuong = Convert.ToInt32(reader["SoLuong"]),
                                    MaLoai = reader["MaLoai"] != DBNull.Value ? Convert.ToInt32(reader["MaLoai"]) : 0
                                };
                            }
                        }
                    }
                }
                Console.WriteLine($"Không tìm thấy sản phẩm với mã {maSP}.");
                return null;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Lỗi SQL trong LaySanPhamTheoMa: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khác trong LaySanPhamTheoMa: {ex.Message}");
                throw;
            }
        }

        public List<SanPhamDTO> TimKiemSanPham(string tuKhoa)
        {
            List<SanPhamDTO> DanhSachSanPham = new List<SanPhamDTO>();
            try
            {
                using (SqlConnection KetNoi = new SqlConnection(connectionString))
                {
                    KetNoi.Open();
                    string CauTruyVan = @"SELECT sp.MaSP, sp.TenSP, sp.DonGia, sp.SoLuong, sp.HinhAnh, sp.MaLoai 
                                 FROM SanPham sp 
                                 LEFT JOIN LoaiSanPham lsp ON sp.MaLoai = lsp.MaLoai 
                                 WHERE sp.MaSP LIKE @TuKhoa OR sp.TenSP LIKE @TuKhoa OR lsp.TenLoai LIKE @TuKhoa";
                    SqlCommand Lenh = new SqlCommand(CauTruyVan, KetNoi);
                    Lenh.Parameters.AddWithValue("@TuKhoa", "%" + tuKhoa + "%");

                    SqlDataReader DocDuLieu = Lenh.ExecuteReader();
                    while (DocDuLieu.Read())
                    {
                        SanPhamDTO SanPham = new SanPhamDTO
                        {
                            MaSP = DocDuLieu.IsDBNull(0) ? null : DocDuLieu[0].ToString(),
                            TenSP = DocDuLieu.IsDBNull(1) ? null : DocDuLieu.GetString(1),
                            DonGia = DocDuLieu.IsDBNull(2) ? 0 : DocDuLieu.GetDecimal(2),
                            SoLuong = DocDuLieu.IsDBNull(3) ? 0 : DocDuLieu.GetInt32(3),
                            HinhAnh = DocDuLieu.IsDBNull(4) ? null : (byte[])DocDuLieu[4],
                            MaLoai = DocDuLieu.IsDBNull(5) ? 0 : DocDuLieu.GetInt32(5) // Add MaLoai
                        };
                        DanhSachSanPham.Add(SanPham);
                    }
                    DocDuLieu.Close();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Lỗi SQL trong TimKiemSanPham: {ex.Message}");
                throw;
            }
            return DanhSachSanPham;
        }

        public List<LoaiSanPhamDTO> LayTatCaLoai()
        {
            List<LoaiSanPhamDTO> danhSach = new List<LoaiSanPhamDTO>();
            try
            {
                using (SqlConnection KetNoi = new SqlConnection(connectionString))
                {
                    KetNoi.Open();
                    string CauTruyVan = "SELECT MaLoai, TenLoai FROM LoaiSanPham";
                    using (SqlCommand Lenh = new SqlCommand(CauTruyVan, KetNoi))
                    {
                        using (SqlDataReader DocDuLieu = Lenh.ExecuteReader())
                        {
                            while (DocDuLieu.Read())
                            {
                                LoaiSanPhamDTO loai = new LoaiSanPhamDTO
                                {
                                    MaLoai = DocDuLieu.GetInt32(0),
                                    TenLoai = DocDuLieu.IsDBNull(1) ? null : DocDuLieu.GetString(1)
                                };
                                danhSach.Add(loai);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Lỗi SQL trong LayTatCaLoai: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khác trong LayTatCaLoai: {ex.Message}");
                throw;
            }
            return danhSach;
        }

        public List<SanPhamDTO> LaySanPhamTheoLoai(int maLoai)
        {
            List<SanPhamDTO> DanhSachSanPham = new List<SanPhamDTO>();
            try
            {
                using (SqlConnection KetNoi = new SqlConnection(connectionString))
                {
                    KetNoi.Open();
                    string CauTruyVan = "SELECT MaSP, TenSP, DonGia, SoLuong, HinhAnh, MaLoai FROM SanPham WHERE MaLoai = @MaLoai";
                    using (SqlCommand Lenh = new SqlCommand(CauTruyVan, KetNoi))
                    {
                        Lenh.Parameters.AddWithValue("@MaLoai", maLoai);
                        using (SqlDataReader DocDuLieu = Lenh.ExecuteReader())
                        {
                            while (DocDuLieu.Read())
                            {
                                SanPhamDTO SanPham = new SanPhamDTO
                                {
                                    MaSP = DocDuLieu.IsDBNull(0) ? null : DocDuLieu[0].ToString(),
                                    TenSP = DocDuLieu.IsDBNull(1) ? null : DocDuLieu.GetString(1),
                                    DonGia = DocDuLieu.IsDBNull(2) ? 0 : DocDuLieu.GetDecimal(2),
                                    SoLuong = DocDuLieu.IsDBNull(3) ? 0 : DocDuLieu.GetInt32(3),
                                    HinhAnh = DocDuLieu.IsDBNull(4) ? null : (byte[])DocDuLieu[4],
                                    MaLoai = DocDuLieu.IsDBNull(5) ? 0 : DocDuLieu.GetInt32(5)
                                };
                                DanhSachSanPham.Add(SanPham);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Lỗi SQL trong LaySanPhamTheoLoai: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khác trong LaySanPhamTheoLoai: {ex.Message}");
                throw;
            }
            return DanhSachSanPham;
        }

        public int ThemLoai(string tenLoai)
        {
            try
            {
                if (string.IsNullOrEmpty(tenLoai))
                {
                    throw new ArgumentException("Tên loại không được để trống!");
                }

                using (SqlConnection KetNoi = new SqlConnection(connectionString))
                {
                    KetNoi.Open();
                    string CauTruyVan = "INSERT INTO LoaiSanPham (TenLoai) OUTPUT INSERTED.MaLoai VALUES (@TenLoai)";
                    using (SqlCommand Lenh = new SqlCommand(CauTruyVan, KetNoi))
                    {
                        Lenh.Parameters.AddWithValue("@TenLoai", tenLoai);
                        return (int)Lenh.ExecuteScalar();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Lỗi SQL trong ThemLoai: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khác trong ThemLoai: {ex.Message}");
                throw;
            }
        }

        public void XoaLoaiKhongCoSanPham()
        {
            try
            {
                using (SqlConnection KetNoi = new SqlConnection(connectionString))
                {
                    KetNoi.Open();
                    string query = @"
                        DELETE FROM LoaiSanPham 
                        WHERE MaLoai NOT IN (SELECT MaLoai FROM SanPham)";
                    using (SqlCommand Lenh = new SqlCommand(query, KetNoi))
                    {
                        Lenh.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Lỗi SQL trong XoaLoaiKhongCoSanPham: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khác trong XoaLoaiKhongCoSanPham: {ex.Message}");
                throw;
            }
        }
    }
}