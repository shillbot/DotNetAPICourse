CREATE DATABASE DotNetCourseDatabase
GO

USE DotNetCourseDatabase
GO

CREATE SCHEMA TutorialAppSchema
GO

CREATE TABLE TutorialAppSchema.Computer
(
	ComputerId INT IDENTITY(1, 1) PRIMARY KEY,
	Motherboard NVARCHAR(50),
	CPUCores INT,
	HasWifi BIT,
	HasLTE BIT,
	Price DECIMAL(18, 4),
	VideoCard NVARCHAR(50)
)
GO

SELECT *
FROM TutorialAppSchema.Computer
GO