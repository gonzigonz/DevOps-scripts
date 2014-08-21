Set objArgs = WScript.Arguments
sourceFolder = objArgs(0)
zipFileName = objArgs(1)

Set objFSO = CreateObject("Scripting.FileSystemObject")
Set objShell = CreateObject("Shell.Application")

' Validate source folder
If objFSO.FileExists(sourceFolder) Then
    Wscript.Echo sourceFolder & " is not a folder!"
	Wscript.Quit
End If
If Not objFSO.FolderExists(sourceFolder) Then
    Wscript.Echo sourceFolder & " not found!"
	Wscript.Quit
End If

' Zip folder
WScript.Echo "Compressing folder '" & sourceFolder & "'..."

With objFSO
	zipFileName = .GetAbsolutePathName(zipFileName)
	sourceFolder = .GetAbsolutePathName(sourceFolder)
	With .CreateTextFile(zipFileName, True)
		.Write Chr(80) & Chr(75) & Chr(5) & Chr(6) & String(18, chr(0))
	End With
End With

With objShell
        .NameSpace(zipFileName).CopyHere .NameSpace(sourceFolder).Items

        Do Until .NameSpace(zipFileName).Items.Count = _
                 .NameSpace(sourceFolder).Items.Count
            WScript.Sleep 1000 
        Loop
    End With

'Set source = objShell.NameSpace(sourceFolder).Items
'objShell.NameSpace(zipFileName).CopyHere(source)
'wScript.Sleep 2000

Wscript.Echo "Done."