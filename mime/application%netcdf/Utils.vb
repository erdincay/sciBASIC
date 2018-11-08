﻿Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO

Module Utils

    ''' <summary>
    ''' Throws a non-valid NetCDF exception if the statement it's true
    ''' </summary>
    ''' <param name="statement">statement - Throws if true</param>
    ''' <param name="reason$">reason - Reason to throw</param>
    Public Sub notNetcdf(statement As Boolean, reason$)
        If (statement) Then
            Throw New FormatException($"Not a valid NetCDF v3.x file: {reason}")
        End If
    End Sub

    ''' <summary>
    ''' Moves 1, 2, Or 3 bytes to next 4-byte boundary
    ''' </summary>
    ''' <param name="buffer">
    ''' buffer - Buffer for the file data
    ''' </param>
    <Extension> Public Sub padding(buffer As BinaryDataReader)
        If ((buffer.offset Mod 4) <> 0) Then
            Call buffer.skip(4 - (buffer.offset Mod 4))
        End If
    End Sub

    ''' <summary>
    ''' Reads the name
    ''' </summary>
    ''' <param name="buffer">
    ''' buffer - Buffer for the file data
    ''' </param>
    ''' <returns>Name</returns>
    <Extension> Public Function readName(buffer As BinaryDataReader) As String
        ' Read name
        Dim nameLength = buffer.ReadUInt32()
        Dim name = buffer.ReadChars(nameLength)

        ' validate name
        ' TODO

        ' Apply padding
        Call buffer.padding()

        Return name
    End Function
End Module