CREATE TABLE [dbo].[SubSuppMarksExamCW]
(
	[YearId] INT NOT NULL, 
    [GradeId] INT NOT NULL, 
    [SemesterId] INT NOT NULL,
	[CourseId] INT NOT NULL,
	[StudentId] INT NOT NULL,

	ExamTypeId INT NOT NULL,
    [CWMark] FLOAT NULL,
	Present BIT NULL,
	Excuse BIT NULL,
	ExamMark FLOAT NULL,

	CONSTRAINT [FK_SubSuppMarksExamCW_TimeYears] FOREIGN KEY ([YearId]) REFERENCES [TimeYears]([Id]) ,
    CONSTRAINT [FK_SubSuppMarksExamCW_Grades] FOREIGN KEY ([GradeId]) REFERENCES [Grades]([Id]) ,
	CONSTRAINT [FK_SubSuppMarksExamCW_Students] FOREIGN KEY ([StudentId]) REFERENCES [Students]([Id]) ,
	CONSTRAINT [FK_SubSuppMarksExamCW_Semesters] FOREIGN KEY ([SemesterId]) REFERENCES [Semesters]([Id]) ,
	CONSTRAINT [FK_SubSuppMarksExamCW_Courses] FOREIGN KEY ([CourseId]) REFERENCES [Courses]([Id]),
	CONSTRAINT [FK_SubSuppMarksExamCW_ExamType] FOREIGN KEY (ExamTypeId) REFERENCES [ExamTypes]([Id]),

	CONSTRAINT [FK_SubSuppMarksExamCW_CourseEnrollment] FOREIGN KEY ([YearId], [GradeId], [SemesterId], [StudentId], [CourseId]) 
			REFERENCES CourseEnrollments ([YearId], [GradeId], [SemesterId], [StudentId], [CourseId]),
	CONSTRAINT [FK_SubSuppMarksExamCW_OfferedCourse] FOREIGN KEY ([YearId], [GradeId], [SemesterId], [CourseId])
			REFERENCES OfferedCourses ([YearId], [GradeId], [SemesterId], [CourseId]),
	CONSTRAINT [FK_SubSuppMarksExamCW_MarksExamCW] FOREIGN KEY ([YearId], [GradeId], [SemesterId], [StudentId], [CourseId])
			REFERENCES MarksExamCW ([YearId], [GradeId], [SemesterId], [StudentId], [CourseId]),
	CONSTRAINT [PK_SubSuppMarksExamCW] PRIMARY KEY ([YearId], [GradeId], [SemesterId], [StudentId], [CourseId]),
)
