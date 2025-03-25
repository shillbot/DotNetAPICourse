USE [DotNetCourseDatabase]
GO

/****** Object:  StoredProcedure [dbo].[spPosts_Upsert]    Script Date: 3/24/2025 7:40:56 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		shillbot
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER PROCEDURE [TutorialAppSchema].[spPosts_Upsert] 
	-- Add the parameters for the stored procedure here
	@UserId INT,
	@PostId INT = NULL,
	@PostTitle NVARCHAR(50),
	@PostContent NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT * FROM TutorialAppSchema.Posts WHERE PostId = @PostId)
		BEGIN
			INSERT INTO TutorialAppSchema.Posts
			(
			    UserId,
			    PostTitle,
			    PostContent,
			    PostCreated,
			    PostUpdated
			)
			VALUES
			(   @UserId, -- UserId - int
			    @PostTitle, -- PostTitle - nvarchar(255)
			    @PostContent, -- PostContent - nvarchar(max)
			    GETDATE(), -- PostCreated - datetime
			    GETDATE()  -- PostUpdated - datetime
			)
		END
	ELSE
		BEGIN
			UPDATE TutorialAppSchema.Posts
			SET PostContent = @PostContent,
				PostTitle = @PostTitle,
				PostUpdated = GETDATE()
			WHERE PostId = @PostId
		END
END
GO


