USE HKShop
GO

CREATE PROC proc_changeImageLink
	@name varchar(255),
	@link varchar(500) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	IF @name IS NULL OR LTRIM(RTRIM(@name)) = ''
	BEGIN
		SET @link = NULL;
		RETURN;
	END

	IF @name LIKE 'http://%' OR @name LIKE 'https://%'
	BEGIN
		SET @link = @name;
	END
	ELSE
	BEGIN
		SET @link = 'https://res.cloudinary.com/dst6r1cf6/image/upload/' + @name;
	END
END

