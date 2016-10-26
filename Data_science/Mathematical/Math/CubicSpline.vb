Imports System.Drawing
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' Cubic spline interpolation
''' </summary>
''' <remarks>
''' https://github.com/CrushedPixel/CubicSplineDemo
''' </remarks>
Public Class CubicSpline
    Implements IEnumerable(Of PointF)

    Dim _points As New List(Of PointF)
    Dim xCubics As New List(Of Cubic)
    Dim yCubics As New List(Of Cubic)

    Public Structure Cubic

        Public ReadOnly a, b, c, d As Double

        Public Sub New(p0 As Double, d2 As Double, e As Double, f As Double)
            d = p0
            c = d2
            b = e
            a = f
        End Sub

        Public Function Eval(u As Double) As Double
            'equals a*x^3 + b*x^2 + c*x + d
            Return (((a * u) + b) * u + c) * u + d
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure

    Sub New()
    End Sub

    Sub New(points As IEnumerable(Of PointF))
        Call _points.AddRange(points)
    End Sub

    Public Sub AddPoint(point As PointF)
        Me._points.Add(point)
    End Sub

    Private Enum PosField
        X
        Y
    End Enum

    Private Function __extractValues(points As IList(Of PointF), field As PosField) As IList(Of Single)
        Dim ints As New List(Of Single)()
        For Each p As PointF In points
            Select Case field
                Case PosField.X
                    ints.Add(p.X)
                Case PosField.Y
                    ints.Add(p.Y)
            End Select
        Next

        Return ints
    End Function

    Public Sub CalcSpline()
        CalcNaturalCubic(__extractValues(_points, PosField.X), xCubics)
        CalcNaturalCubic(__extractValues(_points, PosField.Y), yCubics)
    End Sub

    Public Function GetPoint(position As Single) As PointF
        position = position * xCubics.Count
        Dim cubicNum As Integer = CInt(Fix(Math.Min(xCubics.Count - 1, position)))
        Dim cubicPos As Single = (position - cubicNum)

        Return New PointF(Fix(xCubics(cubicNum).Eval(cubicPos)), Fix(yCubics(cubicNum).Eval(cubicPos)))
    End Function

    Public Sub CalcNaturalCubic(values As IList(Of Single), cubics As ICollection(Of Cubic))
        Dim num As Integer = values.Count - 1

        Dim gamma As Double() = New Double(num) {}
        Dim delta As Double() = New Double(num) {}
        Dim D As Double() = New Double(num) {}

        Dim i As Integer
        '		
        '               We solve the equation
        '	          [2 1       ] [D[0]]   [3(x[1] - x[0])  ]
        '	          |1 4 1     | |D[1]|   |3(x[2] - x[0])  |
        '	          |  1 4 1   | | .  | = |      .         |
        '	          |    ..... | | .  |   |      .         |
        '	          |     1 4 1| | .  |   |3(x[n] - x[n-2])|
        '	          [       1 2] [D[n]]   [3(x[n] - x[n-1])]
        '
        '	          by using row operations to convert the matrix to upper triangular
        '	          and then back substitution.  The D[i] are the derivatives at the knots.
        '		 
        gamma(0) = 1.0F / 2.0F
        For i = 1 To num - 1
            gamma(i) = 1.0F / (4.0F - gamma(i - 1))
        Next
        gamma(num) = 1.0F / (2.0F - gamma(num - 1))

        Dim p0 As Single = values(0)
        Dim p1 As Single = values(1)

        delta(0) = 3.0F * (p1 - p0) * gamma(0)
        For i = 1 To num - 1
            p0 = values(i - 1)
            p1 = values(i + 1)
            delta(i) = (3.0F * (p1 - p0) - delta(i - 1)) * gamma(i)
        Next

        p0 = values(num - 1)
        p1 = values(num)

        delta(num) = (3.0F * (p1 - p0) - delta(num - 1)) * gamma(num)

        D(num) = delta(num)
        For i = num - 1 To 0 Step -1
            D(i) = delta(i) - gamma(i) * D(i + 1)
        Next

        'now compute the coefficients of the cubics
        cubics.Clear()

        For i = 0 To num - 1
            p0 = values(i)
            p1 = values(i + 1)

            cubics.Add(New Cubic(p0, D(i), 3 * (p1 - p0) - 2 * D(i) - D(i + 1), 2 * (p0 - p1) + D(i) + D(i + 1)))
        Next
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="source">假若点的数目少于或者等于2个，则会返回空集合</param>
    ''' <returns></returns>
    Public Shared Iterator Function RecalcSpline(source As IEnumerable(Of PointF)) As IEnumerable(Of PointF)
        Dim spline As New CubicSpline()
        Dim PointFs As PointF() = source.ToArray

        If PointFs.Length > 2 Then
            For Each p As PointF In PointFs
                spline._points.Add(p)
            Next

            Call spline.CalcSpline()

            For f As Single = 0 To 1 Step 0.01
                Yield spline.GetPoint(f)
            Next
        Else
            ' 什么也不做，返回空集合
        End If
    End Function

    Public Shared Function RecalcSpline(source As IEnumerable(Of Point)) As IEnumerable(Of Point)
        Return RecalcSpline(source.Select(Function(pt) New PointF(pt.X, pt.Y)))
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator(Of PointF) Implements IEnumerable(Of PointF).GetEnumerator
        For f As Single = 0 To 1 Step 0.01
            Yield GetPoint(f)
        Next
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield GetEnumerator()
    End Function
End Class
