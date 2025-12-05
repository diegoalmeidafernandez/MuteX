; ============================================================
; MuteX - Installer Script (Inno Setup)
; Author: Diego Almeida
; ============================================================

[Setup]
AppName=MuteX
AppVersion=1.0.0
DefaultDirName={pf}\MuteX
DisableProgramGroupPage=yes
OutputDir=.
OutputBaseFilename=MuteXInstaller
Compression=lzma
SolidCompression=yes

; ============================================================
; FILES TO INSTALL
; ============================================================

[Files]
; Copiar TODO el contenido del publish win-x64
Source: "..\..\src\MuteX.App\bin\Release\net8.0-windows10.0.19041.0\win-x64\*"; \
    DestDir: "{app}"; \
    Flags: ignoreversion recursesubdirs createallsubdirs

; ============================================================
; SHORTCUTS
; ============================================================

[Icons]
; Acceso directo para INICIO AUTOMÁTICO
Name: "{commonstartup}\MuteX"; Filename: "{app}\MuteX.App.exe"

; Acceso directo en el ESCRITORIO
Name: "{commondesktop}\MuteX"; Filename: "{app}\MuteX.App.exe"

; Acceso directo en el menú de inicio
Name: "{group}\MuteX"; Filename: "{app}\MuteX.App.exe"

; ============================================================
; RUN AFTER INSTALL
; ============================================================

[Run]
Filename: "{app}\MuteX.App.exe"; Description: "Launch MuteX"; Flags: nowait postinstall
