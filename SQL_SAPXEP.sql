-- Database creation
CREATE DATABASE QuanLyQuanCafe
GO

USE QuanLyQuanCafe
GO

-- =============================================
-- TABLE CREATION
-- =============================================

-- Table for dining tables
CREATE TABLE TableFood 
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Bạn chưa có tên',
	status NVARCHAR(100) NOT NULL DEFAULT N'Trống'  -- TRỐNG || CÓ NGƯỜI
)
GO

-- User accounts table
CREATE TABLE Account
(
	UserName NVARCHAR(100) NOT NULL PRIMARY KEY,
	DisplayName NVARCHAR(100) NOT NULL DEFAULT N'Nhân viên',
	Password NVARCHAR(1000) NOT NULL DEFAULT 'c4ca4238a0b923820dcc509a6f75849b',
	Type INT NOT NULL DEFAULT 0 -- 1: admin && 0: staff
)
GO

-- Food category table
CREATE TABLE FoodCategory
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên'
)
GO

-- Food items table
CREATE TABLE Food
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên',
	idCategory INT NOT NULL,
	price INT NOT NULL DEFAULT 0,
	FOREIGN KEY (idCategory) REFERENCES dbo.FoodCategory(id)
)
GO

-- Bills table
CREATE TABLE Bill
(
	id INT IDENTITY PRIMARY KEY,
	DateCheckIn DATE NOT NULL DEFAULT GETDATE(),
	DateCheckOut DATE,
	idTable INT NOT NULL,
	status INT NOT NULL DEFAULT 0, -- 1: Đã thanh toán && 0: Chưa thanh toán
	discount INT,
	totalPrice FLOAT,
	FOREIGN KEY (idTable) REFERENCES dbo.TableFood(id)
)
GO

-- Bill details table
CREATE TABLE BillInfo
(
	id INT IDENTITY PRIMARY KEY,
	idBill INT NOT NULL,
	idFood INT NOT NULL,
	count INT NOT NULL DEFAULT 0,
	FOREIGN KEY (idBill) REFERENCES dbo.Bill(id),
	FOREIGN KEY (idFood) REFERENCES dbo.Food(id)
)
GO

-- =============================================
-- INITIAL DATA POPULATION
-- =============================================

-- Insert sample accounts
INSERT INTO dbo.Account (UserName, DisplayName, Password, Type)
VALUES 
	(N'DUYHOANG', N'DUYHOANG', N'1', 1),
	(N'staff', N'staff', N'1', 0)
GO

-- Insert sample tables
DECLARE @i INT = 0
WHILE @i <= 10
BEGIN 
	INSERT dbo.TableFood (name) VALUES (N'Bàn ' + CAST(@i AS nvarchar(100)))
	SET @i = @i + 1
END
GO

-- Insert food categories
INSERT dbo.FoodCategory (name)
VALUES 
	(N'Kem'),
	(N'Trà sữa'), 
	(N'Cà phê'), 
	(N'Sữa chua'), 
	(N'Bánh'), 
	(N'Thạch'), 
	(N'Khác')
GO

-- Insert food items
INSERT dbo.Food (name, idCategory, price)
VALUES 
	(N'Kem các vị', 1, 10000),
	(N'Trà sữa kem trứng', 2, 20000),
	(N'Trà sữa khoai môn kem trứng', 2, 20000),
	(N'Matcha Latte', 2, 20000),
	(N'Trà sữa cốm non', 2, 20000),
	(N'Trà trái cây tươi', 2, 20000),
	(N'Cà phê muối', 3, 30000),
	(N'Cà phê nâu', 3, 30000),
	(N'Cà phê đen', 3, 30000),
	(N'Bạc xỉu', 3, 30000),
	(N'Sữa chua nếp cẩm', 4, 30000),
	(N'Sữa chua uống', 4, 30000),
	(N'Bánh Flan', 5, 30000),
	(N'Thạch các vị', 6, 20000),
	(N'Hạt hướng dương thường', 7, 10000),
	(N'Hạt hướng dương vị dừa', 7, 15000)
GO

-- Insert sample bills
INSERT dbo.Bill (DateCheckIn, DateCheckOut, idTable, status)
VALUES 
	(GETDATE(), NULL, 1, 0),
	(GETDATE(), NULL, 2, 0),
	(GETDATE(), GETDATE(), 2, 1)
GO

-- Insert bill details
INSERT dbo.BillInfo (idBill, idFood, count)
VALUES 
	(1, 1, 2),
	(1, 3, 4),
	(1, 5, 1),
	(2, 1, 2),
	(2, 6, 2),
	(3, 5, 2)
GO

-- Update discount column
UPDATE dbo.Bill SET discount = 0
GO

-- =============================================
-- STORED PROCEDURES
-- =============================================

-- Account procedures
CREATE PROC USP_GetAccountByUserName
@userName nvarchar(100)
AS
BEGIN
	SELECT * FROM dbo.Account WHERE UserName = @userName
END
GO

CREATE PROC USP_Login
@userName nvarchar(100), @passWord nvarchar(100)
AS
BEGIN
	SELECT * FROM dbo.Account WHERE UserName = @userName AND Password = @passWord
END
GO

CREATE PROC USP_UpdateAccount
@userName NVARCHAR(100), @displayName NVARCHAR(100), @password NVARCHAR(100), @newPassword NVARCHAR(100)
AS
BEGIN
	DECLARE @isRightPass INT = 0
	SELECT @isRightPass = COUNT(*) FROM dbo.Account WHERE UserName = @userName AND Password = @password

	IF(@isRightPass = 1)
	BEGIN
		IF (@newPassword = NULL OR @newPassword = '')
		BEGIN
			UPDATE dbo.Account SET DisplayName = @displayName WHERE UserName = @userName
		END
		ELSE
			UPDATE dbo.Account SET DisplayName = @displayName, Password = @newPassword WHERE UserName = @userName
	END
END 
GO

-- Table procedures
CREATE PROC USP_GetTableList
AS 
	SELECT * FROM dbo.TableFood
GO

CREATE PROC USP_SwitchTable
@idTable1 int, @idTable2 int
AS 
BEGIN
	DECLARE @idFirstBill int
	DECLARE @idSecondBill int
	DECLARE @isFirstTableEmpty INT = 1
	DECLARE @isSecondTableEmpty INT = 1

	SELECT @idSecondBill = id FROM dbo.Bill WHERE idTable = @idTable2 AND status = 0
	SELECT @idFirstBill = id FROM dbo.Bill WHERE idTable = @idTable1 AND status = 0

	IF (@idFirstBill IS NULL)
	BEGIN
		INSERT dbo.Bill (DateCheckIn, DateCheckOut, idTable, status)
		VALUES (GETDATE(), GETDATE(), @idTable1, 0)
		SELECT @idFirstBill = MAX(id) FROM dbo.Bill WHERE idTable = @idTable1 AND status = 0
	END

	SELECT @isFirstTableEmpty = COUNT(*) FROM dbo.BillInfo WHERE idBill = @idFirstBill

	IF (@idSecondBill IS NULL)
	BEGIN
		INSERT dbo.Bill (DateCheckIn, DateCheckOut, idTable, status)
		VALUES (GETDATE(), GETDATE(), @idTable2, 0)
		SELECT @idSecondBill = MAX(id) FROM dbo.Bill WHERE idTable = @idTable2 AND status = 0
	END

	SELECT @isSecondTableEmpty = COUNT(*) FROM dbo.BillInfo WHERE idBill = @idSecondBill

	SELECT id INTO IDBillInfoTable FROM dbo.BillInfo WHERE idBill = @idSecondBill
	UPDATE dbo.BillInfo SET idBill = @idSecondBill WHERE idBill = @idFirstBill
	UPDATE dbo.BillInfo SET idBill = @idFirstBill WHERE id IN (SELECT * FROM IDBillInfoTable)
	DROP TABLE IDBillInfoTable

	IF (@isFirstTableEmpty = 0)
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable2
	IF (@isSecondTableEmpty = 0)
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable1
END 
GO

-- Bill procedures
CREATE PROC USP_InsertBill
@idTable INT
AS
BEGIN
	INSERT dbo.Bill (DateCheckIn, DateCheckOut, idTable, status, discount)
	VALUES (GETDATE(), NULL, @idTable, 0, 0)
END
GO

CREATE PROC USP_InsertBillInfo
@idBill INT, @idFood INT, @count INT
AS
BEGIN
	DECLARE @isExitsBillInfo INT
	DECLARE @foodCount INT = 1

	SELECT @isExitsBillInfo = id, @foodCount = b.count
	FROM dbo.BillInfo AS b
	WHERE idBill = @idBill AND idFood = @idFood

	IF (@isExitsBillInfo > 0)
	BEGIN
		DECLARE @newCount INT = @foodCount + @count
		IF (@newCount > 0)
			UPDATE dbo.BillInfo SET count = @foodCount + @count WHERE idFood = @idFood
		ELSE
			DELETE dbo.BillInfo WHERE idBill = @idBill AND idFood = @idFood
	END
	ELSE
	BEGIN
		INSERT dbo.BillInfo (idBill, idFood, count)
		VALUES (@idBill, @idFood, @count)
	END
END
GO

CREATE PROC USP_GetListBillByDate
@checkin date, @checkout date
AS
BEGIN
	SELECT t.name AS [Tên bàn], b.totalPrice AS [Tổng tiền], DateCheckIn AS [Ngày vào], DateCheckOut AS [Ngày ra], discount AS [Giảm giá]
	FROM dbo.Bill AS b, dbo.TableFood AS t
	WHERE DateCheckIn >= @checkin AND DateCheckOut <= @checkout AND b.status = 1
	AND t.id = b.idTable 
END
GO

CREATE PROC USP_GetListBillByDateAndPage
@checkin date, @checkout date, @page int
AS
BEGIN
	DECLARE @pageRows INT = 10
	DECLARE @selectRows INT = @pageRows
	DECLARE @exceptRows INT = (@page - 1) * @pageRows
	
	;WITH BillShow AS (
		SELECT b.ID, t.name AS [Tên bàn], b.totalPrice AS [Tổng tiền], DateCheckIn AS [Ngày vào], DateCheckOut AS [Ngày ra], discount AS [Giảm giá]
		FROM dbo.Bill AS b, dbo.TableFood AS t
		WHERE DateCheckIn >= @checkin AND DateCheckOut <= @checkout AND b.status = 1
		AND t.id = b.idTable 
	)
	SELECT TOP (@selectRows) * FROM BillShow WHERE id NOT IN (SELECT TOP (@exceptRows) id FROM BillShow)
END
GO

CREATE PROC USP_GetNumBillByDate
@checkIn date, @checkOut date
AS
BEGIN
    SELECT COUNT(*)
    FROM dbo.Bill AS b, dbo.TableFood AS t
    WHERE DateCheckIn >= @checkIn AND DateCheckOut <= @checkOut AND b.status = 1
	AND t.id = b.idTable
END
GO

-- =============================================
-- TRIGGERS
-- =============================================

-- Update table status when bill info changes
CREATE TRIGGER UTG_UpDateBillInfo
ON dbo.BillInfo FOR INSERT, UPDATE
AS
BEGIN
	DECLARE @idBill INT
	SELECT @idBill = idBill FROM inserted

	DECLARE @idTable INT
	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill AND status=0

	DECLARE @count INT
	SELECT @count = COUNT(*) FROM dbo.BillInfo WHERE idBill = @idBill

	IF(@count > 0)
	BEGIN
		UPDATE dbo.TableFood SET status = N'Có người' WHERE id = @idTable
	END
	ELSE
	BEGIN
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable
	END
END
GO

-- Update table status when bill changes
CREATE TRIGGER UTG_UpDateBill
ON dbo.Bill FOR UPDATE
AS
BEGIN
	DECLARE @idBill INT
	SELECT @idBill = id FROM inserted

	DECLARE @idTable INT
	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill

	DECLARE @count int = 0
	SELECT @count = COUNT(*) FROM dbo.Bill WHERE idTable = @idTable AND status = 0

	IF (@count = 0)
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable	
END
GO

-- Update table status when bill info is deleted
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
	SELECT @count = COUNT (*) FROM dbo.BillInfo AS bi, dbo.Bill AS b WHERE b.id = bi.idBill AND b.id = @idBill AND b.status = 0

	IF(@count = 0)
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable
END
GO

-- =============================================
-- FUNCTIONS
-- =============================================

-- Function to convert Vietnamese characters to unsigned
CREATE FUNCTION [dbo].[fuConvertToUnsigned1] (@strInput NVARCHAR(4000))
RETURNS NVARCHAR(4000)
AS
BEGIN
    IF @strInput IS NULL RETURN @strInput
    IF @strInput = '' RETURN @strInput

    DECLARE @SIGN_CHARS NCHAR(136)
    DECLARE @UNSIGN_CHARS NCHAR(136)
    DECLARE @COUNTER INT
    DECLARE @COUNTER1 INT

    SET @SIGN_CHARS = N'ăâđôơưàảạãáằẳặẵắầẩậẫấèẻẹẽéềểệễếìỉịĩíòỏọõóồổộỗốờởợỡớùủụũúừửựữứỳỷỵỹýĂÂĐÔƠƯÀẢẠÃÁẰẲẶẴẮẦẨẬẪẤÈẺẸẼÉỀỂỆỴẾÌỈỊĨÍÒỎỌÕÓỒỔỘỖỐỜỞỢỠỚÙỦỤŨÚỪỬỰỮỨỲỶỴỸÝ' + NCHAR(272) + NCHAR(208)
    SET @UNSIGN_CHARS = N'aadouuaaaaaăăăăăâââââeeeeeeeeeiiiiiòòòòòôôôôôơơơơơuuuuuưưưưưyyyyyAADOUUAAAAAĂĂĂĂĂÂÂÂÂÂEEEEEEEEEIIIIIÒÒÒÒÒÔÔÔÔÔƠƠƠƠƠUUUUUƯƯƯƯƯYYYYY'

    SET @COUNTER = 1

    WHILE (@COUNTER <= LEN(@strInput))
    BEGIN
        SET @COUNTER1 = 1
        WHILE (@COUNTER1 <= LEN(@SIGN_CHARS))
        BEGIN
            IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1, 1)) = UNICODE(SUBSTRING(@strInput, @COUNTER, 1))
            BEGIN
                SET @strInput = STUFF(@strInput, @COUNTER, 1, SUBSTRING(@UNSIGN_CHARS, @COUNTER1, 1))
                BREAK
            END
            SET @COUNTER1 = @COUNTER1 + 1
        END
        SET @COUNTER = @COUNTER + 1
    END

    -- Replace spaces with hyphens
    SET @strInput = REPLACE(@strInput, ' ', '-')

    RETURN @strInput
END
GO

-- =============================================
-- TEST QUERIES
-- =============================================

-- Test query for bill details
SELECT f.name, bi.count, f.price, f.price*bi.count AS totalPice 
FROM dbo.BillInfo AS bi, dbo.Bill as b, dbo.Food AS f 
WHERE bi.idBill = b.id AND bi.idFood = f.id AND b.status = 0 AND b.idTable = 2

-- View all data
SELECT * FROM dbo.Bill
SELECT * FROM dbo.BillInfo
SELECT * FROM dbo.Food
SELECT * FROM dbo.FoodCategory
SELECT * FROM dbo.TableFood

-- Test account procedure
EXEC dbo.USP_GetAccountByUserName @userName = N'DUYHOANG'
GO