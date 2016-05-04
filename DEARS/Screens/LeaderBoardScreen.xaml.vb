Public Class LeaderBoardScreen
    Implements IBaseScreen


    Public ReadOnly Property DBContext As AcademicResultsDBEntities Implements IBaseScreen.DBContext
        Get
            Return SharedState.DBContext
        End Get
    End Property

    Public Sub LoadData(PropertyName As String) Implements IBaseScreen.LoadData
        Dim YearID As Integer = SharedState.GetSingleInstance().YearID
        Dim GradeID As Integer = SharedState.GetSingleInstance().GradeID
        Dim SemesterID As Integer = SharedState.GetSingleInstance().SemesterID

        If Not (PropertyName = "GradeID") Then
            Dim q_grades = From bt In DBContext.SemesterBatches.Include("Grades")
                       Where bt.SemesterId = SemesterID And bt.YearId = YearID
                       Select bt.Grade Distinct

            GradesViewSource.Source = q_grades.ToList()

            If q_grades.Count > 0 AndAlso Not (q_grades.ToList().Any(Function(gr) gr.Id = GradeID)) Then
                SharedState.GetSingleInstance.GradeID = q_grades.First().Id
                GradeID = SharedState.GetSingleInstance().GradeID
            End If
        End If

        Dim ofcrs = (From ofc In SharedState.DBContext.OfferedCourses.Include("Course")
                  Where ofc.YearId = YearID And ofc.GradeId = GradeID And SemesterID = ofc.SemesterId
                  Select ofc).ToList()

        Dim leaderItemsList = ofcrs.ConvertAll(Of LeaderItemModel)(Function(s)
                                                                       Dim sc As New LeaderItemModel()
                                                                       sc.CourseCode = s.Course.CourseCode
                                                                       sc.CourseTitle = s.Course.TitleEnglish
                                                                       sc.TotalStudents = s.CourseEnrollments.Count()
                                                                       If sc.TotalStudents = 0 Then
                                                                           Return Nothing
                                                                       End If
                                                                       sc.SubmittedStudentsCW = s.MarksExamCWs.Where(Function(mk) mk.CWMark IsNot Nothing).Count()
                                                                       sc.SubmittedStudentsEX = s.MarksExamCWs.Where(Function(mk) mk.ExamMark IsNot Nothing Or (Not mk.Present)).Count()
                                                                       If s.CourseTeachers.FirstOrDefault IsNot Nothing Then
                                                                           sc.TeacherName = s.CourseTeachers.FirstOrDefault().Teacher.NameEnglish
                                                                           sc.TeacherPhone = "N/A"
                                                                       Else
                                                                           sc.TeacherName = "N/A"
                                                                           sc.TeacherPhone = "N/A"
                                                                       End If
                                                                       sc.OfferedCourse = s
                                                                       Return sc
                                                                   End Function)

        LeaderBoardListBox.ItemsSource = leaderItemsList

        Dim offc As New List(Of OfferedCourseStatisticsModel)

        For Each ldIt In leaderItemsList
            If ldIt IsNot Nothing Then
                offc.Add(GetOfferedCourseStatisticsModel(ldIt.OfferedCourse))
            End If
        Next

        SummaryStatisticsFataGrid.ItemsSource = offc

    End Sub

    Public Sub SaveDataColumnsToEntities(ExtractedData As Dictionary(Of String, List(Of String))) Implements IBaseScreen.SaveDataColumnsToEntities

    End Sub
    Private GradesViewSource As CollectionViewSource

    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        GradesViewSource = CType(Me.FindResource("GradesViewSource"), CollectionViewSource)
        LoadData("")
        QueryParamnsBox.DataContext = SharedState.GetSingleInstance
    End Sub

    Private Sub LeaderBoardListBox_MouseUp(sender As Object, e As MouseButtonEventArgs)
        MsgBox(sender.ToString)
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        'MsgBox(CType(CType(sender, Button).DataContext, LeaderItemModel).CourseTitle)
        Dim statDialog As New OfferedCourseStatisticsDialog()
        If CType(sender, Button).DataContext IsNot Nothing Then
            statDialog.PopulateWithCourseData(CType(CType(sender, Button).DataContext, LeaderItemModel).OfferedCourse)
            statDialog.ShowDialog()
        Else
            MsgBox("Zero Enrollment Course")
        End If
    End Sub
End Class

Class LeaderItemModel
    Property TotalStudents As Integer
    Property SubmittedStudentsCW As Integer
    Property SubmittedStudentsEX As Integer
    Property CourseCode As String
    Property CourseTitle As String
    Property TeacherName As String
    Property OfferedCourse As OfferedCourse
    Property TeacherPhone As String
    'Property TeacherEmail As String
End Class
