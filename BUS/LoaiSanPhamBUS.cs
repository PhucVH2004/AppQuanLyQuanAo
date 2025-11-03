using DAO;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class LoaiSanPhamBUS
    {
        private LoaiSanPhamDAO loaiSanPhamDAO = new LoaiSanPhamDAO();

        public List<LoaiSanPhamDTO> LayTatCaLoaiSanPham()
        {
            try
            {
                return loaiSanPhamDAO.LayTatCaLoaiSanPham();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách loại sản phẩm từ BUS: {ex.Message}");
            }
        }

        public void ThemLoaiSanPham(string tenLoai)
        {
            try
            {
                loaiSanPhamDAO.ThemLoaiSanPham(tenLoai);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm loại sản phẩm từ BUS: {ex.Message}");
            }
        }

        public LoaiSanPhamDTO LayLoaiSanPhamTheoTen(string tenLoai)
        {
            try
            {
                return loaiSanPhamDAO.LayLoaiSanPhamTheoTen(tenLoai);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy loại sản phẩm theo tên từ BUS: {ex.Message}");
            }
        }
    }
}