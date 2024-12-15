insert into roles values('manager','active')
insert into roles values('admin','active')
insert into roles values('staff','active')

insert into users values('staff','Nguyen Van A','12345','abc','123456789','staff@gmail.com','2003-02-02',7,'active',3)
insert into users values('staff2','Nguyen Van A','12345','abc','123456789','staff@gmail.com','2003-02-02',6,'active',3)
insert into users values('staff3','Nguyen Van A','12345','abc','123456789','staff@gmail.com','2003-02-02',4,'active',3)
insert into users values('admin','Nguyen Van B','12345','abc','987654321','admin@gmail.com','2003-03-02',0,'active',2)
insert into users values('manager','Nguyen Van C','12345','abc','1122334455','manager@gmail.com','2003-04-02',0,'active',1)

insert into customers values('Nguyen Van D','0865429351','D@gmail.com','active')
insert into customers values('Nguyen Van E','0865429352','E@gmail.com','active')
insert into customers values('Nguyen Van F','0865429353','F@gmail.com','active')


INSERT INTO  goldTypes VALUES ( '24K Gold', 5000000, 5200000,'2024-01-01', 'active')
insert into goldTypes  values ( '18K Gold', 3000.00, 3200.00, '2024-01-03','active')
insert into goldTypes  values ( '19K Gold', 3000.00, 3200.00,'2024-01-02' ,'inactive')

INSERT INTO categories VALUES ('Rings', 'active')
INSERT INTO categories VALUES  ( 'Necklaces', 'active');
select * from products

INSERT INTO Products VALUES( 1, 1,'KHN000001', 'Gold Ring', 'A beautiful 24K gold ring', 'ring.jpg', 0, 1.0, 200000,1.3, 6000000, 'S', 6,'active')
INSERT INTO products VALUES( 2, 2,'KHN000002', 'Gold Necklace', 'Elegant 18K gold necklace', 'necklace.jpg', 500, 1.3, 300000,1.4, 7000000, 'M', 4,'active')
INSERT INTO products VALUES( 2, 2,'KHN000003', 'Gold Necklace 1', 'Elegant 18K gold necklace', 'necklace.jpg', 500, 1.3, 300000,1.5, 7000000, 'M',3, 'active')
INSERT INTO products VALUES( 1, 1,'KHN000004', 'Gold Ring', 'A beautiful 24K gold ring', 'ring.jpg', 299, 1.0, 200000,1.3, 6000000, 'S', 6,'active')
 INSERT INTO products VALUES( 2, 2,'KHN000005', 'Gold Necklace', 'Elegant 18K gold necklace', 'necklace.jpg', 500, 1.3, 300000,1.4, 7000000, 'M', 4,'active')
INSERT INTO products VALUES( 2, 2,'KHN000006', 'Gold Necklace 1', 'Elegant 18K gold necklace', 'necklace.jpg', 500, 1.3, 300000,1.5, 7000000, 'M',3, 'active')
INSERT INTO products VALUES( 1, 1,'KHN000007', 'Gold Ring', 'A beautiful 24K gold ring', 'ring.jpg', 299, 1.0, 200000,1.3, 6000000, 'S', 6,'active')
INSERT INTO products VALUES( 2, 2,'KHN000008', 'Gold Necklace', 'Elegant 18K gold necklace', 'necklace.jpg', 500, 1.3, 300000,1.4, 7000000, 'M', 4,'active')
INSERT INTO products VALUES( 2, 2,'KHN000009', 'Gold Necklace 1', 'Elegant 18K gold necklace', 'necklace.jpg', 500, 1.3, 300000,1.5, 7000000, 'M',3, 'active')


INSERT INTO stones VALUES
    ( 1,'Diamond', 'Gemstone', 1500000,'active',1, 'White'),
    (2, 'Ruby', 'Gemstone', 1000000,'active' ,1,'Red'),
	 (3, 'Ruby', 'Gemstone', 1200000,'active' ,1,'blue')



