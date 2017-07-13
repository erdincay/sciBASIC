﻿Imports System.Dynamic
Imports Microsoft.VisualBasic.Emit.Delegates

Namespace Language

    ''' <summary>
    ''' Vectorization programming language feature for VisualBasic
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class VectorShadows(Of T) : Inherits DynamicObject
        Implements IEnumerable(Of T)

        ReadOnly vector As T()
        ReadOnly linq As DataValue(Of T)

        Public ReadOnly Property Length As Integer
            Get
                Return vector.Length
            End Get
        End Property

        Sub New(source As IEnumerable(Of T))
            vector = source.ToArray
            linq = New DataValue(Of T)(vector)
        End Sub

        Public Iterator Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            For Each x As T In vector
                Yield x
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace