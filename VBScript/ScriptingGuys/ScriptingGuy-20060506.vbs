strComputer = "."

Set objWMIService = GetObject("winmgmts:\\" & strComputer & "\root\cimv2")
Set objFile = objWMIService.Get("Win32_LogicalFileSecuritySetting='c:\temp\test'")

If objFile.GetSecurityDescriptor(objSD) = 0 Then

Wscript.Echo "Owner: " & objSD.Owner.Name
Wscript.Echo

For Each objAce in objSD.DACL
    Wscript.Echo "Trustee: " & objAce.Trustee.Domain & "\" & objAce.Trustee.Name

    If objAce.AceType = 0 Then
        strAceType = "Allowed"
    Else
        strAceType = "Denied"
    End If
    Wscript.Echo "Ace Type: " & strAceType

    Wscript.Echo "Ace Flags:"

    If objAce.AceFlags AND 1 Then
        Wscript.Echo vbTab & "Child objects that are not containers inherit permissions."
    End If

    If objAce.AceFlags AND 2 Then
        Wscript.Echo vbTab & "Child objects inherit and pass on permissions."
    End If

    If objAce.AceFlags AND 4 Then
        Wscript.Echo vbTab & "Child objects inherit but do not pass on permissions."
    End If

    If objAce.AceFlags AND 8 Then
        Wscript.Echo vbTab & "Object is not affected by but passes on permissions."
    End If

    If objAce.AceFlags AND 16 Then
        Wscript.Echo vbTab & "Permissions have been inherited."
    End If

    Wscript.Echo "Access Masks:" & objAce.AccessMask
    If objAce.AccessMask AND 1048576 Then
        Wscript.Echo vbtab & "Synchronize"
    End If
    If objAce.AccessMask AND 524288 Then
        Wscript.Echo vbtab & "Write owner"
    End If
    If objAce.AccessMask AND 262144 Then
        Wscript.Echo vbtab & "Write ACL"
    End If
    If objAce.AccessMask AND 131072 Then
        Wscript.Echo vbtab & "Read security"
    End If
    If objAce.AccessMask AND 65536 Then
        Wscript.Echo vbtab & "Delete"
    End If
    If objAce.AccessMask AND 256 Then
        Wscript.Echo vbtab & "Write attributes"
    End If
    If objAce.AccessMask AND 128 Then
        Wscript.Echo vbtab & "Read attributes"
    End If
    If objAce.AccessMask AND 64 Then
        Wscript.Echo vbtab & "Delete dir"
    End If
    If objAce.AccessMask AND 32 Then
        Wscript.Echo vbtab & "Execute"
    End If
    If objAce.AccessMask AND 16 Then
        Wscript.Echo vbtab & "Write extended attributes"
    End If
    If objAce.AccessMask AND 8 Then
        Wscript.Echo vbtab & "Read extended attributes"
    End If
    If objAce.AccessMask AND 4 Then
        Wscript.Echo vbtab & "Append"
    End If
    If objAce.AccessMask AND 2 Then
        Wscript.Echo vbtab & "Write"
    End If
    If objAce.AccessMask AND 1 Then
        Wscript.Echo vbtab & "Read"
    End If

    Wscript.Echo
    Wscript.Echo
Next

End If
