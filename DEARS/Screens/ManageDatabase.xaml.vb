﻿Imports System.Data.Entity.Core.Objects
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Public Class ManageDatabase
    Implements IBaseScreen

    Public ReadOnly Property DBContext As AcademicResultsDBEntities Implements IBaseScreen.DBContext
        Get
            Return SharedState.DBContext
        End Get
    End Property

    Public Sub LoadData(PropertyName As String) Implements IBaseScreen.LoadData
        SharedState.DBContext.Grades.ToList()
    End Sub

    Private Sub RecreateDatabaseButton_Click(sender As Object, e As RoutedEventArgs)
        Try
            Dim eConn As System.Data.Common.DbConnection = Nothing
            If DBContext.Database.Exists() Then
                eConn = DirectCast(SharedState.DBContext, IObjectContextAdapter).ObjectContext.Connection
                System.Data.SqlClient.SqlConnection.ClearAllPools()
                SharedState.DBContext.Database.Connection.Close()
                DBContext.Database.ExecuteSqlCommand(System.Data.Entity.TransactionalBehavior.DoNotEnsureTransaction, _
                                                     "ALTER DATABASE AcademicResultsDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE")
                DBContext.Database.Delete()
            End If

            DBContext.Dispose()
            SharedState.DBContext = Nothing

            Dim sqlConnDialog As New SQLConnectWindow()
            
            sqlConnDialog.Show()
            sqlConnDialog.CreateDB()
            DirectCast(My.Application.MainWindow, MainWindow).ReloadData()

        Catch ex As Exception
            MsgBox(Application.FlattenOutException(ex))
        End Try
    End Sub

    Private Sub BasicDataButton_Click(sender As Object, e As RoutedEventArgs)
        Dim BasicDataQuery As String = System.IO.File.ReadAllText("NiceStateDB_Clean.sql")
        DBContext.Database.ExecuteSqlCommand(BasicDataQuery)
        DirectCast(My.Application.MainWindow, MainWindow).ReloadData()
    End Sub


    Public Shared Function Concat(source As IEnumerable(Of String)) As String
        Dim sb As New Text.StringBuilder()
        For Each s As String In source
            sb.Append(s)
        Next
        Return sb.ToString()
    End Function

    Private Sub BulkImportButton_Click(sender As Object, e As RoutedEventArgs)
        Dim importerDialog As New ImporterDialog()
        importerDialog.ShowDialog()
    End Sub

    Public Sub SaveDataColumnsToEntities(ExtractedData As Dictionary(Of String, List(Of String))) Implements IBaseScreen.SaveDataColumnsToEntities

    End Sub

    Private Sub AkodeImporter_Click(sender As Object, e As RoutedEventArgs) Handles AkodeImporter.Click
        Dim akImporter As New AkodeImporter()
        akImporter.ShowDialog()
    End Sub
End Class


'DBContext.Database.ExecuteSqlCommand("EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all'")

'Dim DropQuery = Concat(DBContext.Database.SqlQuery(Of String)("SELECT 'DROP TABLE [' + SCHEMA_NAME(schema_id) + '].[' + name + ']' FROM sys.tables"))

'DBContext.Database.ExecuteSqlCommand(System.Data.Entity.TransactionalBehavior.DoNotEnsureTransaction, DropQuery)
