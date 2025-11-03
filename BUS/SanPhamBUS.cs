using DAO;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class SanPhamBUS
    {
        private SanPhamDAO SanPhamDAO = new SanPhamDAO(); // Khởi tạo đối tượng DAO

        public List<SanPhamDTO> LayTatCaSanPham()
        {
            try
            {
                return SanPhamDAO.LayTatCaSanPham();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách sản phẩm từ BUS:\n" +
                                    $"Thông báo: {ex.Message}\n" +
                                    $"Chi tiết: {ex.StackTrace}\n" +
                                    $"Nguồn: {ex.Source}");
            }
        }

        public void ThemSanPham(SanPhamDTO sanPham)
        {
            try
            {
                if (sanPham == null)
                {
                    throw new ArgumentNullException(nameof(sanPham), "Đối tượng SanPhamDTO không được null!");
                }
                SanPhamDAO.ThemSanPham(sanPham);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm sản phẩm {sanPham?.MaSP} từ BUS:\n" +
                                    $"Thông báo: {ex.Message}\n" +
                                    $"Chi tiết: {ex.StackTrace}\n" +
                                    $"Nguồn: {ex.Source}");
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
                SanPhamDAO.CapNhatSanPham(sanPham);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật sản phẩm {sanPham?.MaSP} từ BUS:\n" +
                                    $"Thông báo: {ex.Message}\n" +
                                    $"Chi tiết: {ex.StackTrace}\n" +
                                    $"Nguồn: {ex.Source}");
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
                return SanPhamDAO.LaySanPhamTheoMa(maSP);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sản phẩm theo mã {maSP} từ BUS:\n" +
                                    $"Thông báo: {ex.Message}\n" +
                                    $"Chi tiết: {ex.StackTrace}\n" +
                                    $"Nguồn: {ex.Source}");
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
                SanPhamDAO.XoaSanPham(maSP);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa sản phẩm {maSP} từ BUS:\n" +
                                    $"Thông báo: {ex.Message}\n" +
                                    $"Chi tiết: {ex.StackTrace}\n" +
                                    $"Nguồn: {ex.Source}");
            }
        }
        public List<SanPhamDTO> TimKiemSanPham(string tuKhoa)
        {
            try
            {
                if (string.IsNullOrEmpty(tuKhoa))
                {
                    throw new ArgumentException("Từ khóa tìm kiếm không được để trống!");
                }
                return SanPhamDAO.TimKiemSanPham(tuKhoa);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm sản phẩm với từ khóa {tuKhoa} từ BUS:\n" +
                                    $"Thông báo: {ex.Message}\n" +
                                    $"Chi tiết: {ex.StackTrace}\n" +
                                    $"Nguồn: {ex.Source}");
            }
        }
    }
}
