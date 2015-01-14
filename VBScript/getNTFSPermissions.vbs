'*  Script name:    getNTFSPermissions.vbs
'*  Created on:     14/01/2015
'*  Author:         Gonzalo Lucero (https://github.com/gonzigonz)
'*                  - Special acknowledgements to The Microsoft Scripting Guys
'*                  - "We All Scream for Security Descriptors" May 2006
'*                  - (http://technet.microsoft.com/en-gb/magazine/2006.05.scriptingguy.aspx)
'*                  - Also special thanks to urkec for solution to retrieving security descriptors 
'*                  - when folders contain an Apostrophe (')
'*                  - (http://microsoft.public.scripting.vbscript.narkive.com/ddYLHSos/apostrophe-problem-when-retrieving-folder-security-details)
'*  License:        The MIT License (MIT) Copyright (c) 2014 Gonzigonz
'*  Purpose:        Retrives a csv list providing a quick way to do a simple audit of 
'*                  NTFS permissions on a local server.
'*  History:        firstname surname date
'*                  Modified to reflect new area code....etc

' Using
Set objFSO = CreateObject("Scripting.FileSystemObject")
Set objWMI = GetObject("winmgmts:\\localhost\root\cimv2")
Set objNetwork = CreateObject("WScript.Network") 'Used to determine dns name
Set objArgs = WScript.Arguments

Dim strPath 'e.g c:\temp
Dim boolOutputToFile 'e.g true
Dim strOutputFilename 'e.g ntfsPermissoins.csv

' Locals
Const ForAppending = 2
Const strComputer = "."

' Construct Arguments
strPath = objArgs(0)
boolOutputToFile = false
strOutputFilename = "ntfsPermissoins.csv"

' Contruct Output
If boolOutputToFile Then
    strFile = objFSO.GetAbsolutePathName(strOutputFilename)
    Set output = objFSO.OpenTextFile(strFile, ForAppending, True)
Else
    Set output = Wscript.Stdout
End If

' MAIN
Wscript.Stdout.WriteLine("Reterving NTFS permissions...")
Set rootFolder = objFSO.GetFolder(strPath)

'Check root folder
strCurrentFolder = Replace(rootFolder.path, "\", "\\") 'This is required to handle for folder names with apostrophes
Set objFile = GetObject("winmgmts:Win32_LogicalFileSecuritySetting.path=""" & strCurrentFolder & """")
If objFile.GetSecurityDescriptor(objSD) = 0 Then
    For Each objAce in objSD.DACL
        'Only output details if ACL (permission) is not inherited
        If Not objAce.AceFlags AND 16 Then
            
            'Output folder path
            output.Write(rootFolder.path)
            
            'Output NT account name
            output.WriteLine("      - " & objAce.Trustee.Domain & "\" & objAce.Trustee.Name)
            
        End If
    Next
End If
    
'Check sub folders
ListSubFolders(rootFolder)

' Subs

Sub ListSubFolders(Folder)
    For Each subFolder in Folder.Subfolders
        REM output.Write(".")
        output.Write(subFolder.path)
         
        'Get current folders permissions
        strCurrentFolder = Replace(subfolder.path, "\", "\\") 'This is required to handle for folder names with apostrophes
        Set objFile = GetObject("winmgmts:Win32_LogicalFileSecuritySetting.path=""" & strCurrentFolder & """")
        If objFile.GetSecurityDescriptor(objSD) = 0 Then
            For Each objAce in objSD.DACL
                'Only output details if ACL (permission) is not inherited
                If Not objAce.AceFlags AND 16 Then
                    REM output.WriteLine()
                    
                    'Output folder path
                    REM output.Write(subfolder.path & ": ")
                    
                    'Output NT account name
                    output.WriteLine("      - " & objAce.Trustee.Domain & "\" & objAce.Trustee.Name)
                    
                    output.Write(subFolder.path)
                    
                End If
            Next
        End If
        
        output.WriteLine()
        
        'Go deeper
        ListSubFolders(subFolder)
    Next
End Sub