#ifndef AppVersion
  #define AppVersion "0.1.17"
#endif
#ifndef PayloadDir
  #define PayloadDir "..\dist\AdminConsolePortable-v" + AppVersion
#endif

[Setup]
AppId={{AE37B230-0834-4934-8793-10CFCA1BD103}
AppName=AdminConsole
AppVersion={#AppVersion}
AppPublisher=AlexSoft LTD
AppPublisherURL=https://github.com/AlexSoftLTD/AdminConsole
DefaultDirName={localappdata}\Programs\AdminConsole
DisableDirPage=no
PrivilegesRequired=lowest
MinVersion=6.2
OutputDir=..\downloads
OutputBaseFilename=AdminConsole-Setup-v{#AppVersion}
SetupIconFile=..\src\assets\AdminConsole.ico
UninstallDisplayIcon={app}\Start Admin Console.exe
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ShowLanguageDialog=yes
CloseApplications=yes
RestartApplications=no
UninstallDisplayName=AdminConsole

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"; LicenseFile: "..\licenses\LICENSE.en.txt"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"; LicenseFile: "..\licenses\LICENSE.ru.txt"

[CustomMessages]
english.LaunchApp=Launch AdminConsole after installation
russian.LaunchApp=Запустить программу после установки
english.DesktopShortcut=Create a &desktop shortcut
russian.DesktopShortcut=Создать ярлык на &рабочем столе
english.StartShortcut=Create a &Start menu shortcut
russian.StartShortcut=Создать ярлык в меню «&Пуск»
english.TaskbarHelp=Open settings after setup to pin to the &taskbar manually
russian.TaskbarHelp=Открыть настройки после установки для ручного закрепления на &панели задач
english.TaskbarMessage=Right-click the AdminConsole taskbar icon and choose Pin to taskbar. If you moved the application, unpin the old icon first.
russian.TaskbarMessage=Нажмите правой кнопкой мыши по значку AdminConsole на панели задач и выберите «Закрепить на панели задач». Если папка приложения изменилась, сначала открепите старый значок.
english.SaveError=Could not save the selected language. Check that the installation folder is writable.
russian.SaveError=Не удалось сохранить выбранный язык. Проверьте доступность папки установки для записи.

[Tasks]
Name: "desktopicon"; Description: "{cm:DesktopShortcut}"
Name: "startmenuicon"; Description: "{cm:StartShortcut}"
Name: "taskbarhelp"; Description: "{cm:TaskbarHelp}"; Flags: unchecked

Name: "launchapp"; Description: "{cm:LaunchApp}"; Flags: unchecked

[Files]
Source: "{#PayloadDir}\Start Admin Console.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#PayloadDir}\Admin Console APP.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\README-installed.txt"; DestDir: "{app}"; DestName: "README.txt"; Flags: ignoreversion

Source: "..\LICENSE.txt"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{userdesktop}\AdminConsole"; Filename: "{app}\Start Admin Console.exe"; WorkingDir: "{app}"; Tasks: desktopicon
Name: "{userprograms}\AdminConsole"; Filename: "{app}\Start Admin Console.exe"; WorkingDir: "{app}"; Tasks: startmenuicon

[Run]
Filename: "{app}\Start Admin Console.exe"; Parameters: "--settings"; WorkingDir: "{app}"; Tasks: taskbarhelp; Flags: nowait skipifsilent
Filename: "{app}\Start Admin Console.exe"; WorkingDir: "{app}"; Tasks: launchapp and not taskbarhelp; Flags: nowait skipifsilent

[UninstallDelete]
Type: files; Name: "{app}\AdminConsole.settings"

[Code]
procedure CurStepChanged(CurStep: TSetupStep);
var
  SettingsPath, Mode, Language: String;
  Lines: TArrayOfString;
begin
  if CurStep = ssPostInstall then begin
    SettingsPath := ExpandConstant('{app}\AdminConsole.settings');
    Mode := 'ask';
    if LoadStringsFromFile(SettingsPath, Lines) then
      if GetArrayLength(Lines) > 0 then
        if (Lines[0] = 'ask') or (Lines[0] = 'powershell') or
           (Lines[0] = 'powershell-admin') or (Lines[0] = 'cmd') or
           (Lines[0] = 'cmd-admin') then Mode := Lines[0];
    Language := 'en';
    if ActiveLanguage = 'russian' then Language := 'ru';
    if not SaveStringToFile(SettingsPath, Mode + #13#10 + Language + #13#10, False) then
      RaiseException(CustomMessage('SaveError'));
    if WizardIsTaskSelected('taskbarhelp') and not WizardSilent then
      MsgBox(CustomMessage('TaskbarMessage'), mbInformation, MB_OK);
  end;
end;
