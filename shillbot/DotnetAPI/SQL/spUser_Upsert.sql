USE [DotNetCourseDatabase];
GO
/****** Object:  StoredProcedure [TutorialAppSchema].[spUser_Upsert]    Script Date: 3/24/2025 7:31:50 PM ******/
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
ALTER PROCEDURE [TutorialAppSchema].[spUser_Upsert]
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(50),
    @Gender NVARCHAR(50),
    @Active BIT = 1,
    @UserId INT = NULL,
    @JobTitle NVARCHAR(50),
    @Department NVARCHAR(50),
    @Salary DECIMAL(18, 4)
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM Users WHERE UserId = @UserId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM Users WHERE Email = @Email)
        BEGIN
            DECLARE @OutUserId INT;

            INSERT INTO Users
            (
                FirstName,
                LastName,
                Email,
                Gender,
                Active
            )
            VALUES
            (@FirstName, @LastName, @Email, @Gender, @Active);

            SET @OutUserId = @@IDENTITY;

            INSERT INTO TutorialAppSchema.UserSalary
            (
                UserId,
                Salary
            )
            VALUES
            (   @OutUserId, -- UserId - int
                @Salary     -- Salary - decimal(18, 4)
                );

            INSERT INTO TutorialAppSchema.UserJobInfo
            (
                UserId,
                JobTitle,
                Department
            )
            VALUES
            (   @OutUserId, -- UserId - int
                @JobTitle,  -- JobTitle - nvarchar(50)
                @Department -- Department - nvarchar(50)
                );
        END;
    END;
    ELSE
    BEGIN
        UPDATE Users
        SET FirstName = @FirstName,
            LastName = @LastName,
            Email = @Email,
            Gender = @Gender,
            Active = @Active
        WHERE UserId = @UserId;

        UPDATE TutorialAppSchema.UserSalary
        SET Salary = @Salary
        WHERE UserId = @UserId;

        UPDATE TutorialAppSchema.UserJobInfo
        SET JobTitle = @JobTitle,
            Department = @Department
        WHERE UserId = @UserId;
    END;
END;