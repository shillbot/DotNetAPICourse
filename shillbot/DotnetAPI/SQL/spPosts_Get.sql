USE [DotNetCourseDatabase]
GO

/****** Object:  StoredProcedure [dbo].[spPosts_Get]    Script Date: 3/24/2025 7:40:08 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER   PROCEDURE [TutorialAppSchema].[spPosts_Get]
-- EXEC spPosts_Get @UserId=1001, @SearchText='is'
	@UserId INT = NULL,
	@SearchText NVARCHAR(MAX) = NULL,
	@PostId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT PostId,
           UserId,
           PostTitle,
           PostContent,
           PostCreated,
           PostUpdated 
	FROM TutorialAppSchema.Posts
	WHERE UserId = ISNULL(@UserId, Posts.UserId)
		AND PostId = ISNULL(@PostId, Posts.PostId)
		AND (@SearchText IS NULL
		OR PostContent LIKE '%' + @SearchText + '%'
		OR PostTitle LIKE '%' + @SearchText + '%')
END
GO


