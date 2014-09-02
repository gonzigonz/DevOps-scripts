:: getADGroupMembers.vbs domain adGroup outputToFile[TRUE|FALSE] outputFilename append[TRUE|FALSE]
cscript getADGroupMembers.vbs . administrators FALSE
:: cscript getADGroupMembers.vbs "yourdomain.local" "Group Name - 1" TRUE "Output_Filename.csv" FALSE
:: cscript getADGroupMembers.vbs "yourdomain.local" "Group Name - 2" TRUE "Output_Filename.csv" TRUE
:: cscript getADGroupMembers.vbs "yourdomain.local" "Group Name - 3" TRUE "Output_Filename.csv" TRUE
pause