CREATE DATABASE QuanLyNhaHang
GO

USE QuanLyNhaHang
GO

-- Food
-- Table
-- FoodCategory
-- Account
-- Bill
-- BillInfo

CREATE TABLE TableFood
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL ,
	status NVARCHAR(100) NOT NULL DEFAULT N'Trống'	-- Trống || Có người
)
GO

CREATE TABLE Account
(
	UserName NVARCHAR(100) PRIMARY KEY,	
	DisplayName NVARCHAR(100) NOT NULL DEFAULT N'admin',
	PassWord1 NVARCHAR(1000) NOT NULL DEFAULT 0,
	Type INT NOT NULL  DEFAULT 0 -- 1: admin && 0: staff
)
GO

CREATE TABLE FoodCategory
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên'
)
GO

CREATE TABLE Food
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên',
	idCategory INT NOT NULL,
	price FLOAT NOT NULL DEFAULT 0
	
	FOREIGN KEY (idCategory) REFERENCES dbo.FoodCategory(id)
)
GO

CREATE TABLE Bill
(
	id INT IDENTITY PRIMARY KEY,
	DateCheckIn DATE NOT NULL DEFAULT GETDATE(),
	DateCheckOut DATE,
	idTable INT NOT NULL,
	status INT NOT NULL DEFAULT 0 -- 1: đã thanh toán && 0: chưa thanh toán
	
	FOREIGN KEY (idTable) REFERENCES dbo.TableFood(id)
)
GO

CREATE TABLE BillInfo
(
	id INT IDENTITY PRIMARY KEY,
	idBill INT NOT NULL,
	idFood INT NOT NULL,
	count INT NOT NULL DEFAULT 0
	
	FOREIGN KEY (idBill) REFERENCES dbo.Bill(id),
	FOREIGN KEY (idFood) REFERENCES dbo.Food(id)
)
GO

INSERT [dbo].[Account] ([UserName], [DisplayName], [PassWord1], [Type]) VALUES (N'admin', N'Phan Duc Duy', N'123456', 1)
INSERT [dbo].[Account] ([UserName], [DisplayName], [PassWord1], [Type]) VALUES (N'staff', N'staff', N'123456', 0)
go

Create proc USP_GetAccountByUsername
@userName nvarchar(100) out
as
begin
select * from dbo.Account where UserName = @userName
end
go


declare @i int = 1
while @i <= 15
begin
insert TableFood (name, status)
values (N'Bàn ' + cast(@i as nvarchar(100)), N'Trống')
set @i = @i + 1
end
go

create proc USP_GetTableList
as
begin
 Select * from TableFood
 end
go

create proc USP_GetTableListByStatus
as
begin
 Select * from TableFood where status = N'Trống'
 end
go

--thêm loại đồ ăn
SET IDENTITY_INSERT [dbo].[FoodCategory] ON
INSERT [dbo].[FoodCategory] ([id], [name]) VALUES (1, N'Khai vị')
INSERT [dbo].[FoodCategory] ([id], [name]) VALUES (2, N'Món chính')
INSERT [dbo].[FoodCategory] ([id], [name]) VALUES (3, N'Tráng miệng')
INSERT [dbo].[FoodCategory] ([id], [name]) VALUES (4, N'Đồ uống')
SET IDENTITY_INSERT [dbo].[FoodCategory] OFF

--thêm đồ ăn
SET IDENTITY_INSERT [dbo].[Food] ON
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (1, N'Gỏi ngó sen tôm thịt', 1, 60000)
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (2, N'Sườn kinh đô', 1, 120000)
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (3, N'Gà Xào Hạt Điều', 1, 75000)
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (4, N'Giò hoa khai vị', 1, 80000)
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (5, N'Salad ức ngan hun khói', 1, 100000)

INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (6, N'Vịt nướng', 2, 120000)
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (7, N'Heo rừng kho tiêu xanh', 2, 90000)
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (8, N'Mực ống nhồi thịt', 2, 150000)
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (9, N'Gà hấp dầu mè', 2, 130000)
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (10, N'Cá diêu hồng hấp xì dầu', 2, 95000)

INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (11, N'Chè hương cốm lá dứa', 3, 20000)
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (12, N'Bánh cupcake sầu riêng', 3, 20000)
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (13, N'Bánh trôi nước', 3, 15000)
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (14, N'Trái cây dầm', 3, 30000)
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (15, N'Bánh đậu đỏ hấp', 3, 25000)

INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (16, N'Coca cola', 4, 12000)
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (17, N'Pepsi', 4, 12000)
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (18, N'7UP', 4, 15000)
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (19, N'Sprite', 4, 20000)
INSERT [dbo].[Food] ([id], [name], [idCategory], [price]) VALUES (20, N'Merinda', 4, 12000)


alter table Bill
add discount int
go

create proc USP_InsertBill
 @idTable int
as
begin
insert Bill (DateCheckIn, DateCheckOut, idTable, status, discount)
values(Getdate(), null, @idTable, 0, 0)
end
go

create proc USP_InsertBillInfo
@idBill int, @idFood int, @count int
as
begin
declare @isExitsBillInfo int
declare @foodCount int = 1
select @isExitsBillInfo = id, @foodCount = b.count 
from BillInfo as b 
where idBill = @idBill and idFood = @idFood
if(@isExitsBillInfo > 0)
begin
declare @newCount int = @foodCount + @count
if (@newCount > 0) update BillInfo set count = @foodCount + @count where idFood = @idFood
else delete BillInfo where idBill = @idBill and idFood = @idFood
end
else
begin
insert BillInfo (idBill, idFood, count)
values (@idBill, @idFood, @count)
end
end
go


create TRIGGER UTG_UpdateBillInfo
ON BillInfo FOR INSERT, UPDATE
AS
BEGIN
	DECLARE @idBill INT
	
	SELECT @idBill = idBill FROM Inserted
	
	DECLARE @idTable INT
	
	SELECT @idTable = idTable FROM Bill WHERE id = @idBill AND status = 0

	declare @count int
	select @count = count(*) from BillInfo where idBill = @idBill

	if(@count > 0) UPDATE dbo.TableFood SET status = N'Có người' WHERE id = @idTable
	else Update TableFood set status = N'Trống' where id = @idTable
END
GO

create trigger UTG_UpdateTable
on TableFood for update
as
begin
   declare @idtable int
   declare @status nvarchar(100)
   select @idTable = id, @status = Inserted.status from Inserted

   declare @idBill int
   select @idBill = id from Bill where idTable = @idTable and status = 0

   declare @countBillInfo int
   select @countBillInfo = count (*) from BillInfo where idBill = @idBill

   if(@countBillInfo > 0 and @status <> N'Có người') update TableFood set status = N'Có người' where id = @idTable
   else if(@countBillInfo <= 0 and @status <> N'Trống') update TableFood set status = N'Trống' where id = @idTable
end
go

create TRIGGER UTG_UpdateBill
ON dbo.Bill FOR UPDATE
AS
BEGIN
	DECLARE @idBill INT
	
	SELECT @idBill = id FROM Inserted	
	
	DECLARE @idTable INT
	
	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill
	
	DECLARE @count int = 0
	
	SELECT @count = COUNT(*) FROM dbo.Bill WHERE idTable = @idTable AND status = 0
	
	IF (@count = 0)
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable
END
GO


update Bill set discount = 0


select * from Bill 


declare @idBillNew int = 1

select id into IDBillInfoTable from BillInfo where idBill = @idBillNew


declare @idBillOld int = 2

update BillInfo set idBill = @idBillOld where id in (select * from IDBillInfoTable)
go

create proc USP_SwitchTable
@idTable1 int,
@idTable2 int
as
begin
   declare @idFirstBill int
   declare @idSecondBill int

   declare @isFirstTableEmpty int = 1
   declare @isSecondTableEmpty int = 1

   select @idSecondBill = id from Bill where idTable = @idTable2 and status = 0
   select @idFirstBill = id from Bill where idTable = @idTable1 and status = 0


   if(@idFirstBill is null)
   begin
      insert into Bill (DateCheckIn, DateCheckOut, idTable, status)
	  values(GETDATE(), null, @idTable1, 0)

	  select @idFirstBill = max(id) From Bill
	
   end

   select @isFirstTableEmpty = count (*)from BillInfo where idBill =@idFirstBill

   if(@idSecondBill is null)
   begin
      insert into Bill (DateCheckIn, DateCheckOut, idTable, status)
	  values(GETDATE(), null, @idTable2, 0)

	  select @idSecondBill = max(id) From Bill
   end

   select @isSecondTableEmpty = count(*) from BillInfo where idBill = @idSecondBill

   select id into IDBillInfoTable  from BillInfo where idBill = @idSecondBill

   update BillInfo set idBill = @idSecondBill where idBill = @idFirstBill

   update BillInfo set idBill = @idFirstBill where id in (select * from IDBillInfoTable)
    
	drop table IDBillInfoTable

	if(@isFirstTableEmpty = 0) update TableFood set status = N'Trống' where id = @idTable2
	if(@isSecondTableEmpty = 0) update TableFood set status = N'Trống' where id = @idTable1
end

alter table Bill add totalPrice float

create proc USP_GetListBillByDate
@checkIn date, @checkOut date
as
begin
SELECT t.name AS [Tên bàn], DateCheckIn AS [Ngày vào], DateCheckOut AS [Ngày ra], b.totalPrice AS [Giá tiền (nghìn đồng)], discount AS [Giảm giá (%)], (totalPrice - (totalPrice * discount / 100)) as [Tổng tiền (nghìn đồng)] 
	FROM dbo.Bill AS b,dbo.TableFood AS t
	WHERE DateCheckIn >= @checkIn AND DateCheckOut <= @checkOut AND b.status = 1
	AND t.id = b.idTable
end
go

create proc USP_UpdateAccount
@userName nvarchar(100), @displayName nvarchar(100), @password nvarchar(100), @newPassword nvarchar(100)
as
begin
declare @isRightPass int
select @isRightPass = count(*) from Account where UserName = @userName and PassWord1 = @password

if(@isRightPass = 1)
begin
if(@newPassword = null or @newPassword = '')
begin
update Account set DisplayName = @displayName where UserName = @userName
end
else update Account set DisplayName = @displayName, PassWord1 = @password where UserName =@userName
end
end
go


CREATE TRIGGER UTG_DeleteBillInfo
ON dbo.BillInfo FOR DELETE
AS 
BEGIN
	DECLARE @idBillInfo INT
	DECLARE @idBill INT
	SELECT @idBillInfo = id, @idBill = Deleted.idBill FROM Deleted
	
	DECLARE @idTable INT
	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill
	
	DECLARE @count INT = 0
	
	SELECT @count = COUNT(*) FROM dbo.BillInfo AS bi, dbo.Bill AS b WHERE b.id = bi.idBill AND b.id = @idBill AND b.status = 0
	
	IF (@count = 0)
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable
END
GO


create proc USP_GetListBillByDateAndPage
@checkIn date, @checkOut date, @page int
as
begin
declare @pageRows int = 10
declare @selectRows int = @pageRows
declare @exceptRows int = (@page - 1) * @pageRows

    ;with BillShow  as (SELECT b.id, t.name AS [Tên bàn], b.totalPrice AS [Giá tiền], DateCheckIn AS [Ngày vào], DateCheckOut AS [Ngày ra], discount AS [Giảm giá],(totalPrice - (totalPrice * discount / 100)) as [Tổng tiền]
	FROM dbo.Bill AS b,dbo.TableFood AS t
	WHERE DateCheckIn >= @checkIn AND DateCheckOut <= @checkOut AND b.status = 1
	AND t.id = b.idTable)

	select top (@selectRows) * from BillShow where id not in
	
	(select top (@exceptRows) id from BillShow)
	
end
go

create proc USP_GetNumBillByDate
@checkIn date, @checkOut date
as
begin
SELECT count(*)
	FROM dbo.Bill AS b,dbo.TableFood AS t
	WHERE DateCheckIn >= @checkIn AND DateCheckOut <= @checkOut AND b.status = 1
	AND t.id = b.idTable
end
go


CREATE FUNCTION [dbo].[fuConvertToUnsign1] ( @strInput NVARCHAR(4000) ) RETURNS NVARCHAR(4000) AS BEGIN IF @strInput IS NULL RETURN @strInput IF @strInput = '' RETURN @strInput DECLARE @RT NVARCHAR(4000) DECLARE @SIGN_CHARS NCHAR(136) DECLARE @UNSIGN_CHARS NCHAR (136) SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý ĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍ ÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ' +NCHAR(272)+ NCHAR(208) SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeee iiiiiooooooooooooooouuuuuuuuuuyyyyy AADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIII OOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD' DECLARE @COUNTER int DECLARE @COUNTER1 int SET @COUNTER = 1 WHILE (@COUNTER <=LEN(@strInput)) BEGIN SET @COUNTER1 = 1 WHILE (@COUNTER1 <=LEN(@SIGN_CHARS)+1) BEGIN IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) = UNICODE(SUBSTRING(@strInput,@COUNTER ,1) ) BEGIN IF @COUNTER=1 SET @strInput = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)-1) ELSE SET @strInput = SUBSTRING(@strInput, 1, @COUNTER-1) +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)- @COUNTER) BREAK END SET @COUNTER1 = @COUNTER1 +1 END SET @COUNTER = @COUNTER +1 END SET @strInput = replace(@strInput,' ','-') RETURN @strInput END