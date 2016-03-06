Public Class DisciplinesScreen
    Implements IBaseScreen

    Public ReadOnly Property DBContext As AcademicResultsDBEntities Implements IBaseScreen.DBContext
        Get
            Return SharedState.DBContext
        End Get
    End Property

    Private DisciplinesViewSource As CollectionViewSource

    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        LoadData("")
    End Sub

    Public Sub LoadData(PropertyName As String) Implements IBaseScreen.LoadData
        DisciplinesViewSource = CType(Me.FindResource("DisciplinesViewSource"), CollectionViewSource)

        Dim grades = (From gr In DBContext.Disciplines
                     Select gr)

        DisciplinesViewSource.Source = New ObservableEntityCollection(Of Discipline)(DBContext, grades)
    End Sub

    Public Sub SaveDataColumnsToEntities(ExtractedData As Dictionary(Of String, List(Of String))) Implements IBaseScreen.SaveDataColumnsToEntities
        Dim disciplinesList As List(Of String) = ExtractedData("Short Name (English)")
        For i As Integer = 0 To disciplinesList.Count - 1
            Dim dsc = (From ds In SharedState.DBContext.Disciplines.Local
                       Where ds.NameEnglishShort = disciplinesList(i)).SingleOrDefault()
            If dsc IsNot Nothing Then
                dsc.NameEnglish = ExtractedData("Name (English)")(i)
                dsc.NameArabic = ExtractedData("Name (Arabic)")(i)
                dsc.NameArabicShort = ExtractedData("Short Name (Arabic)")(i)
            Else
                dsc = New Discipline() With {.NameEnglishShort = disciplinesList(i), _
                                         .NameEnglish = ExtractedData("Name (English)")(i), _
                                         .NameArabic = ExtractedData("Name (Arabic)")(i), _
                                         .NameArabicShort = ExtractedData("Short Name (Arabic)")(i)}

                CType(DisciplinesViewSource.Source, ObservableEntityCollection(Of Discipline)).Add(dsc)
            End If
        Next
    End Sub
End Class
