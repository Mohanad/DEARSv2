'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated from a template.
'
'     Manual changes to this file may cause unexpected behavior in your application.
'     Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic

Partial Public Class SubSuppMarksExamCW
	Implements IEquatable(Of SubSuppMarksExamCW)

	Function Equals1(other as SubSuppMarksExamCW) as Boolean Implements IEquatable(Of SubSuppMarksExamCW).Equals
		
		 If DirectCast(Me, Object).Equals(DirectCast(other, Object)) Then
            Return True
        Else
			If {Me.YearId,other.YearId,Me.GradeId,other.GradeId,Me.SemesterId,other.SemesterId,Me.CourseId,other.CourseId,Me.StudentId,other.StudentId}.Any(Function(s) s = 0) Then 
				Return False
			End If
		    Return (Me.YearId = other.YearId) And(Me.GradeId = other.GradeId) And(Me.SemesterId = other.SemesterId) And(Me.CourseId = other.CourseId) And(Me.StudentId = other.StudentId)
		End If	
	
	End Function
    Public Property YearId As Integer
    Public Property GradeId As Integer
    Public Property SemesterId As Integer
    Public Property CourseId As Integer
    Public Property StudentId As Integer
    Public Property ExamTypeId As Integer
    Public Property CWMark As Nullable(Of Double)
    Public Property Present As Nullable(Of Boolean)
    Public Property Excuse As Nullable(Of Boolean)
    Public Property ExamMark As Nullable(Of Double)

    Public Overridable Property CourseEnrollment As CourseEnrollment
    Public Overridable Property Course As Course
    Public Overridable Property ExamType As ExamType
    Public Overridable Property Grade As Grade
    Public Overridable Property MarksExamCW As MarksExamCW
    Public Overridable Property OfferedCours As OfferedCourse
    Public Overridable Property Semester As Semester
    Public Overridable Property Student As Student
    Public Overridable Property TimeYear As TimeYear

End Class