AdminConsole Portable 0.1.17
РУССКИЙ / ENGLISH

========== РУССКИЙ ==========

Запуск CMD и Windows PowerShell от пользователя или администратора.
Язык интерфейса можно выбрать в верхнем правом углу: Русский / English.

1. УСТАНОВКА И ИСПОЛЬЗОВАНИЕ

Распакуйте архив целиком в удобную постоянную папку. Установка не нужна.
Запустите Start Admin Console.exe и выберите нужный режим.

Оба EXE должны оставаться рядом:
- Start Admin Console.exe — рекомендуемый запуск: проверяет .NET Framework,
  затем запускает приложение.
- Admin Console APP.exe — само приложение; можно запускать напрямую,
  пропуская проверку среды.

Требования: настольная Windows 8 или новее (x86/x64), .NET Framework 4.5+.
Windows RT не поддерживается. EXE не подписаны. Чистая Windows 8 не проверена.
Запуск от администратора требует подтверждения UAC и при необходимости пароля.

Запуск по умолчанию: выберите режим в поле «При запуске ярлыка» и нажмите
«Сохранить». Он действует со следующего обычного запуска. Если закрыть окно
с несохранённым выбором, приложение предложит сохранить изменения.

Вернуться к настройкам можно любым способом:
- удерживать Shift при запуске до появления окна;
- выбрать «Настройки» в меню значка на панели задач;
- запустить Start Admin Console.exe с параметром --settings;
- запустить Create Shortcuts.cmd: он обновит ярлыки и откроет настройки.

Чтобы отключить автоматический запуск, выберите «Показывать окно выбора»
и сохраните. Четыре команды меню панели задач работают независимо от этого выбора.
Настройка хранится рядом с EXE в AdminConsole.settings. Папка должна быть
доступна для записи. Удаление этого файла сбрасывает настройку.

2. СОЗДАНИЕ И ОБНОВЛЕНИЕ ЯРЛЫКОВ

Разместите приложение в постоянной папке и запустите Create Shortcuts.cmd.
Он создаст или обновит Admin Console на рабочем столе и в списке приложений
меню «Пуск» текущего пользователя, затем покажет сообщение. Нажмите ОК.
Откроется окно настроек независимо от выбранного запуска по умолчанию.

Панель задач: нажмите правой кнопкой по значку открытого приложения
и выберите «Закрепить на панели задач». Закрепление в избранном меню «Пуск»
также выполняется вручную через контекстное меню.

После переноса папки снова запустите Create Shortcuts.cmd. Старый значок
панели задач открепите и закрепите окно из новой папки заново.
Старые дубликаты ярлыков со случайными буквами и цифрами удалите вручную.

3. УДАЛЕНИЕ

1) Закройте AdminConsole.
2) Открепите его значок от панели задач и избранного меню «Пуск».
3) Удалите ярлык AdminConsole с рабочего стола.
4) Нажмите Win+R, введите shell:programs и удалите ярлык AdminConsole.
5) Удалите распакованную папку вместе с AdminConsole.settings, если он есть.
   Скачанный ZIP тоже можно удалить.

Деинсталлятор не нужен. Другие пользователи удаляют свои ярлыки отдельно.
Системные .NET Framework и Windows PowerShell удалять не требуется.

========== ENGLISH ==========

Launch CMD and Windows PowerShell normally or as administrator.
Choose Русский / English in the upper-right corner to switch the interface language.

1. SETUP AND USE

Extract the entire ZIP into a permanent folder. No installer is needed.
Run Start Admin Console.exe and choose a launch mode.

Keep both EXE files together:
- Start Admin Console.exe is the recommended entry point: it checks
  .NET Framework and then starts the application.
- Admin Console APP.exe is the application itself. You can run it directly
  to skip the runtime check.

Requirements: desktop Windows 8 or later (x86/x64), .NET Framework 4.5+.
Windows RT is unsupported. EXEs are unsigned. A clean Windows 8 is untested.
Administrator mode requires UAC approval and credentials when requested.

Default launch: choose a mode under «При запуске ярлыка» (When launching
from a shortcut) and click «Сохранить» (Save). It applies on the next normal
launch. Closing with unsaved changes prompts you to save.

To reopen settings, use any of these methods:
- hold Shift while launching until the window appears;
- choose «Настройки» (Settings) from the taskbar icon's context menu;
- run Start Admin Console.exe with the --settings argument;
- run Create Shortcuts.cmd to update shortcuts and open settings.

To disable automatic launch, choose «Показывать окно выбора» (Show the
selection window) and save. The four taskbar commands ignore the default mode.
Preferences are stored in AdminConsole.settings next to the EXEs. The folder
must be writable. Delete this file to reset the preference.

2. CREATE AND UPDATE SHORTCUTS

Place the application in a permanent folder and run Create Shortcuts.cmd.
It creates or updates AdminConsole shortcuts on the current user's desktop
and in the Start menu app list, then displays a message. Click OK.
The settings window opens even when a default shell launch is configured.

Taskbar: right-click the running application's icon and choose Pin to taskbar.
Pinning to the Start menu's pinned area is also a manual context-menu action.

After moving the folder, run Create Shortcuts.cmd again. Unpin the old taskbar
icon and pin the window from the new folder. Manually remove any old duplicate
shortcuts whose names contain random letters and digits.

3. REMOVE

1) Close AdminConsole.
2) Unpin its icon from the taskbar and the Start menu's pinned area.
3) Delete the AdminConsole desktop shortcut.
4) Press Win+R, enter shell:programs, and delete the Admin Console shortcut.
5) Delete the extracted folder, including AdminConsole.settings if present.
   You can also delete the downloaded ZIP.

No uninstaller is needed. Other users remove their own shortcuts separately.
You do not need to remove the system's .NET Framework or Windows PowerShell.

Project / Проект: https://github.com/AlexSoftLTD/AdminConsole


Лицензия / License: AdminConsole Freeware License — LICENSE.txt.
Правообладатель / Rights holder: AlexSoft LTD.
Бесплатное личное и коммерческое использование; бесплатное распространение полного
неизменённого дистрибутива разрешено. Модификация и перепродажа запрещены,
с оговорками, указанными в лицензии. Русский текст является основным.
Free personal and commercial use; free redistribution of the complete unmodified
distribution is allowed. Modification and resale are prohibited, subject to the
exceptions in the license. The Russian text governs.
О программе / About: кнопка в главном окне / button in the main window.
Лицензия доступна в окне «О программе» / Full license is available in About.
Обратная связь / Feedback (public; GitHub account required):
https://github.com/AlexSoftLTD/AdminConsole/issues
Новый значок нарисован с нуля / The new icon was drawn from scratch.
