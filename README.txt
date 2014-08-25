ZIP
===

DevOps tools to assist with the automation of compressing files and folders

VBScript:
  Designed to be called from a batch or commandline using cscript.
  
  Syntax
    zipFiles.vbs source zipfilename overwrite[TRUE|FALSE] deletesource[TRUE|FALSE]
    
  Exampel 
    cscript zipFiles.vbs "c:\my documents\letters\*.doc" letters.zip FALSE TRUE
    cscript zipFiles.vbs "c:\my documents\letters\master.doc" "c:\temp\master.zip" TRUE FALSE
    cscript zipFiles.vbs c:\data\accounts accounts.zip FALSE FALSE