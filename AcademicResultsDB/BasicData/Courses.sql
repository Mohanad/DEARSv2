CREATE TABLE [dbo].[Courses]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [CourseCode] NVARCHAR(10) NULL UNIQUE, 
    [TitleArabic] NVARCHAR(MAX) NULL, 
    [TitleEnglish] NVARCHAR(MAX) NULL
)