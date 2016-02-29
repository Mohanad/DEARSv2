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
        For Each x In ColumnMapping
            If firstRowData.Contains(x.ScreenColumnName) Then
                x.FileColumnName = x.ScreenColumnName
            End If
        Next

        ColumnMappingGroupBox.IsEnabled = True
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
        Dim firstRowNum As Integer = Integer.Parse(StartRowTextBox.Text)
        Dim lastRowNum As Integer = Integer.Parse(EndRowTextBox.Text)

        Dim lastColNum As Integer = CRange.RangeColumn(ws.erange) + CRange.RangeWidth(ws.erange) - 1
        Dim firstRowData As List(Of String) = ws.ExtractDataFromSubrange("A" & firstRowNum & ":" & _
                                                                         CColumn.GetColumnName(lastColNum) & firstRowNum)
        ExtractedData = New Dictionary(Of String, List(Of String))()

        For Each it In ColumnMapping
            Dim colNum = firstRowData.IndexOf(it.FileColumnName) + 1
            If colNum < 1 Then
                Throw New Exception("I am too old for this!! And it is late 1:15am")
            End If

            If WithHeaderRadioButton.IsChecked And Not NoHeaderRadioButton.IsChecked Then
                ExtractedData(it.ScreenColumnName) = ws.ExtractDataColumn(colNum, firstRowNum + 1, lastRowNum)
            ElseIf Not WithHeaderRadioButton.IsChecked And NoHeaderRadioButton.IsChecked Then
                ExtractedData(it.ScreenColumnName) = ws.ExtractDataColumn(colNum, firstRowNum, lastRowNum)
            End If
        Next

        Me.DialogResult = True
        'Me.Close()
    End Sub


    Private Sub CancelButton_Click(sender As Object, e As RoutedEventArgs)
        Me.DialogResult = False
        'Me.Close()
    End Sub
End Class
