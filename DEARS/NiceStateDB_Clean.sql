USE [AcademicResultsDB]


INSERT INTO dbo.TuitionTypes(Id, NameEnglish, NameArabic)
VALUES
(1, N'Lecture', N'محاضرة'),
(2, N'Lab',N'معمل'),
(3, N'Tutorial', N'تمرين')

INSERT [dbo].TimeYears (Id, NameEnglish, NameArabic)
VALUES
(2010, N'2010/2011', N'2010/2011'),
(2011, N'2011/2012', N'2011/2012'),
(2012, N'2012/2013', N'2012/2013'),
(2013, N'2013/2014', N'2013/2014'),
(2014, N'2014/2015', N'2014/2015')


INSERT dbo.Semesters(Id, NameEnglish, NameArabic)
VALUES 
(1, N'First', N'الأول'),
(2, N'Second', N'الثاني')


SET IDENTITY_INSERT [dbo].[Grades] ON 

INSERT [dbo].[Grades] ([Id], [NameEnglish], [NameArabic]) 
VALUES 
(1, N'First', N'الأول'),
(2, N'Second', N'الثاني'),
(3, N'Thrid', N'الثالث'),
(4, N'Fourth', N'الرابع'),
(5, N'Fifth', N'الخامس')
SET IDENTITY_INSERT [dbo].[Grades] OFF


SET IDENTITY_INSERT [dbo].[Disciplines] ON 

INSERT [dbo].[Disciplines] ([Id], [NameEnglish], [NameArabic], [NameEnglishShort], [NameArabicShort]) 
VALUES 
(1, N'Non-Specialized', N'غير متخصص', N'Non-Specialized', N'غير متخصص')
SET IDENTITY_INSERT [dbo].[Disciplines] OFF


DBCC CHECKIDENT ('Courses', RESEED, 0);
SET IDENTITY_INSERT [dbo].[Courses] OFF

DBCC CHECKIDENT ('Teachers', RESEED, 0);
SET IDENTITY_INSERT [dbo].[Teachers] ON

SET IDENTITY_INSERT [dbo].[Teachers] OFF


INSERT [dbo].[ExamTypes] ([Id], [NameArabic], [NameEnglish]) 
VALUES 
(1, N'النظامي', N'Regular'),
(2, N'ملاحق و بديل', N'Subs/Supp')

INSERT [dbo].[EnrollmentTypes] ([Id], NameArabic, [NameEnglish]) 
VALUES 
(1, N'نظامي', N'Regular'),
(2, N'تحويل', N'Transfer'),
(3, N'خارجي', N'External'),
(4, N'إعادة', N'Repeat'),
(5, N'إعادة جلوس', N'Resit')

INSERT [dbo].[RecommendationTypes] ([Id], NameArabic, NameEnglish, [ShortNameEnglish], [Pass]) 
VALUES 
(1, N'مرتبة الشرف اللأولى', N'First Degree Honour', N'I', 1),
(17, N'مرتبة الشرف الثانية', N'Second Degree Honour', N'II', 1),
(2, N'مرتبة الشرف الثانية - القسم الأول', N'Second Degree Honour - First Division', N'II-1', 1),
(3, N'مرتبة الشرف الثانية - القسم الثاني', N'Second Degree Honour - Second Division', N'II-2', 1),
(4, N'مرتبة الشرف الثالثة', N'Third Degree Honour', N'III', 1),
(5, N'نجاح', N'Passed', N'Passed', 1),
(6, N'إعادة', N'Repeat', N'Repeat', 0),
(7, N'رسوب', N'Failed', N'Failed', 0),
(8, N'بديل', N'Substitutes', N'Subs', 0),
(9, N'ملحق', N'Supplementary', N'Supp.', 0),
(10, N'بديل\ملحق', N'Substitute/Supplementary', N'Subs./Supp.', 0),
(11, N'إعادة جلوس', N'Resit', N'Resit', 0),
(12, N'معدل ضعيف', N'Weak GPA', N'WGPA', 0),
(13, N'حالة خاصة', N'Special Case', N'Special Case', 0),
(14, N'تجميد', N'Suspend', N'Suspend', 0),
(15, N'فصل', N'Dismiss', N'Dismiss', 0),
(16, N'عام بديل', N'Substitute Year', N'Sub. Year', 0)


INSERT [dbo].[RecommTranslations] ([ResText], [RecommendationTypeN])
VALUES 
(N'A', 13),
(N'I', 1),
(N'II-1', 2),
(N'II-2', 3),
(N'III', 4),
(N'III-WGPA', 12),
(N'Resit 1', 11),
(N'Resit 2', 11),
(N'Resit 4', 11),
(N'S', 9),
(N'WGPA', 12),
(N'WGPA - ملحق 4', 12),
(N'إعادة', 6),
(N'إعادة جلوس', 11),
(N'إعادة جلوس 1', 11),
(N'إعادة جلوس 2', 11),
(N'إعادة جلوس 3', 11),
(N'اعادة', 6),
(N'اعادة جلوس 2', 11),
(N'بديل', 8),
(N'بديل 1', 8),
(N'بديل 1 + ملحق 2', 10),
(N'بديل 1 + ملحق 4', 10),
(N'بديل 1 ملحق 1', 10),
(N'بديل 2', 8),
(N'بديل 3', 8),
(N'بديل 4', 8),
(N'بديل 4 الدور الثاني', 8),
(N'بديل 4 ملحق 1', 10),
(N'بديل+4 ملحق', 10),
(N'تجميد', 14),
(N'حالة خاصة', 13),
(N'رسوب', 7),
(N'سنة بديلة', 16),
(N'عام بديل', 16),
(N'عام دراسي بديل', 16),
(N'فصل', 15),
(N'ملحق 1', 9),
(N'ملحق 1 + بديل 1', 10),
(N'ملحق 1 + بديل 4', 10),
(N'ملحق 1 + بديل 8', 10),
(N'ملحق 1 + فصل دراسي بديل', 10),
(N'ملحق 1 بديل 8', 10),
(N'ملحق 10', 9),
(N'ملحق 12', 9),
(N'ملحق 14', 9),
(N'ملحق 16', 9),
(N'ملحق 2', 9),
(N'ملحق 2 + بديل 2', 10),
(N'ملحق 2 + بديل 5', 10),
(N'ملحق 2 + بديل 6', 10),
(N'ملحق 3', 9),
(N'ملحق 3 + بديل 1', 10),
(N'ملحق 3 + بديل 4', 10),
(N'ملحق 3 بديل 1', 10),
(N'ملحق 4', 9),
(N'ملحق 4 + بديل 1', 10),
(N'ملحق 4 + بديل 2', 10),
(N'ملحق 4 + بديل 3', 10),
(N'ملحق 4 بديل 1', 10),
(N'ملحق 4 بديل 2', 10),
(N'ملحق 5', 9),
(N'ملحق 5 + بديل 1', 10),
(N'ملحق 5 + بديل 2', 10),
(N'نجاح', 5)