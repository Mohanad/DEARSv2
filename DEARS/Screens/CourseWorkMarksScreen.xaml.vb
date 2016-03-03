Public Class CourseWorkMarksScreen
    Implements IBaseScreen

    Public ReadOnly Property DBContext As AcademicResultsDBEntities Implements IBaseScreen.DBContext
        Get
            Return SharedState.DBContext
        End Get
    End Property

    Private GradesViewSource As CollectionViewSource
    Private StudentsViewSource As CollectionViewSource

    Public Sub LoadData(PropertyName As String) Implements IBaseScreen.LoadData
        Dim YearID As Integer = SharedState.GetSingleInstance().YearID
        Dim GradeID As Integer = SharedState.GetSingleInstance().GradeID
        Dim SemesterID As Integer = SharedState.GetSingleInstance().SemesterID


        If Not (PropertyName = "GradeID" Or PropertyName = "CourseID" Or PropertyName = "DisciplineID") Then
            Dim q_grades = From bt In DBContext.SemesterBatches.Include("Grade").Include("OfferedCourses")
                       Where bt.SemesterId = SemesterID And bt.YearId = YearID
                       Select bt


            GradesViewSource.Source = New ObservableEntityCollection(Of SemesterBatch)(DBContext, q_grades)

            If q_grades.Count > 0 AndAlso Not (q_grades.ToList().Any(Function(gr) gr.GradeId = GradeID)) Then
                SharedState.GetSingleInstance.GradeID = q_grades.First().GradeId
                GradeID = SharedState.GetSingleInstance().GradeID
            End If
        End If

        Dim CourseID As Integer = SharedState.GetSingleInstance().CourseID
        Dim DisciplineID As Integer = SharedState.GetSingleInstance().DisciplineID

        ' Students with records already
        Dim q_cwmarks = From cwm In DBContext.MarksExamCWs.Include("CourseEnrollment.SemesterBatchEnrollment.BatchEnrollment").Include("Student")
                        Where cwm.YearId = YearID And cwm.GradeId = GradeID And cwm.CourseId = CourseID And cwm.SemesterId = SemesterID
                        Select cwm

        ' Students enrolled in course but no marks
        Dim q_cenr = From cenr In DBContext.CourseEnrollments.Include("SemesterBatchEnrollment.BatchEnrollment").Include("Student")
                     Where cenr.YearId = YearID And cenr.GradeId = GradeID And cenr.CourseId = CourseID And cenr.SemesterId = SemesterID And (cenr.MarksExamCW Is Nothing)
                     Select cenr

        Dim StudentsCollection As New ObservableEntityCollection(Of MarksExamCW)(DBContext, q_cwmarks)

        For Each cenr In q_cenr.ToList()
            StudentsCollection.Add(New MarksExamCW() With {.YearId = YearID, .GradeId = GradeID, .SemesterId = SemesterID, .StudentId = cenr.StudentId,
                                                            .Student = cenr.Student, .CourseEnrollment = cenr})
        Next

        Dim offcr = (From ocr In DBContext.OfferedCourses
                    Where ocr.YearId = YearID And ocr.GradeId = GradeID And ocr.CourseId = CourseID And ocr.SemesterId = SemesterID).SingleOrDefault()
        If offcr IsNot Nothing Then
            MaximumMarkValidation.MaximumMark = offcr.CourseWorkFraction
        Else
            MaximumMarkValidation.MaximumMark = 0
        End If

        StudentsViewSource.Source = StudentsCollection
        StudentsViewSource.View.Filter = AddressOf DisciplineFilterFunction
    End Sub
    Private Sub AllDisciplinescheckBox_Checked(sender As Object, e As RoutedEventArgs)
        StudentsViewSource.View.Filter = AddressOf DisciplineFilterFunction
    End Sub

    Private Sub AllDisciplinescheckBox_Unchecked(sender As Object, e As RoutedEventArgs)
        StudentsViewSource.View.Filter = AddressOf DisciplineFilterFunction
    End Sub

    Private Function DisciplineFilterFunction(s As Object) As Boolean
        Dim item = CType(s, MarksExamCW)
        If AllDisciplinescheckBox.IsChecked Then
            Return True
        Else
            Return (SharedState.GetSingleInstance.DisciplineID = item.CourseEnrollment.SemesterBatchEnrollment.DisciplineId)
        End If
    End Function

    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        GradesViewSource = CType(Me.FindResource("GradesViewSource"), CollectionViewSource)
        StudentsViewSource = CType(Me.FindResource("StudentsViewSource"), CollectionViewSource)
        LoadData("")
        QueryParamsBox.DataContext = SharedState.GetSingleInstance
    End Sub

    Private Sub UserControl_Unloaded(sender As Object, e As RoutedEventArgs)
        QueryParamsBox.DataContext = Nothing
    End Sub

    Public Sub SaveDataColumnsToEntities(ExtractedData As Dictionary(Of String, List(Of String))) Implements IBaseScreen.SaveDataColumnsToEntities
        'For Coursework Marks we have YearID, GradeID, SemesterID, CourseID, StudentID.
        Dim YearID As Integer = SharedState.GetSingleInstance().YearID
        Dim GradeID As Integer = SharedState.GetSingleInstance().GradeID
        Dim SemesterID As Integer = SharedState.GetSingleInstance().SemesterID
        Dim CourseID As Integer = SharedState.GetSingleInstance().CourseID

        Dim indexList = ExtractedData("Index").ConvertAll(Function(s) Integer.Parse(s))

        'TODO: Check indices before modifying entities. This ensures either full import or no import.

        For i As Integer = 0 To indexList.Count - 1
            Dim ind = indexList(i)
            Dim StudentID As Integer = (From stud In SharedState.DBContext.Students.Local
                                        Where stud.Index = ind Select stud.Id).Single()
            Dim cwmark = (From cwm In SharedState.DBContext.MarksExamCWs.Local
                          Where cwm.StudentId = StudentID And cwm.GradeId = GradeID And cwm.YearId = YearID And _
                          cwm.SemesterId = SemesterID And cwm.CourseId = CourseID).Single()
            If ExtractedData("Coursework Marks") IsNot Nothing Then
                cwmark.CWMark = ExtractedData("Coursework Marks")(i)
            End If
        Next

        CourseworkMarksDataGrid.Items.Refresh()
    End Sub

End Class

Public Class MaximumMarkValidation
    Inherits ValidationRule
    Public Shared Property MaximumMark As Decimal
    Public Overloads Overrides Function Validate(value As Object, cultureInfo As Globalization.CultureInfo) As ValidationResult
        If value IsNot Nothing Then
            Dim val As Decimal
            If Not Decimal.TryParse(value, val) Then
                Return New ValidationResult(False, "Must be a number")
            ElseIf val > MaximumMark Or val < 0 Then
                Return New ValidationResult(False, "Must be between 0 And " & MaximumMark.ToString)
            Else
                Return New ValidationResult(True, Nothing)
            End If
        End If
    End Function
End Class
