Public Class OfferedCourseStatisticsDialog
    Sub PopulateWithCourseData(ofc As OfferedCourse)
        Dim ofcsm = GetOfferedCourseStatisticsModel(ofc)
        Dim grs = ofc.MarksExamCWs.ToList().ConvertAll(Function(s) ResultsProcessingUtilities.AssignGrade(s))

        ' Generate Distribution Chart from raw data. Exclude students who are absent.
        Dim NoOfBins As Integer = 20
        Dim MaxMark As Double = 100
        Dim BinSize As Double = MaxMark / NoOfBins
        Dim BinMinima As New List(Of Double)
        Dim BinMaxima As New List(Of Double)
        Dim BinCenters As New List(Of Double)
        Dim dataPoints As New PointCollection(NoOfBins)
        Dim IdealPoints As New PointCollection(NoOfBins)
        Dim NonAbsentStudents As Integer = ofc.MarksExamCWs.LongCount(Function(s) s.Present)
        For i = 0 To NoOfBins - 1
            Dim n As Integer = i
            Dim count As Integer = grs.LongCount(Function(s) s.Mark.Present AndAlso (s.Total > (n * BinSize)) And (s.Total < ((n + 1) * BinSize)))
            Dim x As Double = (i + 0.5) * BinSize
            dataPoints.Add(New Point(x, count))
        Next
        For x = 0 To 100 Step 2.5
            IdealPoints.Add(New Point(x, NonAbsentStudents * NormalDist(x, ofcsm.Average, ofcsm.StandardDeviation)))
        Next
        CourseLineSeries.DataContext = dataPoints
        IdealLineSeries.DataContext = IdealPoints

        Me.DataContext = ofcsm
    End Sub
    Function NormalDist(x As Double, mu As Double, sigma As Double) As Double
        Return (1 / Math.Sqrt(2 * Math.PI * sigma)) * Math.Exp(-(x - mu) * (x - mu) / (sigma * sigma))
    End Function
End Class
Public Class OfferedCourseStatisticsModel
    Property CourseCode As String
    Property TitleEnglish As String
    Property TotalStudents As Integer
    Property Absentees As Integer
    Property Pass As Integer
    Property Fail As Integer
    Property Average As Double
    Property StandardDeviation As Double

    Property CountAplus As Integer
    Property CountA As Integer
    Property CountAminus As Integer
    Property CountBplus As Integer
    Property CountB As Integer
    Property CountC As Integer
    Property CountD As Integer
    Property CountF As Integer
    Property CountDstar As Integer
    Property CountFstar As Integer
    Property CountAB As Integer
    Property CountABstar As Integer
End Class
