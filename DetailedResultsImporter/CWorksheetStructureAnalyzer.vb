Imports System.Text.RegularExpressions
Imports Microsoft.Office.Interop
Public Class CWorksheetStructureAnalyzer
	Public Islands As New List(Of String)(1000)
	Dim _SheetName As String
	Public Sub New(ByRef ws As CWorksheet)
		_SheetName = ws.GetWorksheetName()
		Dim islandStart As String
		Dim islandEnd As String = Nothing
		islandStart = ws._ranges.ElementAt(0).Value.Reference
		If ws._ranges.ElementAt(0).Value.Reference.Contains(":") Then
			islandStart = ws._ranges.ElementAt(0).Value.Reference.Split(":")(0)
		Else
			islandStart = ws._ranges.ElementAt(0).Value.Reference
		End If
		Dim lim = ws._ranges.Count - 2
		For i As Integer = 0 To lim
			Dim rn = ws._ranges.ElementAt(i).Value
			Dim nrn = ws._ranges.ElementAt(i + 1).Value
			If Not CompareTwoRanges(rn, nrn) Then
				If rn.Reference.Contains(":") Then
					islandEnd = rn.Reference.Split(":")(1)
				Else
					islandEnd = rn.Reference
				End If
				Islands.Add(islandStart & ":" & islandEnd)
				If nrn.Reference.Contains(":") Then
					islandStart = nrn.Reference.Split(":")(0)
				Else
					islandStart = nrn.Reference
				End If
			ElseIf (i + 1) > lim Then
				If nrn.Reference.Contains(":") Then
					islandEnd = nrn.Reference.Split(":")(1)
				Else
					islandEnd = nrn.Reference
				End If
				Islands.Add(islandStart & ":" & islandEnd)
			End If
		Next

	End Sub

	Function CompareTwoRanges(ByVal r1 As CRange, ByVal r2 As CRange)
		'Weak comparator
		Return (r1.NRows = r2.NRows) And (r1.NCols = r2.NCols) And (r1.Row = r2.Row)
	End Function

End Class