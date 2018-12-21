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
        //đếm xe có lượt thuê cao
        [WebMethod]
        public List<HOPDONGTHUE1> DemLuot(int ngaythue)
        {
            
            var query = from h in db.HOPDONGTHUEs
                        where h.NgayThue.Month == ngaythue
                        group h by h.CHITIETXE.BangSo into g
                        select new HOPDONGTHUE1
                        {
                            Xe = g.Key,
                            SoLuot = g.Count()
                        };
            return query.ToList();
        }
        //
        [WebMethod]
        public List<THANHTOAN> ThongKeTienTam(int thang)
        {
            var query = (from x in db.HOPDONGTHUEs
                        where x.TrangThai == "Đã thanh toán" && x.NgayThue.Month == thang
                        select new THANHTOAN
                        {
                            SoDDT = x.SoDDT,
                            BangSo = x.CHITIETXE.BangSo,
                            HoTenKH = x.KHACHHANG.HoTen,
                            HoTenNV = x.NHANVIEN.HoTen,
                            GiaThueNgay = x.CHITIETXE.DonGia,
                            NgayThue = x.NgayThue,
                            NgayTra = x.NgayTra,
                            SoNgay = (x.NgayTra.Day - x.NgayThue.Day),
                            ThanhTien = ((x.NgayTra.Day - x.NgayThue.Day) * x.CHITIETXE.DonGia) - (x.CHITIETXE.DonGia * x.CHITIETXE.MucGiamGia) / 100
                        }).GroupBy(y=>y.BangSo).Select(g=>new THANHTOAN {BangSo = g.Key, ThanhTien = g.Sum(t=>t.ThanhTien)}).ToList();
            
            return query;

        }
        //
       
      
        [WebMethod]
        public string checkUser(string mail)
        {
            var l = db.User_Users.Where(u => u.Email == mail).Select(x => x.IdRole).First();
            var j = db.USer_Roles.Where(rl => rl.Id == int.Parse(l.ToString())).Select(rl => rl.RoleName).First();
            var n = db.User_Users.Where(u => u.Email == mail).Select(x => x.UserName).First();
            return j.ToString();
          
        }
        //
        [WebMethod]
        public string checknameGG(string mail)
        {
            var l = db.User_Users.Where(u => u.Email == mail).Select(x => x.IdRole).First();
            var j = db.USer_Roles.Where(rl => rl.Id == int.Parse(l.ToString())).Select(rl => rl.RoleName).First();
            var n = db.User_Users.Where(u => u.Email == mail).Select(x => x.UserName).First();
            return n.ToString();

        }
        [WebMethod]
        public string checknameTT(string username)
        {
            var l = db.User_Users.Where(u => u.UserName == username).Select(x => x.IdRole).First();
            var j = db.USer_Roles.Where(rl => rl.Id == int.Parse(l.ToString())).Select(rl => rl.RoleName).First();
            var n = db.User_Users.Where(u => u.UserName == username).Select(x => x.UserName).First();

            return n.ToString();

        }
        [WebMethod]
        public string checkAdmin(string username)
        {
            var l = db.User_Users.Where(u => u.UserName == username).Select(x => x.IdRole).First();
            var j = db.USer_Roles.Where(rl => rl.Id == int.Parse(l.ToString())).Select(rl => rl.RoleName).First();
            var n = db.User_Users.Where(u => u.UserName == username).Select(x => x.UserName).First();
            
            return j.ToString();

        }
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
        public void DangKiTT(string name, string mail, string password)
        {
            User_User u = new User_User();
            u.Id = u.Id = ran.Next(999999999).ToString(); 
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
            return db.CHITIETXEs.OrderByDescending(x => x.MaXE).Where(y=>y.TrangThai=="Trống" || y.TrangThai == "Đang đặt").ToList();
        }
        // Nhân Viên
        [WebMethod]
        public List<NHANVIEN> DanhSachNhanVien()
        {
            return db.NHANVIENs.OrderByDescending(x => x.MaNV).ToList();
        }
        // Khách hàng 
        [WebMethod]
        public List<KHACHHANG1> DanhSachKhachHang()
        {
            var query = (from x in db.KHACHHANGs
                        select new KHACHHANG1
                        {
                            TenKH = x.HoTen + " có SĐT " + x.DienThoai + " ở " + x.DiaChi,
                            MaKH = x.MaKH
                        }).ToList();
            return query;
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
                            TenBangSo = x.TenXE + " có biển số " + x.BangSo +" có màu "+x.MauSac,
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
        public void ThemDatXe(int maxe, DateTime ngaydat, string tenkh, string gioitinh, string namsinh, string diachi,  string cmnd, string mail, string dt)
        {
           
            KHACHHANG k = new KHACHHANG();
           
           
            k.HoTen = tenkh;
            k.NamSinh = namsinh;
            k.CMND = cmnd;
            k.GioiTinh = gioitinh;
            k.Email = mail;
            k.DiaChi = diachi;
            k.DienThoai = dt;
            db.KHACHHANGs.InsertOnSubmit(k);
            db.SubmitChanges();
            CHITIETXE x = db.CHITIETXEs.Where(y => y.MaXE == maxe).FirstOrDefault();
            HOPDONGDATTRUOC h = new HOPDONGDATTRUOC();
            x.TrangThai = "Đang đặt";
            h.MaXe = maxe;
            h.MaHDDT = ran.Next(999999);
            int makh = k.MaKH;
            h.MaKH = makh;
            h.TrangThai = "Đang đặt";
            h.NgayDenThue = ngaydat;
            db.HOPDONGDATTRUOCs.InsertOnSubmit(h);
          
            db.SubmitChanges();
        }
        // Hợp đồng đặt trước
        [WebMethod]
        public List<HOPDONGDATTRUOC2> DanhSachHopDongDatTruoc()
        {
            var query = from h in db.HOPDONGDATTRUOCs
                        where h.TrangThai == "Đang đặt"
                        select new HOPDONGDATTRUOC2
                        {
                            MaDD = h.MaHDDT,
                            Xe = h.CHITIETXE.BangSo,
                            TenKH = h.KHACHHANG.HoTen,
                            DiaChi = h.KHACHHANG.DiaChi,
                            SDT = h.KHACHHANG.DienThoai,
                            NgayDat = h.NgayDenThue,
                            TrangThai = h.TrangThai
                        };
            return query.ToList();

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
        public void ThemChiTietXe(string tenxe, string bienso, int mucgiamgia, int giathue, int malx, int mancc, string mausac,string mota)
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
            x.Mota = mota;
            db.CHITIETXEs.InsertOnSubmit(x);
            db.SubmitChanges();
        }
        // Sửa chi tiết xe
        [WebMethod]
        public void XeChiTietXe(int maxe,string tenxe, string bienso, int mucgiamgia, int giathue, int malx, int mancc, string mausac, string trangthai)
        {

            CHITIETXE x = db.CHITIETXEs.Where(y => y.MaXE == maxe).FirstOrDefault();
            x.TenXE = tenxe;
            x.BangSo = bienso;
            x.DonGia = giathue;
            x.MaLX = malx;
            x.MaNCC = mancc;
            x.MucGiamGia = mucgiamgia;
            x.TrangThai = trangthai;
            x.MauSac = mausac;
          
            db.SubmitChanges();
        }
        // cập nhật trạng thái xe
        [WebMethod]
        public void CapNhatTrangThaiXeTrong(string bangsoxe)
        {
            CHITIETXE h = db.CHITIETXEs.Where(x => x.BangSo == bangsoxe).FirstOrDefault();
            HOPDONGDATTRUOC d = db.HOPDONGDATTRUOCs.Where(x => x.CHITIETXE.BangSo == bangsoxe).FirstOrDefault();
            h.TrangThai = "Trống";
            d.TrangThai = "Đã hủy";
            db.SubmitChanges();
        }
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
            HOPDONGDATTRUOC d = db.HOPDONGDATTRUOCs.Where(c =>c.CHITIETXE.MaXE  == maxe).FirstOrDefault();
            if(d==null)
            {
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
            else
            {
                d.TrangThai = "Đang thuê";
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
        ///// giao diện chính
        ///Trang home
        /// show xe tay ga
        [WebMethod]
        public List<XE_ANH1> ShowChiTiet(int maxe)
        {
            var query = from a in db.XE_ANHs
                        where a.CHITIETXE.MaXE == maxe
                        select new XE_ANH1
                        {
                            Id = a.HINHANH.Id,
                            Name = a.HINHANH.Name,
                            Link = a.HINHANH.Link,
                            GiaThue= a.CHITIETXE.DonGia,
                            MucGiam = a.CHITIETXE.MucGiamGia,
                            BangSo= a.CHITIETXE.BangSo,
                            LoaiXe=a.CHITIETXE.LOAIXE.TenLoai,
                            HangXe=a.CHITIETXE.NHACUNGCAP.TenNCC,
                            Mota = a.CHITIETXE.Mota
                        };
            return query.ToList();
        }
        // show tay ga
        [WebMethod]
        public List<CHITIETXE3> DanhSachTayGaTam(int maXe)
        {
            var query = from x in db.CHITIETXEs 
                        where x.LOAIXE.TenLoai == "Tay Ga"  && x.MaXE == maXe
                        join a in db.XE_ANHs on x.MaXE equals a.MaXe

                        select new CHITIETXE3
                        {
                            MaXe = x.MaXE,
                            TenXe = x.TenXE,
                            GiaThue = x.DonGia,
                            MucGiamGia = x.MucGiamGia,
                            LoaiXe = x.LOAIXE.TenLoai,
                            HangXe = x.NHACUNGCAP.TenNCC,
                            BangSo = x.BangSo,
                            MauSac = x.MauSac,
                            TrangThai = x.TrangThai,
                            Id = a.HINHANH.Id,
                            Name = a.HINHANH.Name,
                            Link = a.HINHANH.Link,
                        };

           

            return query.Take(1).ToList();

        }
        // show tay ga
        [WebMethod]
        public List<CHITIETXE3> DanhSachTayGa()
        {
            var query = from x in db.CHITIETXEs 
                        where x.LOAIXE.TenLoai == "Tay Ga" && x.TrangThai == "Trống"
                        join a in db.XE_ANHs on x.MaXE equals a.MaXe
                        group x by x.MaXE into g
                        select new MoiNE
                        {
                            MaXe = g.Key,

                        };
            List<CHITIETXE3> xeMoi = new List<CHITIETXE3>();
            foreach (var i in query)
            {
                xeMoi = xeMoi.Concat(DanhSachTayGaTam(i.MaXe)).ToList();
            }
            return xeMoi.ToList();

        }
        //show xe số
        [WebMethod]
        public List<CHITIETXE3> DanhSachXeSoTam(int maXe)
        {
            var query = from x in db.CHITIETXEs
                        where x.LOAIXE.TenLoai == "Xe Số" && x.MaXE == maXe
                        join a in db.XE_ANHs on x.MaXE equals a.MaXe

                        select new CHITIETXE3
                        {
                            MaXe = x.MaXE,
                            TenXe = x.TenXE,
                            GiaThue = x.DonGia,
                            MucGiamGia = x.MucGiamGia,
                            LoaiXe = x.LOAIXE.TenLoai,
                            HangXe = x.NHACUNGCAP.TenNCC,
                            BangSo = x.BangSo,
                            MauSac = x.MauSac,
                            TrangThai = x.TrangThai,
                            Id = a.HINHANH.Id,
                            Name = a.HINHANH.Name,
                            Link = a.HINHANH.Link,
                        };



            return query.Take(1).ToList();

        }
        // show tay ga
        [WebMethod]
        public List<CHITIETXE3> DanhSachXeSo()
        {
            var query = from x in db.CHITIETXEs 
                        where x.LOAIXE.TenLoai == "Xe Số" && x.TrangThai == "Trống"
                        join a in db.XE_ANHs on x.MaXE equals a.MaXe
                        group x by x.MaXE into g
                        select new MoiNE
                        {
                            MaXe = g.Key,

                        };
            List<CHITIETXE3> xeMoi = new List<CHITIETXE3>();
            foreach (var i in query)
            {
                xeMoi = xeMoi.Concat(DanhSachXeSoTam(i.MaXe)).ToList();
            }
            return xeMoi.ToList();

        }
        // show xe côn tay
        public List<CHITIETXE3> DanhSachConTayTam(int maXe)
        {
            var query = from x in db.CHITIETXEs
                        where x.LOAIXE.TenLoai == "Côn Tay" && x.MaXE == maXe
                        join a in db.XE_ANHs on x.MaXE equals a.MaXe

                        select new CHITIETXE3
                        {
                            MaXe = x.MaXE,
                            TenXe = x.TenXE,
                            GiaThue = x.DonGia,
                            MucGiamGia = x.MucGiamGia,
                            LoaiXe = x.LOAIXE.TenLoai,
                            HangXe = x.NHACUNGCAP.TenNCC,
                            BangSo = x.BangSo,
                            MauSac = x.MauSac,
                            TrangThai = x.TrangThai,
                            Id = a.HINHANH.Id,
                            Name = a.HINHANH.Name,
                            Link = a.HINHANH.Link,
                        };



            return query.Take(1).ToList();

        }
        // show tay ga
        [WebMethod]
        public List<CHITIETXE3> DanhSachConTay()
        {
            var query = from x in db.CHITIETXEs
                        where x.LOAIXE.TenLoai == "Côn Tay" && x.TrangThai == "Trống"
                        join a in db.XE_ANHs on x.MaXE equals a.MaXe
                        group x by x.MaXE into g
                        select new MoiNE
                        {
                            MaXe = g.Key,

                        };
            List<CHITIETXE3> xeMoi = new List<CHITIETXE3>();
            foreach (var i in query)
            {
                xeMoi = xeMoi.Concat(DanhSachConTayTam(i.MaXe)).ToList();
            }
            return xeMoi.ToList();

        }
        /// show in home
        ///  [WebMethod]
        public List<CHITIETXE3> DanhSachTatCaTam(int maXe)
        {
            var query = from x in db.CHITIETXEs
                        where  x.MaXE == maXe
                        join a in db.XE_ANHs on x.MaXE equals a.MaXe

                        select new CHITIETXE3
                        {
                            MaXe = x.MaXE,
                            TenXe = x.TenXE,
                            GiaThue = x.DonGia,
                            MucGiamGia = x.MucGiamGia,
                            LoaiXe = x.LOAIXE.TenLoai,
                            HangXe = x.NHACUNGCAP.TenNCC,
                            BangSo = x.BangSo,
                            MauSac = x.MauSac,
                            TrangThai = x.TrangThai,
                            Id = a.HINHANH.Id,
                            Name = a.HINHANH.Name,
                            Link = a.HINHANH.Link,
                        };



            return query.Take(1).ToList();

        }
        // show tatca
        [WebMethod]
        public List<CHITIETXE3> DanhSachTatCa(string tenxe)
        {
            var query = (from x in db.CHITIETXEs
                        where  x.TrangThai == "Trống"
                        join a in db.XE_ANHs on x.MaXE equals a.MaXe
                        group x by x.TenXE into g
                        select new MoiNE
                        {
                            TenXe = g.Key,

                        });
            List<CHITIETXE3> xeMoi = new List<CHITIETXE3>();
            foreach (var i in query)
            {
                xeMoi = xeMoi.Concat(DanhSachTatCaTam(i.MaXe)).ToList();
            }
            return xeMoi.Where(y => y.TenXe.Contains(tenxe)).ToList();

        }
        
        //tim kiếm
        [WebMethod]
        public List<CHITIETXE3> TimKiem(int maXe, string tenxe)
        {
            var query = from x in db.CHITIETXEs
                      
                        join a in db.XE_ANHs on x.MaXE equals a.MaXe
                        where x.TrangThai == "Trống" && a.HINHANH.Name.Contains(tenxe) && x.MaXE == maXe

                        select new CHITIETXE3
                        {
                            MaXe = x.MaXE,
                            TenXe = x.TenXE,
                            GiaThue = x.DonGia,
                            MucGiamGia = x.MucGiamGia,
                            LoaiXe = x.LOAIXE.TenLoai,
                            HangXe = x.NHACUNGCAP.TenNCC,
                            BangSo = x.BangSo,
                            MauSac = x.MauSac,
                            TrangThai = x.TrangThai,
                            Id = a.HINHANH.Id,
                            Name = a.HINHANH.Name,
                            Link = a.HINHANH.Link,
                        };
            return query.Take(1).ToList();

        }
        //tim kiếm
        [WebMethod]
        public List<CHITIETXE3> TimKiem2(string tenxe)
        {
            var query = from x in db.CHITIETXEs

                        join a in db.XE_ANHs on x.MaXE equals a.MaXe
                        where x.TrangThai == "Trống" && a.HINHANH.Name.Contains(tenxe)
                        group x by x.MaXE into g
                        select new MoiNE
                        {
                            MaXe = g.Key,
                            
                        };
            List<CHITIETXE3> xeMoi = new List<CHITIETXE3>();
            foreach(var i in query)
            {
                xeMoi = xeMoi.Concat(TimKiem(i.MaXe, tenxe)).ToList();
            }
            return xeMoi;

        }
        // Thêm khách hàng
        [WebMethod]
        public List<HOPDONGDATTRUOC1> DanhSachDat(int maxe)
        {
            var query = from d in db.HOPDONGDATTRUOCs
                        where d.MaXe == maxe
                        select new HOPDONGDATTRUOC1
                        {
                            SoDD = d.MaHDDT,
                            HoTenKH = d.KHACHHANG.HoTen,
                            NgayDat = d.NgayDenThue,
                            
                        };
            return query.ToList();
        }
        // Sưa thông tin xe
        [WebMethod]
        public void SuaChiTietXe()
        {

        }

    }

    public class HOPDONGDATTRUOC2
    {
        public int MaDD { get; set; }
        public string Xe { get; set; }
        public string TenKH { get; set; }
        public string DiaChi { get; set; }
        public string SDT { get; set; }
        public DateTime NgayDat { get; set; }
        public string TrangThai { get; set; }
    }

    public class MoiNE1
    {
        public string MaXe { get; set; }
        public DateTime NgayThue { get; set; }
        public string BangSo { get;  set; }
    }

    public class HOPDONGTHUE1
    {
        public string Xe { get;  set; }
        public int SoLuot { get;  set; }
    }

    public class HOPDONGDATTRUOC1
    {
        public int SoDD { get; set; }
        public string HoTenKH { get; set; }
        public DateTime NgayDat { get; set; }
    }

    public class KHACHHANG1
    {
        public string TenKH { get; set; }
        public int MaKH { get; set; }
    }

    public class MoiNE
    {
        public int MaXe { get; set; }
        public string TenXe { get;  set; }
    }

    public class CHITIETXE3
    {
        public int MaXe { get; set; }

        public string TenXe { get; set; }

        public int GiaThue { get; set; }

        public int? MucGiamGia { get; set; }

        public string LoaiXe { get; set; }

        public string HangXe { get; set; }

        public string BangSo { get; set; }

        public string MauSac { get; set; }

        public string TrangThai { get; set; }
        public int Id { get;  set; }
        public string Name { get;  set; }
        public string Link { get; set; }
    }

    public class XE_ANH1
    {
        public int Id { get;  set; }
        public string Link { get;  set; }
        public string Name { get;  set; }
        public int GiaThue { get;  set; }
        public int? MucGiam { get;  set; }
        public string BangSo { get;  set; }
        public string LoaiXe { get;  set; }
        public string HangXe { get;  set; }
        public string Mota { get;  set; }
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
        public string BangSo { get;  set; }
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
