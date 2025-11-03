using BUS;
using DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class FrmSuaAnh : Form
    {
        private string maSP;
        private FrmKho frmKho;
        private SanPhamBUS sanPhamBUS = new SanPhamBUS();
        private byte[] hinhAnhMoi;

        public FrmSuaAnh(string maSP, FrmKho frmKho)
        {
            InitializeComponent();
            this.maSP = maSP;
            this.frmKho = frmKho;
            LoadAnhHienTai();
        }

        private void LoadAnhHienTai()
        {
            SanPhamDTO sanPham = sanPhamBUS.LaySanPhamTheoMa(maSP);
            if (sanPham != null && sanPham.HinhAnh != null && sanPham.HinhAnh.Length > 0)
            {
                using (var ms = new MemoryStream(sanPham.HinhAnh))
                {
                    pictureBox1.Image = Image.FromStream(ms);
                }
            }
        }

        private void btnChonAnh_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Chọn ảnh mới"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                using (MemoryStream ms = new MemoryStream())
                {
                    pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                    hinhAnhMoi = ms.ToArray();
                }
            }
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (hinhAnhMoi != null)
            {
                SanPhamDTO sanPham = sanPhamBUS.LaySanPhamTheoMa(maSP);
                if (sanPham != null)
                {
                    sanPham.HinhAnh = hinhAnhMoi;
                    sanPhamBUS.CapNhatSanPham(sanPham);
                    MessageBox.Show("Đã cập nhật ảnh thành công!");
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn ảnh mới!");
            }
        }
    }
}
