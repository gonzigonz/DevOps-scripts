'*  Script name:    zipFiles.vbs
'*  Created on:     21/08/2014
'*  Author:         Gonzalo Lucero (https://github.com/gonzigonz)
'*  License:        The MIT License (MIT) Copyright (c) 2014 Gonzigonz
'*  Purpose:        Zips files or a folder with the options to 
'*                  overwrite existing zip file and or delete the original
'*                  source files.
'*  History:        firstname surname date
'*                  Modified to reflect new area code....etc

' Using
Set fso = CreateObject("Scripting.FileSystemObject")
Set oShell = CreateObject("Shell.Application")
Set tLib = CreateObject("Scriptlet.TypeLib")
Set objArgs = WScript.Arguments

Dim sourceDir, source, zipFileName, overWriteOption, zipExtension, useTempFolder, timer

' Construct Arguments
source = fso.GetAbsolutePathName(objArgs(0))
zipFileName = fso.GetAbsolutePathName(objArgs(1))
overWriteOption = not strComp(objArgs(2), "true", vbTextCompare)    '(-1) for true, (0) for false
deleteSource = not strComp(objArgs(3), "true", vbTextCompare)       '(-1) for true, (0) for false

' Ensure a valid zip extension is used
zipExtension = fso.GetExtensionName(zipFileName)
If strComp(zipExtension, "zip", vbTextCompare) = -1 Then
    zipFileName = zipFileName & ".zip"
End If

' Quit if overwrite set to false and a zip file already exists
If overWriteOption = 0 Then 
    If fso.FileExists(zipFileName) Then 
        Wscript.Echo "You have set the overwrite option to 'FALSE' but the zip file '" & zipFileName & "' already exists."
        Wscript.Echo "Task aborted!"
        Wscript.Quit
    End If
End If

' Setup and use a temp source director if give source is not a folder. ie a filepath or wildcard
If fso.FolderExists(source) Then
    useTempFolder = false
    sourceDir = source
Else
    useTempFolder = true
    SetupTempSourceDir fso.GetParentFolderName(zipFileName)
    CopyFilesToFolder source, sourceDir
End If

' Zip content
On Error Resume Next
    Wscript.Stdout.WriteLine("Compressing: """ & source & "") 
    Wscript.Stdout.Write("To: """ & zipFileName & """.") 
    With fso.CreateTextFile(zipFileName, overWriteOption)
	    .Write Chr(80) & Chr(75) & Chr(5) & Chr(6) & String(18, chr(0))
    End With

    If Err.number <> 0 Then
        Wscript.Stdout.WriteLine("! ")
        Wscript.Echo "Could not create the zip file. (" & Err.Description & ")"
        Wscript.Echo "Task aborted!"
        Err.Clear
        Dispose
        Wscript.Quit
    End If

    With oShell
            .NameSpace(zipFileName).CopyHere .NameSpace(sourceDir).Items

            ' Make sure to wait until every file is copied into the new zip file
			timer = 0 'In seconds
            Do Until .NameSpace(zipFileName).Items.Count = .NameSpace(sourceDir).Items.Count

                If Err.number <> 0 Then
                    Wscript.Stdout.WriteLine("! ")
                    Wscript.Echo "Compression failed. (" & Err.Description & ")"
                    Wscript.Echo "Task aborted!"
                    Err.Clear
                    Dispose
                    Wscript.Quit
                End If

                timer = timer + 1
                WScript.Sleep 1000 
            Loop
            Wscript.Stdout.WriteLine(".")
    End With

On Error Goto 0

' Delete source files or folder if the user has ask
If deleteSource = -1 Then 
    If useTempFolder = false Then
        Wscript.Echo "Deleteing the source folder..."
        fso.DeleteFolder source
    Else
        Wscript.Echo "Deleteing the source files..."
        fso.DeleteFile source
    End If
End If

' Call dispose and we are done
Dispose

Wscript.Echo "Task Complete! (" & timer & " seconds)"

''' SUB ROUTINES '''

'Sub to copy a file or files using wild card if source is not a folder
Sub CopyFilesToFolder(sourceFiles, targetFolder)
        On Error Resume Next
            fso.CopyFile sourceFiles, targetFolder  & "\"
            If Err.number <> 0 Then
                Wscript.Echo sourceFiles & " not found. (" & Err.Description & ")"
                Wscript.Echo "Task aborted!"
                Err.Clear
                Dispose
                Wscript.Quit
            End If
        On Error Goto 0
End Sub

'Sub to Create a temp folder and set as sourceDir
Sub SetupTempSourceDir(parentFolder)
    guid = tLib.Guid
    sourceDir = fso.BuildPath(parentFolder, Left(guid, Len(guid)-2))
    fso.CreateFolder(sourceDir)
End Sub

'Dispose sub to clean up each time the script exits
Sub Dispose()
    If useTempFolder = true Then fso.DeleteFolder sourceDir, true
End Sub