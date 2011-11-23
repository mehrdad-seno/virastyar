Public Function GetVirastyarObject() As Object
    Dim addIn As COMAddIn
    Dim automationObject As Object
    Set addIn = GetAddin("VirastyarWordAddin")
    
    If addIn Is Nothing Then ' Office 2003
        Set addIn = GetAddin("Virastyar")
    End If
    
    If addIn Is Nothing Then
        Set GetVirastyarObject = Nothing
    Else
        Set automationObject = addIn.Object
        Set GetVirastyarObject = automationObject
    End If
End Function

Function GetAddin(ByVal name As String)
    Set GetAddin = Nothing
    Dim addIn As COMAddIn
    For i = 1 To Application.COMAddIns.Count
        Set addIn = Application.COMAddIns.Item(i)
        If addIn.ProgID = name Then
            Set GetAddin = addIn
            Exit For
        End If
    Next
End Function

Public Sub VirastyarPinglishConvert_Action()
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.PinglishConvert_Action
    End If
End Sub

Public Sub VirastyarPinglishConvertAll_Action()
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.PinglishConvertAll_Action
    End If
End Sub

Public Sub VirastyarCheckDates_Action()
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.CheckDates_Action
    End If
End Sub

Public Sub VirastyarCheckNumbers_Action()
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.CheckNumbers_Action
    End If
End Sub

Public Sub VirastyarCheckSpell_Action()
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.CheckSpell_Action
    End If
End Sub

Public Sub VirastyarPreCheckSpell_Action()
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.PreCheckSpell_Action
    End If
End Sub

Public Sub VirastyarCheckPunctuation_Action()
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.CheckPunctuation_Action
    End If
End Sub

Public Sub VirastyarCheckAllPunctuation_Action()
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.CheckAllPunctuation_Action
    End If
End Sub

Public Sub VirastyarRefineAllCharacters_Action()
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.RefineAllCharacters_Action
    End If
End Sub

Public Sub VirastyarAddinSettings_Action()
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.AddinSettings_Action
    End If
End Sub

Public Sub VirastyarAbout_Action()
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.About_Action
    End If
End Sub


Public Sub VirastyarAutoComplete_Action()
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.AutoComplete_Action
    End If
End Sub


Public Sub VirastyarHelp_Action()
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.Help_Action
    End If
End Sub


Public Sub VirastyarTip_Action()
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.Tip_Action
    End If
End Sub
