using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Shell;

[assembly: System.Reflection.AssemblyVersion(AppInfo.Version)]
[assembly: System.Reflection.AssemblyFileVersion(AppInfo.Version)]
internal static class AppInfo {
    internal const string Name = "AdminConsole";
    internal const string Version = "0.1.17";
}

internal static class Program {
    [STAThread]
    private static void Main(string[] args) { StarterIntegration.SetCurrentProcessExplicitAppUserModelID(StarterIntegration.AppId);
        if (args.Length == 1 && args[0] == "--setup-menu") {
            ConfigureJumpList();
            return;
        }
        if (args.Length >= 2 && args[0] == "--jump") {
            LaunchFromJumpList(args);
            return;
        }
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        if (!DefaultLaunch.ShowSettings(args) && DefaultLaunch.TryLaunch()) return;
        var picker = new PickerForm();
        picker.Shown += delegate {
            try {
                Process.Start(new ProcessStartInfo(Assembly.GetExecutingAssembly().Location, "--setup-menu") {
                    UseShellExecute = false, CreateNoWindow = true
                });
            } catch { }
        };
        Application.Run(picker);
    }

    private static void ConfigureJumpList() {
        try {
            var list = new JumpList();
            list.ShowRecentCategory = false;
            list.ShowFrequentCategory = false;
            list.JumpItems.Add(CreateTask("PowerShell", Ui.Text("Обычный запуск"), "powershell", false));
            list.JumpItems.Add(CreateTask("PowerShell", Ui.Text("От администратора"), "powershell", true));
            list.JumpItems.Add(CreateTask(Ui.Text("Командная строка"), Ui.Text("Обычный запуск"), "cmd", false));
            list.JumpItems.Add(CreateTask(Ui.Text("Командная строка"), Ui.Text("От администратора"), "cmd", true));
            list.JumpItems.Add(new JumpTask {
                ApplicationPath = StarterIntegration.Path, Arguments = "--settings",
                Title = Ui.Text("Настройки"), CustomCategory = Ui.Text("Приложение"),
                IconResourcePath = StarterIntegration.Path, IconResourceIndex = 0
            });
            JumpList.SetJumpList(new System.Windows.Application { ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown }, list);
        } catch { }
    }

    private static JumpTask CreateTask(string category, string title, string shell, bool elevated) {
        string system = Environment.GetFolderPath(Environment.SpecialFolder.System);
        string exe = shell == "cmd" ? System.IO.Path.Combine(system, "cmd.exe") : System.IO.Path.Combine(system, "WindowsPowerShell\\v1.0\\powershell.exe");
        return new JumpTask {
            ApplicationPath = StarterIntegration.Path,
            Arguments = "--jump " + shell + (elevated ? " --admin" : ""),
            WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            IconResourcePath = exe,
            IconResourceIndex = 0,
            Title = title,
            CustomCategory = category
        };
    }

    private static void LaunchFromJumpList(string[] args) {
        try {
            string system = Environment.GetFolderPath(Environment.SpecialFolder.System);
            bool cmd = args[1] == "cmd";
            string exe = cmd ? System.IO.Path.Combine(system, "cmd.exe") : System.IO.Path.Combine(system, "WindowsPowerShell\\v1.0\\powershell.exe");
            var psi = ShellLaunch.Create(exe, cmd ? "/k" : "-NoLogo -NoExit", args.Length > 2 && args[2] == "--admin");
            Process.Start(psi);
        } catch (Win32Exception ex) {
            if (ex.NativeErrorCode != 1223) MessageBox.Show(Ui.Text("Не удалось запустить оболочку:\n") + ex.Message, Ui.Text("Ошибка запуска"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        } catch (Exception ex) {
            MessageBox.Show(Ui.Text("Не удалось запустить оболочку:\n") + ex.Message, Ui.Text("Ошибка запуска"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

internal static class DefaultLaunch {
    internal static readonly string[] Values = { "ask", "powershell", "powershell-admin", "cmd", "cmd-admin" };
    internal static string SettingsPath { get { return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AdminConsole.settings"); } }
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int key);
    internal static bool ShowSettings(string[] args) {
        return Array.IndexOf(args, "--settings") >= 0 || (GetAsyncKeyState(0x10) & 0x8000) != 0;
    }
    internal static int Read() {
        try { int index = Array.IndexOf(Values, System.IO.File.ReadAllLines(SettingsPath)[0].Trim()); return index < 0 ? 0 : index; }
        catch { return 0; }
    }
    internal static void Save(int index) {
        if (index < 0 || index >= Values.Length) throw new ArgumentOutOfRangeException("index");
        System.IO.File.WriteAllText(SettingsPath, Values[index] + Environment.NewLine + Ui.Language);
    }
    internal static ProcessStartInfo Create(int index) {
        if (index <= 0 || index >= Values.Length) throw new ArgumentOutOfRangeException("index");
        bool cmd = index >= 3;
        string system = Environment.GetFolderPath(Environment.SpecialFolder.System);
        string file = System.IO.Path.Combine(system, cmd ? "cmd.exe" : "WindowsPowerShell\\v1.0\\powershell.exe");
        return ShellLaunch.Create(file, cmd ? "/k" : "-NoLogo -NoExit", index == 2 || index == 4);
    }
    internal static bool TryLaunch() {
        int index = Read();
        if (index == 0) return false;
        try { Process.Start(Create(index)); return true; }
        catch (Win32Exception ex) {
            if (ex.NativeErrorCode != 1223) MessageBox.Show(Ui.Text("Не удалось выполнить запуск по умолчанию:\n") + ex.Message, AppInfo.Name);
        } catch (Exception ex) { MessageBox.Show(Ui.Text("Не удалось выполнить запуск по умолчанию:\n") + ex.Message, AppInfo.Name); }
        return false;
    }
}

internal static class ShellLaunch {
    internal static ProcessStartInfo Create(string file, string args, bool elevated) {
        var info = new ProcessStartInfo(file, args) {
            UseShellExecute = true,
            WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        };
        if (elevated) info.Verb = "runas";
        return info;
    }
}

internal sealed class PickerForm : DpiForm {
    private ComboBox defaultChoice;
    private int savedChoice;
    internal bool HasUnsavedChanges { get { return defaultChoice != null && defaultChoice.SelectedIndex != savedChoice; } }
    protected override void OnHandleCreated(EventArgs e) {
        base.OnHandleCreated(e);
        try { StarterIntegration.Attach(this); }
        catch (Exception ex) { MessageBox.Show(Ui.Text("Не удалось настроить закрепление на панели задач:\n") + ex.Message, AppInfo.Name); }
    }
    protected override void OnFormClosing(FormClosingEventArgs e) {
        if (e.CloseReason == CloseReason.UserClosing && HasUnsavedChanges) {
            var answer = MessageBox.Show(this, Ui.Text("Сохранить изменённый режим запуска по умолчанию?"), Ui.Text("Настройки"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (answer == DialogResult.Cancel) e.Cancel = true;
            else if (answer == DialogResult.Yes) {
                try { SaveChoice(); }
                catch (Exception ex) { e.Cancel = true; MessageBox.Show(this, Ui.Text("Не удалось сохранить настройку:\n") + ex.Message, Ui.Text("Ошибка сохранения")); }
            }
        }
        base.OnFormClosing(e);
    }
    internal void SaveChoice() {
        DefaultLaunch.Save(defaultChoice.SelectedIndex);
        savedChoice = defaultChoice.SelectedIndex;
    }
    private static readonly Color Card = Color.FromArgb(30, 39, 55);
    private static readonly Color Muted = Color.FromArgb(164, 180, 203);
    private static readonly Color Admin = Color.FromArgb(157, 102, 31);
    private static readonly Color AdminBorder = Color.FromArgb(242, 184, 75);

    internal PickerForm() {
        Text = AppInfo.Name + " v" + AppInfo.Version;
        ClientSize = new Size(570, 381);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(20, 26, 38);
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 9.5F);
        try { Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location); } catch { }

        Controls.Add(new Label {
            Text = AppInfo.Name,
            Font = new Font("Segoe UI Semibold", 16F),
            ForeColor = Color.FromArgb(238,243,255),
            Location = new Point(24, 17), Size = new Size(350, 32)
        });
        var language = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(400, 22), Size = new Size(146, 28), AccessibleName = "Language / Язык" };
        language.Items.AddRange(new object[] { "Русский", "English" });
        language.SelectedIndex = Ui.Language == "en" ? 1 : 0;
        Controls.Add(language);
        Controls.Add(new Label {
            Text = Ui.Text("Для запуска от имени администратора Windows покажет запрос UAC."),
            ForeColor = Muted, Location = new Point(26, 51), Size = new Size(520, 23)
        });

        string system = Environment.GetFolderPath(Environment.SpecialFolder.System);
        string powerShell = System.IO.Path.Combine(system, "WindowsPowerShell", "v1.0", "powershell.exe");
        string commandPrompt = System.IO.Path.Combine(system, "cmd.exe");
        AddShellCard(24, "PowerShell", powerShell, "-NoLogo -NoExit",
            Color.FromArgb(30, 48, 69), Color.FromArgb(46, 83, 119));
        AddShellCard(292, Ui.Text("Командная строка"), commandPrompt, "/k",
            Color.FromArgb(38, 40, 44), Color.FromArgb(62, 65, 70));
        Controls.Add(new Label { Text = Ui.Text("При запуске ярлыка:"), ForeColor = Muted,
            Location = new Point(24, 269), Size = new Size(520, 23) });
        var choice = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(24, 293), Size = new Size(350, 28) };
        choice.Items.AddRange(new object[] { Ui.Text("Показывать окно выбора"), Ui.Text("PowerShell — обычный запуск"),
            Ui.Text("PowerShell — от администратора"), Ui.Text("CMD — обычный запуск"), Ui.Text("CMD — от администратора") });
        choice.SelectedIndex = DefaultLaunch.Read();
        defaultChoice = choice;
        savedChoice = choice.SelectedIndex;
        Controls.Add(choice);
        var save = new Button { Text = Ui.Text("Сохранить"), Location = new Point(390, 291), Size = new Size(156, 32) };
        save.Click += delegate {
            try {
                SaveChoice();
                MessageBox.Show(this, Ui.Text("Настройка сохранена. Для возврата удерживайте Shift при запуске\nили выберите «Настройки» в меню значка на панели задач."), AppInfo.Name);
            } catch (Exception ex) { MessageBox.Show(this, Ui.Text("Не удалось сохранить настройку в папке приложения:\n") + ex.Message, Ui.Text("Ошибка сохранения")); }
        };
        Controls.Add(save);
        Controls.Add(new Label { Text = Ui.Text("Настройки: Shift при запуске или пункт\n«Настройки» в меню значка на панели задач."),
            ForeColor = Muted, Location = new Point(24, 333), Size = new Size(350, 38) });
        var about = new Button { Text = Ui.Language == "ru" ? "О программе" : "About", Location = new Point(390, 333), Size = new Size(156, 32), Name = "AboutButton" };
        about.Click += delegate { using (var dialog = new AboutForm()) dialog.ShowDialog(this); };
        Controls.Add(about);
        language.SelectedIndexChanged += delegate {
            if (Ui.Language == (language.SelectedIndex == 1 ? "en" : "ru")) return;
            string previous = Ui.Language;
            Ui.Language = language.SelectedIndex == 1 ? "en" : "ru";
            try { DefaultLaunch.Save(DefaultLaunch.Read()); }
            catch (Exception ex) {
                Ui.Language = previous;
                language.SelectedIndex = previous == "en" ? 1 : 0;
                MessageBox.Show(this, Ui.Text("Не удалось сохранить настройку:\n") + ex.Message, Ui.Text("Ошибка сохранения"));
                return;
            }
            Ui.Translate(this);
            about.Text = Ui.Language == "ru" ? "О программе" : "About";
            int selected = choice.SelectedIndex;
            for (int i = 0; i < choice.Items.Count; i++) {
                string text = (string)choice.Items[i];
                foreach (var pair in Ui.English) if (text == pair.Key || text == pair.Value) { choice.Items[i] = Ui.Text(pair.Key); break; }
            }
            choice.SelectedIndex = selected;
            StarterIntegration.Attach(this);
            try { Process.Start(new ProcessStartInfo(Assembly.GetExecutingAssembly().Location, "--setup-menu") { UseShellExecute = false, CreateNoWindow = true }); } catch { }
        };
    }

    private void AddShellCard(int x, string title, string executable, string arguments, Color cardColor, Color normalButtonColor) {
        var card = new Panel {
            Location = new Point(x, 84), Size = new Size(254, 178),
            BackColor = cardColor, BorderStyle = BorderStyle.FixedSingle
        };
        Controls.Add(card);

        var iconBox = new PictureBox {
            Location = new Point(16, 13), Size = new Size(34, 34),
            SizeMode = PictureBoxSizeMode.CenterImage,
            BackColor = Color.Transparent,
            Image = GetShellIcon(executable)
            ,Tag = executable
        };
        card.Controls.Add(iconBox);
        card.Controls.Add(new Label {
            Text = title, Font = new Font("Segoe UI Semibold", 12F),
            ForeColor = Color.FromArgb(238,243,255),
            Location = new Point(59, 16), Size = new Size(178, 30)
        });

        var normal = MakeButton(Ui.Text("Обычный запуск"), false, normalButtonColor);
        normal.Location = new Point(16, 59);
        normal.Click += delegate { Launch(executable, arguments, false); };
        card.Controls.Add(normal);

        var elevated = MakeButton(Ui.Text("От администратора"), true, normalButtonColor);
        elevated.Location = new Point(16, 116);
        elevated.Image = CreateShield();
        elevated.ImageAlign = ContentAlignment.MiddleLeft;
        elevated.TextImageRelation = TextImageRelation.ImageBeforeText;
        elevated.Padding = new Padding(25, 0, 0, 0);
        elevated.Click += delegate { Launch(executable, arguments, true); };
        card.Controls.Add(elevated);
    }

    private Image GetShellIcon(string executable) {
        try {
            using (Icon icon = Icon.ExtractAssociatedIcon(executable)) {
                return icon == null ? null : icon.ToBitmap();
            }
        } catch { return null; }
    }

    private Button MakeButton(string text, bool elevated, Color normalButtonColor) {
        var b = new Button {
            Text = text, Size = new Size(220, 46),
            FlatStyle = FlatStyle.Flat,
            BackColor = normalButtonColor,
            ForeColor = elevated ? Color.FromArgb(255, 170, 122) : Color.White,
            Font = elevated ? new Font("Segoe UI", 10F, FontStyle.Bold) : new Font("Segoe UI Semibold", 10F),
            Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleCenter
        };
        b.FlatAppearance.BorderSize = 0;
        b.FlatAppearance.BorderColor = AdminBorder;
        b.FlatAppearance.MouseOverBackColor = Color.FromArgb(
            Math.Min(255, normalButtonColor.R + 12),
            Math.Min(255, normalButtonColor.G + 12),
            Math.Min(255, normalButtonColor.B + 12));
        return b;
    }

    private Image CreateShield() {
        return CreateShield(96);
    }

    private Image CreateShield(int dpi) {
        var bmp = new Bitmap((int)Math.Round(24 * dpi / 96.0), (int)Math.Round(26 * dpi / 96.0));
        using (var g = Graphics.FromImage(bmp)) {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.ScaleTransform(dpi / 96f, dpi / 96f);
            var points = new Point[] { new Point(12,1), new Point(22,5), new Point(21,14), new Point(17,20), new Point(12,25), new Point(7,20), new Point(3,14), new Point(2,5) };
            using (var fill = new SolidBrush(Color.FromArgb(255, 214, 125)))
            using (var pen = new Pen(Color.FromArgb(255, 237, 184), 1.4F)) {
                g.FillPolygon(fill, points);
                g.DrawPolygon(pen, points);
            }
            using (var pen = new Pen(Color.FromArgb(83, 57, 23), 2.2F)) {
                pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round;
                g.DrawLines(pen, new Point[] { new Point(7,13), new Point(10,16), new Point(17,9) });
            }
        }
        return bmp;
    }

    [System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    private static extern int SHDefExtractIcon(string path, int index, uint flags, out IntPtr large, out IntPtr small, uint size);
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool DestroyIcon(IntPtr icon);

    protected override void OnDpiImagesChanged(int dpi) {
        foreach (Control card in Controls) foreach (Control control in card.Controls) {
            PictureBox picture = control as PictureBox;
            if (picture != null && picture.Tag is string) {
                IntPtr large = IntPtr.Zero, small = IntPtr.Zero;
                try {
                    int size = (int)Math.Round(32 * dpi / 96.0);
                    if (SHDefExtractIcon((string)picture.Tag, 0, 0, out large, out small, (uint)size) >= 0 && large != IntPtr.Zero) {
                        using (Icon icon = Icon.FromHandle(large)) {
                            Image previous = picture.Image;
                            picture.Image = icon.ToBitmap();
                            if (previous != null) previous.Dispose();
                        }
                    }
                } finally { if (large != IntPtr.Zero) DestroyIcon(large); if (small != IntPtr.Zero) DestroyIcon(small); }
            }
            Button button = control as Button;
            if (button != null && button.Image != null) {
                Image previous = button.Image;
                button.Image = CreateShield(dpi);
                previous.Dispose();
            }
        }
    }

    private void Launch(string file, string args, bool elevated) {
        try {
            var psi = ShellLaunch.Create(file, args, elevated);
            Process.Start(psi);
            Close();
        } catch (Win32Exception ex) {
            if (ex.NativeErrorCode == 1223) MessageBox.Show(this, Ui.Text("Запуск отменён."), AppInfo.Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show(this, Ui.Text("Не удалось запустить оболочку:\n") + ex.Message, Ui.Text("Ошибка запуска"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        } catch (Exception ex) {
            MessageBox.Show(this, Ui.Text("Не удалось запустить оболочку:\n") + ex.Message, Ui.Text("Ошибка запуска"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

internal static class Ui {
    internal static string Language = LoadLanguage();
    private static string LoadLanguage() {
        try { var lines = System.IO.File.ReadAllLines(DefaultLaunch.SettingsPath); if (lines.Length > 1 && (lines[1] == "en" || lines[1] == "ru")) return lines[1]; } catch { }
        return System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ru" ? "ru" : "en";
    }
    internal static readonly System.Collections.Generic.Dictionary<string,string> English = new System.Collections.Generic.Dictionary<string,string> {
        { "Командная консоль", "Admin Console" },
        { "Обычный запуск", "Normal launch" },
        { "От администратора", "Run as administrator" },
        { "Настройки", "Settings" },
        { "Приложение", "Application" },
        { "Не удалось запустить оболочку:\n", "Could not launch the shell:\n" },
        { "Ошибка запуска", "Launch error" },
        { "Не удалось выполнить запуск по умолчанию:\n", "Could not perform the default launch:\n" },
        { "Не удалось настроить закрепление на панели задач:\n", "Could not configure taskbar pinning:\n" },
        { "Сохранить изменённый режим запуска по умолчанию?", "Save the changed default launch mode?" },
        { "Не удалось сохранить настройку:\n", "Could not save the preference:\n" },
        { "Ошибка сохранения", "Save error" },
        { "Для запуска от имени администратора Windows покажет запрос UAC.", "Windows will request UAC approval for administrator mode." },
        { "При запуске ярлыка:", "When launching from a shortcut:" },
        { "Показывать окно выбора", "Show the selection window" },
        { "PowerShell — обычный запуск", "PowerShell — normal launch" },
        { "PowerShell — от администратора", "PowerShell — administrator" },
        { "CMD — обычный запуск", "CMD — normal launch" },
        { "CMD — от администратора", "CMD — administrator" },
        { "Сохранить", "Save" },
        { "Настройка сохранена. Для возврата удерживайте Shift при запуске\nили выберите «Настройки» в меню значка на панели задач.", "Preference saved. Hold Shift while launching to return,\nor choose Settings from the taskbar icon menu." },
        { "Не удалось сохранить настройку в папке приложения:\n", "Could not save the preference in the application folder:\n" },
        { "Настройки: Shift при запуске или пункт\n«Настройки» в меню значка на панели задач.", "Settings: hold Shift while launching or use\nSettings in the taskbar icon menu." },
        { "Запуск отменён.", "Launch cancelled." },
        { "Командная строка", "Command Prompt" }
    };
    internal static string Text(string russian) { string result; return Language == "en" && English.TryGetValue(russian, out result) ? result : russian; }
    internal static void Translate(Control parent) {
        foreach (var pair in English) if (parent.Text == pair.Key || parent.Text == pair.Value) { parent.Text = Text(pair.Key); break; }
        foreach (Control child in parent.Controls) Translate(child);
    }
}

internal sealed class AboutForm : Form {
    internal const string FeedbackUrl = "https://github.com/AlexSoftLTD/AdminConsole/issues";
    internal AboutForm() {
        bool ru = Ui.Language == "ru";
        Text = ru ? "О программе — AdminConsole" : "About AdminConsole";
        try { Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location); } catch { }
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(570, 480);
        MinimumSize = new Size(450, 350);
        StartPosition = FormStartPosition.CenterParent;
        Font = new Font("Segoe UI", 10F);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(16), ColumnCount = 1, RowCount = 4 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 90));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.Controls.Add(new Label { Dock = DockStyle.Fill, Text = "AdminConsole v" + AppInfo.Version + "\nCopyright © 2026 AlexSoft LTD\n" + (ru ? "Правообладатель: AlexSoft LTD" : "Rights holder: AlexSoft LTD") }, 0, 0);
        layout.Controls.Add(new Label { Dock = DockStyle.Fill, Text = ru ? "Лицензия: Freeware License.\nБесплатное использование, включая коммерческое." : "License: proprietary Freeware License.\nFree use, including commercial use." }, 0, 1);
        string license;
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AdminConsole.License." + (ru ? "ru" : "en")))
        using (var reader = new System.IO.StreamReader(stream)) license = reader.ReadToEnd();
        var text = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill, Text = license.Replace("\r\n", "\n").Replace("\n", Environment.NewLine), BackColor = SystemColors.Window };
        layout.Controls.Add(text, 0, 2);
        var feedback = new Button { Text = ru ? "Обратная связь — GitHub Issues" : "Feedback — GitHub Issues", Dock = DockStyle.Fill };
        feedback.Click += delegate {
            try { Process.Start(new ProcessStartInfo(FeedbackUrl) { UseShellExecute = true }); }
            catch (Exception ex) { MessageBox.Show(this, (ru ? "Откройте в браузере:\n" : "Open in your browser:\n") + FeedbackUrl + "\n" + ex.Message, "AdminConsole"); }
        };
        layout.Controls.Add(feedback, 0, 3);
        Controls.Add(layout);
        Shown += delegate { text.Select(0, 0); ActiveControl = feedback; };
    }
}