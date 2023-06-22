 create database QuanLyQuanCafe
 GO

 USE QuanLyQuanCafe
 GO
-- Món ăn
-- Bàn ăn
-- Loại món ăn
-- Tài Khoản
-- Hóa đơn
-- Chi tiết hóa đơn


--Bàn
CREATE TABLE TableFood
(
	id INT IDENTITY PRIMARY KEY,
	tableName NVARCHAR (100) NOT NULL DEFAULT N'Bàn chưa có tên',
	status NVARCHAR (100) NOT NULL DEFAULT N'Bàn trống',
	
)
GO

create table Account
(
	UserName NVARCHAR(100) PRIMARY KEY,
	DisplayName NVARCHAR(100) NOT NULL DEFAULT N'Ritter',
	Password NVARCHAR(1000) NOT NULL DEFAULT 0, 
	Type INT NOT NULL DEFAULT 0,
)
GO

CREATE TABLE FoodCategory
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) DEFAULT N'Chưa đặt tên',
)
GO

CREATE TABLE Food
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên',
	idCategory INT NOT NULL,
	price INT  NOT NULL,

	FOREIGN KEY (idCategory) REFERENCES dbo.FoodCategory(id),
)
GO

CREATE TABLE Bill
(
	id INT IDENTITY PRIMARY KEY,
	DateCheckIn DATE DEFAULT GETDATE(),
	DateCheckOut Date,
	idTable INT NOT NULL,
	status INT NOT NULL DEFAULT 0, -- 1 la da thanh toan , 0 la chua thanh toan

	FOREIGN KEY (idTable) REFERENCES dbo.TableFood(id),

)
GO

CREATE TABLE BillInfo
(
	id INT IDENTITY PRIMARY KEY,
	idBill INT NOT NULL,
	idFood INT NOT NULL,
	count INT NOT NULL DEFAULT 0,

	FOREIGN KEY (idBill) REFERENCES dbo.Bill(id),
	FOREIGN KEY (idFood) REFERENCES dbo.Food(id),
)
GO
-------------------

INSERT INTO dbo.Account
		(
			UserName,
			DisplayName,
			Password,
			Type
		)
VALUES (N'Kouta',
		N'koutarou',
		N'123456',
		1
		)
INSERT INTO dbo.Account
		(
			UserName,
			DisplayName,
			Password,
			Type
		)
VALUES (N'Nanami',
		N'Nanami',
		N'123',
		0
		)
Go
------------------------
CREATE PROC USP_GetAccountByUserName
@username nvarchar(100)
AS
BEGIN
	SELECT * FROM dbo.Account WHERE UserName = @username
END
GO
----------------
SELECT * FROM dbo.Account WHERE UserName = N'Kouta' AND Password = N'123456'
GO
----các PROC cần thiết để tránh xảy ra lỗi SQL
CREATE PROC USP_Login
@username nvarchar(100),
@password nvarchar(100)
AS
BEGIN
	SELECT * FROM dbo.Account WHERE UserName = @username AND Password = @password
END
GO
--------THêm bàn ăn

DECLARE @i INT = 0
WHILE @i <= 10
BEGIN
	INSERT dbo.TableFood(tableName, status) VALUES(N'Bàn ' + CAST(@i AS nvarchar(100)),DEFAULT)
	SET @i = @i + 1
END
GO
---------------
CREATE PROC USP_GetTableList
AS SELECT * FROM dbo.TableFood
GO

--Để hiện thị thông tin hóa đơn khi nhấp vào bàn
--Cần thêm thông tin từ các bảng như"
	--Bill
	--BiiInfo
	--Food
	--FoodCategory
--thêm Category
INSERT dbo.FoodCategory (name) VALUES(N'Trà sữa')
INSERT dbo.FoodCategory (name) VALUES(N'Cà phê')
INSERT dbo.FoodCategory (name) VALUES(N'Trà')
INSERT dbo.FoodCategory (name) VALUES(N'Nước ngọt')
INSERT dbo.FoodCategory (name) VALUES(N'Trà trái cây')
INSERT dbo.FoodCategory (name) VALUES(N'Nước suối')
INSERT dbo.FoodCategory (name) VALUES(N'Bánh')
INSERT dbo.FoodCategory (name) VALUES(N'topping')
select * from dbo.FoodCategory
--Thêm món ăn 
INSERT dbo.Food (name,idCategory,price) VALUES(N'Trà sữa trân châu đường đen',1,45000)
INSERT dbo.Food (name,idCategory,price) VALUES(N'Purple Latte',2,50000)
INSERT dbo.Food (name,idCategory,price) VALUES(N'Trà hoa cúc mật ong',3,35000)
INSERT dbo.Food (name,idCategory,price) VALUES(N'Warrior',4,20000)
INSERT dbo.Food (name,idCategory,price) VALUES(N'Trà xanh xoài',5,44000)
INSERT dbo.Food (name,idCategory,price) VALUES(N'Nước suối Lavie',6,6000)
INSERT dbo.Food (name,idCategory,price) VALUES(N'Creme Brulee',7,55000)
INSERT dbo.Food (name,idCategory,price) VALUES(N'Trân châu đường đen',8,15000)
--thêm bill
INSERT dbo.Bill (DateCheckIn,DateCheckOut,idTable,status) Values(GETDATE(),null,1,0)
INSERT dbo.Bill (DateCheckIn,DateCheckOut,idTable,status) Values(GETDATE(),null,2,0)
INSERT dbo.Bill (DateCheckIn,DateCheckOut,idTable,status) Values(GETDATE(),GETDATE(),3,1)
-- thêm Bill Info
INSERT dbo.BillInfo (idBill,idFood,count) Values(1,1,2)
INSERT dbo.BillInfo (idBill,idFood,count) Values(1,2,4)
INSERT dbo.BillInfo (idBill,idFood,count) Values(2,4,3)
INSERT dbo.BillInfo (idBill,idFood,count) Values(2,7,3)
INSERT dbo.BillInfo (idBill,idFood,count) Values(3,3,4)
INSERT dbo.BillInfo (idBill,idFood,count) Values(3,6,4)

select * from dbo.Bill where idTable = 3 and status = 1
SELECT * FROM dbo.BillInfo WHERE idBill = 3

SELECT f.name, bi.count, f.price, f.price*bi.count AS totalPrice FROM dbo.BillInfo AS bi, dbo.Bill AS b, Food AS f
WHERE bi.idBill = b.id AND b.id = 0 AND bi.idFood = f.id AND b.idTable = 3
GO
 -------------------
 -- thêm bill vào bàn
 create proc USP_InsertBill
 @idTable INT
 as
 begin
	INSERT Bill
			(
				DateCheckIn,
				DateCheckOut,
				idTable,
				status,
				discount
			)
	VALUES  (
				GETDATE(),
				NULL,
				@idTable,
				0,
				0
			)
 end
 go

 --Tạo bill Info
 -- Rất chi là mệt với mấy câu truy vấn này 
 create proc USP_InsertBillInfo
 @idBill int,
 @idFood int,
 @count int
 AS
 BEGIN
	declare @isExistBillInfo int --khỏi tại 1 biến tến như bên => dùng để ktra có tồn tại hay ko
	declare @countFood INT = 1; --tạo 1 cái biến tên như bên để lưu giá trị số lượng của food

	select @isExistBillInfo = id, @countFood = count --chọn 2 cái bên 
	from	dbo.BillInfo  -- từ bảng BillInfo
	WHERE idBill = @idBill AND idFood = @idFood -- với điều kiện này

	if(@isExistBillInfo > 0) --neus như có tồn tại cái bill rồi
	BEGIN
		DECLARE @newCount INT = @countFood + @count --khởi tạo 1 biến số lượng mới để lưu số lượng khi cộng dồn
		IF(@newCount > 0) -- nếu mà biến số lượng dương (tứng là có trường hợp trừ bớt phần đã gọi đi
															--(tức là trượng hợp mà cout nhập vào là âm)
			--nếu nó không âm thì cập nhật lại số lượng món đó
			--cộng thêm vào count 
			UPDATE dbo.BillInfo SET count = @countFood + @count WHERE idFood = @idFood
		ELSE-- ngược lại nếu nó âm
			--thì mình xóa luôn cái món đó OK????
			DELETE dbo.BillInfo WHERE idBill = @idBill AND idFood = @idFood
	END
	ELSE
	BEGIN
		insert dbo.BillInfo
			(
				idBill,
				idFood,
				count
			)
		values  (
					@idBill,
					@idFood,
					@count
				)
	END
 END
 GO
 --Quớ mệt mũi với cái câu truy vấn này 
 ---------
--Tạo tringger cho việc update status của table
create trigger UTG_UpdateBillInfo
ON dbo.BillInfo For insert , update
as
begin
	declare @idBill INT --khỏi tạo biến idBill => được nhập vào

	select @idBill = idBill From inserted --lấy cái @idBill tương ứng với idBill trong SQL trong bản inserted
	
	declare @idTable INT --khỏi tạo biến @idTable

	--chọn cái @idTable tương ứng vs idTable trong SQL từ bản Bill , với điều kiện là:
	--id của bàn = @idTable dc nhập vào và status của bàn là 0
	select @idTable = idTable from dbo.Bill where id = @idBill and status = 0 --chưa checkOut

	declare @count int
	select @count = count(*) from BillInfo where idBill = @idBill

	if(@count > 0)
		BEGIN
			--thức hiện update lại giá trị status của bàn
			update dbo.TableFood set status = N'Có người' where id = @idTable
		END
	else
		BEGIN
			update dbo.TableFood set status = N'Bàn trống' where id = @idTable
		END
	
end
go
-----------------------

-------------
create trigger UTG_UpdateBill
on dbo.Bill for update
as
begin
	--mấy cái declare thì tương tự ở trên để tạo thôi
	declare @idBill INT 

	select @idBill = id From inserted -- chọn @idBill = vs id trong bảng inserted 
	--bảng inserted là 1 trong 2 bảng của trigger

	declare @idTable INT
	--chọn @idTable(nhập vào) mà = idTable(trong SQL) từ bản Bill khi mà id(của Bill) = @idBill(nhập vào)
	select @idTable = idTable from dbo.Bill where id = @idBill

	declare @count int = 0
	--lấy ra cái số lượng bill của cái bàn dựa vào id bàn và status = 0 tức ch thanh toán
	select @count = COUNT(*) from dbo.Bill where idTable = @idTable and status = 0

	if(@count = 0) -- nếu ko có bill nào
		--update lại cái status của bàn thành bàn trống
		update dbo.TableFood set status = N'Bàn trống' where id = @idTable
end
go
----
create table dbo.Bill
add discount int

update dbo.Bill set discount = 0

select * from Bill
go
-----------chuyển bàn
alter proc USP_SwitchTable
@idTable1 int , @idTable2 int --bởi vì table có thể null còn billInfo thì ko nên ta sẽ truyền vào Proc 2 idTable
as
begin
	--Khỏi tạo 2 id Bill 
	--một số lưu ý nhỏ , mà thật ra có 1 lưu ý thôi
	-- ở đây hỏi ngược 1 tí @idFirstBill là cái id của bill cũ, còn @idSecondBill là của bill mới 
	--làm hơi ngược tí xiu
	declare @idFirstBil int 
	declare @idSecondBill int

	declare @isFirstTableEmpty int = 1
	declare @isSecondTableEmpty int = 1

	--cho @idSencondBill có giá trị bằng cái id bill hiện tại với điều kiện idTable hiện tại bằng @idTable 2(là cái table dag được ta chọn)
	--và nó phải là 1 cái bill chưa thanh toán
	select @idSecondBill = id from dbo.Bill where idTable = @idTable2 and status = 0
	--tương tự với cái trên nhưng lần này là id bill của cái bàn mà ta muốn chuyển tới
	select @idFirstBil = id from dbo.Bill where idTable = @idTable1 and status = 0


	--trong trường hợp nếu như 1 trong 2 cái bàn đó chưa có bill thì sao? thì cái idBill bị null
	--ta sẽ tiến hành tạo cái bill cho nó
	--trường hợp @idFirtBill bị Null
	if(@idFirstBil IS NULL)
	begin
		insert into dbo.Bill 
					(
						DateCheckIn,
						DateCheckOut,
						idTable,
						status
					)
				values(
						GETDATE(),
						NULL,
						@idTable1,
						0
					  )
		
		select @idFirstBil = MAX(id) from Bill where idTable = @idTable1 and status = 0
		
		
	end

	select @isFirstTableEmpty = COUNT(*) FROM dbo.BillInfo where idBill = @idFirstBil

	---trường hợp với idSecondBill
	if(@idSecondBill IS NULL)
	begin
		insert into dbo.Bill 
					(
						DateCheckIn,
						DateCheckOut,
						idTable,
						status
					)
				values(
						GETDATE(),
						NULL,
						@idTable2,
						0
					  )
		select @idSecondBill = MAX(id) from Bill where idTable = @idTable2 and status = 0
		
	end

	select @isSecondTableEmpty = COUNT(*) from dbo.BillInfo where idBill = @idSecondBill 

	--sau đó ta sẽ tiến hành lấy ra nhưng thằng id có điều kiện idBill = @idSecondBill
	-- cái IDBillInfoTable ở đây sẽ là một cái table tạm tao ra để lưu hết id của billInfo trong bàn hiện tại
	SELECT id INTO IDBillInfoTable FROM dbo.BillInfo where idBill = @idSecondBill
	--sau đó thì ta sẽ thay đổi giá trị của bill của bản đổi và cần đổi với nhau
	update dbo.BillInfo set idBill = @idSecondBill where idBill = @idFirstBil
	--rồi lấy dữ liệu từ cái IDBIllInfoTable chuyển qua cái bàn ta muốn đổi
	update dbo.BillInfo set idBill = @idFirstBil where id in (select * from IDBillInfoTable)
	--cuối cùng là bỏ cái IDBillInfoTable đi
	DROP TABLE IDBillInfoTable
	--nôm na thì nó như cái thuật toán đổi vị trí trong code thui
	--một lưu ý nhỏ khí sử dụng điệu kiện so sánh với null thì sử dụng "IS" nhớ nha

	if(@isFirstTableEmpty = 0)
		update dbo.TableFood set status = N'Bàn trống' where id = @idTable2

	if(@isSecondTableEmpty = 0)
		update dbo.TableFood set status = N'Bàn trống' where id = @idTable1
end
go

------------
alter table dbo.Bill add totalPrice float


delete dbo.BillInfo
delete dbo.Bill
go
--------------

--PROC cho hóa đơn đã thanh toán 

create proc USP_GetListBillByDate
@checkIn date, @checkOut date
as
begin
	select t.tableName as [Tên bàn], b.totalPrice as [Tổng tiền], DateCheckIn as [Ngày tạo], DateCheckOut as [Ngày thanh toán] , discount as [Giảm giá] 
	from dbo.Bill as b, dbo.TableFood as t 
	where DateCheckIn >= @checkIn and DateCheckOut <= @checkOut and b.status = 1
	and t.id = b.idTable
end
go
--------Account
create proc USP_UpdateAccount
@userName NVARCHAR(100), @displayName NVARCHAR(100),
@password NVARCHAR(100),@newPassword NVARCHAR(100)
as
begin
	declare @isRightPass int = 0

	select @isRightPass = COUNT(*) from dbo.Account where UserName = @userName and Password =@password

	if(@isRightPass >= 1)
	Begin
		IF(@newPassword = NULL or @newPassword ='')
		begin
			 update dbo.Account set DisplayName = @displayName where UserName = @userName
		end
		else
			update dbo.Account SET  DisplayName = @displayName, Password =@newPassword where UserName = @userName
	end
end
go

-------------
 
-------------trigger delete billInfo
alter trigger UTG_DeleteBillInfo
on dbo.BillInfo for delete
as
begin
	declare @idBillInfo int 
	declare @idBill int

	select @idBillInfo = id, @idBill = deleted.idBill from Deleted

	declare @idTable int
	select @idTable = idTable from dbo.Bill where id = @idBill
	
	declare @count int = 0
	select @count = count(*) from dbo.BillInfo as bi, dbo.Bill as b where b.id = bi.idBill and b.id = @idBill and status = 0

	if(@count = 0)
		update dbo.TableFood set status = N'Bàn trống' where id = @idTable
end
go

-------------- loại bỏ dấu trong TV
CREATE FUNCTION [dbo].[fuConvertToUnsign1]
(
 @strInput NVARCHAR(4000)
)
RETURNS NVARCHAR(4000)
AS
BEGIN 
 IF @strInput IS NULL RETURN @strInput
 IF @strInput = '' RETURN @strInput
 DECLARE @RT NVARCHAR(4000)
 DECLARE @SIGN_CHARS NCHAR(136)
 DECLARE @UNSIGN_CHARS NCHAR (136)
 SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế
 ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý
 ĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍ
 ÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ'
 +NCHAR(272)+ NCHAR(208)
 SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeee
 iiiiiooooooooooooooouuuuuuuuuuyyyyy
 AADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIII
 OOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD'
 DECLARE @COUNTER int
 DECLARE @COUNTER1 int
 SET @COUNTER = 1
 WHILE (@COUNTER <=LEN(@strInput))
 BEGIN 
 SET @COUNTER1 = 1
 WHILE (@COUNTER1 <=LEN(@SIGN_CHARS)+1)
 BEGIN
 IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1))
 = UNICODE(SUBSTRING(@strInput,@COUNTER ,1) )
 BEGIN 
 IF @COUNTER=1
 SET @strInput = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1)
 + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)-1) 
 ELSE
 SET @strInput = SUBSTRING(@strInput, 1, @COUNTER-1)
 +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1)
 + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)- @COUNTER)
 BREAK
 END
 SET @COUNTER1 = @COUNTER1 +1
 END
 SET @COUNTER = @COUNTER +1
 END
 SET @strInput = replace(@strInput,' ','-')
 RETURN @strInput
END

select * from dbo.Food where dbo.fuConvertToUnsign1(name) like N'%' + dbo.fuConvertToUnsign1(N'Tra') + '%'
go
----------------Category
create proc USP_GetListCattegory
as
begin
	select * from dbo.FoodCategory
end
go
-----------------
select Username, DisplayName, Type from dbo.Account
go

create proc USP_GetListBillByDateAndPage
@checkIn date, @checkOut date, @page int
as
begin
	declare @pageRows int = 10
	declare @selectRows int = @pageRows
	declare @excepRows int  = (@page - 1) * @pageRows


	;With BillShow AS ( select b.id, t.tableName as [Tên bàn], b.totalPrice as [Tổng tiền], DateCheckIn as [Ngày tạo], DateCheckOut as [Ngày thanh toán] , discount as [Giảm giá] 
	from dbo.Bill as b, dbo.TableFood as t 
	where DateCheckIn >= @checkIn and DateCheckOut <= @checkOut and b.status = 1
	and t.id = b.idTable)

	select TOP (@selectRows) * from BillShow where BillShow.id NOT IN (select top (@excepRows) id from BillShow)
end
go
-------------------

create proc USP_GetNumBillByDate
@checkIn date, @checkOut date
as
begin
	select COUNT(*)
	from dbo.Bill as b, dbo.TableFood as t 
	where DateCheckIn >= @checkIn and DateCheckOut <= @checkOut and b.status = 1
	and t.id = b.idTable
end
go