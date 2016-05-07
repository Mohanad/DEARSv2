Public Class BoardResultsScreen
    Implements IBaseScreen

    Public ReadOnly Property DBContext As AcademicResultsDBEntities Implements IBaseScreen.DBContext
        Get
            Return SharedState.DBContext
        End Get
    End Property
    Sub LoadData(PropertyName As String) Implements IBaseScreen.LoadData
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
    End Sub

    Public Sub SaveDataColumnsToEntities(ExtractedData As Dictionary(Of String, List(Of String))) Implements IBaseScreen.SaveDataColumnsToEntities

    End Sub

    Private GradesViewSource As CollectionViewSource
    Private GPAViewSource As CollectionViewSource
    Private Sub AllDisciplinescheckBox_Checked(sender As Object, e As RoutedEventArgs)
        'GPAViewSource.View.Filter = AddressOf DisciplineFilterFunction
    End Sub

    Private Sub AllDisciplinescheckBox_Unchecked(sender As Object, e As RoutedEventArgs)
        'GPAViewSource.View.Filter = AddressOf DisciplineFilterFunction
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
                resIssy.AddDisciplineResultsBoard(disc.Id, False)
            Next
            resIssy.Save()
            MsgBox("Done!")
        Else
            Return
        End If
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
End Class
