Imports DetailedResultsImporter
Public Class ExcelImporterDialog
    Public wb As DetailedResultsImporter.CWorkbook
    Public RequiredDataColumns As List(Of String)
    Public ExtractedData As Dictionary(Of String, List(Of String))

    Private ColumnMappingViewSource As CollectionViewSource
    Private FileHeadersViewSource As CollectionViewSource

    Private ColumnMapping As List(Of ColumnMappingPair)

    Private ws As DetailedResultsImporter.CWorksheet

    Private Sub ComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim sheetName As String = e.AddedItems(0)
        ws = wb.GetWorksheet(sheetName)
        StartRowTextBox.Text = DetailedResultsImporter.CRange.RangeRow(ws.erange)
        EndRowTextBox.Text = DetailedResultsImporter.CRange.RangeRow(ws.erange) + DetailedResultsImporter.CRange.RangeHeight(ws.erange) - 1
    End Sub

    Private Sub RadioButton_Checked(sender As Object, e As RoutedEventArgs)
        ColumnMapping = New List(Of ColumnMappingPair)()
        For Each dcol In RequiredDataColumns
            ColumnMapping.Add(New ColumnMappingPair(dcol, Nothing))
        Next
        ColumnMappingViewSource = CType(Me.FindResource("ColumnMappingViewSource"), CollectionViewSource)
        ColumnMappingViewSource.Source = ColumnMapping
        FileHeadersViewSource = CType(Me.FindResource("FileHeadersViewSource"), CollectionViewSource)

        Dim firstRowNum As Integer = Integer.Parse(StartRowTextBox.Text)
        Dim lastColNum As Integer = CRange.RangeColumn(ws.erange) + CRange.RangeWidth(ws.erange) - 1
        Dim firstRowData As List(Of String) = ws.ExtractDataFromSubrange("A" & firstRowNum & ":" & _
                                                                         CColumn.GetColumnName(lastColNum) & firstRowNum)
        FileHeadersViewSource.Source = firstRowData

        If WithHeaderRadioButton.IsChecked And Not NoHeaderRadioButton.IsChecked Then

        ElseIf Not WithHeaderRadioButton.IsChecked And NoHeaderRadioButton.IsChecked Then

        End If

    End Sub

    Class ColumnMappingPair
        Sub New(ScreenColumnName, FileColumnName)
            Me.FileColumnName = FileColumnName
            Me.ScreenColumnName = ScreenColumnName
        End Sub
        Property ScreenColumnName As String
        Property FileColumnName As String
    End Class

    Private Sub ImportButton_Click(sender As Object, e As RoutedEventArgs)

    End Sub
End Class
