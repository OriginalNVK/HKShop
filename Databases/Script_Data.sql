Use HKShop;

INSERT INTO AppUser (Username, Password, RandomKey, IsActive, Role, CreatedDate)
VALUES ('admin', 'c75e103e14e271ad3e5ceb674a4d6a29', 'fer5f', 1, 1, GETDATE());

Insert into Customer(Id, UserId, HoTen, GioiTinh, NgaySinh, Email, DiaChi, DienThoai, Hinh)
VALUES ('admin', SCOPE_IDENTITY(), 'Nguyen Van admin', 1, '2004-07-07', 'admin@example.com', '123 Admin St', '0123456789', NULL);

Insert into Category(Name, Description, Image)
Values ('Laptop', 'Product of technology', 'https://res.cloudinary.com/dst6r1cf6/image/upload/v1776182147/laptop_bscmjj.jpg'),
       ('SmartPhone', 'Product of SmartPhone', 'https://res.cloudinary.com/dst6r1cf6/image/upload/v1776182147/iphone_mqg9x7.jpg'),
       ('AirPhone', 'Product of AirPhone', 'https://res.cloudinary.com/dst6r1cf6/image/upload/v1776182147/airphone_h2f3hp.jpg'),
       ('Others', 'Product of Others', 'https://res.cloudinary.com/dst6r1cf6/image/upload/v1776182147/others_us9we1.png');

