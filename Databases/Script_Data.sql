Use HKShop;

select * from LOAI;

INSERT INTO NguoiDung (TenDangNhap, MatKhau, RandomKey, HieuLuc, VaiTro, NgayTao)
VALUES ('admin', 'c85218f435279e424defbc2c00d0cd40', 'fer5f', 1, 1, GETDATE());

Insert into KhachHang(MaKH, UserId, HoTen, GioiTinh, NgaySinh, Email, DiaChi, DienThoai, Hinh)
VALUES ('admin', SCOPE_IDENTITY(), 'Nguyen Van admin', 1, '2004-07-07', 'admin@example.com', '123 Admin St', '0123456789', NULL);

Insert into LOAI(TENLOAI, TENLOAIALIAS, MOTA, HINH)
Values ('Laptop', 'Laptop', 'Sản phẩm laptop chất lượng cao', NULL),
       ('Điện thoại', 'Dien-thoai', 'Sản phẩm điện thoại thông minh', NULL),
       ('Tai nghe', 'Tai-nghe', 'Thiết bị âm thanh đa dạng', NULL),
       ('Phụ kiện', 'Phu-kien', 'Phụ kiện điện tử đa dạng', NULL);

