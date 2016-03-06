Public Class OfferedDisciplinesScreen
    Implements IBaseScreen

    Private _db As AcademicResultsDBEntities
    Public ReadOnly Property DBContext As AcademicResultsDBEntities Implements IBaseScreen.DBContext
        Get
            Return SharedState.DBContext
        End Get
    End Property


    Private GradesViewSource As CollectionViewSource
    Private DisciplinesViewSource As CollectionViewSource
    Private OfferedDisciplinesViewSource As CollectionViewSource


    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        LoadData("")
    End Sub

    Public Sub LoadData(PropertyName As String) Implements IBaseScreen.LoadData
        Dim YearID As Integer = SharedState.GetSingleInstance.YearID
        Dim SemesterID As Integer = SharedState.GetSingleInstance.SemesterID

        GradesViewSource = CType(Me.FindResource("GradesViewSource"), CollectionViewSource)
        DisciplinesViewSource = CType(Me.FindResource("DisciplinesViewSource"), CollectionViewSource)
        OfferedDisciplinesViewSource = CType(Me.FindResource("OfferedDisciplinesViewSource"), CollectionViewSource)

        GradesViewSource.Source = New ObservableEntityCollection(Of Grade)(DBContext, DBContext.Grades)
        DisciplinesViewSource.Source = New ObservableEntityCollection(Of Discipline)(DBContext, DBContext.Disciplines)

        Dim q_offereddisciplines = From od In DBContext.OfferedDisciplines
                                   Where od.YearId = YearID And od.SemesterId = SemesterID
                                   Select od

        OfferedDisciplinesViewSource.Source = New ObservableEntityCollection(Of OfferedDiscipline)(DBContext, q_offereddisciplines)
    End Sub

    Public Sub SaveDataColumnsToEntities(ExtractedData As Dictionary(Of String, List(Of String))) Implements IBaseScreen.SaveDataColumnsToEntities
        Dim YearID As Integer = SharedState.GetSingleInstance().YearID
        Dim SemesterID As Integer = SharedState.GetSingleInstance().SemesterID

        'TODO: Check Entities to be formed

        For i As Integer = 0 To ExtractedData("Grade").Count - 1
            Dim grade = ExtractedData("Grade")(i)
            Dim discipline = ExtractedData("Discipline")(i)
            Dim GradeID As Integer? = (From gr In SharedState.DBContext.Grades.Local
                           Where gr.NameEnglish = grade Select gr.Id).SingleOrDefault()
            Dim DisciplineID As Integer? = (From ds In SharedState.DBContext.Disciplines.Local
                                            Where ds.NameEnglishShort = discipline Select ds.Id).SingleOrDefault()
            Dim dsc = (From od In SharedState.DBContext.OfferedDisciplines.Local
                       Where od.YearId = YearID And od.SemesterId = SemesterID And od.GradeId = GradeID And od.DisciplineId = DisciplineID).SingleOrDefault()
            If dsc Is Nothing Then
                dsc = New OfferedDiscipline() With {.YearId = YearID, .GradeId = GradeID, .SemesterId = SemesterID, .DisciplineId = DisciplineID}
                CType(OfferedDisciplinesViewSource.Source, ObservableEntityCollection(Of OfferedDiscipline)).Add(dsc)
            End If
        Next

        OfferedDisciplinesDataGrid.Items.Refresh()
    End Sub
End Class
