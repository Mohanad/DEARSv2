CREATE TABLE [dbo].[SubSuppGPAwRecomm]
(
	[YearId] INT NOT NULL, 
    [GradeId] INT NOT NULL, 
    [StudentId] INT NOT NULL,
	GPA FLOAT,
	YearRecommId INT NOT NULL,
	CGPA FLOAT ,
	CumulativeRecommId INT,
	Comment NVARCHAR(50),
	CONSTRAINT [FK_SubSuppGPAwRecomm_TimeYears] FOREIGN KEY ([YearId]) REFERENCES [TimeYears]([Id]) ,
    CONSTRAINT [FK_SubSuppGPAwRecomm_Grades] FOREIGN KEY ([GradeId]) REFERENCES [Grades]([Id]) ,
	CONSTRAINT [FK_SubSuppGPAwRecomm_Students] FOREIGN KEY ([StudentId]) REFERENCES [Students]([Id]) ,
	CONSTRAINT [FK_SubSuppGPAwRecomm_YearRecommendations] FOREIGN KEY (YearRecommId) REFERENCES RecommendationTypes(Id),
	CONSTRAINT [FK_SubSuppGPAwRecomm_CumulativeRecommendations] FOREIGN KEY (CumulativeRecommId) REFERENCES RecommendationTypes(Id),
	CONSTRAINT [FK_SubSuppGPAwRecomm_Batch] FOREIGN KEY (YearId, GradeId) REFERENCES [Batches](YearId, GradeId) , 
	CONSTRAINT [FK_SubSuppGPAwRecomm_BatchEnrollments] FOREIGN KEY ([YearId], [GradeId], [StudentId]) REFERENCES BatchEnrollments,
	CONSTRAINT [FK_SubSuppGPAwRecomm_GPAwRecomm] FOREIGN KEY ([YearId], [GradeId], [StudentId]) REFERENCES GPAwRecomm,
	CONSTRAINT [PK_SubSuppGPAwRecomm] PRIMARY KEY ([YearId], [GradeId], [StudentId])
)
