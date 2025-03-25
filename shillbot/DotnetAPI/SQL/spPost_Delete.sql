USE [DotNetCourseDatabase]
GO

/****** Object:  StoredProcedure [dbo].[spPost_Delete]    Script Date: 3/24/2025 7:38:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		shillbot
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER   PROCEDURE [TutorialAppSchema].[spPost_Delete]
    -- Add the parameters for the stored procedure here
    @PostId INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM TutorialAppSchema.Posts
    WHERE PostId = @PostId
          AND UserId = @UserId;
END;
GO


