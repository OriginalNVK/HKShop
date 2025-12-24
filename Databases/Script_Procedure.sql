USE HKShop
GO

CREATE PROC proc_changeImageLink
	@name varchar(255),
	@link varchar(500) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET @link = 'https://res.cloudinary.com/dst6r1cf6/image/upload/v1766442447/' + @name
END

