Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.Entity
Imports System.Linq
Imports System.Net
Imports System.Web
Imports System.Web.Mvc
Imports Week01

Namespace Controllers
    Public Class StudentsController
        Inherits System.Web.Mvc.Controller

        Private db As New Database1Entities

        ' GET: Students
        Function Index() As ActionResult
            Return View(db.Students.ToList())
        End Function

        ' GET: Students/Details/5
        Function Details(ByVal id As Integer?) As ActionResult
            If IsNothing(id) Then
                Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
            End If
            Dim student As Student = db.Students.Find(id)
            If IsNothing(student) Then
                Return HttpNotFound()
            End If
            Return View(student)
        End Function

        ' GET: Students/Create
        Function Create() As ActionResult
            Return View()
        End Function

        ' POST: Students/Create
        'To protect from overposting attacks, enable the specific properties you want to bind to, for 
        'more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function Create(<Bind(Include:="Id,FirstName,LastName")> ByVal student As Student) As ActionResult
            If ModelState.IsValid Then
                db.Students.Add(student)
                db.SaveChanges()
                Return RedirectToAction("Index")
            End If
            Return View(student)
        End Function

        ' GET: Students/Edit/5
        Function Edit(ByVal id As Integer?) As ActionResult
            If IsNothing(id) Then
                Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
            End If
            Dim student As Student = db.Students.Find(id)
            If IsNothing(student) Then
                Return HttpNotFound()
            End If
            Return View(student)
        End Function

        ' POST: Students/Edit/5
        'To protect from overposting attacks, enable the specific properties you want to bind to, for 
        'more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function Edit(<Bind(Include:="Id,FirstName,LastName")> ByVal student As Student) As ActionResult
            If ModelState.IsValid Then
                db.Entry(student).State = EntityState.Modified
                db.SaveChanges()
                Return RedirectToAction("Index")
            End If
            Return View(student)
        End Function

        ' GET: Students/Delete/5
        Function Delete(ByVal id As Integer?) As ActionResult
            If IsNothing(id) Then
                Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
            End If
            Dim student As Student = db.Students.Find(id)
            If IsNothing(student) Then
                Return HttpNotFound()
            End If
            Return View(student)
        End Function

        ' POST: Students/Delete/5
        <HttpPost()>
        <ActionName("Delete")>
        <ValidateAntiForgeryToken()>
        Function DeleteConfirmed(ByVal id As Integer) As ActionResult
            Dim student As Student = db.Students.Find(id)
            db.Students.Remove(student)
            db.SaveChanges()
            Return RedirectToAction("Index")
        End Function

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If (disposing) Then
                db.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub
    End Class
End Namespace
