'*  Script name:   checkDiskSpace.vbs
'*  Created on:    26/08/2014
'*  Author:        Gonzalo Lucero (https://github.com/gonzigonz)
'*  License:       The MIT License (MIT) Copyright (c) 2014 Gonzigonz
'*  Purpose:       Check free disk space on windows machines.
'*                 Alerts if space less than defined threshold
'*  History:       firstname surname date
'*                 Modified to reflect new area code....etc

' Using
Set fso = CreateObject("Scripting.FileSystemObject")
Set objArgs = WScript.Arguments

Dim driveSpec 'e.g. c:
Dim threshold 'e.g. 54 (54%)

' Construct Arguments
driveSpec = objArgs(0)
threshold = objArgs(1)

' Check free disk space
WScript.Echo 
Wscript.Echo "Checking disk space on """ & driveSpec & """ drive..."
WScript.Echo 
WScript.Echo "  Threshold set to " & FormatPercent(threshold / 100)
WScript.Echo 

With fso.GetDrive(driveSpec)

    precentAvailable = .AvailableSpace / .TotalSize
    precentFree = .FreeSpace / .TotalSize

    WScript.Echo "  Volume Name:       " & .VolumeName
    WScript.Echo "  Total Size:        " & FormatGB(.TotalSize)
    WScript.Echo "  Available Space:   " & FormatGB(.AvailableSpace) & " - " & FormatPercent(precentAvailable)
    WScript.Echo "  Free Space:        " & FormatGB(.FreeSpace) & " - " & FormatPercent(precentFree)
    WScript.Echo 

    If (.AvailableSpace / .TotalSize) < (threshold / 100) Then
        WScript.Echo "Low Diskspace Alert! - Less then " & FormatPercent(threshold / 100)
	Else
		WScript.Echo "Disk Space OK."
    End If

End With

''' FUNCTIONS '''

Function FormatGB(size)
  FormatGB = FormatNumber(size / (1024 * 1024 * 1024), 2,0,0,0) & " GB"
End Function

Function FormatPercent(decimal)
    FormatPercent = FormatNumber(decimal * 100, 1,0,0,0) & "%"
End Function