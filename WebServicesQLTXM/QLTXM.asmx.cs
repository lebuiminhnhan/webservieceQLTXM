using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebServicesQLTXM
{
    /// <summary>
    /// Summary description for QLTXM
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class QLTXM : System.Web.Services.WebService
    {
        QLTXMDataContext db = new QLTXMDataContext();

       
        // Xử lí thuê xe
        // danh sách hợp đồng thuê xe
        [WebMethod]
        public List<HOPDONGTHUE> DanhSachThue()
        {
            return db.HOPDONGTHUEs.ToList();
        }
        // danh sách hợp đồng thuê xe
        [WebMethod]
        public List<HOPDONGTHUE> DanhSachThueChuaThanhToan()
        {
            return db.HOPDONGTHUEs.Where(x=>x.TrangThai=="Đang Thuê").ToList();
        }
        // Danh sách xe chỉ hiển thị xe chưa thuê
        [WebMethod]
        public List<CHITIETXE1> DanhSachXe()
        {
            var query = from x in db.CHITIETXEs
                      
                        select new CHITIETXE1
                        {
                            MaXe= x.MaXE,
                            GiaThue=x.DonGia,
                            MucGiamGia = x.MucGiamGia,
                            LoaiXe = x.LOAIXE.TenLoai,
                            HangXe = x.NHACUNGCAP.TenNCC,
                            BangSo = x.BangSo,
                            MauSac= x.MauSac,
                            TrangThai = x.TrangThai
                        };
            return query.ToList();
                        
        }
        // cập nhật trạng thái xe
        public void CapNhatTrangThaiXe(int maxe)
        {
            CHITIETXE h = db.CHITIETXEs.Where(x => x.MaXE == maxe).FirstOrDefault();

            h.TrangThai = "Đang thuê";

            db.SubmitChanges();
        }
        // thêm hợp đồng thuê
        [WebMethod]
        public void ThemHopDongThue( int maxe, int makh, int manv, DateTime ngaythue, DateTime ngaytra)
        {
            HOPDONGTHUE h = new HOPDONGTHUE();
            CHITIETXE x = db.CHITIETXEs.Where(y => y.MaXE == maxe).FirstOrDefault();
         
            h.TrangThai = "Đang thuê";
            h.MaXe = maxe;
            h.GiaThue = x.DonGia;
            h.MucGiamGia = (int)x.MucGiamGia;
            h.MaKH = makh;
            h.MaNV = manv;
            h.TrangThai = "Đang Thuê";
            x.TrangThai = "Đang Thuê";
            h.NgayThue = ngaythue;
            h.NgayTra = ngaytra;
            db.HOPDONGTHUEs.InsertOnSubmit(h);
            db.SubmitChanges();
        }
        // xử lí trả xe
        // cập nhật  hợp đồng xe
        [WebMethod]
        public void CapNhatHopDong (int sddt)
        {
            HOPDONGTHUE h = db.HOPDONGTHUEs.Where(t=>t.SoDDT == sddt).FirstOrDefault();
            CHITIETXE x = db.CHITIETXEs.Where(y => y.MaXE == h.CHITIETXE.MaXE).FirstOrDefault();
            h.TrangThai = "Đã thanh toán";
            x.TrangThai = "Trống";
            db.SubmitChanges();
        }
        //thanh toan
        [WebMethod]
        public List<THANHTOAN> ThanhToanTien(int sddt)
        {
            var query = from x in db.HOPDONGTHUEs
                        where x.SoDDT == sddt
                        select new THANHTOAN
                        {
                            SoDDT = x.SoDDT,
                            HoTenKH = x.KHACHHANG.HoTen,
                            HoTenNV = x.NHANVIEN.HoTen,
                            GiaThueNgay = x.CHITIETXE.DonGia,
                            NgayThue = x.NgayThue,
                            NgayTra = x.NgayTra,
                            SoNgay = (x.NgayTra.Day - x.NgayThue.Day),
                            ThanhTien = ((x.NgayTra.Day - x.NgayThue.Day) * x.CHITIETXE.DonGia) - (x.CHITIETXE.DonGia*x.CHITIETXE.MucGiamGia)/100
                        };
            return query.ToList();

        }
    }

    public class THANHTOAN
    {
        public int SoDDT { get;  set; }
        public string HoTenKH { get;  set; }
        public string HoTenNV { get;  set; }
        public int? GiaThueNgay { get;  set; }
        public DateTime? NgayTra { get;  set; }
        public int SoNgay { get;  set; }
        public DateTime? NgayThue { get;  set; }
        public double? ThanhTien { get;  set; }
        
    }

    // class khởi tạo get dữ liệu
    public class CHITIETXE1
    {
        public int MaXe { get;  set; }
        public int GiaThue { get;  set; }
        public int? MucGiamGia { get;  set; }
        public string LoaiXe { get;  set; }
        public string HangXe { get;  set; }
        public string BangSo { get;  set; }
        public string MauSac { get;  set; }
        public string TrangThai { get;  set; }
    }
}
