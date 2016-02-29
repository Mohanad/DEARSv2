Public Class CoursesScreen
    Implements IBaseScreen

    Public ReadOnly Property DBContext As AcademicResultsDBEntities Implements IBaseScreen.DBContext
        Get
            Return SharedState.DBContext
        End Get
    End Property

    Private CoursesViewSource As CollectionViewSource

    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        LoadData("")
    End Sub

    Public Sub LoadData(PropertyName As String) Implements IBaseScreen.LoadData
        CoursesViewSource = CType(Me.FindResource("CoursesViewSource"), CollectionViewSource)

        Dim q_courses = (From gr In DBContext.Courses
                     Select gr)

        CoursesViewSource.Source = New ObservableEntityCollection(Of Course)(DBContext, q_courses)
    End Sub

    Public Sub SaveDataColumnsToEntities(ExtractedData As Dictionary(Of String, List(Of String))) Implements IBaseScreen.SaveDataColumnsToEntities

        Dim courseCodeList As List(Of String) = ExtractedData("Course Code")
        For i As Integer = 0 To courseCodeList.Count - 1
            Dim crs = (From cr In SharedState.DBContext.Courses.Local
                       Where cr.CourseCode = courseCodeList(i)).SingleOrDefault()
            If crs IsNot Nothing Then
                crs.TitleEnglish = ExtractedData("Course Title (English)")(i)
                crs.TitleArabic = ExtractedData("Course Title (Arabic)")(i)
            Else
                crs = New Course() With {.CourseCode = courseCodeList(i), _
                                         .TitleEnglish = ExtractedData("Course Title (English)")(i), _
                                         .TitleArabic = ExtractedData("Course Title (Arabic)")(i)}
                CType(CoursesViewSource.Source, ObservableEntityCollection(Of Course)).Add(crs)
            End If
        Next
    End Sub
End Class
