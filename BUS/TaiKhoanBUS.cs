using DAO;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class TaiKhoanBUS
    {
        TaiKhoanDAO dao = new TaiKhoanDAO();

        public TaiKhoanDTO DangNhap(string tenDN, string mk)
        {
            return dao.DangNhap(tenDN, mk);
        }

        public bool DangKy(TaiKhoanDTO tk)
        {
            return dao.DangKy(tk);
        }

        public bool KiemTraTrungTaiKhoan(string tenDangNhap, string email)
        {
            return dao.KiemTraTrungTaiKhoan(tenDangNhap, email);
        }

        public bool KiemTraTaiKhoan(string tenDangNhap, string email)
        {
            try
            {
                bool result = dao.KiemTraTaiKhoan(tenDangNhap, email);
                Console.WriteLine($"KiemTraTaiKhoan (BUS): Result={result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KiemTraTaiKhoan (BUS) Error: {ex.Message}");
                throw;
            }
        }

        public bool CapNhatMatKhau(string tenDangNhap, string matKhauMoi)
        {
            try
            {
                bool result = dao.CapNhatMatKhau(tenDangNhap, matKhauMoi);
                Console.WriteLine($"CapNhatMatKhau (BUS): Result={result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapNhatMatKhau (BUS) Error: {ex.Message}");
                throw;
            }
        }
    }
}