!include LogicLib.nsh
!include MUI2.nsh
!include WinVer.nsh
!include x64.nsh

!cd "..\BuildRelease"

!define MUI_COMPONENTSPAGE_SMALLDESC
!define MUI_ABORTWARNING
!define MUI_FINISHPAGE_SHOWREADME_NOTCHECKED
!define MUI_WELCOMEFINISHPAGE_BITMAP "..\TombEditor\Resources\misc\misc_InstallerSplash.bmp"
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "..\TombEditor\Resources\misc\misc_InstallerSplash.bmp" 
!define MUI_ICON "..\TombEditor\Resources\Misc\tomb_editor.ico"
!define MUI_FINISHPAGE_SHOWREADME "Changes.txt"

!getdllversion "TombEditor.exe" Version_

!define MUI_WELCOMEPAGE_TEXT \
"You are ready to install Tomb Editor ${Version_1}.${Version_2}.${Version_3}. $\r$\n\
$\r$\n\
Please make sure your system complies with following system requirements: $\r$\n\
$\r$\n\
  ${U+2022} Windows 7 or later (preferably 64-bit) $\r$\n\
  ${U+2022} Installed .NET Framework 4.5 $\r$\n\
  ${U+2022} Videocard with DirectX 10 support $\r$\n\
  ${U+2022} At least 2 gigabytes of RAM $\r$\n\
$\r$\n\
Enjoy! $\r$\n\
Tomb Editor dev team."

;--------------------------------

SetCompressor lzma
Unicode true
Name "Tomb Editor"
OutFile "TombEditorInstall.exe"
InstallDir "$PROGRAMFILES\Tomb Editor"
RequestExecutionLevel user

;--------------------------------

InstType "Standard"
InstType "Include stock assets"
InstType "Include stock assets and TRNG runtimes"

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "English"

;--------------------------------

Section "Tomb Editor" Section1

  SectionIn RO ; Always install this section
  
  SetOutPath $INSTDIR
  File /r \
  /x "Game" \
  /x "Resources" \
  /x "TombEditorLog*.txt" \
  /x "WadToolLog*.txt" \
  /x "ScriptEditorLog*.txt" \
  /x "*.prj2" \
  /x "*.pdb" \
  /x "*.so" \
  /x "install_script.nsi" \
  /x "TombEditorInstall.exe" \
  /x "TombEditorConfiguration.xml" \
  /x "WadToolConfiguration.xml" \
  *.*
  
  ; Add readme from installer folder
  File "..\Installer\Changes.txt"
  
  ; Choose 32-bit or 64-bit d3dcompiler dll based on system version
  ${If} ${RunningX64}
      Rename "$INSTDIR\Native\64 bit\d3dcompiler_43.dll" "$INSTDIR\d3dcompiler_43.dll"
  ${Else}
      Rename "$INSTDIR\Native\32 bit\d3dcompiler_43.dll" "$INSTDIR\d3dcompiler_43.dll"
  ${EndIf}
  
  RMDir /r "$INSTDIR\Native"
  
  ; Write the uninstall keys
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TombEditor" "DisplayName" "Tomb Editor"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TombEditor" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TombEditor" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TombEditor" "NoRepair" 1
  
  ; Write uninstaller itself
  WriteUninstaller "uninstall.exe"
  
SectionEnd

Section "Stock game assets" Section2

  SectionIn 2 3
  
  SetOutPath $INSTDIR
  File /r "Resources"
  
SectionEnd

Section "TRNG Runtimes" Section3

  SectionIn 3
  
  SetOutPath $INSTDIR
  File /r "Game"  
  
SectionEnd

Section "Start Menu Shortcuts" Section4

  SectionIn 1 2 3

  CreateDirectory "$SMPROGRAMS\Tomb Editor"
  CreateShortcut "$SMPROGRAMS\Tomb Editor\Tomb Editor.lnk" "$INSTDIR\TombEditor.exe" "" "$INSTDIR\TombEditor.exe" 0
  CreateShortcut "$SMPROGRAMS\Tomb Editor\Wad Tool.lnk" "$INSTDIR\WadTool.exe" "" "$INSTDIR\WadTool.exe" 0
  CreateShortcut "$SMPROGRAMS\Tomb Editor\Script Editor.lnk" "$INSTDIR\ScriptEditor.exe" "" "$INSTDIR\ScriptEditor.exe" 0
  
  CreateShortcut "$SMPROGRAMS\Tomb Editor\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  
SectionEnd

Section "Desktop Shortcut" Section5

  SectionIn 1 2 3
  CreateShortcut "$DESKTOP\Tomb Editor.lnk" "$INSTDIR\TombEditor.exe" "" "$INSTDIR\TombEditor.exe" 0  

SectionEnd

LangString DESC_Section1 ${LANG_ENGLISH} "Basic Tomb Editor components. This includes WadTool and ScriptEditor test versions."
LangString DESC_Section2 ${LANG_ENGLISH} "Stock assets, such as textures, font/sky graphics, sounds and default TRLE wads."
LangString DESC_Section3 ${LANG_ENGLISH} "Runtime TRNG game components are needed if you have no existing TRNG installation."
LangString DESC_Section4 ${LANG_ENGLISH} "Shortcuts for Tomb Editor in Start Menu."
LangString DESC_Section5 ${LANG_ENGLISH} "Shortcut for Tomb Editor on Desktop."

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${Section1} $(DESC_Section1)
  !insertmacro MUI_DESCRIPTION_TEXT ${Section2} $(DESC_Section2)
  !insertmacro MUI_DESCRIPTION_TEXT ${Section3} $(DESC_Section3)
  !insertmacro MUI_DESCRIPTION_TEXT ${Section4} $(DESC_Section4)
  !insertmacro MUI_DESCRIPTION_TEXT ${Section5} $(DESC_Section5)
!insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Autogenerated by Unlist script from barebones BuildRelease folder
  ; https://nsis.sourceforge.io/mediawiki/images/9/9f/Unlist.zip
  ; PLEASE UPDATE THIS BLOCK IF BUILD FILE SET IS CHANGED!

  Delete "$INSTDIR\Rendering\DirectX11\TextShaderVS.cso"
  Delete "$INSTDIR\Rendering\DirectX11\TextShaderPS.cso"
  Delete "$INSTDIR\Rendering\DirectX11\TestShaderVS.cso"
  Delete "$INSTDIR\Rendering\DirectX11\TestShaderPS.cso"
  Delete "$INSTDIR\Rendering\DirectX11\SpriteShaderVS.cso"
  Delete "$INSTDIR\Rendering\DirectX11\SpriteShaderPS.cso"
  Delete "$INSTDIR\Rendering\DirectX11\RoomShaderVS.cso"
  Delete "$INSTDIR\Rendering\DirectX11\RoomShaderPS.cso"
  Delete "$INSTDIR\Editor\Shaders\StaticModel.fx"
  Delete "$INSTDIR\Editor\Shaders\Solid.fx"
  Delete "$INSTDIR\Editor\Shaders\RoomGeometry.fx"
  Delete "$INSTDIR\Editor\Shaders\Model.fx"
  Delete "$INSTDIR\Catalogs\TrCatalog.xml"
  Delete "$INSTDIR\Catalogs\NgCatalog.xml"
  Delete "$INSTDIR\WadTool.exe.config"
  Delete "$INSTDIR\WadTool.exe"
  Delete "$INSTDIR\TombLib.Rendering.dll"
  Delete "$INSTDIR\TombLib.Forms.dll"
  Delete "$INSTDIR\TombLib.dll"
  Delete "$INSTDIR\TombEditor.exe.config"
  Delete "$INSTDIR\TombEditor.exe"
  Delete "$INSTDIR\System.Numerics.Vectors.dll"
  Delete "$INSTDIR\System.Drawing.PSD.dll"
  Delete "$INSTDIR\SharpDX.Toolkit.Graphics.dll"
  Delete "$INSTDIR\SharpDX.Toolkit.dll"
  Delete "$INSTDIR\SharpDX.Toolkit.Compiler.dll"
  Delete "$INSTDIR\SharpDX.DXGI.dll"
  Delete "$INSTDIR\SharpDX.dll"
  Delete "$INSTDIR\SharpDX.Direct3D11.Effects.dll"
  Delete "$INSTDIR\SharpDX.Direct3D11.dll"
  Delete "$INSTDIR\SharpDX.D3DCompiler.dll"
  Delete "$INSTDIR\ScriptTemplate.txt"
  Delete "$INSTDIR\ScriptEditor.exe.config"
  Delete "$INSTDIR\ScriptEditor.exe"
  Delete "$INSTDIR\Pfim.dll"
  Delete "$INSTDIR\NVorbis.dll"
  Delete "$INSTDIR\NLog.dll"
  Delete "$INSTDIR\NAudio.Vorbis.dll"
  Delete "$INSTDIR\NAudio.Flac.dll"
  Delete "$INSTDIR\NAudio.dll"
  Delete "$INSTDIR\MiniZ64.dll"
  Delete "$INSTDIR\MiniZ32.dll"
  Delete "$INSTDIR\MiniZ.Net.dll"
  Delete "$INSTDIR\FastColoredTextBox.xml"
  Delete "$INSTDIR\FastColoredTextBox.dll"
  Delete "$INSTDIR\DarkUI.dll"
  Delete "$INSTDIR\CH.SipHash.dll"
  Delete "$INSTDIR\AssimpNet.dll"
  Delete "$INSTDIR\Assimp64.dll"
  Delete "$INSTDIR\Assimp32.dll"
  
  RMDir "$INSTDIR\Rendering\DirectX11"
  RMDir "$INSTDIR\Editor\Shaders"
  RMDir "$INSTDIR\Rendering"
  RMDir "$INSTDIR\Editor"
  RMDir "$INSTDIR\Catalogs"
  
  ; End of autogenerated Unlist block.
  
  ; Remove extra stuff if installed
  RMDir /r "$INSTDIR\Game"
  RMDir /r "$INSTDIR\Resources"

  ; Remove logs
  Delete "$INSTDIR\TombEditorLog.txt"
  Delete "$INSTDIR\TombEditorLog*.txt"
  Delete "$INSTDIR\WadToolLog.txt"
  Delete "$INSTDIR\WadToolLog*.txt"
  Delete "$INSTDIR\ScriptEditorLog.txt"
  Delete "$INSTDIR\ScriptEditorLog*.txt"
  
  ; Remove configs
  Delete "$INSTDIR\TombEditorConfiguration.xml"
  Delete "$INSTDIR\WadToolConfiguration.xml"

  ; Remove readme
  Delete "$INSTDIR\Changes.txt"
  
  ; Remove d3dcompiler.dll which was externally copied
  Delete "$INSTDIR\d3dcompiler_43.dll"
  
  ; Remove uninstaller
  Delete "$INSTDIR\uninstall.exe"
  
  ; Remove settings
  RMDir /r "$LOCALAPPDATA\TombEditor"
  RMDir /r "$LOCALAPPDATA\ScriptEditor"

  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TombEditor"
  
  ; Remove shortcuts, if any
  Delete "$DESKTOP\Tomb Editor.lnk"
  Delete "$SMPROGRAMS\Tomb Editor\*.*"
  RMDir "$SMPROGRAMS\Tomb Editor"

  ; Only remove program dir if it's empty
   SetOutPath $TEMP
   Push $INSTDIR
   Call un.isEmptyDir
   Pop $0
   StrCmp $0 1 0 +2
     RMDir /r $INSTDIR
   StrCmp $0 0 0 +2
     MessageBox MB_OK \
     "Installation folder contains extra files. $\r$\n\
     Check if these files are important and remove folder manually."
     
SectionEnd

;--------------------------------

; Helper functions

Function .onInit
  ${IfNot} ${AtLeastWin7}
    MessageBox MB_OK \
    "At least Windows 7 is required to use Tomb Editor. $\r$\n\
    The installer will now quit."
    Quit
  ${EndIf}
FunctionEnd

Function un.isEmptyDir
  Exch $0
  Push $1
  FindFirst $0 $1 "$0\*.*"
  strcmp $1 "." 0 _notempty
    FindNext $0 $1
    strcmp $1 ".." 0 _notempty
      ClearErrors
      FindNext $0 $1
      IfErrors 0 _notempty
        FindClose $0
        Pop $1
        StrCpy $0 1
        Exch $0
        goto _end
     _notempty:
       FindClose $0
       ClearErrors
       Pop $1
       StrCpy $0 0
       Exch $0
  _end:
FunctionEnd