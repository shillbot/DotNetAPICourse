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
CREATE OR ALTER PROCEDURE TutorialAppSchema.spLoginConfirmation_Get 
	@Email NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT PasswordHash,
           PasswordSalt 
	FROM TutorialAppSchema.Auth
	WHERE @Email = @Email
END
GO
