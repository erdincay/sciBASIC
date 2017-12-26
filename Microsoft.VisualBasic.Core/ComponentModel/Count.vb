﻿#Region "Microsoft.VisualBasic::e0fadaff9351799c69efc4a7f11f09e3, ..\sciBASIC#\Microsoft.VisualBasic.Core\ComponentModel\Count.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Namespace ComponentModel

    ''' <summary>
    ''' The object counter
    ''' </summary>
    Public Class Counter : Inherits int

        Sub New()
            MyBase.New(0)
        End Sub

        Sub New(hits As Integer)
            Call MyBase.New(x:=hits)
        End Sub

        ''' <summary>
        ''' ``++i``
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Hit() As Integer
            Return ++Me
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Narrowing Operator CType(c As Counter) As Integer
            Return c.Value
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Narrowing Operator CType(c As Counter) As Double
            Return CDbl(c.Value)
        End Operator
    End Class

    Public Module CounterExtensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function AsNumeric(Of K)(counter As Dictionary(Of K, Counter)) As Dictionary(Of K, Double)
            Return counter.ToDictionary(Function(c) c.Key, Function(c) CDbl(c.Value))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function AsInteger(Of K)(counter As Dictionary(Of K, Counter)) As Dictionary(Of K, Integer)
            Return counter.ToDictionary(Function(c) c.Key, Function(c) CInt(c.Value))
        End Function
    End Module
End Namespace