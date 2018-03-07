﻿#Region "Microsoft.VisualBasic::10ddbab1257604facac99b528c85f6be, Microsoft.VisualBasic.Core\ApplicationServices\Tools\Network\Mailto\MailContents.vb"

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

'     Class MailContents
' 
'         Properties: Attatchments, Body, Logo, Subject
' 
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Net.Mail
Imports System.Net.Mime
Imports System.Runtime.CompilerServices

Namespace Net.Mailto

    ''' <summary>
    ''' E-Mail content data model
    ''' </summary>
    Public Class MailContents

        ''' <summary>
        ''' The message html text template.
        ''' </summary>
        Shared ReadOnly MessageHtml As String =
            <html>
                <body>
                    <table border="2">
                        <tr width="100%">
                            <td><img src="cid:Logo" alt="companyname"/></td>
                            <td>MY COMPANY DESCRIPTION</td>
                        </tr>
                    </table>
                    <hr/>

                    $text
                </body>
            </html>

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property Subject As String
        ''' <summary>
        ''' Body html
        ''' </summary>
        ''' <returns></returns>
        Public Property Body As String

        ''' <summary>
        ''' The path list of the attachments file.
        ''' </summary>
        ''' <remarks></remarks>
        Public Property Attatchments As New List(Of String)
        ''' <summary>
        ''' The file path of the logo image.
        ''' </summary>
        ''' <remarks></remarks>
        Public Property Logo As String

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0} -- {1}", Subject, Body)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(msg As MailContents) As MailMessage
            Return FromMailContent(msg)
        End Operator

        Public Shared Function FromMailContent(content As MailContents) As MailMessage
            Dim altView As AlternateView
            Dim msg As MailMessage = New MailMessage With {
                .Subject = content.Subject,
                .Body = content.Body
            }
            Dim html$

            If Not content.Attatchments.IsNullOrEmpty Then
                For Each path As String In content.Attatchments
                    Call msg.Attachments.Add(New Attachment(fileName:=path))
                Next
            End If

            If Len(content.Body) = 0 Then content.Body = String.Empty
            If Len(content.Logo) > 0 Then
                Dim logo As New LinkedResource(content.Logo) With {
                    .ContentId = "Logo"
                }
                html = MessageHtml.Replace("$text", content.Body)
                altView = AlternateView.CreateAlternateViewFromString(
                    html, Nothing, MediaTypeNames.Text.Html)
                altView.LinkedResources.Add(logo)
            Else
                html = content.Body
                altView = AlternateView.CreateAlternateViewFromString(
                    html, Nothing, MediaTypeNames.Text.Html)
            End If

            msg.AlternateViews.Add(altView)
            msg.IsBodyHtml = True

            Return msg
        End Function
    End Class
End Namespace
