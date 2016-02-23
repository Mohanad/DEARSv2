Public Class ExcelImporterDialog
    Public wb As DetailedResultsImporter.CWorkbook
    Private Sub ComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim sheetName As String = e.AddedItems(0)
        Dim ws As DetailedResultsImporter.CWorksheet = wb.GetWorksheet(sheetName)

        For Each rn In ws._ranges
            MsgBox(rn.Value)
        Next
    End Sub
End Class
