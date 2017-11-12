﻿#Region "Microsoft.VisualBasic::7d917a9aae1f227954924417a5286616, ..\sciBASIC#\Data\Trinity\TextRank.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
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
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Data.Graph.Analysis.PageRank
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Text
Imports r = System.Text.RegularExpressions.Regex

''' <summary>
''' This module implements TextRank, an unsupervised keyword
''' significance scoring algorithm. TextRank builds a weighted
''' graph representation Of a document Using words As nodes
''' And coocurrence frequencies between pairs of words as edge 
''' weights.It then applies PageRank to this graph, And
''' treats the PageRank score of each word as its significance.
''' The original research paper proposing this algorithm Is
''' available here
'''
''' > https://web.eecs.umich.edu/~mihalcea/papers/mihalcea.emnlp04.pdf
''' 
''' </summary>
Public Module TextRank

    ''' <summary>
    ''' Delimiter that using for split the large text block into seperated sentenses.
    ''' </summary>
    ReadOnly sdeli As Char() = {"."c, "?"c, "!"c, ";"c}
    ''' <summary>
    ''' Split text as words
    ''' </summary>
    ReadOnly allSymbols As Char() = ASCII.Symbols.AsList + {" "c, ASCII.TAB}

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Public Function Sentences(text$) As String()
        Return text.Split(TextRank.sdeli)
    End Function

    <Extension> Public Function StripMessy(text$) As String
        text = r.Replace(text, "\s+", " ")

        Return text
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Public Function Words(text$) As String()
        Return text _
            .Split(allSymbols) _
            .Where(Function(s) Not String.IsNullOrEmpty(s)) _
            .ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function Removes(words As IEnumerable(Of String), stopwords As StopWords) As IEnumerable(Of String)
        Return stopwords.Removes(words)
    End Function

    ''' <summary>
    ''' ##### 使用TextRank提取关键字
    ''' 
    ''' 将原文本拆分为句子，在每个句子中过滤掉停用词（可选），并只保留指定词性的单词（可选）。由此可以得到句子的集合和单词的集合。
    ''' 每个单词作为pagerank中的一个节点。设定窗口大小为k，假设一个句子依次由下面的单词组成
    ''' 
    ''' ```
    ''' w1, w2, w3, w4, w5, ..., wn
    ''' ```
    ''' 
    ''' ``w1, w2, ..., wk``、``w2, w3, ...,wk+1``、``w3, w4, ...,wk+2``等都是一个窗口。在一个窗口中的任两个单词对应的节点之间存在一个无向无权的边。
    ''' 基于上面构成图，可以计算出每个单词节点的重要性。最重要的若干单词可以作为关键词。
    ''' </summary>
    ''' <param name="sentences"></param>
    ''' <returns></returns>
    <Extension>
    Public Function TextGraph(sentences As IEnumerable(Of String), Optional win_size% = 2, Optional stopwords As StopWords = Nothing) As GraphMatrix
        Dim g As New Graph.Graph
        Dim source As String() = sentences _
            .Select(AddressOf Trim) _
            .Where(Function(s) Not String.IsNullOrEmpty(s)) _
            .ToArray

        stopwords = stopwords Or StopWords.DefaultStopWords

        For Each text As String In source

            ' 假设每一句话之中的单词之间的顺序就是网络连接的方向
            Dim blocks = text _
                .ToLower _
                .Words _
                .Removes(stopwords) _
                .SlideWindows(win_size) _
                .ToArray

            For Each textBlock As SlideWindow(Of String) In blocks

                For Each word As String In textBlock
                    If Not g.ExistVertex(word) Then
                        Call g.AddVertex(word)
                    End If
                Next

                For Each combine As (a$, b$) In textBlock.FullCombination
                    Call g.AddEdge(combine.a, combine.b)
                Next
            Next
        Next

        Return New GraphMatrix(g)
    End Function

    ''' <summary>
    ''' 默认的用于计算两个句子相似度的函数。
    ''' </summary>
    ''' <param name="wordList1">分别代表两个句子，都是由单词组成的列表</param>
    ''' <param name="wordList2">分别代表两个句子，都是由单词组成的列表</param>
    ''' <returns></returns>
    Public Function Similarity(wordList1$(), wordList2$()) As Double
        Dim words$() = (wordList1.AsList + wordList2) _
            .Distinct _
            .ToArray
        Dim vector1 As New Vector(From word As String In words Select wordList1.Count(word))
        Dim vector2 As New Vector(From word As String In words Select wordList2.Count(word))

        ' 使用乘法计算出共同出现的单词的数量
        Dim vector3 = vector1 * vector2
        Dim coOccurNum = vector3.Where(Function(n) n > 0).Count

        If coOccurNum <= 0 Then
            Return 0
        End If

        Dim denominator = Math.Log(wordList1.Count) + Math.Log(wordList2.Count)

        If Math.Abs(denominator) = 0R Then
            Return 0
        End If

        Return coOccurNum / denominator
    End Function
End Module
