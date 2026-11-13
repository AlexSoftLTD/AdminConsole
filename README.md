# AdminConsole

[Русский](#русский) | [English](#english)

## Русский

Документация доступна на двух языках. Язык интерфейса можно выбрать в верхнем правом углу: Русский / English.

Небольшое приложение для запуска командной строки (CMD) и Windows PowerShell от текущего пользователя или от имени администратора. Четыре варианта запуска также доступны через меню значка на панели задач.

## Варианты скачивания

В [последнем релизе](https://github.com/AlexSoftLTD/AdminConsole/releases/latest) откройте **Assets** и выберите:

| Вариант | Файл | Как использовать |
| --- | --- | --- |
| **Установщик** | `AdminConsole-Setup-v0.1.17.exe` | Выбор языка, папки и ярлыков. Удаление через настройки Windows. |
| **Portable** | `AdminConsolePortable-v0.1.17.zip` | Распаковать папку и запустить без установки. |
| **Исходники** | **Source code (zip / tar.gz)** | Код проекта для изучения и самостоятельной сборки. |

Установщик работает для текущего пользователя. Выбранный в мастере язык становится языком приложения; режим запуска по умолчанию сохраняется при повторной установке. Ярлыки рабочего стола и меню «Пуск» выбираются отдельно. Для панели задач можно включить открытие настроек после установки и закрепить значок вручную. Деинсталлятор `unins000.exe` создаётся автоматически в папке установки.

Название **AdminConsole** одинаково для обоих языков; текущая версия видна в заголовке окна. После обновления открепите прежний значок панели задач и закрепите приложение заново.

## 1. Установка и использование

### Скачать и запустить

1. Откройте [Releases](https://github.com/AlexSoftLTD/AdminConsole/releases/latest). На главной странице репозитория ссылка **Releases** находится справа.
2. В разделе **Assets** скачайте **AdminConsolePortable-v0.1.17.zip**. Пункты **Source code** содержат исходники для разработчиков.
3. Распакуйте архив целиком в удобную постоянную папку. Установщик не требуется.
4. Запустите **Start Admin Console.exe** и выберите нужный вариант запуска.

Оба EXE должны оставаться рядом:

| Файл | Назначение |
| --- | --- |
| **Start Admin Console.exe** | Рекомендуемая точка запуска: проверяет необходимый .NET Framework и запускает основное приложение. |
| **Admin Console APP.exe** | Само приложение. Можно запускать напрямую при исправном .NET Framework, пропуская проверку стартера. |

Для запуска от имени администратора Windows покажет запрос UAC и при необходимости запросит учётные данные администратора.

### Выбрать запуск по умолчанию

В поле **«При запуске ярлыка»** выберите один из четырёх режимов и нажмите **«Сохранить»**. Со следующего обычного запуска приложение сразу откроет выбранную оболочку. При закрытии окна с несохранённым выбором появится предложение сохранить изменения.

Чтобы вернуться к настройкам, используйте любой способ:

- Удерживайте **Shift** при запуске до появления окна.
- Выберите **«Настройки»** в меню правой кнопки мыши по значку приложения на панели задач.
- Запустите **Start Admin Console.exe** с параметром `--settings`.
- Запустите **Create Shortcuts.cmd**: после обновления ярлыков он откроет окно настроек.

Для отключения автоматического запуска выберите **«Показывать окно выбора»** и сохраните. Четыре команды меню панели задач запускают выбранную команду независимо от режима по умолчанию.

Предпочтение хранится в файле **AdminConsole.settings** рядом с EXE. Папка должна быть доступна для записи. Удаление файла сбрасывает настройку.

### Требования

Настольная Windows 8 или новее (x86/x64), .NET Framework 4.5 или новее. Windows RT не поддерживается. EXE не подписаны цифровой подписью. Работа на чистой Windows 8 пока не проверена.

## 2. Создание и обновление ярлыков

Сначала разместите приложение в постоянной папке, затем запустите **Create Shortcuts.cmd** двойным щелчком.

### Рабочий стол и меню «Пуск»

Скрипт создаст **AdminConsole** на рабочем столе и в списке приложений меню **«Пуск»** текущего пользователя. Если ярлыки с этим именем уже есть, обновит их путь, значок и параметры запуска. Для закрепления в избранном меню «Пуск» воспользуйтесь контекстным меню приложения вручную.

### Панель задач

1. После сообщения скрипта нажмите **ОК** — откроется окно приложения, даже если выбран запуск оболочки по умолчанию.
2. Нажмите правой кнопкой мыши по его значку на панели задач.
3. Выберите **«Закрепить на панели задач»**.

Автоматически скрипт на панели задач не закрепляет. После закрепления обычный щелчок использует сохранённый режим запуска; правый щелчок открывает четыре команды и пункт **«Настройки»**.

### После переноса или обновления

Закройте старое окно приложения. Если путь к папке изменился, снова запустите **Create Shortcuts.cmd**, чтобы обновить ярлыки рабочего стола и меню «Пуск». Старый значок панели задач открепите и закрепите окно из новой папки заново. Копии в разных папках имеют отдельные идентификаторы панели задач.

Если от ранних версий остались ярлыки со случайными буквами и цифрами в имени, удалите эти дубликаты вручную.

## 3. Удаление

**Установленная версия:** закройте приложение, открепите его значок от панели задач и избранного «Пуска», затем удалите **AdminConsole** через список установленных приложений Windows или запустите **unins000.exe** в папке установки. Деинсталлятор удалит установленные файлы, созданные им ярлыки и настройки; посторонние файлы останутся.

**Portable-версия:**

1. Закройте окно AdminConsole.
2. Нажмите правой кнопкой мыши по его закреплённому значку на панели задач и выберите **«Открепить от панели задач»**.
3. Удалите ярлык **AdminConsole** с рабочего стола.
4. Удалите ярлык из меню **«Пуск»**: нажмите **Win+R**, введите `shell:programs`, нажмите Enter и удалите **AdminConsole**. Если приложение закреплено в избранном «Пуска», открепите его отдельно.
5. Удалите распакованную папку приложения вместе с **AdminConsole.settings**, если он существует. Скачанный ZIP также можно удалить.

Деинсталлятор не нужен. Созданные ярлыки относятся к текущему пользователю; если приложение использовали другие пользователи, они удаляют свои ярлыки отдельно. Удалять системный .NET Framework или Windows PowerShell не требуется.

## Информация для разработчиков

Исходники находятся в `src`, проверки — в `tests`. В `downloads` размещены актуальный ZIP и контрольная сумма SHA-256. Условия кода и дистрибутива: собственная Freeware License, см. LICENSE.txt.

Для сборки нужны Windows, Windows PowerShell 5.1 или PowerShell 7, .NET Framework 4.x и Visual Studio Build Tools с компонентами **Desktop development with C++** и Windows SDK.

```powershell
.\build.ps1
.\test.ps1
```

Результат: `dist/AdminConsolePortable-v0.1.17.zip` и SHA-256 рядом. Исходный ICO нужен только для сборки и хранится в `src/assets`.


## English

A portable launcher for Command Prompt (CMD) and Windows PowerShell, normally or as administrator. All four modes are also available from the taskbar icon's context menu. Choose Русский / English in the upper-right corner to switch the interface language.

### Download options

Open **Assets** in the [latest release](https://github.com/AlexSoftLTD/AdminConsole/releases/latest):

| Option | File | Use |
| --- | --- | --- |
| **Installer** | `AdminConsole-Setup-v0.1.17.exe` | Choose language, location and shortcuts; uninstall through Windows Settings. |
| **Portable** | `AdminConsolePortable-v0.1.17.zip` | Extract and run without installation. |
| **Source code** | **Source code (zip / tar.gz)** | Read the code or build it yourself. |

Setup installs for the current user. Its language selection becomes the application language; reinstalling preserves the default launch mode. Desktop and Start menu shortcuts are optional. The taskbar option opens settings after setup for manual pinning. Setup creates `unins000.exe` in the installation folder.

The **AdminConsole** name is the same in both languages; the current version appears in the window title. After upgrading, unpin the old taskbar icon and pin the application again.

### 1. Setup and use

#### Download and launch

1. Open [Releases](https://github.com/AlexSoftLTD/AdminConsole/releases/latest), also linked on the right of the repository home page.
2. Under **Assets**, download **AdminConsolePortable-v0.1.17.zip**. The **Source code** items are for developers.
3. Extract the entire ZIP into a permanent folder. No installer is needed.
4. Run **Start Admin Console.exe** and choose a launch mode.

Keep both EXEs together:

| File | Purpose |
| --- | --- |
| **Start Admin Console.exe** | Recommended entry point: checks .NET Framework and starts the application. |
| **Admin Console APP.exe** | The application itself; direct launch skips the runtime check. |

Administrator mode requires UAC approval and credentials when requested.

#### Default launch and settings

Choose a mode under **«При запуске ярлыка» (When launching from a shortcut)** and click **«Сохранить» (Save)**. The next normal launch opens the selected shell directly. Closing with unsaved changes prompts you to save.

To reopen settings:

- Hold **Shift** while launching until the window appears.
- Choose **«Настройки» (Settings)** from the taskbar icon's context menu.
- Run **Start Admin Console.exe** with `--settings`.
- Run **Create Shortcuts.cmd** to update shortcuts and open settings.

To disable automatic launch, choose **«Показывать окно выбора» (Show the selection window)** and save. The four taskbar launch commands work independently of this preference.

Preferences are stored in **AdminConsole.settings** next to the EXEs. The folder must be writable. Deleting this file resets the preference.

#### Requirements

Desktop Windows 8 or later (x86/x64), .NET Framework 4.5 or later. Windows RT is unsupported. EXEs are unsigned. A clean Windows 8 installation has not been tested.

### 2. Create and update shortcuts

Place the application in a permanent folder and double-click **Create Shortcuts.cmd**.

#### Desktop and Start menu

The script creates **AdminConsole** on the current user's desktop and in the Start menu app list. Existing shortcuts with this name have their target, icon, and launch arguments updated. Pinning to the Start menu's pinned area is a manual context-menu action.

#### Taskbar

1. Click **OK** in the script's message. The application window opens even if a default shell launch is configured.
2. Right-click the running application's taskbar icon.
3. Select **Pin to taskbar**.

Pinning is manual. A normal click on the pinned icon uses the saved launch preference; right-clicking shows four launch commands and **«Настройки» (Settings)**.

#### Moving or updating

Close the old application window. After moving the folder, run **Create Shortcuts.cmd** again to update desktop and Start menu shortcuts. Unpin the old taskbar icon and pin the window from the new folder. Copies in different folders have separate taskbar identities.

Manually remove any duplicate shortcuts from early versions whose names contain random letters and digits.

### 3. Remove

**Installed version:** close the app, remove its taskbar and Start pins, then uninstall **AdminConsole** through Windows Settings or run **unins000.exe** in the installation folder. Uninstall removes installed files, shortcuts created by Setup, and preferences; unrelated files are preserved.

**Portable version:**

1. Close AdminConsole.
2. Right-click its pinned taskbar icon and select **Unpin from taskbar**.
3. Delete the **AdminConsole** desktop shortcut.
4. Press **Win+R**, enter `shell:programs`, and delete the **AdminConsole** shortcut. Unpin it from the Start menu's pinned area separately if needed.
5. Delete the extracted application folder, including **AdminConsole.settings** if present. You can also delete the downloaded ZIP.

No uninstaller is needed. Shortcuts belong to the current user; other users remove their own shortcuts separately. You do not need to remove the system's .NET Framework or Windows PowerShell.

### Developer information

Sources are in `src`, checks are in `tests`, and the current ZIP and SHA-256 checksum are in `downloads`. Code and distributions use the proprietary Freeware License in LICENSE.txt.

Building requires Windows, Windows PowerShell 5.1 or PowerShell 7, .NET Framework 4.x, and Visual Studio Build Tools with **Desktop development with C++** and the Windows SDK.

```powershell
.\build.ps1
.\test.ps1
```

Output: `dist/AdminConsolePortable-v0.1.17.zip` and its SHA-256 checksum. The source icon in `src/assets` is needed only for building.



Сборка установщика / Build the installer: install Inno Setup 6, then run `.\build-installer.ps1`. Installer checks: `.\tests\Test-Installer.ps1` (clean test account required).


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
