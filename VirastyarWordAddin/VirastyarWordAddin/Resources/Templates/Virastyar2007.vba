Public Function GetVirastyarObject() As Object
    Dim addIn As COMAddIn
    Dim automationObject As Object
    Set addIn = GetAddin("VirastyarWordAddin")
    
    If addIn Is Nothing Then ' Office 2007
        Set addIn = GetAddin("Virastyar2007")
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

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Sub VirastyarPinglishConvert_Action2(ByVal obj As Object)
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.PinglishConvert_Action
    End If
End Sub
Public Sub VirastyarPinglishConvert_Action()
    VirastyarPinglishConvert_Action2 Nothing
End Sub

Public Sub VirastyarPinglishConvertAll_Action2(ByVal obj As Object)
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.PinglishConvertAll_Action
    End If
End Sub
Public Sub VirastyarPinglishConvertAll_Action()
    VirastyarPinglishConvertAll_Action2 Nothing
End Sub

Public Sub VirastyarCheckDates_Action2(ByVal obj As Object)
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    virastyar.CheckDates_Action
End Sub
Public Sub VirastyarCheckDates_Action()
    VirastyarCheckDates_Action2 Nothing
End Sub

Public Sub VirastyarCheckNumbers_Action2(ByVal obj As Object)
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.CheckNumbers_Action
    End If
End Sub
Public Sub VirastyarCheckNumbers_Action()
    VirastyarCheckNumbers_Action2 Nothing
End Sub

Public Sub VirastyarCheckSpell_Action2(ByVal obj As Object)
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.CheckSpell_Action
    End If
End Sub
Public Sub VirastyarCheckSpell_Action()
    VirastyarCheckSpell_Action2 Nothing
End Sub

Public Sub VirastyarPreCheckSpell_Action2(ByVal obj As Object)
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    virastyar.PreCheckSpell_Action
End Sub
Public Sub VirastyarPreCheckSpell_Action()
    VirastyarPreCheckSpell_Action2 Nothing
End Sub

Public Sub VirastyarCheckPunctuation_Action2(ByVal obj As Object)
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.CheckPunctuation_Action
    End If
End Sub
Public Sub VirastyarCheckPunctuation_Action()
VirastyarCheckPunctuation_Action2 Nothing
End Sub

Public Sub VirastyarCheckAllPunctuation_Action2(ByVal obj As Object)
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.CheckAllPunctuation_Action
    End If
End Sub
Public Sub VirastyarCheckAllPunctuation_Action()
    VirastyarCheckAllPunctuation_Action2 Nothing
End Sub

Public Sub VirastyarRefineAllCharacters_Action2(ByVal obj As Object)
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.RefineAllCharacters_Action
    End If
End Sub
Public Sub VirastyarRefineAllCharacters_Action()
    VirastyarRefineAllCharacters_Action2 Nothing
End Sub

Public Sub VirastyarAddinSettings_Action2(ByVal obj As Object)
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.AddinSettings_Action
    End If
End Sub
Public Sub VirastyarAddinSettings_Action()
    VirastyarAddinSettings_Action2 Nothing
End Sub

Public Sub VirastyarAbout_Action2(ByVal obj As Object)
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.About_Action
    End If
End Sub
Public Sub VirastyarAbout_Action()
    VirastyarAutoComplete_Action2 Nothing
End Sub

Public Sub VirastyarAutoComplete_Action2(ByVal obj As Object)
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.AutoComplete_Action
    End If
End Sub
Public Sub VirastyarAutoComplete_Action()
    VirastyarAutoComplete_Action2 Nothing
End Sub

Public Sub VirastyarHelp_Action2(ByVal obj As Object)
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.Help_Action
    End If
End Sub

Public Sub VirastyarHelp_Action()
    VirastyarHelp_Action2 Nothing
End Sub


Public Sub VirastyarTip_Action2(ByVal obj As Object)
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.Tip_Action
    End If
End Sub

Public Sub VirastyarTip_Action()
    VirastyarTip_Action2 Nothing
End Sub

Public Sub VirastyarUpdate_Action2(ByVal obj As Object)
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        virastyar.VirastyarUpdate_Action
    End If
End Sub

Public Sub VirastyarUpdate_Action()
    VirastyarUpdate_Action2 Nothing
End Sub
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Sub VirastyarAddinGroup_IsVisible(control As IRibbonControl, ByRef visible)
    visible = VirastyarUpdate_IsVisible
End Sub

Public Sub VirastyarUpdate_IsVisible(control As IRibbonControl, ByRef visible)
    Dim virastyar As Object
    Set virastyar = GetVirastyarObject
    If Not virastyar Is Nothing Then
        visible = virastyar.Get_IsUpdateAvailable
    Else
        visible = False
    End If
End Sub

Public Function VirastyarUpdate_IsEnabled(control As IRibbonControl)
    VirastyarUpdate_IsEnabled = True
End Function
