USE DotNetCourseDatabase
GO

CREATE OR ALTER PROCEDURE TutorialAppSchema.sp_User_Upsert
	@FirstName NVARCHAR(50),
	@LastName NVARCHAR(50),
	@Email NVARCHAR(50),
	@Gender NVARCHAR(50),
	@Active BIT = 1,
	@UserId INT = NULL
AS
BEGIN
	IF NOT EXISTS (SELECT * FROM Users WHERE UserId = @UserId)
		BEGIN
		IF NOT EXISTS (SELECT * FROM Users WHERE Email = @Email)
			BEGIN
				INSERT INTO Users(FirstName, LastName, Email, Gender, Active)
				VALUES (@FirstName, @LastName, @Email, @Gender, @Active)
			END
		END
	ELSE
		BEGIN
			UPDATE Users
			SET FirstName = @FirstName,
			    LastName  = @LastName,
			    Email     = @Email,
			    Gender    = @Gender,
			    Active    = @Active
			WHERE UserId = @UserId
		END
END