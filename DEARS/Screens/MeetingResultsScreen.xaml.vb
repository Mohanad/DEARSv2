Imports DetailedResultsImporter

Public Class MeetingResultsScreen
    Implements IBaseScreen

    Private _db As AcademicResultsDBEntities
    Public ReadOnly Property DBContext As AcademicResultsDBEntities Implements IBaseScreen.DBContext
        Get
            Return SharedState.DBContext
        End Get
    End Property

    Private GradesViewSource As CollectionViewSource
    Private GPAViewSource As CollectionViewSource
    Public Sub LoadData(PropertyName As String) Implements IBaseScreen.LoadData
        Dim YearID As Integer = SharedState.GetSingleInstance().YearID
        Dim GradeID As Integer = SharedState.GetSingleInstance().GradeID
        Dim SemesterID As Integer = SharedState.GetSingleInstance().SemesterID

        If Not (PropertyName = "GradeID" Or PropertyName = "CourseID" Or PropertyName = "DisciplineID") Then
            Dim q_grades = From bt In DBContext.SemesterBatches.Include("Grade").Include("OfferedDisciplines").Include("OfferedDisciplines.Discipline")
                       Where bt.SemesterId = SemesterID And bt.YearId = YearID
                       Select bt


            GradesViewSource.Source = New ObservableEntityCollection(Of SemesterBatch)(DBContext, q_grades)

            If q_grades.Count > 0 AndAlso Not (q_grades.ToList().Any(Function(gr) gr.GradeId = GradeID)) Then
                SharedState.GetSingleInstance.GradeID = q_grades.First().GradeId
                GradeID = SharedState.GetSingleInstance().GradeID
            End If
        End If

        GradeID = SharedState.GetSingleInstance().GradeID
        Dim CourseID As Integer = SharedState.GetSingleInstance().CourseID
        Dim DisciplineID As Integer = SharedState.GetSingleInstance().DisciplineID

        Dim prefetch_bt = (From benr In SharedState.DBContext.BatchEnrollments.Include("GPAwRecomm")
                  Where benr.YearId = YearID And GradeID = benr.GradeId).ToList()

        Dim q_gpas = (From benr In prefetch_bt
                     Where benr.GPAwRecomm IsNot Nothing
                     Select benr.GPAwRecomm).ToList()

        DBContext.ChangeTracker.DetectChanges()

        Dim StudentsCollection As New ObservableEntityCollection(Of GPAwRecomm)(DBContext, q_gpas.ToList())

        For Each cenr In ((From x In prefetch_bt Where x.GPAwRecomm Is Nothing).ToList())
            StudentsCollection.Add(New GPAwRecomm() With {.YearId = YearID, .GradeId = GradeID, .StudentId = cenr.StudentId,
                                                          .Student = cenr.Student})
        Next

        GPAViewSource.Source = StudentsCollection
        GPAViewSource.View.Filter = AddressOf DisciplineFilterFunction
    End Sub

    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        GradesViewSource = CType(Me.FindResource("GradesViewSource"), CollectionViewSource)
        GPAViewSource = CType(Me.FindResource("GPAViewSource"), CollectionViewSource)
        LoadData("")
        QueryParamsBox.DataContext = SharedState.GetSingleInstance
    End Sub

    Private Sub UserControl_Unloaded(sender As Object, e As RoutedEventArgs)
        QueryParamsBox.DataContext = Nothing
    End Sub

    Private Sub GenerateButton_Click(sender As Object, e As RoutedEventArgs)
        Dim flbrwsr As New Forms.SaveFileDialog()
        flbrwsr.Filter = "Excel OpenXML Document (*.xlsx) |*.xlsx"
        If flbrwsr.ShowDialog() = Forms.DialogResult.OK Then
            Dim resIssy As New ResultsIssue(flbrwsr.FileName, SharedState.GetSingleInstance.YearID, SharedState.GetSingleInstance.SemesterID = 1, SharedState.GetSingleInstance.GradeID)
            Dim YearID As Integer = SharedState.GetSingleInstance().YearID
            Dim GradeID As Integer = SharedState.GetSingleInstance().GradeID
            Dim SemesterID As Integer = SharedState.GetSingleInstance().SemesterID
            Dim DisciplineID As Integer = SharedState.GetSingleInstance.DisciplineID

            Dim bD = SharedState.GetSingleInstance.AllDisciplines
            Dim discs = (From d In DBContext.OfferedDisciplines
                         Where d.YearId = YearID And d.GradeId = GradeID And d.SemesterId = SemesterID And (bD Or (d.DisciplineId = DisciplineID))
                         Select d.Discipline).ToList()
            For Each disc In discs
                resIssy.AddDisciplineResults(disc.Id, False)
            Next
            resIssy.Save()
            MsgBox("Done!")
        Else
            Return
        End If
    End Sub

    Private Sub ProcessButton_Click(sender As Object, e As RoutedEventArgs)
        Try
            Dim alldiscp As Boolean = Me.AllDisciplinescheckBox.IsChecked
            Me.GradeComboBox.IsEnabled = False
            Me.DisciplineComboBox.IsEnabled = False
            Me.AllDisciplinescheckBox.IsEnabled = False
            Me.GenerateButton.IsEnabled = False

            Me.ProcessButton.Content = "Cancel"

            SharedState.DBContext.RecommendationTypes.ToList()

            ' Start the processing operation.
            Dim YearID As Integer = SharedState.GetSingleInstance().YearID
            Dim GradeID As Integer = SharedState.GetSingleInstance().GradeID
            Dim SemesterID As Integer = SharedState.GetSingleInstance().SemesterID
            Dim DisciplineID As Integer = SharedState.GetSingleInstance.DisciplineID

            SharedState.DBContext.Configuration.AutoDetectChangesEnabled = False

            ResultsProcessingUtilities.SecondSemesterProcessing(YearID, GradeID, DisciplineID, ExamTypeEnum.SecondSemester)
        Catch ex As Exception
            MsgBox(Application.FlattenOutException(ex))
        Finally
            SharedState.DBContext.ChangeTracker.DetectChanges()
            SharedState.DBContext.Configuration.AutoDetectChangesEnabled = True

            Me.GradeComboBox.IsEnabled = True
            Me.AllDisciplinescheckBox.IsEnabled = True
            Me.AllDisciplinescheckBox.IsChecked = SharedState.GetSingleInstance.AllDisciplines
            Me.DisciplineComboBox.IsEnabled = Not Me.AllDisciplinescheckBox.IsChecked
            Me.GenerateButton.IsEnabled = True
            Me.ProcessButton.Content = "Process"
            Me.ResultsDataGrid.Items.Refresh()
        End Try
    End Sub

    Private Sub AllDisciplinescheckBox_Checked(sender As Object, e As RoutedEventArgs)
        GPAViewSource.View.Filter = AddressOf DisciplineFilterFunction
    End Sub

    Private Sub AllDisciplinescheckBox_Unchecked(sender As Object, e As RoutedEventArgs)
        GPAViewSource.View.Filter = AddressOf DisciplineFilterFunction
    End Sub

    Private Function DisciplineFilterFunction(s As Object) As Boolean
        Dim item = CType(s, GPAwRecomm)
        If AllDisciplinescheckBox.IsChecked Then
            Return True
        Else
            Dim SemesterID = SharedState.GetSingleInstance.SemesterID
            Return (SharedState.GetSingleInstance.DisciplineID = _
                    item.BatchEnrollment.SemesterBatchEnrollments.Where(Function(q) q.SemesterId = SemesterID).First.DisciplineId)
        End If
    End Function
End Class
