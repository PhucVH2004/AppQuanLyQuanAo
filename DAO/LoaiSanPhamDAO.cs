using DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class LoaiSanPhamDAO
    {
        private string connectionString = "Data Source=LAPTOP-H0J8DGVP\\SQLEXPRESS;Initial Catalog=QLQA;Integrated Security=True;Encrypt=False";

        public List<LoaiSanPhamDTO> LayTatCaLoaiSanPham()
        {
            List<LoaiSanPhamDTO> danhSachLoai = new List<LoaiSanPhamDTO>();
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(connectionString))
                {
                    string query = "SELECT MaLoai, TenLoai FROM LoaiSanPham";
                    SqlCommand lenh = new SqlCommand(query, ketNoi);
                    ketNoi.Open();
                    SqlDataReader reader = lenh.ExecuteReader();
                    while (reader.Read())
                    {
                        LoaiSanPhamDTO loai = new LoaiSanPhamDTO
                        {
                            MaLoai = reader.GetInt32(0),
                            TenLoai = reader.GetString(1)
                        };
                        danhSachLoai.Add(loai);
                    }
                    reader.Close();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi SQL khi lấy danh sách loại sản phẩm: {ex.Message}");
            }
            return danhSachLoai;
        }

        public void ThemLoaiSanPham(string tenLoai)
        {
            try
            {
                if (string.IsNullOrEmpty(tenLoai))
                    throw new ArgumentException("Tên loại không được để trống!");

                using (SqlConnection ketNoi = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO LoaiSanPham (TenLoai) VALUES (@TenLoai)";
                    SqlCommand lenh = new SqlCommand(query, ketNoi);
                    lenh.Parameters.AddWithValue("@TenLoai", tenLoai);
                    ketNoi.Open();
                    lenh.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi SQL khi thêm loại sản phẩm: {ex.Message}");
            }
        }

        public LoaiSanPhamDTO LayLoaiSanPhamTheoTen(string tenLoai)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(connectionString))
                {
                    string query = "SELECT MaLoai, TenLoai FROM LoaiSanPham WHERE TenLoai = @TenLoai";
                    SqlCommand lenh = new SqlCommand(query, ketNoi);
                    lenh.Parameters.AddWithValue("@TenLoai", tenLoai);
                    ketNoi.Open();
                    SqlDataReader reader = lenh.ExecuteReader();
                    if (reader.Read())
                    {
                        return new LoaiSanPhamDTO
                        {
                            MaLoai = reader.GetInt32(0),
                            TenLoai = reader.GetString(1)
                        };
                    }
                    return null;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi SQL khi lấy loại sản phẩm theo tên: {ex.Message}");
            }
        }
    }
}