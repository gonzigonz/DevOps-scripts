'*  Script name:    getSharePermissions.vbs
'*  Created on:     01/09/2014
'*  Author:         Gonzalo Lucero (https://github.com/gonzigonz)
'*  License:        The MIT License (MIT) Copyright (c) 2014 Gonzigonz
'*  Purpose:        Retrives a csv list of every ACL for every share found
'*                  on the local computer
'*  History:        firstname surname date
'*                  Modified to reflect new area code....etc

' Using
Set fso = CreateObject("Scripting.FileSystemObject")
Set wmi = GetObject("winmgmts:\\localhost\root\cimv2") 
Set objArgs = WScript.Arguments

' Locals
Const ForAppending = 2

' Construct Arguments
outputToFile = true
outputFilename = "sharePermissions.csv"

' Contruct Output
If outputToFile Then
    strFile = fso.GetAbsolutePathName(outputFilename)
    Set output = fso.OpenTextFile(strFile, ForAppending, True)
Else
    Set output = Wscript.Stdout
End If

' //Main
Wscript.Stdout.WriteLine("Reterving ACL for local shares...")
Wscript.Stdout.WriteLine
output.WriteLine("NAME,PATH,OWNER,USER_OR_GROUP,TYPE,ACCESS,INHERITANCE,APPLIES_TO")
For Each share In GetLocalShares()
    Wscript.Stdout.WriteLine("- " & share.name)
    For Each aclLine in GetACLDetails(share)
        output.WriteLine(aclLine)
        
    Next
    WScript.Sleep 100
Next
Wscript.Echo
Wscript.Stdout.WriteLine("Finished.")


' //Funcs
Function GetACLDetails(share)

    If share.Path = "" Then
        'Return empty array as no path exists
        GetACLDetails = Array()

    Else
        Set objFile = wmi.Get("Win32_LogicalFileSecuritySetting='" & share.Path & "'")
        If objFile.GetSecurityDescriptor(sd) = 0 Then

            owner = sd.Owner.Domain & "\" & sd.Owner.Name

            'Create an array to save all ACL details
            Dim arrACLDetails()
            Redim arrACLDetails(UBound(sd.DACL))
            index = 0

            'Loop throught each ACE
            For Each objAce in sd.DACL
                
                'Get User or Group to whom this ACL applies
                userOrGroup = objAce.Trustee.Domain & "\" & objAce.Trustee.Name

                'Get the ACL Type
                If objAce.AceType = 0 Then
                    aceType = "Allowed"
                Else
                    aceType = "Denied"
                End If

                'Get weather this ACL is inherited or not
                If objAce.AceFlags AND 16 Then
                    inheritance =  "Inherited"
                Else 
                    inheritance = "Not Inherited"
                End If

                'Get whom this ACL will apply to
                files = false
                subfolders = false
                thisfolder = true
                If objAce.AceFlags AND 1 Then files = true
                If objAce.AceFlags AND 2 Then subfolders = true
                If objAce.AceFlags AND 4 Then subfolders = true
                If objAce.AceFlags AND 8 Then thisfolder = false

                If thisfolder Then 
                    If subfolders Then
                        If files Then
                            appliesTo = "This folder; subfolders and files" 
                        Else                 
                            appliesTo = "This folder and subfolders"
                        End If               
                    Else                     
                        If files Then        
                            appliesTo = "This folder and files"
                        Else                 
                            appliesTo = "This folder only"
                        End If               
                    End If                   
                Else                         
                    If subfolders Then       
                        If files Then        
                            appliesTo = "Subfolders and files only" 
                        Else                 
                            appliesTo = "Subfolders only"
                        End If              
                    Else                    
                        If files Then       
                            appliesTo = "Files only"
                        End If
                    End If
                End If

                'Get the access or permission roles
                Select Case objAce.AccessMask
                    Case 2032127 access = "Full Control"
                    Case 1245631 access = "Modify"
                    Case 1180095 access = "Read, Write & Execute"
                    Case 1180063 access = "Read, Write"
                    Case 1179817 If files Then access = "Read & Execute" Else access = "List Folder Content"
                    Case 1179785 access = "Read Only"
                    Case Else access = "Special"
                End Select

                'Save each of the ACLs details as single entry to the array
                arrACLDetails(index) = _
                    share.name & "," & _ 
                    share.path & "," & _ 
                    owner & "," & _ 
                    userOrGroup & "," & _
                    aceType & "," & _
                    access & "," & _
                    inheritance & "," & _
                    appliesTo

                'DisplayAccess(objAce)

                index = index + 1

            Next

            'Return the array
            GetACLDetails = arrACLDetails

        End If
    End If
End Function

Function GetLocalShares
    Set GetLocalShares = wmi.ExecQuery("Select * from win32_share")
End Function

' //Subs

' Dispose sub to clean up Each time the script exits
Sub Dispose()
    If outputToFile = true Then output.Close
End Sub