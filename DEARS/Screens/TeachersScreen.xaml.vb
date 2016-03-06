Public Class TeachersScreen
    Implements IBaseScreen

    Private _db As AcademicResultsDBEntities
    Public ReadOnly Property DBContext As AcademicResultsDBEntities Implements IBaseScreen.DBContext
        Get
            Return SharedState.DBContext
        End Get
    End Property


    Private TeachersViewSource As CollectionViewSource

    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        LoadData("")
    End Sub

    Public Sub LoadData(PropertyName As String) Implements IBaseScreen.LoadData
        TeachersViewSource = CType(Me.FindResource("TeachersViewSource"), CollectionViewSource)

        Dim q_teachers = From tr In DBContext.Teachers
                     Select tr

        TeachersViewSource.Source = New ObservableEntityCollection(Of Teacher)(DBContext, q_teachers)
    End Sub

    Public Sub SaveDataColumnsToEntities(ExtractedData As Dictionary(Of String, List(Of String))) Implements IBaseScreen.SaveDataColumnsToEntities
        Dim teachersList As List(Of String) = ExtractedData("Name (Arabic)")
        For i As Integer = 0 To teachersList.Count - 1
            Dim tch = (From tr In SharedState.DBContext.Teachers.Local
                       Where tr.NameArabic = teachersList(i)).SingleOrDefault()
            If tch IsNot Nothing Then
                tch.NameEnglish = ExtractedData("Name (English)")(i)
            Else
                tch = New Teacher() With {.NameArabic = teachersList(i), _
                                         .NameEnglish = ExtractedData("Name (English)")(i)}
                CType(TeachersViewSource.Source, ObservableEntityCollection(Of Teacher)).Add(tch)
            End If
        Next
    End Sub
End Class
