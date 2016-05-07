Public Class DisciplineEnrollmentScreen
    Implements IBaseScreen

    Public ReadOnly Property DBContext As AcademicResultsDBEntities Implements IBaseScreen.DBContext
        Get
            Return SharedState.DBContext
        End Get
    End Property

    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        GradesViewSource = CType(Me.FindResource("GradesViewSource"), CollectionViewSource)
        DisciplineEnrollmentsViewSource = CType(Me.FindResource("DisciplineEnrollmentsViewSource"), CollectionViewSource)
        LoadData("")
        QueryParamsBox.DataContext = SharedState.GetSingleInstance()
    End Sub
    Private Sub UserControl_Unloaded(sender As Object, e As RoutedEventArgs)
        QueryParamsBox.DataContext = Nothing
    End Sub
    Private GradesViewSource As CollectionViewSource
    Private DisciplineEnrollmentsViewSource As CollectionViewSource

    Sub LoadData(PropertyName As String) Implements IBaseScreen.LoadData
        Dim YearID As Integer = SharedState.GetSingleInstance().YearID
        Dim GradeID As Integer = SharedState.GetSingleInstance().GradeID
        Dim SemesterID As Integer = SharedState.GetSingleInstance().SemesterID

        If Not (PropertyName = "GradeID" Or PropertyName = "DisciplineID") Then
            Dim q_grades = From bt In DBContext.SemesterBatches.Include("Grade").Include("OfferedDisciplines")
                       Where bt.SemesterId = SemesterID And bt.YearId = YearID
                       Select bt


            GradesViewSource.Source = q_grades.ToList()

            If q_grades.Count > 0 AndAlso Not (q_grades.ToList().Any(Function(gr) gr.GradeId = GradeID)) Then
                SharedState.GetSingleInstance.GradeID = q_grades.First().GradeId
                GradeID = SharedState.GetSingleInstance().GradeID
            End If
        End If

        GradeID = SharedState.GetSingleInstance().GradeID

        SharedState.DBContext.Configuration.AutoDetectChangesEnabled = False
        SharedState.DBContext.Configuration.ValidateOnSaveEnabled = False

        Dim q_discipenr = From denr In DBContext.SemesterBatchEnrollments.Include("Student")
                          Where denr.YearId = YearID And denr.SemesterId = SemesterID And denr.GradeId = GradeID
                          Select denr


        Dim q_nodiscp = From senr In DBContext.BatchEnrollments.Include("Student")
                        Where senr.YearId = YearID And senr.GradeId = GradeID And Not (senr.SemesterBatchEnrollments.Any(Function(s) s.SemesterId = SemesterID))
                        Select senr

        Dim DiscpEnrCollection As New ObservableEntityCollection(Of SemesterBatchEnrollment)(DBContext, q_discipenr)

        'Auto Enroll if only one specialization.
        Dim discpoff = (From offd In SharedState.DBContext.OfferedDisciplines
                       Where offd.YearId = YearID And offd.GradeId = GradeID And offd.SemesterId = SemesterID
                       Select offd).ToList()

        Dim countDisciplines As Integer = discpoff.Count()

        For Each benr In q_nodiscp.ToList()
            If countDisciplines = 1 Then
                DiscpEnrCollection.Add(New SemesterBatchEnrollment() With {.Student = benr.Student, .YearId = YearID, .GradeId = GradeID, .SemesterId = SemesterID,
                                                                           .DisciplineId = discpoff.Single().DisciplineId})
            Else
                DiscpEnrCollection.Add(New SemesterBatchEnrollment() With {.Student = benr.Student, .YearId = YearID, .GradeId = GradeID, .SemesterId = SemesterID})
            End If
        Next

        SharedState.DBContext.Configuration.AutoDetectChangesEnabled = True
        SharedState.DBContext.Configuration.ValidateOnSaveEnabled = True

        DisciplineEnrollmentsViewSource.Source = DiscpEnrCollection
    End Sub

    Public Sub SaveDataColumnsToEntities(ExtractedData As Dictionary(Of String, List(Of String))) Implements IBaseScreen.SaveDataColumnsToEntities
        ''For Student Registration YearID, GradeID, StudentID.
        'Dim YearID As Integer = SharedState.GetSingleInstance().YearID
        'Dim GradeID As Integer = SharedState.GetSingleInstance().GradeID

        'Dim indexList = ExtractedData("Index").ConvertAll(Function(s) Integer.Parse(s))

        ''TODO: Check indices before modifying entities. This ensures either full import or no import.

        'For i As Integer = 0 To indexList.Count - 1
        '    Dim ind = indexList(i)
        '    Dim student As Student = (From stud In SharedState.DBContext.Students
        '                                Where stud.Index = ind Select stud).SingleOrDefault()
        '    Dim senr = (From enr In SharedState.DBContext.BatchEnrollments.Local
        '                Where enr.StudentId = student.Id And enr.YearId = YearID And enr.GradeId = GradeID).SingleOrDefault()

        '    Dim xet = ExtractedData("Enrollment")(i)
        '    If senr Is Nothing Then
        '        Dim stud_searcher = New StudentSearcher() With {.Student = student}
        '        StudsCollection.Add(stud_searcher)
        '        senr = stud_searcher.BatchEnrollment
        '    End If
        '    senr.EnrollmentTypeId = (From et In SharedState.DBContext.EnrollmentTypes.Local
        '                                 Where et.NameEnglish = xet Select et.Id).SingleOrDefault()
        'Next
    End Sub
End Class
