Public Module ResultsProcessingUtilities
    Public Enum ExamTypeEnum
        FirstSemester
        SecondSemester
        SubSuppExams
    End Enum
    Public Enum EnrollmentTypeEnum
        Regular = 1
        Transfer
        External
        Repeat
        Resit
    End Enum
    Public Enum RecommTypeEnum
        I = 1
        II1
        II2
        III
        Passed
        Repeat
        Failed
        Subs
        Supp
        SubsSupp
        Resit
        WGPA
        SpecialCase
        Suspend
        Dismiss
        SubYear
        II
    End Enum
    Sub SecondSemesterProcessing(YearId As Integer, GradeId As Integer, DisciplineId As Integer, ExamType As ExamTypeEnum)
        Dim StudList As List(Of BatchEnrollment) = GetStudentEnrollmentList(YearId, GradeId, DisciplineId, ExamType)
        Dim RecommsList As List(Of GPAwRecomm) = New List(Of GPAwRecomm)(StudList.Count)
        For StudInd As Integer = 0 To StudList.Count - 1
            RecommsList.Add(SecondSemesterStudProcess(StudList(StudInd)))
        Next
    End Sub

    Private Function GetStudentEnrollmentList(YearId As Integer, GradeId As Integer, DisciplineId As Integer, ExamType As ExamTypeEnum) As List(Of BatchEnrollment)
        Dim studList = From benr In SharedState.DBContext.BatchEnrollments
                       Where benr.YearId = YearId And benr.GradeId = GradeId
                       Select benr

        Return studList.ToList()
    End Function

    Private Function SecondSemesterStudProcess(studEnr As BatchEnrollment) As GPAwRecomm
        Select Case studEnr.EnrollmentTypeId
            Case EnrollmentTypeEnum.Regular
                Return SecondSemesterProcessRegularStudent(studEnr)
            Case EnrollmentTypeEnum.Transfer
            Case EnrollmentTypeEnum.External
            Case EnrollmentTypeEnum.Repeat
            Case EnrollmentTypeEnum.Resit
            Case Else
                Throw New ArgumentException("Student Enrollment Typeis not Known")
        End Select
        Throw New NotImplementedException
    End Function
    Class GradeTotal
        Public Mark As MarksExamCW
        Public Grade As String
        Public Total As Double
    End Class
    Private Function SecondSemesterProcessRegularStudent(studEnr As BatchEnrollment) As GPAwRecomm
        Dim gpw As GPAwRecomm = (From gp In SharedState.DBContext.GPAwRecomms.Local Where
                                 gp.StudentId = studEnr.StudentId And gp.GradeId = studEnr.GradeId And gp.YearId = studEnr.YearId).SingleOrDefault()
        If gpw Is Nothing Then
            gpw = New GPAwRecomm() With {.StudentId = studEnr.StudentId, .GradeId = studEnr.GradeId, .YearId = studEnr.YearId}
        End If

        Dim Comment As String = ""
        If studEnr.Student.Index = 104012 Then
            Comment = ""
        End If

        Dim Marks As List(Of MarksExamCW) = GetStudentMarks(studEnr, ExamTypeEnum.SecondSemester)
        Dim GradesTotalList As List(Of GradeTotal) = Marks.ConvertAll(Of GradeTotal)(Function(s) AssignGrade(s))
        Dim PreviousCGPA As Decimal? = GetPreviousCGPA(studEnr)

        Dim GPA As Double = EvaluateGPA(GradesTotalList)
        Dim CGPA As Double? = EvaluateCGPA(GPA, PreviousCGPA, studEnr.GradeId)

        gpw.GPA = GPA
        gpw.CGPA = CGPA

        Dim PrjCourseID As Integer = GetProjectCourseID()

        If studEnr.GradeId = 5 And GradesTotalList.Any(Function(s) s.Mark.CourseId = PrjCourseID And (s.Grade = "F" Or s.Grade = "D")) Then
            gpw.GPA = Nothing
            gpw.YearRecommId = RecommTypeEnum.Repeat
            gpw.CGPA = Nothing
            gpw.CumulativeRecommId = RecommTypeEnum.Repeat
            gpw.Comment = "FG 10.5"
            Return gpw
        End If

        gpw.YearRecommId = NiceRecomm(GPA, Comment)
        gpw.CumulativeRecommId = GNiceRecomm(gpw.CGPA, PreviousCGPA, gpw.YearRecommId, Comment)
        gpw.Comment = Comment

        'gpw.RecommendationType = NiceRecomm(GPA) 'Applies 5.11, 8.2
        'gpw.CumulativeRecommendationType = GNiceRecomm(CGPA, PreviousCGPA, Recomm.RecommendationType) 'Applies 5.12

        Dim CountFs As Integer = GradesTotalList.Where(Function(s) s.Grade = "F").Count()
        Dim CountDs As Integer = GradesTotalList.Where(Function(s) s.Grade = "D").Count()
        Dim CountABs As Integer = GradesTotalList.Where(Function(s) s.Grade = "AB" Or s.Grade = "AB*").Count()
        Dim CountSubjects As Integer = GradesTotalList.Count
        Dim CountNonExcused As Integer = Marks.Where(Function(mk) Not mk.Present And (mk.Excuse.HasValue AndAlso Not mk.Excuse)).Count()

        If CountNonExcused > (CountSubjects / 3) Then
            gpw.YearRecommId = RecommTypeEnum.Dismiss
            gpw.Comment = "(FG 11.f)"
        ElseIf CountABs > 0 Then
            gpw.YearRecommId = RecommTypeEnum.Subs
            gpw.CumulativeRecommId = Nothing
            If Double.IsNaN(GPA) And (CountFs + CountDs) < Math.Ceiling(CountSubjects / 3) + 1 Then
                gpw.YearRecommId = RecommTypeEnum.SubsSupp
                gpw.Comment = String.Format("Sub ({0}) + Supp ({1})", CountABs, CountDs + CountFs)
            End If
            If (CountFs + CountDs) = 0 Then
                gpw.Comment = String.Format("Sub ({0})", CountABs)
            End If
        ElseIf CountFs = 0 And CountDs = 0 And GPA >= 4.5 Then
            '// Hurrah! Go for vacation!
            'Return // Applies 8.1
        ElseIf (4.3 <= CGPA) And (CGPA < 4.5) And CountFs = 0 And CountDs = 0 Then
            'Return // 8.2 Already Applied
            gpw.Comment = "(FG 8.2)"
        ElseIf GPA >= 4.5 And CountFs = 0 And CountDs = 1 Then
            'Return // 8.3 Already Applied
            gpw.Comment = "(FG 8.3)"
        ElseIf GPA >= 4.3 And (CountFs + CountDs) < Math.Ceiling(CountSubjects / 3) Then
            'Recomms.YearRecomm.Append( Supp ( CountFs + CountDs ) ) // Applies 9.1
            gpw.YearRecommId = RecommTypeEnum.Supp
            gpw.CumulativeRecommId = RecommTypeEnum.Supp
            gpw.Comment = String.Format("Supp ({0})", CountFs + CountDs)
        ElseIf GPA >= 4.5 And (CountDs + CountFs) < Math.Ceiling(CountSubjects / 3) + 1 And (CountDs >= 1) Then
            'Recomms.YearRecomm .Append(‘Special Case (FG 9.2) ⇒ Supp ( CountFs + CountDs - 1))’)
            gpw.YearRecommId = RecommTypeEnum.SpecialCase
            gpw.CumulativeRecommId = RecommTypeEnum.SpecialCase
            gpw.Comment = String.Format("Special Case (FG 9.2) ⇒ Supp ({0})", CountFs + CountDs)
            '// Applies 9.2
            '// Skip 9.3 applies to Supp
        ElseIf 3.5 <= GPA And GPA < 4.3 And CountFs = 0 And CountDs = 0 Then
            gpw.YearRecommId = RecommTypeEnum.SpecialCase
            gpw.CumulativeRecommId = RecommTypeEnum.SpecialCase
            gpw.Comment = "Special Case (FG 10.1) ⇒ Repeat"
            'Recomms.YearRecomm = “Special Case (FG 10.1) ⇒ Repeat” //Applies 10.1
        ElseIf GPA >= 3.5 And (CountFs + CountDs) > Math.Ceiling(CountSubjects / 3) Then
            gpw.YearRecommId = RecommTypeEnum.SpecialCase
            gpw.CumulativeRecommId = RecommTypeEnum.SpecialCase
            gpw.Comment = "Special Case (FG 10.2) ⇒ Repeat"
            'Recomms.YearRecomm = “Special Case (FG 10.2) ⇒ Repeat” //Applies 10.2
        ElseIf 4.3 <= PreviousCGPA And PreviousCGPA < 4.5 And CGPA < 4.5 Then
            'Return //Already Applied 10.3
            '// Skip 10.4 applies to Supp
            '// 10.5 Applied above (Project)
            '// 10.6 Below (Double Repeats)
        ElseIf GPA < 3.5 Then
            'Recomms.YearRecomm = “Dismiss (FG 11.a)” // Applies 11.a
            gpw.YearRecommId = RecommTypeEnum.Dismiss
            gpw.CumulativeRecommId = RecommTypeEnum.Dismiss
            gpw.Comment = "(FG 11.a)"
        Else
            Throw New Exception("ResultsProcessingUtilities: We don't know what this case Is")
        End If
        If gpw.YearRecommId = 0 Then
            Throw New Exception("ResultsProcessingUtilities: YearRecomm must be assigned")
        End If
        If gpw.CumulativeRecommId = 0 Then
            gpw.CumulativeRecommId = Nothing
        End If
        If gpw.YearRecommId <> 0 Then
            gpw.YearRecommendationType = (From rec In SharedState.DBContext.RecommendationTypes.Local Where rec.Id = gpw.YearRecommId).Single()
        End If
        If gpw.CumulativeRecommId IsNot Nothing Then
            gpw.CumulativeRecommendationType = (From rec In SharedState.DBContext.RecommendationTypes.Local Where rec.Id = gpw.CumulativeRecommId).Single()
        End If

        Return gpw

    End Function

    Private Function GetStudentMarks(studEnr As BatchEnrollment, examTypeEnum As ExamTypeEnum) As List(Of MarksExamCW)
        Dim marks = From mk In SharedState.DBContext.MarksExamCWs
                    Where mk.YearId = studEnr.YearId And mk.StudentId = studEnr.StudentId And mk.GradeId = studEnr.GradeId
                    Select mk

        Return marks.ToList()
    End Function

    Public Function AssignGrade(mk As MarksExamCW) As GradeTotal
        AssignGrade = New GradeTotal()
        AssignGrade.Mark = mk
        Dim CWFraction As Decimal = mk.OfferedCourse.CourseWorkFraction
        Dim ExFraction As Decimal = mk.OfferedCourse.ExamFraction

        If mk.Present Then
            If ExFraction = 100 And CWFraction = 0 Then
                AssignGrade.Total = mk.ExamMark
            ElseIf CWFraction = 100 And ExFraction = 0 Then
                AssignGrade.Total = mk.CWMark
            Else
                AssignGrade.Total = 0
                If mk.CWMark.HasValue Then
                    AssignGrade.Total += mk.CWMark
                End If
                If mk.ExamMark.HasValue Then
                    AssignGrade.Total += mk.ExamMark
                End If
            End If

            If mk.CWMark < 0.3 * CWFraction Or mk.ExamMark < 0.3 * ExFraction Then
                AssignGrade.Grade = "F"
            ElseIf mk.CWMark < 0.4 * CWFraction Or mk.ExamMark < 0.4 * ExFraction Then
                AssignGrade.Grade = "D"
            ElseIf AssignGrade.Total >= 90 Then
                AssignGrade.Grade = "A+"
            ElseIf AssignGrade.Total >= 80 Then
                AssignGrade.Grade = "A"
            ElseIf AssignGrade.Total >= 70 Then
                AssignGrade.Grade = "A-"
            ElseIf AssignGrade.Total >= 60 Then
                AssignGrade.Grade = "B+"
            ElseIf AssignGrade.Total >= 50 Then
                AssignGrade.Grade = "B"
            ElseIf AssignGrade.Total >= 40 Then
                AssignGrade.Grade = "C"
            ElseIf AssignGrade.Total >= 30 Then
                AssignGrade.Grade = "D"
            End If
            Return AssignGrade
        ElseIf Not mk.Present And Not mk.Excuse Then
            AssignGrade.Total = mk.CWMark
            AssignGrade.Grade = "F"
            Return AssignGrade
        ElseIf Not mk.Present And mk.Excuse Then
            AssignGrade.Total = Double.NaN
            If mk.CWMark < 0.4 * CWFraction Then
                AssignGrade.Grade = "AB*"
            Else
                AssignGrade.Grade = "AB"
            End If
        End If
    End Function

    Private Function EvaluateGPA(GradesTotalList As List(Of GradeTotal)) As Double
        Dim TotalMark As Double = 0
        Dim TotalCredits As Double = 0
        For Each gr In GradesTotalList
            TotalMark += gr.Total * gr.Mark.OfferedCourse.CreditHours
            TotalCredits += gr.Mark.OfferedCourse.CreditHours
        Next
        Return (TotalMark / TotalCredits) / 10
    End Function

    Private Function GetPreviousCGPA(studEnr As BatchEnrollment) As Decimal?
        Dim CurrentClass As Integer = studEnr.GradeId
        If CurrentClass = 1 Then
            Return Nothing
        Else
            Dim x = (From gpw In SharedState.DBContext.GPAwRecomms
                    Where gpw.GradeId = CurrentClass - 1 And gpw.StudentId = studEnr.StudentId
                    Order By gpw.YearId Descending Select gpw).FirstOrDefault
            If x Is Nothing Then
                Return Nothing
            Else
                Return x.CGPA
            End If
        End If
    End Function

    Private Function EvaluateCGPA(GPA As Decimal, PreviousCGPA As Decimal?, CurrentClass As Integer) As Double?

        Dim PreviousWeight As Integer = 0
        For i As Integer = 1 To CurrentClass - 1
            PreviousWeight += i
        Next
        Dim NewWieght As Integer = PreviousWeight + CurrentClass
        If PreviousWeight = 0 Then
            Return (CurrentClass * GPA) / NewWieght
        Else
            If PreviousCGPA IsNot Nothing Then
                Return (PreviousCGPA * PreviousWeight + CurrentClass * GPA) / NewWieght
            Else
                Return Nothing
            End If

        End If

    End Function

    Private Function GetProjectCourseID() As Integer
        Return (From cr In SharedState.DBContext.Courses Where cr.CourseCode = "PR5202" Select cr.Id).Single()
    End Function

    Private Function NiceRecomm(GPA As Decimal, ByRef Comment As String) As RecommTypeEnum
        If GPA >= 7.0 Then
            Return RecommTypeEnum.I
        ElseIf GPA >= 6.0 Then
            Return RecommTypeEnum.II
        ElseIf GPA >= 4.3 Then
            Return RecommTypeEnum.III
        ElseIf 3.5 <= GPA < 4.3 Then
            Comment = "Special Case (FG 10.1) => Repeat"
            Return RecommTypeEnum.SpecialCase
        End If
    End Function

    Private Function GNiceRecomm(CGPA As Decimal?, PreviousCGPA As Decimal?, YRecomm As RecommTypeEnum, ByRef Comment As String) As RecommTypeEnum?
        Dim GRecomm As RecommTypeEnum? = Nothing
        If PreviousCGPA >= 4.3 And PreviousCGPA < 4.5 And CGPA < 4.5 Then
            GRecomm = RecommTypeEnum.SpecialCase
            Comment = "(FG10.3) => Repeat"
        ElseIf CGPA >= 4.3 And CGPA < 4.5 Then
            GRecomm = RecommTypeEnum.WGPA
            Comment = "(FG 8.2)"
        ElseIf YRecomm = RecommTypeEnum.I Or YRecomm = RecommTypeEnum.II Or YRecomm = RecommTypeEnum.III Then
            If CGPA > 7.0 Then
                GRecomm = RecommTypeEnum.I
            ElseIf CGPA > 6.5 Then
                GRecomm = RecommTypeEnum.II1
            ElseIf CGPA > 5.7 Then
                GRecomm = RecommTypeEnum.II2
            ElseIf CGPA > 4.5 Then
                GRecomm = RecommTypeEnum.III
            End If
        Else
            GRecomm = YRecomm
        End If
        Return GRecomm
    End Function
End Module
