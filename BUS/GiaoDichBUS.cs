using DAO;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class GiaoDichBUS
    {
        private GiaoDichDAO giaoDichDAO = new GiaoDichDAO();

        public void LuuGiaoDich(GiaoDichDTO giaoDich)
        {
            try
            {
                if (giaoDich == null)
                {
                    throw new ArgumentNullException(nameof(giaoDich), "Đối tượng GiaoDichDTO không được null!");
                }
                giaoDichDAO.LuuGiaoDich(giaoDich);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu giao dịch từ BUS: {ex.Message}");
            }
        }

        public List<GiaoDichDTO> LayTatCaGiaoDich()
        {
            try
            {
                return giaoDichDAO.LayTatCaGiaoDich();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách giao dịch từ BUS: {ex.Message}");
            }
        }

        public void LuuDanhSachGiaoDich(List<GiaoDichDTO> danhSachGiaoDich)
        {
            try
            {
                if (danhSachGiaoDich == null || danhSachGiaoDich.Count == 0)
                {
                    throw new ArgumentNullException(nameof(danhSachGiaoDich), "Danh sách giao dịch không được rỗng!");
                }
                giaoDichDAO.LuuDanhSachGiaoDich(danhSachGiaoDich);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu danh sách giao dịch từ BUS: {ex.Message}");
            }
        }

        // Xóa giao dịch theo khoảng thời gian
        public void XoaGiaoDichTheoKhoangThoiGian(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            try
            {
                if (ngayBatDau > ngayKetThuc)
                {
                    throw new Exception("Ngày bắt đầu không được lớn hơn ngày kết thúc!");
                }
                giaoDichDAO.XoaGiaoDichTheoKhoangThoiGian(ngayBatDau, ngayKetThuc);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa giao dịch từ BUS: {ex.Message}");
            }
        }

        // Khôi phục giao dịch theo khoảng thời gian
        public void KhoiPhucGiaoDichTheoKhoangThoiGian(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            try
            {
                if (ngayBatDau > ngayKetThuc)
                {
                    throw new Exception("Ngày bắt đầu không được lớn hơn ngày kết thúc!");
                }
                giaoDichDAO.KhoiPhucGiaoDichTheoKhoangThoiGian(ngayBatDau, ngayKetThuc);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi khôi phục giao dịch từ BUS: {ex.Message}");
            }
        }
    }
}