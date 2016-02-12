Const CSIDL_COMMON_PROGRAMS = &H17
Const CSIDL_START_MENU = &Hb&
Const CSIDL_PROGRAMS = &H2 

Set objShell = CreateObject("Shell.Application") 
Set objStartMenuFolder = objShell.NameSpace(CSIDL_START_MENU) 
strAllUsersProgramsPath = objStartMenuFolder.Self.Path 
Set objFolder = objShell.Namespace(strAllUsersProgramsPath & "\DeiCon") 
Set objFolderItem = objFolder.ParseName("TilesDavis.lnk") 
Set colVerbs = objFolderItem.Verbs 
For Each objVerb in colVerbs 
    If Replace(objVerb.name, "&", "") = "Pin to Start Menu" Then objVerb.DoIt 
Next