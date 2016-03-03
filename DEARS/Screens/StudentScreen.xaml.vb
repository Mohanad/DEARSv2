Public Class StudentScreen
    Implements IBaseScreen

    Private _db As AcademicResultsDBEntities
    Public ReadOnly Property DBContext As AcademicResultsDBEntities Implements IBaseScreen.DBContext
        Get
            Return SharedState.DBContext
        End Get
    End Property

    Private StudentsViewSource As CollectionViewSource

    Private Sub SearchButton_Click(sender As Object, e As RoutedEventArgs)

        Dim IsindexNo As Boolean
        Integer.TryParse(StudentSearchTextBox.Text, IsindexNo)

        If String.IsNullOrWhiteSpace(StudentSearchTextBox.Text) Then
            StudentsViewSource.Source = New ObservableEntityCollection(Of Student)(DBContext)
        ElseIf IsindexNo Then
            Dim IndexNo As Integer = Integer.Parse(StudentSearchTextBox.Text)
            Dim q_student = From st In DBContext.Students
                            Where st.Index = IndexNo
                            Select st
            StudentsViewSource.Source = New ObservableEntityCollection(Of Student)(DBContext, q_student)
        Else
            Dim q_str As String = StudentSearchTextBox.Text
            Dim q_students = From st In DBContext.Students _
                             Where st.NameArabic.Contains(q_str) Or st.NameEnglish.Contains(q_str) Or st.UnivNo.Contains(q_str)
                             Select st

            StudentsViewSource.Source = New ObservableEntityCollection(Of Student)(DBContext, q_students)
        End If
    End Sub

    Private Sub NewStudentButton_Click(sender As Object, e As RoutedEventArgs)
        If StudentsViewSource.Source Is Nothing Then
            StudentsViewSource.Source = New ObservableEntityCollection(Of Student)(DBContext)
        End If
        CType(StudentsViewSource.Source, ObservableEntityCollection(Of Student)).Add(New Student())
    End Sub

    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        StudentsViewSource = CType(Me.FindResource("StudentsViewSource"), CollectionViewSource)
        Dim NationalitiesText As String = System.IO.File.ReadAllText("Nationalities.txt")
        NationalityComboBox.ItemsSource = NationalitiesText.Split(",")
    End Sub

    Private Sub DeleteStudentButton_Click(sender As Object, e As RoutedEventArgs)
        If Not StudentsViewSource.Source Is Nothing Then
            CType(StudentsViewSource.Source, ObservableEntityCollection(Of Student)).Remove(StudentsDataGrid.SelectedItem)
        End If
    End Sub

    Public Sub LoadData(PropertyName As String) Implements IBaseScreen.LoadData

    End Sub

    Public Sub SaveDataColumnsToEntities(ExtractedData As Dictionary(Of String, List(Of String))) Implements IBaseScreen.SaveDataColumnsToEntities
        Dim indexList As List(Of String) = ExtractedData("Index")
        Dim studPrefecth = SharedState.DBContext.Students.Where(Function(s) indexList.Contains(s.Id)).ToList()


        Dim obsstud = New ObservableEntityCollection(Of Student)(DBContext)
        For i As Integer = 0 To indexList.Count - 1
            Dim index = indexList(i)
            Dim stud = (From st In SharedState.DBContext.Students.Local
                       Where st.Index = index).SingleOrDefault()
            If stud Is Nothing Then
                stud = New Student() With {.Index = ExtractedData("Index")(i)}
            End If
            With stud
                .NameArabic = ExtractedData("Name (Arabic)")(i)
                .NameEnglish = ExtractedData("Name (English)")(i)
                .UnivNo = ExtractedData("University No")(i)
            End With
            obsstud.Add(stud)
        Next
        StudentsViewSource.Source = obsstud

    End Sub

    Private Sub ImageBrowseButton_Click(sender As Object, e As RoutedEventArgs)
        Dim openFileDialog As New Microsoft.Win32.OpenFileDialog()
        openFileDialog.CheckFileExists = True
        openFileDialog.Filter = "BMP|*.bmp|GIF|*.gif|JPG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif;*.tiff|" _
       & "All Graphics Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff"

        If openFileDialog.ShowDialog() = True Then
            Dim src As BitmapImage = New BitmapImage()
            src.BeginInit()
            src.UriSource = New Uri(openFileDialog.FileName, UriKind.Absolute)
            src.EndInit()
            If src.PixelWidth = 600 Or src.PixelHeight = 600 Then
                StudentImage.Source = src
                Exit Sub
            End If
            MsgBox("Image Must be 600 x 600")
        End If


    End Sub
End Class
