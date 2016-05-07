USE [master]
GO
/****** Object:  Database [AcademicResultsDB]    Script Date: 5/5/2016 09:03:42 ******/
CREATE DATABASE [AcademicResultsDB] CONTAINMENT = NONE
GO
ALTER DATABASE [AcademicResultsDB] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [AcademicResultsDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [AcademicResultsDB] SET ANSI_NULL_DEFAULT ON 
GO
ALTER DATABASE [AcademicResultsDB] SET ANSI_NULLS ON 
GO
ALTER DATABASE [AcademicResultsDB] SET ANSI_PADDING ON 
GO
ALTER DATABASE [AcademicResultsDB] SET ANSI_WARNINGS ON 
GO
ALTER DATABASE [AcademicResultsDB] SET ARITHABORT ON 
GO
ALTER DATABASE [AcademicResultsDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [AcademicResultsDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [AcademicResultsDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [AcademicResultsDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [AcademicResultsDB] SET CURSOR_DEFAULT  LOCAL 
GO
ALTER DATABASE [AcademicResultsDB] SET CONCAT_NULL_YIELDS_NULL ON 
GO
ALTER DATABASE [AcademicResultsDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [AcademicResultsDB] SET QUOTED_IDENTIFIER ON 
GO
ALTER DATABASE [AcademicResultsDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [AcademicResultsDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [AcademicResultsDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [AcademicResultsDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [AcademicResultsDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [AcademicResultsDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [AcademicResultsDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [AcademicResultsDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [AcademicResultsDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [AcademicResultsDB] SET RECOVERY FULL 
GO
ALTER DATABASE [AcademicResultsDB] SET  MULTI_USER 
GO
ALTER DATABASE [AcademicResultsDB] SET PAGE_VERIFY NONE  
GO
ALTER DATABASE [AcademicResultsDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [AcademicResultsDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [AcademicResultsDB] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [AcademicResultsDB] SET DELAYED_DURABILITY = DISABLED 
GO
USE [AcademicResultsDB]
GO
/****** Object:  Table [dbo].[BatchEnrollments]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BatchEnrollments](
	[YearId] [int] NOT NULL,
	[GradeId] [int] NOT NULL,
	[StudentId] [int] NOT NULL,
	[EnrollmentTypeId] [int] NOT NULL,
 CONSTRAINT [PK_BatchEnrollments] PRIMARY KEY CLUSTERED 
(
	[YearId] ASC,
	[GradeId] ASC,
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Batches]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Batches](
	[YearId] [int] NOT NULL,
	[GradeId] [int] NOT NULL,
	[NameEnglish] [nvarchar](50) NULL,
	[NameArabic] [nvarchar](50) NULL,
 CONSTRAINT [PK_Batch1] PRIMARY KEY CLUSTERED 
(
	[YearId] ASC,
	[GradeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CourseDisciplines]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourseDisciplines](
	[YearId] [int] NOT NULL,
	[GradeId] [int] NOT NULL,
	[SemesterId] [int] NOT NULL,
	[CourseId] [int] NOT NULL,
	[DisciplineId] [int] NOT NULL,
	[Optional] [bit] NOT NULL,
 CONSTRAINT [PK_CourseDisciplines] PRIMARY KEY CLUSTERED 
(
	[YearId] ASC,
	[GradeId] ASC,
	[SemesterId] ASC,
	[CourseId] ASC,
	[DisciplineId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CourseEnrollments]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourseEnrollments](
	[YearId] [int] NOT NULL,
	[GradeId] [int] NOT NULL,
	[SemesterId] [int] NOT NULL,
	[StudentId] [int] NOT NULL,
	[CourseId] [int] NOT NULL,
 CONSTRAINT [PK_CourseEnrollments] PRIMARY KEY CLUSTERED 
(
	[YearId] ASC,
	[GradeId] ASC,
	[SemesterId] ASC,
	[StudentId] ASC,
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Courses]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Courses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CourseCode] [nchar](10) NULL,
	[TitleArabic] [nvarchar](max) NULL,
	[TitleEnglish] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[CourseCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CourseTeachers]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourseTeachers](
	[YearId] [int] NOT NULL,
	[GradeId] [int] NOT NULL,
	[SemesterId] [int] NOT NULL,
	[CourseId] [int] NOT NULL,
	[TeacherId] [int] NOT NULL,
	[TuitionTypeId] [int] NOT NULL,
 CONSTRAINT [PK_CourseTeachers] PRIMARY KEY CLUSTERED 
(
	[YearId] ASC,
	[GradeId] ASC,
	[SemesterId] ASC,
	[CourseId] ASC,
	[TeacherId] ASC,
	[TuitionTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Disciplines]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Disciplines](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NameEnglish] [nvarchar](50) NULL,
	[NameArabic] [nvarchar](50) NULL,
	[NameEnglishShort] [nvarchar](50) NULL,
	[NameArabicShort] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EnrollmentTypes]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EnrollmentTypes](
	[Id] [int] NOT NULL,
	[NameArabic] [nvarchar](50) NULL,
	[NameEnglish] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ExamTypes]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExamTypes](
	[Id] [int] NOT NULL,
	[NameArabic] [nvarchar](50) NULL,
	[NameEnglish] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GPAwRecomm]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GPAwRecomm](
	[YearId] [int] NOT NULL,
	[GradeId] [int] NOT NULL,
	[StudentId] [int] NOT NULL,
	[GPA] [float] NULL,
	[YearRecommId] [int] NOT NULL,
	[CGPA] [float] NULL,
	[CumulativeRecommId] [int] NULL,
	[Comment] [nvarchar](50) NULL,
 CONSTRAINT [PK_GPAwRecomm] PRIMARY KEY CLUSTERED 
(
	[YearId] ASC,
	[GradeId] ASC,
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Grades]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Grades](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NameEnglish] [nvarchar](50) NULL,
	[NameArabic] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MarksExamCW]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MarksExamCW](
	[YearId] [int] NOT NULL,
	[GradeId] [int] NOT NULL,
	[SemesterId] [int] NOT NULL,
	[CourseId] [int] NOT NULL,
	[StudentId] [int] NOT NULL,
	[CWMark] [float] NULL,
	[Present] [bit] NULL,
	[Excuse] [bit] NULL,
	[ExamMark] [float] NULL,
 CONSTRAINT [PK_MarksExamCW] PRIMARY KEY CLUSTERED 
(
	[YearId] ASC,
	[GradeId] ASC,
	[SemesterId] ASC,
	[StudentId] ASC,
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OfferedCourses]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OfferedCourses](
	[YearId] [int] NOT NULL,
	[GradeId] [int] NOT NULL,
	[SemesterId] [int] NOT NULL,
	[CourseId] [int] NOT NULL,
	[ExamFraction] [int] NOT NULL,
	[CourseWorkFraction] [int] NOT NULL,
	[CreditHours] [int] NOT NULL,
 CONSTRAINT [PK_OfferedCourses] PRIMARY KEY CLUSTERED 
(
	[YearId] ASC,
	[GradeId] ASC,
	[SemesterId] ASC,
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OfferedDisciplines]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OfferedDisciplines](
	[YearId] [int] NOT NULL,
	[GradeId] [int] NOT NULL,
	[SemesterId] [int] NOT NULL,
	[DisciplineId] [int] NOT NULL,
 CONSTRAINT [PK_OfferedDisciplines] PRIMARY KEY CLUSTERED 
(
	[YearId] ASC,
	[GradeId] ASC,
	[SemesterId] ASC,
	[DisciplineId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RecommendationTypes]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecommendationTypes](
	[Id] [int] NOT NULL,
	[NameArabic] [nvarchar](50) NULL,
	[NameEnglish] [nvarchar](50) NULL,
	[Pass] [bit] NOT NULL,
	[ShortNameEnglish] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RecommTranslations]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecommTranslations](
	[ResText] [nvarchar](50) NOT NULL,
	[RecommendationTypeN] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ResText] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SemesterBatchEnrollments]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SemesterBatchEnrollments](
	[YearId] [int] NOT NULL,
	[GradeId] [int] NOT NULL,
	[SemesterId] [int] NOT NULL,
	[StudentId] [int] NOT NULL,
	[DisciplineId] [int] NOT NULL,
 CONSTRAINT [PK_SemesterBatchEnrollments] PRIMARY KEY CLUSTERED 
(
	[YearId] ASC,
	[GradeId] ASC,
	[SemesterId] ASC,
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SemesterBatches]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SemesterBatches](
	[YearId] [int] NOT NULL,
	[GradeId] [int] NOT NULL,
	[SemesterId] [int] NOT NULL,
 CONSTRAINT [PK_SemesterBatch] PRIMARY KEY CLUSTERED 
(
	[YearId] ASC,
	[GradeId] ASC,
	[SemesterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Semesters]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Semesters](
	[Id] [int] NOT NULL,
	[NameEnglish] [nchar](10) NULL,
	[NameArabic] [nchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Students]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Students](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Index] [int] NULL,
	[UnivNo] [nchar](10) NULL,
	[NameArabic] [nvarchar](max) NULL,
	[NameEnglish] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SubSuppGPAwRecomm]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubSuppGPAwRecomm](
	[YearId] [int] NOT NULL,
	[GradeId] [int] NOT NULL,
	[StudentId] [int] NOT NULL,
	[GPA] [float] NULL,
	[YearRecommId] [int] NOT NULL,
	[CGPA] [float] NULL,
	[CumulativeRecommId] [int] NULL,
	[Comment] [nvarchar](50) NULL,
 CONSTRAINT [PK_SubSuppGPAwRecomm] PRIMARY KEY CLUSTERED 
(
	[YearId] ASC,
	[GradeId] ASC,
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SubSuppMarksExamCW]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubSuppMarksExamCW](
	[YearId] [int] NOT NULL,
	[GradeId] [int] NOT NULL,
	[SemesterId] [int] NOT NULL,
	[CourseId] [int] NOT NULL,
	[StudentId] [int] NOT NULL,
	[ExamTypeId] [int] NOT NULL,
	[CWMark] [float] NULL,
	[Present] [bit] NULL,
	[Excuse] [bit] NULL,
	[ExamMark] [float] NULL,
 CONSTRAINT [PK_SubSuppMarksExamCW] PRIMARY KEY CLUSTERED 
(
	[YearId] ASC,
	[GradeId] ASC,
	[SemesterId] ASC,
	[StudentId] ASC,
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Teachers]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Teachers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NameArabic] [nvarchar](max) NULL,
	[NameEnglish] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TimeYears]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TimeYears](
	[Id] [int] NOT NULL,
	[NameEnglish] [nvarchar](50) NULL,
	[NameArabic] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TuitionTypes]    Script Date: 5/5/2016 09:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TuitionTypes](
	[Id] [int] NOT NULL,
	[NameArabic] [nvarchar](50) NULL,
	[NameEnglish] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[BatchEnrollments] ADD  DEFAULT ((0)) FOR [EnrollmentTypeId]
GO
ALTER TABLE [dbo].[BatchEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_BatchEnrollments_Batch] FOREIGN KEY([YearId], [GradeId])
REFERENCES [dbo].[Batches] ([YearId], [GradeId])
GO
ALTER TABLE [dbo].[BatchEnrollments] CHECK CONSTRAINT [FK_BatchEnrollments_Batch]
GO
ALTER TABLE [dbo].[BatchEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_BatchEnrollments_EnrollmentType] FOREIGN KEY([EnrollmentTypeId])
REFERENCES [dbo].[EnrollmentTypes] ([Id])
GO
ALTER TABLE [dbo].[BatchEnrollments] CHECK CONSTRAINT [FK_BatchEnrollments_EnrollmentType]
GO
ALTER TABLE [dbo].[BatchEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_BatchEnrollments_Grades] FOREIGN KEY([GradeId])
REFERENCES [dbo].[Grades] ([Id])
GO
ALTER TABLE [dbo].[BatchEnrollments] CHECK CONSTRAINT [FK_BatchEnrollments_Grades]
GO
ALTER TABLE [dbo].[BatchEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_BatchEnrollments_Students] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([Id])
GO
ALTER TABLE [dbo].[BatchEnrollments] CHECK CONSTRAINT [FK_BatchEnrollments_Students]
GO
ALTER TABLE [dbo].[BatchEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_BatchEnrollments_TimeYears] FOREIGN KEY([YearId])
REFERENCES [dbo].[TimeYears] ([Id])
GO
ALTER TABLE [dbo].[BatchEnrollments] CHECK CONSTRAINT [FK_BatchEnrollments_TimeYears]
GO
ALTER TABLE [dbo].[Batches]  WITH CHECK ADD  CONSTRAINT [FK_Batch_Grades] FOREIGN KEY([GradeId])
REFERENCES [dbo].[Grades] ([Id])
GO
ALTER TABLE [dbo].[Batches] CHECK CONSTRAINT [FK_Batch_Grades]
GO
ALTER TABLE [dbo].[Batches]  WITH CHECK ADD  CONSTRAINT [FK_Batch_TimeYears] FOREIGN KEY([YearId])
REFERENCES [dbo].[TimeYears] ([Id])
GO
ALTER TABLE [dbo].[Batches] CHECK CONSTRAINT [FK_Batch_TimeYears]
GO
ALTER TABLE [dbo].[CourseDisciplines]  WITH CHECK ADD  CONSTRAINT [FK_CourseDisciplines_Courses] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
GO
ALTER TABLE [dbo].[CourseDisciplines] CHECK CONSTRAINT [FK_CourseDisciplines_Courses]
GO
ALTER TABLE [dbo].[CourseDisciplines]  WITH CHECK ADD  CONSTRAINT [FK_CourseDisciplines_Disciplines] FOREIGN KEY([DisciplineId])
REFERENCES [dbo].[Disciplines] ([Id])
GO
ALTER TABLE [dbo].[CourseDisciplines] CHECK CONSTRAINT [FK_CourseDisciplines_Disciplines]
GO
ALTER TABLE [dbo].[CourseDisciplines]  WITH CHECK ADD  CONSTRAINT [FK_CourseDisciplines_Grades] FOREIGN KEY([GradeId])
REFERENCES [dbo].[Grades] ([Id])
GO
ALTER TABLE [dbo].[CourseDisciplines] CHECK CONSTRAINT [FK_CourseDisciplines_Grades]
GO
ALTER TABLE [dbo].[CourseDisciplines]  WITH CHECK ADD  CONSTRAINT [FK_CourseDisciplines_OfferedCourses] FOREIGN KEY([YearId], [GradeId], [SemesterId], [CourseId])
REFERENCES [dbo].[OfferedCourses] ([YearId], [GradeId], [SemesterId], [CourseId])
GO
ALTER TABLE [dbo].[CourseDisciplines] CHECK CONSTRAINT [FK_CourseDisciplines_OfferedCourses]
GO
ALTER TABLE [dbo].[CourseDisciplines]  WITH CHECK ADD  CONSTRAINT [FK_CourseDisciplines_OfferedDisciplines] FOREIGN KEY([YearId], [GradeId], [SemesterId], [DisciplineId])
REFERENCES [dbo].[OfferedDisciplines] ([YearId], [GradeId], [SemesterId], [DisciplineId])
GO
ALTER TABLE [dbo].[CourseDisciplines] CHECK CONSTRAINT [FK_CourseDisciplines_OfferedDisciplines]
GO
ALTER TABLE [dbo].[CourseDisciplines]  WITH CHECK ADD  CONSTRAINT [FK_CourseDisciplines_SemesterBatch] FOREIGN KEY([YearId], [GradeId], [SemesterId])
REFERENCES [dbo].[SemesterBatches] ([YearId], [GradeId], [SemesterId])
GO
ALTER TABLE [dbo].[CourseDisciplines] CHECK CONSTRAINT [FK_CourseDisciplines_SemesterBatch]
GO
ALTER TABLE [dbo].[CourseDisciplines]  WITH CHECK ADD  CONSTRAINT [FK_CourseDisciplines_Semesters] FOREIGN KEY([SemesterId])
REFERENCES [dbo].[Semesters] ([Id])
GO
ALTER TABLE [dbo].[CourseDisciplines] CHECK CONSTRAINT [FK_CourseDisciplines_Semesters]
GO
ALTER TABLE [dbo].[CourseDisciplines]  WITH CHECK ADD  CONSTRAINT [FK_CourseDisciplines_TimeYears] FOREIGN KEY([YearId])
REFERENCES [dbo].[TimeYears] ([Id])
GO
ALTER TABLE [dbo].[CourseDisciplines] CHECK CONSTRAINT [FK_CourseDisciplines_TimeYears]
GO
ALTER TABLE [dbo].[CourseEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_CourseEnrollments_Courses] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
GO
ALTER TABLE [dbo].[CourseEnrollments] CHECK CONSTRAINT [FK_CourseEnrollments_Courses]
GO
ALTER TABLE [dbo].[CourseEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_CourseEnrollments_Grades] FOREIGN KEY([GradeId])
REFERENCES [dbo].[Grades] ([Id])
GO
ALTER TABLE [dbo].[CourseEnrollments] CHECK CONSTRAINT [FK_CourseEnrollments_Grades]
GO
ALTER TABLE [dbo].[CourseEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_CourseEnrollments_OfferedCourses] FOREIGN KEY([YearId], [GradeId], [SemesterId], [CourseId])
REFERENCES [dbo].[OfferedCourses] ([YearId], [GradeId], [SemesterId], [CourseId])
GO
ALTER TABLE [dbo].[CourseEnrollments] CHECK CONSTRAINT [FK_CourseEnrollments_OfferedCourses]
GO
ALTER TABLE [dbo].[CourseEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_CourseEnrollments_SemesterBatchEnrollments] FOREIGN KEY([YearId], [GradeId], [SemesterId], [StudentId])
REFERENCES [dbo].[SemesterBatchEnrollments] ([YearId], [GradeId], [SemesterId], [StudentId])
GO
ALTER TABLE [dbo].[CourseEnrollments] CHECK CONSTRAINT [FK_CourseEnrollments_SemesterBatchEnrollments]
GO
ALTER TABLE [dbo].[CourseEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_CourseEnrollments_Semesters] FOREIGN KEY([SemesterId])
REFERENCES [dbo].[Semesters] ([Id])
GO
ALTER TABLE [dbo].[CourseEnrollments] CHECK CONSTRAINT [FK_CourseEnrollments_Semesters]
GO
ALTER TABLE [dbo].[CourseEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_CourseEnrollments_Students] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([Id])
GO
ALTER TABLE [dbo].[CourseEnrollments] CHECK CONSTRAINT [FK_CourseEnrollments_Students]
GO
ALTER TABLE [dbo].[CourseEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_CourseEnrollments_TimeYears] FOREIGN KEY([YearId])
REFERENCES [dbo].[TimeYears] ([Id])
GO
ALTER TABLE [dbo].[CourseEnrollments] CHECK CONSTRAINT [FK_CourseEnrollments_TimeYears]
GO
ALTER TABLE [dbo].[CourseTeachers]  WITH CHECK ADD  CONSTRAINT [FK_CourseTeachers_Disciplines] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
GO
ALTER TABLE [dbo].[CourseTeachers] CHECK CONSTRAINT [FK_CourseTeachers_Disciplines]
GO
ALTER TABLE [dbo].[CourseTeachers]  WITH CHECK ADD  CONSTRAINT [FK_CourseTeachers_Grades] FOREIGN KEY([GradeId])
REFERENCES [dbo].[Grades] ([Id])
GO
ALTER TABLE [dbo].[CourseTeachers] CHECK CONSTRAINT [FK_CourseTeachers_Grades]
GO
ALTER TABLE [dbo].[CourseTeachers]  WITH CHECK ADD  CONSTRAINT [FK_CourseTeachers_OfferedCourses] FOREIGN KEY([YearId], [GradeId], [SemesterId], [CourseId])
REFERENCES [dbo].[OfferedCourses] ([YearId], [GradeId], [SemesterId], [CourseId])
GO
ALTER TABLE [dbo].[CourseTeachers] CHECK CONSTRAINT [FK_CourseTeachers_OfferedCourses]
GO
ALTER TABLE [dbo].[CourseTeachers]  WITH CHECK ADD  CONSTRAINT [FK_CourseTeachers_Semesters] FOREIGN KEY([SemesterId])
REFERENCES [dbo].[Semesters] ([Id])
GO
ALTER TABLE [dbo].[CourseTeachers] CHECK CONSTRAINT [FK_CourseTeachers_Semesters]
GO
ALTER TABLE [dbo].[CourseTeachers]  WITH CHECK ADD  CONSTRAINT [FK_CourseTeachers_Teachers] FOREIGN KEY([TeacherId])
REFERENCES [dbo].[Teachers] ([Id])
GO
ALTER TABLE [dbo].[CourseTeachers] CHECK CONSTRAINT [FK_CourseTeachers_Teachers]
GO
ALTER TABLE [dbo].[CourseTeachers]  WITH CHECK ADD  CONSTRAINT [FK_CourseTeachers_TimeYears] FOREIGN KEY([YearId])
REFERENCES [dbo].[TimeYears] ([Id])
GO
ALTER TABLE [dbo].[CourseTeachers] CHECK CONSTRAINT [FK_CourseTeachers_TimeYears]
GO
ALTER TABLE [dbo].[CourseTeachers]  WITH CHECK ADD  CONSTRAINT [FK_CourseTeachers_TuitionType] FOREIGN KEY([TuitionTypeId])
REFERENCES [dbo].[TuitionTypes] ([Id])
GO
ALTER TABLE [dbo].[CourseTeachers] CHECK CONSTRAINT [FK_CourseTeachers_TuitionType]
GO
ALTER TABLE [dbo].[GPAwRecomm]  WITH CHECK ADD  CONSTRAINT [FK_GPAwRecomm_Batch] FOREIGN KEY([YearId], [GradeId])
REFERENCES [dbo].[Batches] ([YearId], [GradeId])
GO
ALTER TABLE [dbo].[GPAwRecomm] CHECK CONSTRAINT [FK_GPAwRecomm_Batch]
GO
ALTER TABLE [dbo].[GPAwRecomm]  WITH CHECK ADD  CONSTRAINT [FK_GPAwRecomm_BatchEnrollments] FOREIGN KEY([YearId], [GradeId], [StudentId])
REFERENCES [dbo].[BatchEnrollments] ([YearId], [GradeId], [StudentId])
GO
ALTER TABLE [dbo].[GPAwRecomm] CHECK CONSTRAINT [FK_GPAwRecomm_BatchEnrollments]
GO
ALTER TABLE [dbo].[GPAwRecomm]  WITH CHECK ADD  CONSTRAINT [FK_GPAwRecomm_CumulativeRecommendations] FOREIGN KEY([CumulativeRecommId])
REFERENCES [dbo].[RecommendationTypes] ([Id])
GO
ALTER TABLE [dbo].[GPAwRecomm] CHECK CONSTRAINT [FK_GPAwRecomm_CumulativeRecommendations]
GO
ALTER TABLE [dbo].[GPAwRecomm]  WITH CHECK ADD  CONSTRAINT [FK_GPAwRecomm_Grades] FOREIGN KEY([GradeId])
REFERENCES [dbo].[Grades] ([Id])
GO
ALTER TABLE [dbo].[GPAwRecomm] CHECK CONSTRAINT [FK_GPAwRecomm_Grades]
GO
ALTER TABLE [dbo].[GPAwRecomm]  WITH CHECK ADD  CONSTRAINT [FK_GPAwRecomm_Students] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([Id])
GO
ALTER TABLE [dbo].[GPAwRecomm] CHECK CONSTRAINT [FK_GPAwRecomm_Students]
GO
ALTER TABLE [dbo].[GPAwRecomm]  WITH CHECK ADD  CONSTRAINT [FK_GPAwRecomm_TimeYears] FOREIGN KEY([YearId])
REFERENCES [dbo].[TimeYears] ([Id])
GO
ALTER TABLE [dbo].[GPAwRecomm] CHECK CONSTRAINT [FK_GPAwRecomm_TimeYears]
GO
ALTER TABLE [dbo].[GPAwRecomm]  WITH CHECK ADD  CONSTRAINT [FK_GPAwRecomm_YearRecommendations] FOREIGN KEY([YearRecommId])
REFERENCES [dbo].[RecommendationTypes] ([Id])
GO
ALTER TABLE [dbo].[GPAwRecomm] CHECK CONSTRAINT [FK_GPAwRecomm_YearRecommendations]
GO
ALTER TABLE [dbo].[MarksExamCW]  WITH CHECK ADD  CONSTRAINT [FK_MarksExamCW_CourseEnrollment] FOREIGN KEY([YearId], [GradeId], [SemesterId], [StudentId], [CourseId])
REFERENCES [dbo].[CourseEnrollments] ([YearId], [GradeId], [SemesterId], [StudentId], [CourseId])
GO
ALTER TABLE [dbo].[MarksExamCW] CHECK CONSTRAINT [FK_MarksExamCW_CourseEnrollment]
GO
ALTER TABLE [dbo].[MarksExamCW]  WITH CHECK ADD  CONSTRAINT [FK_MarksExamCW_Courses] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
GO
ALTER TABLE [dbo].[MarksExamCW] CHECK CONSTRAINT [FK_MarksExamCW_Courses]
GO
ALTER TABLE [dbo].[MarksExamCW]  WITH CHECK ADD  CONSTRAINT [FK_MarksExamCW_Grades] FOREIGN KEY([GradeId])
REFERENCES [dbo].[Grades] ([Id])
GO
ALTER TABLE [dbo].[MarksExamCW] CHECK CONSTRAINT [FK_MarksExamCW_Grades]
GO
ALTER TABLE [dbo].[MarksExamCW]  WITH CHECK ADD  CONSTRAINT [FK_MarksExamCW_OfferedCourse] FOREIGN KEY([YearId], [GradeId], [SemesterId], [CourseId])
REFERENCES [dbo].[OfferedCourses] ([YearId], [GradeId], [SemesterId], [CourseId])
GO
ALTER TABLE [dbo].[MarksExamCW] CHECK CONSTRAINT [FK_MarksExamCW_OfferedCourse]
GO
ALTER TABLE [dbo].[MarksExamCW]  WITH CHECK ADD  CONSTRAINT [FK_MarksExamCW_Semesters] FOREIGN KEY([SemesterId])
REFERENCES [dbo].[Semesters] ([Id])
GO
ALTER TABLE [dbo].[MarksExamCW] CHECK CONSTRAINT [FK_MarksExamCW_Semesters]
GO
ALTER TABLE [dbo].[MarksExamCW]  WITH CHECK ADD  CONSTRAINT [FK_MarksExamCW_Students] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([Id])
GO
ALTER TABLE [dbo].[MarksExamCW] CHECK CONSTRAINT [FK_MarksExamCW_Students]
GO
ALTER TABLE [dbo].[MarksExamCW]  WITH CHECK ADD  CONSTRAINT [FK_MarksExamCW_TimeYears] FOREIGN KEY([YearId])
REFERENCES [dbo].[TimeYears] ([Id])
GO
ALTER TABLE [dbo].[MarksExamCW] CHECK CONSTRAINT [FK_MarksExamCW_TimeYears]
GO
ALTER TABLE [dbo].[OfferedCourses]  WITH CHECK ADD  CONSTRAINT [FK_OfferedCourses_Courses] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
GO
ALTER TABLE [dbo].[OfferedCourses] CHECK CONSTRAINT [FK_OfferedCourses_Courses]
GO
ALTER TABLE [dbo].[OfferedCourses]  WITH CHECK ADD  CONSTRAINT [FK_OfferedCourses_Grades] FOREIGN KEY([GradeId])
REFERENCES [dbo].[Grades] ([Id])
GO
ALTER TABLE [dbo].[OfferedCourses] CHECK CONSTRAINT [FK_OfferedCourses_Grades]
GO
ALTER TABLE [dbo].[OfferedCourses]  WITH CHECK ADD  CONSTRAINT [FK_OfferedCourses_SemesterBatch] FOREIGN KEY([YearId], [GradeId], [SemesterId])
REFERENCES [dbo].[SemesterBatches] ([YearId], [GradeId], [SemesterId])
GO
ALTER TABLE [dbo].[OfferedCourses] CHECK CONSTRAINT [FK_OfferedCourses_SemesterBatch]
GO
ALTER TABLE [dbo].[OfferedCourses]  WITH CHECK ADD  CONSTRAINT [FK_OfferedCourses_Semesters] FOREIGN KEY([SemesterId])
REFERENCES [dbo].[Semesters] ([Id])
GO
ALTER TABLE [dbo].[OfferedCourses] CHECK CONSTRAINT [FK_OfferedCourses_Semesters]
GO
ALTER TABLE [dbo].[OfferedCourses]  WITH CHECK ADD  CONSTRAINT [FK_OfferedCourses_TimeYears] FOREIGN KEY([YearId])
REFERENCES [dbo].[TimeYears] ([Id])
GO
ALTER TABLE [dbo].[OfferedCourses] CHECK CONSTRAINT [FK_OfferedCourses_TimeYears]
GO
ALTER TABLE [dbo].[OfferedDisciplines]  WITH CHECK ADD  CONSTRAINT [FK_OfferedDisciplines_Disciplines] FOREIGN KEY([DisciplineId])
REFERENCES [dbo].[Disciplines] ([Id])
GO
ALTER TABLE [dbo].[OfferedDisciplines] CHECK CONSTRAINT [FK_OfferedDisciplines_Disciplines]
GO
ALTER TABLE [dbo].[OfferedDisciplines]  WITH CHECK ADD  CONSTRAINT [FK_OfferedDisciplines_Grades] FOREIGN KEY([GradeId])
REFERENCES [dbo].[Grades] ([Id])
GO
ALTER TABLE [dbo].[OfferedDisciplines] CHECK CONSTRAINT [FK_OfferedDisciplines_Grades]
GO
ALTER TABLE [dbo].[OfferedDisciplines]  WITH CHECK ADD  CONSTRAINT [FK_OfferedDisciplines_SemesterBatch] FOREIGN KEY([YearId], [GradeId], [SemesterId])
REFERENCES [dbo].[SemesterBatches] ([YearId], [GradeId], [SemesterId])
GO
ALTER TABLE [dbo].[OfferedDisciplines] CHECK CONSTRAINT [FK_OfferedDisciplines_SemesterBatch]
GO
ALTER TABLE [dbo].[OfferedDisciplines]  WITH CHECK ADD  CONSTRAINT [FK_OfferedDisciplines_Semesters] FOREIGN KEY([SemesterId])
REFERENCES [dbo].[Semesters] ([Id])
GO
ALTER TABLE [dbo].[OfferedDisciplines] CHECK CONSTRAINT [FK_OfferedDisciplines_Semesters]
GO
ALTER TABLE [dbo].[OfferedDisciplines]  WITH CHECK ADD  CONSTRAINT [FK_OfferedDisciplines_TimeYears] FOREIGN KEY([YearId])
REFERENCES [dbo].[TimeYears] ([Id])
GO
ALTER TABLE [dbo].[OfferedDisciplines] CHECK CONSTRAINT [FK_OfferedDisciplines_TimeYears]
GO
ALTER TABLE [dbo].[RecommTranslations]  WITH CHECK ADD  CONSTRAINT [RecommTranslations_RecommendationTypes] FOREIGN KEY([RecommendationTypeN])
REFERENCES [dbo].[RecommendationTypes] ([Id])
GO
ALTER TABLE [dbo].[RecommTranslations] CHECK CONSTRAINT [RecommTranslations_RecommendationTypes]
GO
ALTER TABLE [dbo].[SemesterBatchEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_SemesterBatchEnrollments_BatchEnrollments] FOREIGN KEY([YearId], [GradeId], [StudentId])
REFERENCES [dbo].[BatchEnrollments] ([YearId], [GradeId], [StudentId])
GO
ALTER TABLE [dbo].[SemesterBatchEnrollments] CHECK CONSTRAINT [FK_SemesterBatchEnrollments_BatchEnrollments]
GO
ALTER TABLE [dbo].[SemesterBatchEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_SemesterBatchEnrollments_Disciplines] FOREIGN KEY([DisciplineId])
REFERENCES [dbo].[Disciplines] ([Id])
GO
ALTER TABLE [dbo].[SemesterBatchEnrollments] CHECK CONSTRAINT [FK_SemesterBatchEnrollments_Disciplines]
GO
ALTER TABLE [dbo].[SemesterBatchEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_SemesterBatchEnrollments_Grades] FOREIGN KEY([GradeId])
REFERENCES [dbo].[Grades] ([Id])
GO
ALTER TABLE [dbo].[SemesterBatchEnrollments] CHECK CONSTRAINT [FK_SemesterBatchEnrollments_Grades]
GO
ALTER TABLE [dbo].[SemesterBatchEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_SemesterBatchEnrollments_OfferedDisciplines] FOREIGN KEY([YearId], [GradeId], [SemesterId], [DisciplineId])
REFERENCES [dbo].[OfferedDisciplines] ([YearId], [GradeId], [SemesterId], [DisciplineId])
GO
ALTER TABLE [dbo].[SemesterBatchEnrollments] CHECK CONSTRAINT [FK_SemesterBatchEnrollments_OfferedDisciplines]
GO
ALTER TABLE [dbo].[SemesterBatchEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_SemesterBatchEnrollments_SemesterBatches] FOREIGN KEY([YearId], [GradeId], [SemesterId])
REFERENCES [dbo].[SemesterBatches] ([YearId], [GradeId], [SemesterId])
GO
ALTER TABLE [dbo].[SemesterBatchEnrollments] CHECK CONSTRAINT [FK_SemesterBatchEnrollments_SemesterBatches]
GO
ALTER TABLE [dbo].[SemesterBatchEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_SemesterBatchEnrollments_Semesters] FOREIGN KEY([SemesterId])
REFERENCES [dbo].[Semesters] ([Id])
GO
ALTER TABLE [dbo].[SemesterBatchEnrollments] CHECK CONSTRAINT [FK_SemesterBatchEnrollments_Semesters]
GO
ALTER TABLE [dbo].[SemesterBatchEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_SemesterBatchEnrollments_Students] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([Id])
GO
ALTER TABLE [dbo].[SemesterBatchEnrollments] CHECK CONSTRAINT [FK_SemesterBatchEnrollments_Students]
GO
ALTER TABLE [dbo].[SemesterBatchEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_SemesterBatchEnrollments_TimeYears] FOREIGN KEY([YearId])
REFERENCES [dbo].[TimeYears] ([Id])
GO
ALTER TABLE [dbo].[SemesterBatchEnrollments] CHECK CONSTRAINT [FK_SemesterBatchEnrollments_TimeYears]
GO
ALTER TABLE [dbo].[SemesterBatches]  WITH CHECK ADD  CONSTRAINT [FK_SemesterBatch_Batch] FOREIGN KEY([YearId], [GradeId])
REFERENCES [dbo].[Batches] ([YearId], [GradeId])
GO
ALTER TABLE [dbo].[SemesterBatches] CHECK CONSTRAINT [FK_SemesterBatch_Batch]
GO
ALTER TABLE [dbo].[SemesterBatches]  WITH CHECK ADD  CONSTRAINT [FK_SemesterBatch_Grades] FOREIGN KEY([GradeId])
REFERENCES [dbo].[Grades] ([Id])
GO
ALTER TABLE [dbo].[SemesterBatches] CHECK CONSTRAINT [FK_SemesterBatch_Grades]
GO
ALTER TABLE [dbo].[SemesterBatches]  WITH CHECK ADD  CONSTRAINT [FK_SemesterBatch_Semesters] FOREIGN KEY([SemesterId])
REFERENCES [dbo].[Semesters] ([Id])
GO
ALTER TABLE [dbo].[SemesterBatches] CHECK CONSTRAINT [FK_SemesterBatch_Semesters]
GO
ALTER TABLE [dbo].[SemesterBatches]  WITH CHECK ADD  CONSTRAINT [FK_SemesterBatch_TimeYears] FOREIGN KEY([YearId])
REFERENCES [dbo].[TimeYears] ([Id])
GO
ALTER TABLE [dbo].[SemesterBatches] CHECK CONSTRAINT [FK_SemesterBatch_TimeYears]
GO
ALTER TABLE [dbo].[SubSuppGPAwRecomm]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppGPAwRecomm_Batch] FOREIGN KEY([YearId], [GradeId])
REFERENCES [dbo].[Batches] ([YearId], [GradeId])
GO
ALTER TABLE [dbo].[SubSuppGPAwRecomm] CHECK CONSTRAINT [FK_SubSuppGPAwRecomm_Batch]
GO
ALTER TABLE [dbo].[SubSuppGPAwRecomm]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppGPAwRecomm_BatchEnrollments] FOREIGN KEY([YearId], [GradeId], [StudentId])
REFERENCES [dbo].[BatchEnrollments] ([YearId], [GradeId], [StudentId])
GO
ALTER TABLE [dbo].[SubSuppGPAwRecomm] CHECK CONSTRAINT [FK_SubSuppGPAwRecomm_BatchEnrollments]
GO
ALTER TABLE [dbo].[SubSuppGPAwRecomm]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppGPAwRecomm_CumulativeRecommendations] FOREIGN KEY([CumulativeRecommId])
REFERENCES [dbo].[RecommendationTypes] ([Id])
GO
ALTER TABLE [dbo].[SubSuppGPAwRecomm] CHECK CONSTRAINT [FK_SubSuppGPAwRecomm_CumulativeRecommendations]
GO
ALTER TABLE [dbo].[SubSuppGPAwRecomm]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppGPAwRecomm_GPAwRecomm] FOREIGN KEY([YearId], [GradeId], [StudentId])
REFERENCES [dbo].[GPAwRecomm] ([YearId], [GradeId], [StudentId])
GO
ALTER TABLE [dbo].[SubSuppGPAwRecomm] CHECK CONSTRAINT [FK_SubSuppGPAwRecomm_GPAwRecomm]
GO
ALTER TABLE [dbo].[SubSuppGPAwRecomm]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppGPAwRecomm_Grades] FOREIGN KEY([GradeId])
REFERENCES [dbo].[Grades] ([Id])
GO
ALTER TABLE [dbo].[SubSuppGPAwRecomm] CHECK CONSTRAINT [FK_SubSuppGPAwRecomm_Grades]
GO
ALTER TABLE [dbo].[SubSuppGPAwRecomm]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppGPAwRecomm_Students] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([Id])
GO
ALTER TABLE [dbo].[SubSuppGPAwRecomm] CHECK CONSTRAINT [FK_SubSuppGPAwRecomm_Students]
GO
ALTER TABLE [dbo].[SubSuppGPAwRecomm]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppGPAwRecomm_TimeYears] FOREIGN KEY([YearId])
REFERENCES [dbo].[TimeYears] ([Id])
GO
ALTER TABLE [dbo].[SubSuppGPAwRecomm] CHECK CONSTRAINT [FK_SubSuppGPAwRecomm_TimeYears]
GO
ALTER TABLE [dbo].[SubSuppGPAwRecomm]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppGPAwRecomm_YearRecommendations] FOREIGN KEY([YearRecommId])
REFERENCES [dbo].[RecommendationTypes] ([Id])
GO
ALTER TABLE [dbo].[SubSuppGPAwRecomm] CHECK CONSTRAINT [FK_SubSuppGPAwRecomm_YearRecommendations]
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppMarksExamCW_CourseEnrollment] FOREIGN KEY([YearId], [GradeId], [SemesterId], [StudentId], [CourseId])
REFERENCES [dbo].[CourseEnrollments] ([YearId], [GradeId], [SemesterId], [StudentId], [CourseId])
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW] CHECK CONSTRAINT [FK_SubSuppMarksExamCW_CourseEnrollment]
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppMarksExamCW_Courses] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW] CHECK CONSTRAINT [FK_SubSuppMarksExamCW_Courses]
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppMarksExamCW_ExamType] FOREIGN KEY([ExamTypeId])
REFERENCES [dbo].[ExamTypes] ([Id])
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW] CHECK CONSTRAINT [FK_SubSuppMarksExamCW_ExamType]
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppMarksExamCW_Grades] FOREIGN KEY([GradeId])
REFERENCES [dbo].[Grades] ([Id])
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW] CHECK CONSTRAINT [FK_SubSuppMarksExamCW_Grades]
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppMarksExamCW_MarksExamCW] FOREIGN KEY([YearId], [GradeId], [SemesterId], [StudentId], [CourseId])
REFERENCES [dbo].[MarksExamCW] ([YearId], [GradeId], [SemesterId], [StudentId], [CourseId])
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW] CHECK CONSTRAINT [FK_SubSuppMarksExamCW_MarksExamCW]
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppMarksExamCW_OfferedCourse] FOREIGN KEY([YearId], [GradeId], [SemesterId], [CourseId])
REFERENCES [dbo].[OfferedCourses] ([YearId], [GradeId], [SemesterId], [CourseId])
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW] CHECK CONSTRAINT [FK_SubSuppMarksExamCW_OfferedCourse]
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppMarksExamCW_Semesters] FOREIGN KEY([SemesterId])
REFERENCES [dbo].[Semesters] ([Id])
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW] CHECK CONSTRAINT [FK_SubSuppMarksExamCW_Semesters]
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppMarksExamCW_Students] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([Id])
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW] CHECK CONSTRAINT [FK_SubSuppMarksExamCW_Students]
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW]  WITH CHECK ADD  CONSTRAINT [FK_SubSuppMarksExamCW_TimeYears] FOREIGN KEY([YearId])
REFERENCES [dbo].[TimeYears] ([Id])
GO
ALTER TABLE [dbo].[SubSuppMarksExamCW] CHECK CONSTRAINT [FK_SubSuppMarksExamCW_TimeYears]
GO
USE [master]
GO
ALTER DATABASE [AcademicResultsDB] SET  READ_WRITE 
GO
