'*  Script name:    getADGroupMembers.vbs
'*  Created on:     02/09/2014
'*  Author:         Gonzalo Lucero (https://github.com/gonzigonz)
'*                  - Special acknowledgements to Wouter-Trust @ Spiceworks Community
'*                  - "Export members AD group to txt"
'*                  - (http://community.spiceworks.com/scripts/show/411-export-members-ad-group-to-txt)
'*                  - Special acknowledgements to The Microsoft Scripting Guys
'*                  - "How Can I Get the Full Name and Description For My Local User Accounts?" Jul 2006
'*                  - (http://blogs.technet.com/b/heyscriptingguy/archive/2006/07/03/how-can-i-get-the-full-name-and-description-for-my-local-user-accounts.aspx)
'*  License:        The MIT License (MIT) Copyright (c) 2014 Gonzigonz
'*  Purpose:        Retrieves a list of members for a given AD group and user can
'*                  optionally output to a csv file. Will not return members where access is denied.
'*  History:        firstname surname date
'*                  Modified to reflect new area code....etc

' //Locals
Const ForReading = 1
Const ForWriting = 2
Const ForAppending = 8
Dim objGroup, objUser, objFSO, strDomain, strGroup, Domain, Group

' //Using
Set objFSO = CreateObject("Scripting.FileSystemObject")
Set objArgs = WScript.Arguments

' //Construct Arguments
'***** Reverse comments to use user inputs instead****
strDomain = objArgs(0)
strGroup = objArgs(1)
outputToFile = not strComp(objArgs(2), "true", vbTextCompare)    '(-1) for true, (0) for false
'strDomain = Inputbox ("Enter the Domain name", "Data needed", "Default domain name")
'strGroup = InputBox ("Enter the Group name", "Data needed", "Default group name")
'outputToFile = false

' //Contruct Output
If outputToFile Then
    includeHeaders = true
    outputFilename = objFSO.GetAbsolutePathName(objArgs(3))
    append = not strComp(objArgs(4), "true", vbTextCompare)    '(-1) for true, (0) for false

    fileExtension = objFSO.GetExtensionName(outputFilename)
    If strComp(fileExtension, "csv", vbTextCompare) = -1 Then
        outputFilename = outputFilename & ".csv"
    End If

    strFile = objFSO.GetAbsolutePathName(outputFilename)

    If append Then
        If objFSO.FileExists(outputFilename) Then includeHeaders = false
		Set output = objFSO.OpenTextFile(strFile,ForAppending, True) ' Appending to CSV File
	Else
		Set output = objFSO.CreateTextFile(strFile,ForWriting) ' Creating CSV File
	End If

Else
    Set output = Wscript.Stdout
End If

' //Main
Wscript.Stdout.WriteLine("Reterving AD Group Memberships...")
Wscript.Stdout.WriteLine
On Error Resume Next

    Set objGroup = GetObject("WinNT://" & strDomain & "/" & strGroup & ",group")
    If Err.number <> 0 Then
        Wscript.Echo "The Group " & strGroup & "@" & strDomain & " was not found! (" & Err.Description & ")"
        Wscript.Echo "Task aborted!"
        Err.Clear
        Dispose
        Wscript.Quit
    End If

    If includeHeaders Then output.WriteLine("RUN_DATE,DOMIN,GROUP,FULLNAME,ACCOUNT,TYPE,DISABLED")
    For Each objUser In objGroup.Members
        output.WriteLine _
            uCase(strDomain) & "," & _
            objGroup.Name & "," & _
            objUser.FullName & "," & _
            objUser.Name & "," & _
            objUser.Class & "," & _
            objUser.AccountDisabled

    Next
    Wscript.Sleep 100
On Error Goto 0

'Call dispose and we are done
Dispose
Wscript.Echo
Wscript.Stdout.WriteLine("Finished.")

' Dispose sub to clean up Each time the script exits
Sub Dispose()
    If outputToFile = true Then output.Close
    Set output = Nothing
    Set objFSO = Nothing
    Set objUser = Nothing
    Set objGroup = Nothing
End Sub