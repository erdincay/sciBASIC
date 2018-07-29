Imports System.Collections.Generic

Namespace Exporter.YAML

    Public NotInheritable Class YAMLSequenceNode
        Inherits YAMLNode

        ReadOnly m_children As New List(Of YAMLNode)()

        Public Sub New()
        End Sub

        Public Sub New(style As SequenceStyle)
            Me.Style = style
        End Sub

        Public Sub Add(value As Boolean)
            Dim node As New YAMLScalarNode(value)
            node.Style = Style.ToScalarStyle()
            Add(node)
        End Sub

        Public Sub Add(value As Byte)
            Dim node As New YAMLScalarNode(value)
            node.Style = Style.ToScalarStyle()
            Add(node)
        End Sub

        Public Sub Add(value As Short)
            Dim node As New YAMLScalarNode(value)
            node.Style = Style.ToScalarStyle()
            Add(node)
        End Sub

        Public Sub Add(value As UShort)
            Dim node As New YAMLScalarNode(value)
            node.Style = Style.ToScalarStyle()
            Add(node)
        End Sub

        Public Sub Add(value As Integer)
            Dim node As New YAMLScalarNode(value)
            node.Style = Style.ToScalarStyle()
            Add(node)
        End Sub

        Public Sub Add(value As UInteger)
            Dim node As New YAMLScalarNode(value)
            node.Style = Style.ToScalarStyle()
            Add(node)
        End Sub

        Public Sub Add(value As Long)
            Dim node As New YAMLScalarNode(value)
            node.Style = Style.ToScalarStyle()
            Add(node)
        End Sub

        Public Sub Add(value As ULong)
            Dim node As New YAMLScalarNode(value)
            node.Style = Style.ToScalarStyle()
            Add(node)
        End Sub

        Public Sub Add(value As Single)
            Dim node As New YAMLScalarNode(value)
            node.Style = Style.ToScalarStyle()
            Add(node)
        End Sub

        Public Sub Add(value As Double)
            Dim node As New YAMLScalarNode(value)
            node.Style = Style.ToScalarStyle()
            Add(node)
        End Sub

        Public Sub Add(value As String)
            Dim node As New YAMLScalarNode(value)
            node.Style = Style.ToScalarStyle()
            Add(node)
        End Sub

        Public Sub Add(child As YAMLNode)
            m_children.Add(child)
        End Sub

        Friend Overrides Sub Emit(emitter As Emitter)
            MyBase.Emit(emitter)

            StartChildren(emitter)
            For Each child As YAMLNode In m_children
                StartChild(emitter, child)
                child.Emit(emitter)
                EndChild(emitter, child)
            Next
            EndChildren(emitter)
        End Sub

        Private Sub StartChildren(emitter As Emitter)
            If Style = SequenceStyle.Block Then
                If m_children.Count = 0 Then
                    emitter.Write("["c)
                End If
            ElseIf Style = SequenceStyle.Flow Then
                emitter.Write("["c)
            ElseIf Style = SequenceStyle.Raw Then
                If m_children.Count = 0 Then
                    emitter.Write("["c)
                End If
            End If
        End Sub

        Private Sub EndChildren(emitter As Emitter)
            If Style = SequenceStyle.Block Then
                If m_children.Count = 0 Then
                    emitter.Write("]"c)
                End If
                emitter.WriteLine()
            ElseIf Style = SequenceStyle.Flow Then
                emitter.WriteClose("]"c)
            ElseIf Style = SequenceStyle.Raw Then
                If m_children.Count = 0 Then
                    emitter.Write("]"c)
                End If
                emitter.WriteLine()
            End If
        End Sub

        Private Sub StartChild(emitter As Emitter, [next] As YAMLNode)
            If Style = SequenceStyle.Block Then
                emitter.IncreaseIntent()
                emitter.Write("-"c).WriteWhitespace()

                If [next].NodeType = NodeType Then
                    emitter.IncreaseIntent()
                End If
            End If
            If [next].IsIndent Then
                emitter.IncreaseIntent()
            End If
        End Sub

        Private Sub EndChild(emitter As Emitter, [next] As YAMLNode)
            If Style = SequenceStyle.Block Then
                emitter.WriteLine()
                emitter.DecreaseIntent()

                If [next].NodeType = NodeType Then
                    emitter.DecreaseIntent()
                End If
            ElseIf Style = SequenceStyle.Flow Then
                emitter.WriteSeparator().WriteWhitespace()
            End If
            If [next].IsIndent Then
                emitter.DecreaseIntent()
            End If
        End Sub

        Public Shared Empty As New YAMLSequenceNode()

        Public Overrides ReadOnly Property NodeType() As YAMLNodeType
            Get
                Return YAMLNodeType.Sequence
            End Get
        End Property

        Public Overrides ReadOnly Property IsMultyline() As Boolean
            Get
                Return Style = SequenceStyle.Block AndAlso m_children.Count > 0
            End Get
        End Property

        Public Overrides ReadOnly Property IsIndent() As Boolean
            Get
                Return False
            End Get
        End Property

        Public Property Style As SequenceStyle

    End Class
End Namespace
