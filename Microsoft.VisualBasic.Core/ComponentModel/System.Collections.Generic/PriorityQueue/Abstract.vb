﻿#Region "Microsoft.VisualBasic::bca0d86dd22bc409d262feb73d47f50b, ..\sciBASIC#\Microsoft.VisualBasic.Core\ComponentModel\System.Collections.Generic\PriorityQueue\Abstract.vb"

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

Namespace ComponentModel.Collection

    Public Interface IPriorityQueue(Of T)
        Inherits ICollection
        Inherits ICloneable
        Inherits IList

        Function Push(O As T) As Integer
        Function Pop() As T
        Function Peek() As T

    End Interface
End Namespace