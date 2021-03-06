﻿#Region "Microsoft.VisualBasic::253b52145970fcc48e49ad15076160c7, gr\Microsoft.VisualBasic.Imaging\Drawing2D\Extensions.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Module Extensions
    ' 
    '         Function: (+3 Overloads) Enlarge, GetTextAnchor, (+2 Overloads) Move, (+2 Overloads) MoveTo, Rotate
    ' 
    '         Sub: ShapeGlow
    '         Enum MoveTypes
    ' 
    '             BoundsBoxTopLeft, PolygonCentre
    ' 
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Extensions

Namespace Drawing2D

    Public Module Extensions

        <Extension>
        Public Sub ShapeGlow(ByRef g As IGraphics, path As GraphicsPath, glowColor As Color, Optional glowSize! = 10)
            For i As Integer = 1 To glowSize
                Using pen As New Pen(glowColor, i) With {
                    .LineJoin = LineJoin.Round
                }
                    g.DrawPath(pen, path)
                End Using
            Next
        End Sub

        Public ReadOnly BlackBrush As DefaultValue(Of Brush) = Brushes.Black

        ''' <summary>
        ''' 以当前的点为圆心，向<paramref name="angle"/>方向移动给定的距离
        ''' </summary>
        ''' <param name="pt"></param>
        ''' <param name="distance#"></param>
        ''' <param name="angle#"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Move(pt As Point, distance#, angle#) As Point
            Dim X = pt.X + distance * Math.Sin(angle * Math.PI / 180)
            Dim Y = pt.Y + distance * Math.Cos(angle * Math.PI / 180)

            Return New Point(X, Y)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Move(rect As RectangleF, distance#, angle#) As RectangleF
            Return New RectangleF With {
                .Location = rect.Location _
                    .ToPoint _
                    .Move(distance, angle),
                .Size = rect.Size
            }
        End Function

        ''' <summary>
        ''' 分别计算出<paramref name="textLayout"/>的上下左右对<paramref name="anchor"/>的距离，取最小的距离的位置并返回
        ''' </summary>
        ''' <param name="textLayout">标签文本的大小和位置，生成一个<see cref="Rectangle"/>布局对象</param>
        ''' <param name="anchor">这个标签文本所属的对象的锚点</param>
        ''' <returns></returns>
        <Extension>
        Public Function GetTextAnchor(textLayout As Rectangle, anchor As PointF) As Point
            With textLayout
                Dim points As Point() = {
                    New Point(.Left + .Width / 2, .Top),    ' top
                    New Point(.Left + .Width / 2, .Bottom), ' bottom,
                    New Point(.Left, .Top + .Height / 2),   ' left,
                    New Point(.Right, .Top + .Height / 2)   ' right
                }
                Dim d#() = points.Distance(anchor.ToPoint)

                Return points(Which.Min(d))
            End With
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Enlarge(size As SizeF, fold#) As SizeF
            With size
                Return New SizeF(.Width * fold, .Height * fold)
            End With
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension> Public Function Enlarge(shape As IEnumerable(Of Point), scale#) As Point()
            Return shape.PointF.Enlarge(scale).ToPoints
        End Function

        ''' <summary>
        ''' 将一个多边形放大指定的倍数<paramref name="scale"/>
        ''' </summary>
        ''' <param name="shape">矢量图形的点集合</param>
        ''' <param name="scale#"></param>
        ''' <returns></returns>
        <Extension> Public Function Enlarge(shape As IEnumerable(Of PointF), scale#) As PointF()
            Dim shapeVector = shape.ToArray
            Dim center = shapeVector.Centre
            Dim x As New Vector(shapeVector.Select(Function(pt) pt.X))
            Dim y As New Vector(shapeVector.Select(Function(pt) pt.Y))
            Dim b = x - CDbl(center.X)
            Dim a = y - CDbl(center.Y)
            Dim c = Vector.Sqrt(b ^ 2 + a ^ 2)
            Dim cs = c * scale
            Dim dc = cs - c
            Dim dx = (b / c) * dc
            Dim dy = (a / c) * dc

            For i As Integer = 0 To c.Length - 1
                ' 2018-3-6 如果有个点是位于shape的中心，那么在scale之后c值为零
                ' 则计算出来的差异量为NaN，会导致出错
                ' 在这里将所有c值为零的点都设置为原来的值，即意味着在多边形放大之后其位置没有发生变化
                If c(i) = 0R Then
                    dx(i) = 0#
                    dy(i) = 0#
                End If
            Next

            x = x + dx
            y = y + dy

            ' 返回放大之后的矢量图形向量
            Return shapeVector _
                .Sequence _
                .Select(Function(i) New PointF(x(i), y(i))) _
                .ToArray
        End Function

        ''' <summary>
        ''' 请注意，这个是围绕坐标轴远点进行的旋转，如果想要围绕指定点进行旋转，还需要进行平移操作
        ''' </summary>
        ''' <param name="shape"></param>
        ''' <param name="alpha#"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Rotate(shape As IEnumerable(Of PointF), alpha#) As PointF()
            Dim vector = shape.ToArray
            Dim x0 As New Vector(vector.Select(Function(pt) pt.X))
            Dim y0 As New Vector(vector.Select(Function(pt) pt.Y))
            Dim x1 = x0 * Math.Cos(alpha) + y0 * Math.Sin(alpha)
            Dim y1 = -x0 * Math.Sin(alpha) + y0 * Math.Cos(alpha)
            Return (x1, y1).Point2D.ToArray
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="shape"></param>
        ''' <param name="location"></param>
        ''' <param name="type">By default, is move the shape its bounds box topleft to target place.</param>
        ''' <returns></returns>
        <Extension>
        Public Function MoveTo(shape As IEnumerable(Of PointF), location As PointF, Optional type As MoveTypes = MoveTypes.BoundsBoxTopLeft) As PointF()
            Dim polygon = shape.ToArray
            Dim offset As PointF

            Select Case type
                Case MoveTypes.BoundsBoxTopLeft
                    With polygon.GetBounds
                        offset = New PointF(.Left - location.X, .Top - location.Y)
                    End With
                Case Else
                    With polygon.Centre
                        offset = New PointF(.X - location.X, .Y - location.Y)
                    End With
            End Select

            Dim out As PointF() = polygon _
                .Select(Function(point)
                            Return New PointF With {
                                .X = point.X - offset.X,
                                .Y = point.Y - offset.Y
                            }
                        End Function) _
                .ToArray

            Return out
        End Function

        '<Extension>
        'Public Function MoveTopLeft(polygon As PointF(), topLeft As PointF) As PointF()

        'End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function MoveTo(shape As IEnumerable(Of Point), location As PointF, Optional type As MoveTypes = MoveTypes.BoundsBoxTopLeft) As Point()
            Return shape _
                .Select(Function(point) point.PointF) _
                .MoveTo(location) _
                .Select(Function(point) point.ToPoint) _
                .ToArray
        End Function

        Public Enum MoveTypes As Byte
            ''' <summary>
            ''' Move the shape its bounds box topleft to target place
            ''' </summary>
            BoundsBoxTopLeft
            PolygonCentre
        End Enum
    End Module
End Namespace
