using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class GiaoDichDTO
    {
        public int GiaoDichID { get; set; }
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public DateTime NgayGiaoDich { get; set; }
        public bool DaXoa { get; set; } // Thêm trường DaXoa để hỗ trợ soft delete
    }
}