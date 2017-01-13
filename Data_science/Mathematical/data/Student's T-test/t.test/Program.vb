﻿Imports Microsoft.VisualBasic.Mathematical.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Serialization.JSON

Module Program

    Sub Main()
        Dim a#() = {175, 168, 168, 190, 156, 181, 182, 175, 174, 179}
        Dim b#() = {185, 169, 173, 173, 188, 186, 175, 174, 179, 180}

        With t.Test(a, b)
            Call $"alternative hypothesis: { .Valid}".__DEBUG_ECHO
            Call .GetJson(True).__DEBUG_ECHO
        End With

        Dim x#() = {0, 1, 1, 1}

        With t.Test(x, mu:=1)
            Call $"alternative hypothesis: { .Valid}".__DEBUG_ECHO
            Call .GetJson(True).__DEBUG_ECHO
        End With

        a = {1846523.253, 6840877.665, 2806323.704}
        b = {3056565.388, 1831431.105, 2933659.497}

        Call t.Test(a, b).GetJson.__DEBUG_ECHO
        Call t.Test(a, b, varEqual:=False).GetJson.__DEBUG_ECHO


        Pause()
    End Sub
End Module
