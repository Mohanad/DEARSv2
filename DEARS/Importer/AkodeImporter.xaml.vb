Imports System.Text.RegularExpressions
Public Class AkodeImporter

    Private Sub FolderLocationBrowseButton_Click(sender As Object, e As RoutedEventArgs)
        Dim selectFolderDialog As New Forms.FolderBrowserDialog()
        If selectFolderDialog.ShowDialog() = Forms.DialogResult.OK Then
            FolderLocationTextBox.Text = selectFolderDialog.SelectedPath
        End If
    End Sub

    Private Sub BoardResultButton_Click(sender As Object, e As RoutedEventArgs)
        Dim openFileDialog As New Forms.OpenFileDialog()
        openFileDialog.Filter = "Excel OpenXML Document (*.xlsx) |*.xlsx"
        If openFileDialog.ShowDialog() = Forms.DialogResult.OK Then
            BoardResultTextBox.Text = openFileDialog.FileName
        End If
    End Sub
    Dim failMsg = vbCrLf & "Processing Failed!"
    Private Sub ProcessButton_Click(sender As Object, e As RoutedEventArgs)
        'We perform some checking to confirm this is an Akode folder.
        ' Check the folder exists
        If Not System.IO.Directory.Exists(FolderLocationTextBox.Text) Then
            AkodeImporterLog.AppendText("RUN Folder not found" & failMsg)
            Exit Sub
        End If

        Dim bBaseExists As Boolean = False
        Dim bNameExists As Boolean = False
        Dim baseFileName = FolderLocationTextBox.Text + "\BASE" & SharedState.GetSingleInstance.GradeID
        ' First we check for the base file. There should be a file named BASEn where n is the Grade we are in now.
        bBaseExists = IO.File.Exists(baseFileName)
        ' Check for existence of Name file
        bNameExists = IO.File.Exists(FolderLocationTextBox.Text + "\NAME" & SharedState.GetSingleInstance.GradeID)

        If Not bBaseExists Then
            AkodeImporterLog.AppendText("Base file not found in folder. Should be named ---> " & "BASE" _
                                        & SharedState.GetSingleInstance.GradeID.ToString & vbCrLf)
            AkodeImporterLog.AppendText(failMsg)
            Exit Sub
        End If

        'Parse base file for courses Data
        Dim baseData = ParseAkodeBaseFile(baseFileName)
        

        ' Check the directory for data files
        For Each crCode In baseData.CourseCodes
            Dim crDataFile As String = FolderLocationTextBox.Text + "\" + crCode
            If Not IO.File.Exists(crDataFile) Then
                AkodeImporterLog.AppendText("FILE NOT FOUND:" & crDataFile & failMsg)
                Exit Sub
            Else
                AkodeImporterLog.AppendText("FILE FOUND:" & crDataFile & vbCr)
            End If
        Next

        ' Find the names file and create a list of student index numbers
        Dim namesDataFile = FolderLocationTextBox.Text & "\NAME" & SharedState.GetSingleInstance.GradeID
        Dim bNamesExists = IO.File.Exists(namesDataFile)
        If Not bNameExists Then
            AkodeImporterLog.AppendText("NAMES FILE NOT FOUND:" & namesDataFile & failMsg)
            Exit Sub
        End If

        'Parse file for student indices.
        Dim nmData = ParseAkodeNamesFile(namesDataFile, SharedState.GetSingleInstance.GradeID)
        If nmData Is Nothing Then
            AkodeImporterLog.AppendText("NAMES File: Could not parse NAMES File" & failMsg)
            Exit Sub
        End If
        AkodeImporterLog.AppendText("NAMES FILE DONE!" & vbCr)

        ' Now we parse the data files for marks. The result of each file is a triplet of lists. Each row in the triplets 
        ' represents the mark of the student in one subject. According to Akode's program user Absent is represented by a -1.
        ' Each data file must contain the same number of marks as the names file.

        Dim ExtractedMarksList As New List(Of AkodeMarksFileData)
        For Each crCode In baseData.CourseCodes
            Dim crDataFile As String = FolderLocationTextBox.Text + "\" + crCode
            AkodeImporterLog.AppendText("MARKS File: " & crCode)
            Dim crData = ParseAkodeMarksFile(crDataFile, crCode)
            If crData Is Nothing Then
                AkodeImporterLog.AppendText(vbCr & vbTab & "MARKS File: " & crCode & " PARSING FAILURE" & failMsg)
                Exit Sub
            End If
            AkodeImporterLog.AppendText(" DONE!" & vbCr)
            ExtractedMarksList.Add(crData)
        Next
        AkodeImporterLog.AppendText("<========= PARSING SUCCESS! =========> " & vbCr)

        ' By the time we reach here we have parsed all the data files we have that are named in the BASEn file. 
        ' Now we need to feed these to the database. The only missing peice of information now is which course goes into which
        ' semester.Althugh parsing the course code can show us which goes into which for most courses, there are always exceptions.
        ' Maybe we can parse and then ask user to flip courses left/rigth depending on the semester.

        AkodeFeedToDatabase(nmData, ExtractedMarksList, baseData)

    End Sub
    Class AkodeBaseFileData
        Public CourseCodes As New List(Of String)
        Public CreditHours As New List(Of Integer)
        Public ExamFractions As New List(Of Integer)
        Public CWFractions As New List(Of Integer)
    End Class

    Class AkodeMarksFileData
        Public CourseCode As String
        Public IndexList As List(Of Integer)
        Public CWMarkList As List(Of Double)
        Public ExamMarkList As List(Of Double)
        Public PresentList As List(Of Boolean)
    End Class

    Class AkodeNamesFileData
        Public IndexListCount As Integer
        Public IndexList As List(Of Integer)
        Public ListOfGPALists As List(Of List(Of Double))
        Public RegStatusPrevRec As List(Of String)
    End Class

    Function ParseAkodeBaseFile(baseFileName As String) As AkodeBaseFileData
        ParseAkodeBaseFile = Nothing

        Dim CourseCodes As New List(Of String)
        Dim CreditHours As New List(Of Integer)
        Dim ExamFractions As New List(Of Integer)
        Dim CWFractions As New List(Of Integer)

        Using baseFileReader As IO.TextReader = IO.File.OpenText(baseFileName)
            'Skip the first line which are descriptive strings.
            Dim line As String = baseFileReader.ReadLine()
            line = baseFileReader.ReadLine()
            Dim sepChars() As Char = {vbTab, " "}
            Dim sepStr() As String = {vbTab, "    ", "   "}

            'Parse base file for course codes.
            Do Until String.IsNullOrWhiteSpace(line)
                Dim CreditHour As Integer
                Dim ExamFraction As Integer
                Dim CWFraction As Integer

                Dim splitStr = line.Split(sepChars, StringSplitOptions.RemoveEmptyEntries)
                If splitStr.Length() < 8 Then
                    Exit Do
                End If

                Dim crCode = splitStr(0)
                Dim crCodeCheck = splitStr(1)
                Dim crHrs = splitStr(2)
                Dim exfr = splitStr(3)
                Dim cwfr = splitStr(4)

                crCode = crCode.Trim("'")
                crCodeCheck = crCodeCheck.Trim("'")

                If Not (crCode = crCodeCheck) Then
                    AkodeImporterLog.AppendText("Course Code Check Failed" & crCode & "!=" & crCodeCheck & vbCrLf & failMsg)
                    Exit Function
                End If
                CourseCodes.Add(crCode)

                If Not Integer.TryParse(crHrs, CreditHour) Then
                    AkodeImporterLog.AppendText("Could not convert Credit Hours entry to integer for " & crCode & vbCrLf & failMsg)
                    Exit Function
                End If
                CreditHours.Add(CreditHour)

                If Not Integer.TryParse(exfr, ExamFraction) Then
                    AkodeImporterLog.AppendText("Could not convert Exam Fraction entry to integer for " & crCode & vbCrLf & failMsg)
                    Exit Function
                End If
                ExamFractions.Add(ExamFraction)

                If Not Integer.TryParse(cwfr, CWFraction) Then
                    AkodeImporterLog.AppendText("Could not convert CW Fraction entry to integer for " & crCode & vbCrLf & failMsg)
                    Exit Function
                End If
                CWFractions.Add(CWFraction)

                AkodeImporterLog.AppendText("Parsed Course Code = " & crCode & vbTab & "CrH/EX/CW = " & CreditHour & _
                                            "/" & ExamFraction & "/" & CWFraction & vbCr)

                line = baseFileReader.ReadLine()
            Loop
        End Using

        ParseAkodeBaseFile = New AkodeBaseFileData()
        With ParseAkodeBaseFile
            .CourseCodes = CourseCodes
            .CreditHours = CreditHours
            .CWFractions = CWFractions
            .ExamFractions = ExamFractions
        End With
    End Function

    Function ParseAkodeNamesFile(filePath As String, GradeID As Integer) As AkodeNamesFileData
        ParseAkodeNamesFile = Nothing
        Dim IndexListCount As Integer = 0
        Dim IndexList As New List(Of Integer)
        Dim ListOfGPALists As New List(Of List(Of Double))
        Dim RegStatusPrevRec As New List(Of String)

        For i As Integer = 1 To GradeID - 1
            ListOfGPALists.Add(New List(Of Double)())
        Next

        ' One approach to parse this file would be to parse using similar rules to scanf in C Language. 
        ' The reading stops on White spaces. The last column however is not delimited by white space but rather by single quote
        Using namesFileReader As IO.TextReader = New IO.StreamReader(filePath)
            Dim lineStr = namesFileReader.ReadLine()
            If Not Integer.TryParse(lineStr, IndexListCount) Then
                AkodeImporterLog.AppendText(vbTab & "NAMES File: " & "Count Line not number")
                Exit Function
            End If

            Do
                lineStr = namesFileReader.ReadLine().Trim()

                Dim srNo As Integer = 0
                Dim index As Integer = 0
                Dim regStatPrevRec As String = Nothing

                'Skip Empty Lines
                If String.IsNullOrWhiteSpace(lineStr) Then
                    Continue Do
                End If

                ' Since we are here we know this line is not empty. Start parsing. We keep track of our current position by removing things 
                ' we parsed The data items are Serial No (1) + Index (1) + GPAs (GradeID - 1) + RegStatusPrevRec (1).
                Dim nxtSpace As Integer = 0

                'Extract serial no
                nxtSpace = lineStr.IndexOfAny({" ", vbTab})
                If Not Integer.TryParse(lineStr.Substring(0, nxtSpace), srNo) Then
                    AkodeImporterLog.AppendText(vbTab & "NAMES File: " & "NO Serial no in Line starting with " & lineStr.Substring(0, nxtSpace))
                    Exit Function
                End If

                ' Extract index no
                lineStr = lineStr.Substring(nxtSpace, lineStr.Length - nxtSpace).TrimStart()
                nxtSpace = lineStr.IndexOfAny({" ", vbTab})
                If Not Integer.TryParse(lineStr.Substring(0, nxtSpace), index) Then
                    AkodeImporterLog.AppendText(vbTab & "NAMES File: " & "NO Index in Line starting with " & srNo)
                    Exit Function
                End If

                ' Extract reg status/prev rec
                lineStr = lineStr.Substring(nxtSpace, lineStr.Length - nxtSpace).TrimStart()
                Dim firstQuote As Integer = lineStr.IndexOf("'")
                Dim lastQuote As Integer = lineStr.LastIndexOf("'")
                If lastQuote > firstQuote Then
                    regStatPrevRec = lineStr.Substring(firstQuote, lastQuote - firstQuote + 1)
                    lineStr = lineStr.Remove(firstQuote)
                End If

                ' Remaining should be just gpas. Split at white spaces ignoring empty entries. Check count to GradeID - 1
                Dim sep() As Char = {vbTab, " "}
                Dim splitStr() As String = lineStr.Split(sep, StringSplitOptions.RemoveEmptyEntries)
                If splitStr.Count <> GradeID - 1 Then
                    AkodeImporterLog.AppendText(vbTab & "NAMES File: " & "Count of GPA entries in Line " & srNo & " NOT Correct")
                    Exit Function
                End If

                For i As Integer = 0 To GradeID - 2
                    Dim gpa As Double = Double.NaN
                    If Not Double.TryParse(splitStr(i), gpa) Then
                        AkodeImporterLog.AppendText(vbTab & "NAMES File: " & "Failure in Parsing GPA " & splitStr(i) & " at line " & srNo)
                    End If
                    ListOfGPALists(i).Add(gpa)
                Next
                IndexList.Add(index)
                RegStatusPrevRec.Add(regStatPrevRec)
            Loop Until CType(namesFileReader, IO.StreamReader).EndOfStream
        End Using

        If IndexList.Count <> IndexListCount Then
            AkodeImporterLog.AppendText("NAMES File: Count of Indices not same as number at start of file")
            Exit Function
        End If

        If RegStatusPrevRec.Count <> IndexListCount Then
            AkodeImporterLog.AppendText("NAMES File: Count of RegStatus/PrevRec not same as number at start of file")
            Exit Function
        End If

        For i As Integer = 0 To GradeID - 2
            If ListOfGPALists(i).Count <> IndexListCount Then
                AkodeImporterLog.AppendText("NAMES File: Count of GPA " & (i + 1) & "not same as number at start of file")
                Exit Function
            End If
        Next

        ParseAkodeNamesFile = New AkodeNamesFileData
        ParseAkodeNamesFile.IndexListCount = IndexListCount
        ParseAkodeNamesFile.IndexList = IndexList
        ParseAkodeNamesFile.RegStatusPrevRec = RegStatusPrevRec
        ParseAkodeNamesFile.ListOfGPALists = ListOfGPALists
    End Function

    Function ParseAkodeMarksFile(filepath As String, crCode As String) As AkodeMarksFileData
        ParseAkodeMarksFile = Nothing

        Dim count As Integer = 0
        Dim IndexList As New List(Of Integer)
        Dim CWMarkList As New List(Of Double)
        Dim ExamMarkList As New List(Of Double)
        Dim PresentList As New List(Of Boolean)

        Using marksFileReader As IO.TextReader = New IO.StreamReader(filepath)
            Dim lineStr As String = marksFileReader.ReadLine()
            If Not Integer.TryParse(lineStr, count) Then
                AkodeImporterLog.AppendText(vbTab & "MARKS File: " & crCode & "Count Line not number")
            End If

            Dim sep() As Char = {vbTab, " "}
            Do
                lineStr = marksFileReader.ReadLine().Trim()
                'Skip Empty Lines
                If String.IsNullOrWhiteSpace(lineStr) Then
                    Continue Do
                End If

                Dim splitStr = lineStr.Split(sep, StringSplitOptions.RemoveEmptyEntries)
                If splitStr.Length() <> 4 Then
                    AkodeImporterLog.AppendText(vbTab & "MARKS File: " & "Line Starting with " & splitStr(0) & " Contains more than 4 items ")
                    Exit Function
                End If

                Dim Index As Integer = 0
                Dim ExMark As Integer = 0
                Dim CWMark As Integer = 0

                If Not Integer.TryParse(splitStr(1), Index) Then
                    AkodeImporterLog.AppendText(vbTab & "MARKS File: " & splitStr(1) & " Is not an Index")
                    Exit Function
                End If

                If Not Integer.TryParse(splitStr(2), ExMark) Then
                    AkodeImporterLog.AppendText(vbTab & "MARKS File: " & splitStr(2) & " Is not an exam mark")
                    Exit Function
                End If

                If Not Integer.TryParse(splitStr(3), CWMark) Then
                    AkodeImporterLog.AppendText(vbTab & "MARKS File: " & splitStr(3) & " Is not an cw mark")
                    Exit Function
                End If

                IndexList.Add(Index)
                ExamMarkList.Add(ExMark)
                CWMarkList.Add(CWMark)
                PresentList.Add(ExMark >= 0)
            Loop Until CType(marksFileReader, IO.StreamReader).EndOfStream
        End Using

        If (count <> IndexList.Count) Or (count <> ExamMarkList.Count) Or (CWMarkList.Count <> count) Then
            AkodeImporterLog.AppendText("MARKS File: " & crCode & "Unequal length of lists" & vbCrLf & failMsg)
            Exit Function
        End If

        ParseAkodeMarksFile = New AkodeMarksFileData()
        ParseAkodeMarksFile.CourseCode = crCode
        ParseAkodeMarksFile.IndexList = IndexList
        ParseAkodeMarksFile.PresentList = PresentList
        ParseAkodeMarksFile.ExamMarkList = ExamMarkList
        ParseAkodeMarksFile.CWMarkList = CWMarkList

    End Function

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)

        'TODO: Remove this code once importer is in satisfactory shape.
        Dim sqlConnDialog As New SQLConnectWindow()
        If sqlConnDialog.ShowDialog() = True Then
            If SharedState.DBContext Is Nothing Then
                Me.Close()
            End If
        Else
            Me.Close()
            Exit Sub
        End If

        SharedState.DBContext.TimeYears.ToList()
        SharedState.DBContext.Semesters.ToList()
        SharedState.DBContext.Grades.ToList()
        SharedState.DBContext.Semesters.ToList()

        'SharedState.GetSingleInstance.YearID = SharedState.DBContext.TimeYears.Local.Last.Id
        'SharedState.GetSingleInstance.SemesterID = 1
        'SharedState.GetSingleInstance.GradeID = 4
        'SharedState.GetSingleInstance.DisciplineID = 1
        '''''''''''''''''''''''''

        Dim TimeYearsViewSource = DirectCast(Me.FindResource("TimeYearsViewSource"), CollectionViewSource)
        Dim GradesViewSource = DirectCast(Me.FindResource("GradesViewSource"), CollectionViewSource)
        DataLocationGroupBox.DataContext = SharedState.GetSingleInstance()
        TimeYearsViewSource.Source = SharedState.DBContext.TimeYears.Include("Batches").ToList()
    End Sub

    Private Sub AkodeFeedToDatabase(nmData As AkodeNamesFileData,
                                    ExtractedMarksList As List(Of AkodeMarksFileData),
                                    baseData As AkodeBaseFileData)

        Using db As New AcademicResultsDBEntities(SharedState.DBContext.Database.Connection, False)

            Dim YearID As Integer = SharedState.GetSingleInstance.YearID
            Dim SemesterID As Integer = SharedState.GetSingleInstance.SemesterID
            Dim GradeID As Integer = SharedState.GetSingleInstance.GradeID

            ' Check if prelim data is there if not create it. Note that an akode importer is always a non sub supp exam importer.
            Dim ty = (From y In db.TimeYears
                        Where y.Id = YearID Select y).SingleOrDefault()
            If ty Is Nothing Then
                ty = New TimeYear() With {.Id = YearID, .NameArabic = YearID.ToString + "/" + (YearID + 1).ToString(),
                                         .NameEnglish = YearID.ToString + "/" + (YearID + 1).ToString()}
                db.TimeYears.Add(ty)
            End If

            Dim bt = (From b In db.Batches
                         Where b.YearId = YearID And b.GradeId = GradeID).SingleOrDefault()
            If bt Is Nothing Then
                bt = New Batch() With {.YearId = YearID, .GradeId = GradeID}
                db.Batches.Add(bt)
            End If

            Dim sbt1 = (From sb In db.SemesterBatches
                       Where sb.YearId = YearID And sb.GradeId = GradeID And sb.SemesterId = 1).SingleOrDefault()
            If sbt1 Is Nothing Then
                sbt1 = New SemesterBatch() With {.YearId = YearID, .GradeId = GradeID, .SemesterId = 1}
                db.SemesterBatches.Add(sbt1)
            End If

            Dim sbt2 = (From sb In db.SemesterBatches
                       Where sb.YearId = YearID And sb.GradeId = GradeID And sb.SemesterId = 2).SingleOrDefault()
            If sbt2 Is Nothing Then
                sbt2 = New SemesterBatch() With {.YearId = YearID, .GradeId = GradeID, .SemesterId = 2}
                db.SemesterBatches.Add(sbt2)
            End If
            db.SaveChanges()

            db.Configuration.AutoDetectChangesEnabled = False
            db.Configuration.ValidateOnSaveEnabled = False

            ' No disciplines in Akode program.
            ' There should be only one discipline
            Dim discipline = db.Disciplines.Single()
            Dim disc1 = discipline.Id
            Dim disc2 = discipline.Id
            Dim dsc1 = (From d In db.OfferedDisciplines
                               Where d.YearId = YearID And d.GradeId = GradeID And d.SemesterId = 1 And d.DisciplineId = disc1).SingleOrDefault()
            If dsc1 Is Nothing Then
                dsc1 = New OfferedDiscipline() With {.YearId = YearID, .GradeId = GradeID, .SemesterId = 1, .DisciplineId = disc1}
                db.OfferedDisciplines.Add(dsc1)
            End If
            Dim dsc2 = (From d In db.OfferedDisciplines
                      Where d.YearId = YearID And d.GradeId = GradeID And d.SemesterId = 2 And d.DisciplineId = disc2).SingleOrDefault()
            If dsc2 Is Nothing Then
                dsc2 = New OfferedDiscipline() With {.YearId = YearID, .GradeId = GradeID, .SemesterId = 2, .DisciplineId = disc2}
                db.OfferedDisciplines.Add(dsc2)
            End If
            db.SaveChanges()


            
            ' Add Students and the Enrollment Data for Batch and Semester Batch
            db.Students.ToList()
            db.BatchEnrollments.Where(Function(s) s.YearId = YearID And s.GradeId = GradeID).ToList()
            db.SemesterBatchEnrollments.Where(Function(s) s.YearId = YearID And s.GradeId = GradeID).ToList()
            For Each stud In nmData.IndexList
                Dim ind = stud
                Dim st = (From s In db.Students.Local Where s.Index = ind).SingleOrDefault()
                If st Is Nothing Then
                    st = New Student() With {.Index = stud}
                    db.Students.Add(st)
                End If
                
            Next

            db.ChangeTracker.DetectChanges()
            db.SaveChanges()

            For Each stud In nmData.IndexList
                Dim ind = stud
                Dim st = (From s In db.Students.Local Where s.Index = ind).SingleOrDefault()
                Dim sreg = (From s In db.BatchEnrollments.Local Where s.StudentId = st.Id).SingleOrDefault()
                If sreg Is Nothing Then
                    sreg = New BatchEnrollment() With {.Student = st, .GradeId = GradeID, .YearId = YearID, .EnrollmentTypeId = 1}
                    db.BatchEnrollments.Add(sreg)
                End If
                Dim bsreg1 = (From bs In db.SemesterBatchEnrollments.Local
                              Where bs.StudentId = st.Id And bs.SemesterId = 1 And bs.YearId = YearID And bs.GradeId = GradeID).SingleOrDefault()
                Dim bsreg2 = (From bs In db.SemesterBatchEnrollments.Local
                              Where bs.StudentId = st.Id And bs.SemesterId = 2 And bs.YearId = YearID And bs.GradeId = GradeID).SingleOrDefault()
                If bsreg1 Is Nothing Then
                    bsreg1 = New SemesterBatchEnrollment() _
                        With {.StudentId = st.Id, .GradeId = GradeID, .YearId = YearID, .SemesterId = 1, .DisciplineId = disc1}
                    db.SemesterBatchEnrollments.Add(bsreg1)
                End If
                If bsreg2 Is Nothing Then
                    bsreg2 = New SemesterBatchEnrollment() _
                        With {.StudentId = st.Id, .GradeId = GradeID, .YearId = YearID, .SemesterId = 2, .DisciplineId = disc2}
                    db.SemesterBatchEnrollments.Add(bsreg2)
                End If
            Next

            db.ChangeTracker.DetectChanges()
            db.SaveChanges()

            ' Courses
            For courseIndex As Integer = 0 To baseData.CourseCodes.Count - 1
                'TODO: Find a way to distribute courses into second and first semester.
                Dim SemID = 2
                'If ((crs.Semester Mod 2) = 0) Then
                '    SemID = 2
                'Else
                '    SemID = 1
                'End If
                Dim ci = courseIndex
                Dim crcode = baseData.CourseCodes(ci)
                Dim cr = (From xcr In db.Courses
                           Where xcr.CourseCode = crcode).SingleOrDefault()
                If cr Is Nothing Then
                    Throw New Exception("Course Not already in Database this should not happen")
                End If

                Dim cof = (From xcrs In db.OfferedCourses
                           Where xcrs.YearId = YearID And xcrs.GradeId = GradeID And xcrs.CourseId = cr.Id).SingleOrDefault()
                If cof Is Nothing Then
                    cof = New OfferedCourse() With {.YearId = YearID, .GradeId = GradeID, .SemesterId = SemID, .CourseId = cr.Id}
                    db.OfferedCourses.Add(cof)
                End If
                With cof
                    .CreditHours = baseData.CreditHours(courseIndex)
                    .ExamFraction = baseData.ExamFractions(courseIndex)
                    .CourseWorkFraction = baseData.CWFractions(courseIndex)
                End With


                Dim disc As Integer = 0
                If SemID = 1 Then
                    disc = disc1
                Else
                    disc = disc2
                End If
                Dim cds = (From cd In db.CourseDisciplines
                       Where cd.YearId = YearID And cd.GradeId = GradeID And cd.SemesterId = SemID And cd.CourseId = cr.Id And disc = cd.DisciplineId).SingleOrDefault()
                If cds Is Nothing Then
                    cds = New CourseDiscipline() With {.YearId = YearID, .GradeId = GradeID, .SemesterId = SemID, .CourseId = cr.Id, .DisciplineId = disc}
                    db.CourseDisciplines.Add(cds)
                End If
                With cds
                    .Optional = False
                End With

            Next
            db.SaveChanges()

            ' Marks
            db.CourseEnrollments.Include("MarksExamCW").Where(Function(s) s.YearId = YearID And s.GradeId = GradeID).ToList()
            For i As Integer = 0 To nmData.IndexListCount - 1
                ' Get student
                Dim stind = nmData.IndexList(i)
                Dim st = (From s In db.Students.Local Where s.Index = stind).Single()

                ' Form course enrollments and marksexamcw for each course
                For j As Integer = 0 To baseData.CourseCodes.Count - 1
                    Dim courseCode As String = baseData.CourseCodes(j).Trim()
                    Dim courseID As Integer = (From cr In db.Courses.Local Where
                                      cr.CourseCode.Trim() = courseCode Select cr.Id).Single()
                    Dim cenr = (From cen In db.CourseEnrollments.Local Where
                               cen.YearId = YearID And cen.StudentId = st.Id And cen.CourseId = courseID And cen.GradeId = GradeID).SingleOrDefault()
                    Dim mkcw As MarksExamCW = Nothing
                    If cenr Is Nothing Then
                        cenr = New CourseEnrollment()
                        With cenr
                            .YearId = YearID
                            .GradeId = GradeID
                            .SemesterId = 2
                            .StudentId = st.Id
                            .CourseId = courseID
                        End With
                        db.CourseEnrollments.Add(cenr)
                    Else
                        mkcw = cenr.MarksExamCW
                    End If

                    If mkcw Is Nothing Then
                        mkcw = New MarksExamCW()
                        With mkcw
                            .YearId = YearID
                            .GradeId = GradeID
                            .SemesterId = 2
                            .StudentId = st.Id
                            .CourseId = courseID
                        End With
                        db.MarksExamCWs.Add(mkcw)
                    End If

                    With mkcw
                        .ExamMark = ExtractedMarksList(j).ExamMarkList(i)
                        .CWMark = ExtractedMarksList(j).CWMarkList(i)
                        .Present = ExtractedMarksList(j).PresentList(i)
                    End With
                Next
            Next

            db.SaveChanges()

            ' GPAs and recommendations not needed.
        End Using
    End Sub

End Class
