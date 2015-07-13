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
Set objRegEx = new regexp

Dim strPath 'e.g c:\temp
Dim boolSaveToFile 'e.g true

' Locals
Const ForAppending = 2
Const strComputer = "."
Const strOutputFilename = "ntfsPermissoins.csv"
Const strFolderBlackList = "Recycle.Bin|Boot|MSOCache|ProgramData|Program Files|System Volume Information|Config.Msi"

' Construct Arguments
strPath = objArgs(0)
boolSaveToFile = strComp(objArgs(1), "true", vbTextCompare)  '(0) for true


' Contruct Output
If boolSaveToFile = 0 Then
    strFile = objFSO.GetAbsolutePathName(strOutputFilename)
    Set output = objFSO.OpenTextFile(strFile, ForAppending, True)
Else
    Set output = Wscript.Stdout
End If

' MAIN
Wscript.Stdout.WriteLine("Reterving NTFS permissions...")
If boolSaveToFile = 0 Then
    output.WriteLine("FOLDER,ACCOUNT")
    Wscript.Stdout.WriteLine("- Saving to file " & strOutputFilename)
End If

Set rootFolder = objFSO.GetFolder(strPath)

'Check root folder
CheckFolderPermissions(rootFolder.path)
    
'Check sub folders
CheckSubFoldersPermissions(rootFolder)

Wscript.Stdout.WriteLine("Done!")

' SUBS
Sub CheckSubFoldersPermissions(Folder)
    For Each subFolder in Folder.Subfolders
    
        'Only check non-system folders defined in the regurlar expression
        objRegEx.Pattern = strFolderBlackList
        objRegEx.IgnoreCase = true
        If Not objRegEx.Test(subFolder.path) Then
        
            'Check current folders permissions
            CheckFolderPermissions(subFolder.path)
            
            'Go deeper
            CheckSubFoldersPermissions(subFolder)
            
        End If

    Next
End Sub

Sub CheckFolderPermissions(folderPath)

    'Output folder path. For GUI only
    If Not boolSaveToFile = 0 Then output.Write(folderPath)
    
    'Added escape blackslash escape characters. This is required to use syntax that can handle for folder names with apostrophes
    strCurrentFolder = Replace(folderPath, "\", "\\")
    
    'Read permissions
    Set objFile = GetObject("winmgmts:Win32_LogicalFileSecuritySetting.path=""" & strCurrentFolder & """")
    If objFile.GetSecurityDescriptor(objSD) = 0 Then
    
        permissionsCount = 1
        
        For Each objAce in objSD.DACL
        
            'Only output details if ACL (permission) is NOT inherited
            If Not objAce.AceFlags And 16 Then
            
                If  permissionsCount = 1 And Not boolSaveToFile = 0 Then 
                    output.WriteLine(" - NEW PERMISSIONS FOUND")
                    output.WriteLine("*************************************")
                End If
                permissionsCount = permissionsCount + 1
                
                'Output NT account name
                If boolSaveToFile = 0 Then
                    output.WriteLine(folderPath & "," & objAce.Trustee.Domain & "\" & objAce.Trustee.Name)
                Else
                    output.WriteLine(" -" & objAce.Trustee.Domain & "\" & objAce.Trustee.Name)
                End If
            End If
            
        Next
    End If
    
    'Next line. For GUI only
    If Not boolSaveToFile = 0 Then output.WriteLine()
End Sub