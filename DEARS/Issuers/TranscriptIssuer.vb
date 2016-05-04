Imports DocumentFormat.OpenXml.Wordprocessing
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml
Imports System.ComponentModel
Public Class TranscriptIssuer
    Implements INotifyPropertyChanged

    Private _selectedStudent As Student
    Property SelectedStudent As Student
        Get
            Return _selectedStudent
        End Get
        Set(value As Student)
            _selectedStudent = value
            OnPropertyChanged("SelectedStudent")
        End Set
    End Property

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Sub IssueTranscript(OutputFolder As String, TemplatesFolder As String)
        Dim LastSemesterEnroll = (From d In SelectedStudent.SemesterBatchEnrollments
                   Order By d.GradeId Descending, d.YearId Descending, d.SemesterId Descending
                   Take 1).SingleOrDefault()

        If LastSemesterEnroll Is Nothing Then
            Throw New InvalidOperationException("Cannot extract a transcript for a student who was never enrolled")
        End If

        Dim TranscriptFilename As String = OutputFolder + "\" + SelectedStudent.Index.ToString() + ".docx"

        ' Get template file name
        Dim TemplateFilename As String = TemplatesFolder & "\" & LastSemesterEnroll.GradeId.ToString() & "\" & LastSemesterEnroll.Discipline.NameEnglishShort & ".docx"

        If Not IO.File.Exists(TemplateFilename) Then
            Throw New IO.FileNotFoundException("Template File not found exception" + TemplateFilename)
        End If

        If IO.File.Exists(TranscriptFilename) Then
            IO.File.Delete(TranscriptFilename)
        End If

        IO.File.Copy(TemplateFilename, TranscriptFilename)

        Dim ts As New TranscriptTemplate(TranscriptFilename)

        Dim tags = ts.GetTagNames()

        If Not String.IsNullOrWhiteSpace(SelectedStudent.NameEnglish) Then
            ts.SetCtrlValue("SNameE", SelectedStudent.NameEnglish)
            tags.Remove("SNameE")
        ElseIf Not String.IsNullOrWhiteSpace(SelectedStudent.NameArabic) Then
            ts.SetCtrlValue("SNameE", SelectedStudent.NameArabic)
        End If

        If Not String.IsNullOrWhiteSpace(SelectedStudent.UnivNo) Then
            ts.SetCtrlValue("SUNumber", SelectedStudent.UnivNo)
            tags.Remove("SUNumber")
        End If

        For Each courseTag In ts.GetTagNames().ToList()
            courseTag = courseTag.Trim(" ")
            Dim crs = (From cr In SharedState.DBContext.Courses Where cr.CourseCode = courseTag).SingleOrDefault()
            If crs IsNot Nothing Then

                Dim mrk = (From x In SelectedStudent.MarksExamCWs
                       Where x.CourseId = crs.Id).SingleOrDefault()
                If mrk IsNot Nothing Then
                    Dim gr = ResultsProcessingUtilities.AssignGrade(mrk)
                    ts.SetCtrlValue(courseTag, gr.Grade)
                    tags.Remove(courseTag)
                End If
            End If
        Next

        If LastSemesterEnroll.BatchEnrollment.GPAwRecomm IsNot Nothing AndAlso LastSemesterEnroll.BatchEnrollment.GPAwRecomm.CGPA IsNot Nothing Then
            ts.SetCtrlValue("CGPA", System.Math.Round(LastSemesterEnroll.BatchEnrollment.GPAwRecomm.CGPA.Value, 2))
            If LastSemesterEnroll.BatchEnrollment.GPAwRecomm.CumulativeRecommendationType IsNot Nothing Then
                ts.SetCtrlValue("Grade", LastSemesterEnroll.BatchEnrollment.GPAwRecomm.CumulativeRecommendationType.NameEnglish)
            End If
        End If
        tags.Remove("CGPA")
        tags.Remove("Grade")

        For Each gpatag In tags.Where(Function(s) s.StartsWith("GPA"))
            Dim gr = Integer.Parse(gpatag.Substring(3))
            Dim lastRes = (From enr In SelectedStudent.BatchEnrollments
                           Where enr.GPAwRecomm IsNot Nothing And enr.GPAwRecomm.GradeId = gr
                           Order By enr.GPAwRecomm.YearId Descending Take 1).SingleOrDefault()
            If lastRes IsNot Nothing Then
                If lastRes.GPAwRecomm.GPA IsNot Nothing Then
                    ts.SetCtrlValue(gpatag, System.Math.Round(lastRes.GPAwRecomm.GPA.Value, 2))
                End If
                ts.SetCtrlValue("Class" & gr, lastRes.GPAwRecomm.YearRecommendationType.NameEnglish)
            End If
            Dim firstYr = (From enr In SelectedStudent.BatchEnrollments
                           Where enr.GradeId = gr Order By enr.YearId Ascending Take 1).SingleOrDefault()
            If firstYr IsNot Nothing Then
                ts.SetCtrlValue(firstYr.Grade.NameEnglish & "TY" & 1, firstYr.YearId)
            End If
            If lastRes IsNot Nothing Then
                ts.SetCtrlValue(lastRes.Grade.NameEnglish & "TY" & 2, lastRes.YearId + 1)
            End If
        Next

        ts.SetCtrlValue("IssueData", System.DateTime.Today)

        ts.Close()

        Process.Start("winword", """" + TranscriptFilename + """")
    End Sub

    Function MakeGrade(fin As Double) As String
        If fin > 90 Then
            Return "A+"
        ElseIf fin > 80 Then
            Return "A"
        ElseIf fin > 70 Then
            Return "A-"
        ElseIf fin > 60 Then
            Return "B+"
        ElseIf fin > 50 Then
            Return "B"
        ElseIf fin > 40 Then
            Return "C"
        ElseIf fin > 30 Then
            Return "D"
        Else
            Return "F"
        End If
    End Function

    Private Class TranscriptTemplate
        Private wDoc As WordprocessingDocument
        Private taggedSDTs As Dictionary(Of String, SdtElement)
        Sub New(ByVal FileName As String)
            wDoc = WordprocessingDocument.Open(FileName, True)
            taggedSDTs = wDoc.MainDocumentPart.Document.Body.Descendants(Of SdtElement).Where(Function(s) s.Descendants(Of Tag).Count() > 0).ToDictionary(Of String)(Function(s) s.Descendants(Of Tag).Single().Val)
            Beep()
        End Sub
        Function GetRun(ByVal Tagtext As String) As Run
            Dim cont = taggedSDTs(Tagtext).Descendants(Of Run)().Single()
            Return cont
        End Function
        Sub SetCtrlValue(ByVal TagText As String, ByVal Value As String)
            If taggedSDTs.ContainsKey(TagText) Then
                taggedSDTs(TagText).Descendants(Of Run)().Single().Elements(Of Text).Single.Text = Value
            End If
        End Sub
        Function GetTagNames() As List(Of String)
            Return taggedSDTs.Keys.ToList()
        End Function
        Sub Close()
            wDoc.MainDocumentPart.Document.Save()
            wDoc.Close()
        End Sub
    End Class


End Class