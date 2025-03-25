-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER PROCEDURE [TutorialAppSchema].spRegistration_Upsert
	@Email NVARCHAR(50),
	@PasswordHash VARBINARY(MAX),
	@PasswordSalt VARBINARY(MAX)

AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT * FROM TutorialAppSchema.Auth WHERE Email = @Email)
		BEGIN
			INSERT INTO TutorialAppSchema.Auth
			(
			    Email,
			    PasswordHash,
			    PasswordSalt
			)
			VALUES
			(   @Email,  -- Email - nvarchar(50)
			    @PasswordHash, -- PasswordHash - varbinary(max)
			    @PasswordSalt  -- PasswordSalt - varbinary(max)
			    )
		END
	ELSE
		BEGIN
			UPDATE TutorialAppSchema.Auth
			SET PasswordHash = @PasswordHash,
				PasswordSalt = @PasswordSalt
			WHERE Email = @Email
		END
END
GO
