using System;
using System.Collections.Generic;
using System.IO;
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
        // Đăng nhập
        [WebMethod]
        public void DangNhapGG(string mail)
        {
            var l = db.User_Users.Where(u => u.Email == mail).Select(x => x.IdRole).First();
            var j = db.USer_Roles.Where(rl => rl.Id == int.Parse(l.ToString())).Select(rl => rl.RoleName).First();
            var n = db.User_Users.Where(u => u.Email == mail).Select(x => x.UserName).First();
            Session["name"] = n;
            Session["role"] = j;
        }
        // Đăng nhập TT
        [WebMethod]
        public bool DangNhapTT(string name, string pass)
        {
            if (db.User_Users.Where(x => name == x.UserName && x.Password == mahoa(pass)).First() != null)
            {
                var l = db.User_Users.Where(u => u.UserName == name).Select(x => x.IdRole).First();
                var j = db.USer_Roles.Where(rl => rl.Id == int.Parse(l.ToString())).Select(rl => rl.RoleName).First();
                var n = db.User_Users.Where(u => u.UserName == name).Select(x => x.UserName).First();
                Session["name"] = n;
                Session["role"] = j;
                if ((string)Session["role"] == "Admin")
                {
                    return true;
                }
                else
                {

                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }

        }

        // Đăng kí
        // Đăng kí Google
        [WebMethod]
        public void DangKiGG(string name, string mail, string id)
        {

            User_User u = new User_User();
            u.Id = id;
            u.UserName = name;
            u.Email = mail;
            u.IdRole = 2;
            db.User_Users.InsertOnSubmit(u);
            db.SubmitChanges();


        }
        Random ran = new Random();
        // Đăng kí thông thường
        [WebMethod]
        public void DangKiTT(string name, string mail, string password, string id)
        {
            User_User u = new User_User();
            u.Id = id;
            u.UserName = name;
            u.Email = mail;
            u.Password = mahoa(password);
            u.IdRole = 2;
            db.User_Users.InsertOnSubmit(u);
            db.SubmitChanges();
        }
        // Danh sách các thuộc tính
        // chi tiết xe drop
        [WebMethod]
        public List<CHITIETXE> DanhSachXe()
        {
            return db.CHITIETXEs.OrderByDescending(x => x.MaXE).Where(y=>y.TrangThai=="Trống").ToList();
        }
        // Nhân Viên
        [WebMethod]
        public List<NHANVIEN> DanhSachNhanVien()
        {
            return db.NHANVIENs.OrderByDescending(x => x.MaNV).ToList();
        }
        // Khách hàng 
        [WebMethod]
        public List<KHACHHANG> DanhSachKhachHang()
        {
            return db.KHACHHANGs.OrderByDescending(x=>x.MaKH).ToList();
        }
        // Loại xe
        [WebMethod]
        public List<LOAIXE> DanhSachLoaiXe()
        {
            return db.LOAIXEs.OrderByDescending(x => x.MaLoai).ToList();
        }
        // Hãng xe
        [WebMethod]
        public List<NHACUNGCAP> DanhSachHangXe()
        {
            return db.NHACUNGCAPs.OrderByDescending(x => x.MaNCC).ToList();
        }
        // upload ảnh
        [WebMethod]
        public List<CHITIETXE2> DanhSachAllXe()
        {
            var query = from x in db.CHITIETXEs
                        select new CHITIETXE2
                        {
                            TenBangSo = x.TenXE + " có biển số " + x.BangSo,
                            MaXe = x.MaXE
                        };
            return query.ToList();
        }
        [WebMethod]
        public void UploadHinhAnh(string file, string name, int maxe)
        {
            HINHANH h = new HINHANH();
            
              
            h.Link = file;
            h.Name = name;
           
            db.HINHANHs.InsertOnSubmit(h);
            db.SubmitChanges();
            int idanh = h.Id;
            XE_ANH x = new XE_ANH();
            x.IdAnh = idanh;
            x.MaXe = maxe;
            db.XE_ANHs.InsertOnSubmit(x);
            db.SubmitChanges();

        }
         [WebMethod]
        public void UploadXeAnh(int maxe, int idanh)
        {
            XE_ANH x = new XE_ANH();
            x.IdAnh = idanh;
            x.MaXe = maxe;
            db.XE_ANHs.InsertOnSubmit(x);
            db.SubmitChanges();
        }
        // Xử lí đặt xe
        // Thêm đơn đặt xe
        [WebMethod]
        public void ThemDatXe(int maxe, DateTime ngaydat, string trangthai, int makh)
        {
            HOPDONGDATTRUOC h = new HOPDONGDATTRUOC();
            CHITIETXE x = db.CHITIETXEs.Where(y => y.MaXE == maxe).FirstOrDefault();
            x.TrangThai = "Đang đặt";
            h.MaXe = maxe;
            h.MaKH = makh;
            h.TrangThai = "Đang đặt";
            h.NgayDenThue = ngaydat;
            db.HOPDONGDATTRUOCs.InsertOnSubmit(h);
            db.SubmitChanges();
        }
        // Hợp đồng đặt trước
        [WebMethod]
        public List<HOPDONGDATTRUOC> DanhSachHopDongDatTruoc()
        {
            return db.HOPDONGDATTRUOCs.OrderByDescending(x => x.MaKH).ToList();
        }
        // Xử lí thuê xe
        // danh sách hợp đồng thuê xe
        [WebMethod]
        public List<HOPDONGTHUE> DanhSachThue()
        {
            return db.HOPDONGTHUEs.OrderByDescending(x => x.SoDDT).ToList();
        }
        // danh sách hợp đồng thuê xe
        [WebMethod]
        public List<HOPDONGTHUE> DanhSachThueChuaThanhToan()
        {
            return db.HOPDONGTHUEs.Where(x=>x.TrangThai=="Đang Thuê").OrderByDescending(x => x.MaKH).ToList();
        }
        // Danh sách xe chỉ hiển thị xe chưa thuê
        [WebMethod]
        public List<CHITIETXE1> DanhSachKhoXe()
        {
            var query = from x in db.CHITIETXEs
                      
                        select new CHITIETXE1
                        {
                            MaXe= x.MaXE,
                            TenXe = x.TenXE,
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
        // Thêm chi tiết xe
        [WebMethod]
        public void ThemChiTietXe(string tenxe, string bienso, int mucgiamgia, int giathue, int malx, int mancc, string mausac)
        {

            CHITIETXE x = new CHITIETXE();
            x.TenXE = tenxe;
            x.BangSo = bienso;
            x.DonGia = giathue;
            x.MaLX = malx;
            x.MaNCC = mancc;
            x.MucGiamGia = mucgiamgia;
            x.TrangThai = "Trống";
            x.MauSac = mausac;
            db.CHITIETXEs.InsertOnSubmit(x);
            db.SubmitChanges();
        }
        // Sửa chi tiết xe
        [WebMethod]
        public void XeChiTietXe(int maxe,string tenxe, string bienso, int mucgiamgia, int giathue, int malx, int mancc, string mausac)
        {

            CHITIETXE x = db.CHITIETXEs.Where(y => y.MaXE == maxe).FirstOrDefault();
            x.TenXE = tenxe;
            x.BangSo = bienso;
            x.DonGia = giathue;
            x.MaLX = malx;
            x.MaNCC = mancc;
            x.MucGiamGia = mucgiamgia;
            x.TrangThai = "Trống";
            x.MauSac = mausac;
          
            db.SubmitChanges();
        }
        // cập nhật trạng thái xe
        [WebMethod]
        public void CapNhatTrangThaiXe(int maxe)
        {
            CHITIETXE h = db.CHITIETXEs.Where(x => x.MaXE == maxe).FirstOrDefault();

            h.TrangThai = "Đang thuê";

            db.SubmitChanges();
        }
        // loc hop dong thue
        [WebMethod]
        public List<HOPDONGTHUE> LocHopDongThue(string trangthai)
        {
            return db.HOPDONGTHUEs.Where(x => x.TrangThai == trangthai).OrderByDescending(x => x.MaKH).ToList();

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

        public string mahoa(string pass)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(pass.Trim(), "SHA1");
        }
    }

    public class CHITIETXE2
    {
        public string TenBangSo { get; set; }
        public int MaXe { get;  set; }
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

        public string TenXe { get; set; }

        public int GiaThue { get;  set; }

        public int? MucGiamGia { get;  set; }

        public string LoaiXe { get;  set; }

        public string HangXe { get;  set; }

        public string BangSo { get;  set; }

        public string MauSac { get;  set; }

        public string TrangThai { get;  set; }

       
    }
}
