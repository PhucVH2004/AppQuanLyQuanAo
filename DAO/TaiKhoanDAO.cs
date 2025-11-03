using DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAO
{
    public class TaiKhoanDAO
    {
        SqlConnection conn = new SqlConnection("Data Source=LAPTOP-H0J8DGVP\\SQLEXPRESS;Initial Catalog=QLQA;Integrated Security=True;Encrypt=False");

        public TaiKhoanDTO DangNhap(string tenDN, string matKhau)
        {
            string query = "SELECT TenDangNhap, MatKhau, VaiTro FROM TaiKhoan WHERE TenDangNhap = @ten AND MatKhau = @mk";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ten", tenDN);
            cmd.Parameters.AddWithValue("@mk", matKhau);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            TaiKhoanDTO tk = null;

            if (reader.Read())
            {
                tk = new TaiKhoanDTO()
                {
                    TenDangNhap = reader["TenDangNhap"].ToString(),
                    VaiTro = reader["VaiTro"].ToString()
                };
            }

            reader.Close();
            conn.Close();
            return tk;
        }

        public bool KiemTraTrungTaiKhoan(string tenDangNhap, string email)
        {
            string query = "SELECT COUNT(*) FROM TaiKhoan WHERE TenDangNhap = @TenDangNhap OR Email = @Email";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
            cmd.Parameters.AddWithValue("@Email", email);

            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            conn.Close();

            Console.WriteLine($"KiemTraTrungTaiKhoan: TenDangNhap={tenDangNhap}, Email={email}, Count={count}");
            return count > 0;
        }

        public bool DangKy(TaiKhoanDTO tk)
        {
            try
            {
                Console.WriteLine($"Connection String: {conn.ConnectionString}");
                bool isDuplicate = KiemTraTrungTaiKhoan(tk.TenDangNhap, tk.Email);
                Console.WriteLine($"DangKy: IsDuplicate={isDuplicate}, Email={tk.Email}, TenDangNhap={tk.TenDangNhap}, MatKhau={tk.MatKhau}, GioiTinh={tk.GioiTinh}, NgaySinh={tk.NgaySinh}");
                if (isDuplicate)
                {
                    Console.WriteLine("DangKy: Duplicate found, returning false.");
                    return false;
                }

                string query = "INSERT INTO TaiKhoan (Email, SDT, TenDangNhap, MatKhau, GioiTinh, NgaySinh, VaiTro) " +
                               "VALUES (@Email, @SDT, @Ten, @MatKhau, @GioiTinh, @NgaySinh, @VaiTro)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", tk.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SDT", tk.SDT ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Ten", tk.TenDangNhap ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MatKhau", tk.MatKhau ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GioiTinh", tk.GioiTinh ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NgaySinh", tk.NgaySinh != DateTime.MinValue ? tk.NgaySinh : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@VaiTro", tk.VaiTro ?? "user");

                conn.Open();
                int result = cmd.ExecuteNonQuery();
                Console.WriteLine($"DangKy: ExecuteNonQuery result={result}");
                conn.Close();

                return result > 0;
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                Console.WriteLine($"DangKy Error: {ex.Message}");
                throw new Exception("Lỗi khi đăng ký: " + ex.Message);
            }
        }

        public bool KiemTraTaiKhoan(string tenDangNhap, string email)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM TaiKhoan WHERE TenDangNhap = @TenDangNhap AND Email = @Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                conn.Close();

                Console.WriteLine($"KiemTraTaiKhoan (DAO): TenDangNhap={tenDangNhap}, Email={email}, Count={count}");
                return count > 0;
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                Console.WriteLine($"KiemTraTaiKhoan Error: {ex.Message}");
                throw new Exception("Lỗi khi kiểm tra tài khoản: " + ex.Message);
            }
        }

        public bool CapNhatMatKhau(string tenDangNhap, string matKhauMoi)
        {
            try
            {
                string query = "UPDATE TaiKhoan SET MatKhau = @MatKhau WHERE TenDangNhap = @TenDangNhap";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                cmd.Parameters.AddWithValue("@MatKhau", matKhauMoi);

                conn.Open();
                int result = cmd.ExecuteNonQuery();
                conn.Close();

                Console.WriteLine($"CapNhatMatKhau (DAO): TenDangNhap={tenDangNhap}, RowsAffected={result}");
                return result > 0;
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                Console.WriteLine($"CapNhatMatKhau Error: {ex.Message}");
                throw new Exception("Lỗi khi cập nhật mật khẩu: " + ex.Message);
            }
        }
    }
}
