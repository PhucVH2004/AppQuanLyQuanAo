using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class LoaiSanPhamDTO
    {
        public int MaLoai { get; set; } // Đảm bảo là int
        public string TenLoai { get; set; }
    }
}