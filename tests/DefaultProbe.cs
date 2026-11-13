using System;
using System.IO;
internal static class DefaultProbe {
    [STAThread] static int Main() {
        string path = DefaultLaunch.SettingsPath;
        if (File.Exists(path)) throw new Exception("Run probe in an isolated build directory");
        try {
            if (DefaultLaunch.Read() != 0) throw new Exception("Missing settings must show picker");
            for (int i = 0; i < 5; i++) {
                DefaultLaunch.Save(i);
                if (DefaultLaunch.Read() != i) throw new Exception("Setting did not persist");
                if (i == 0) continue;
                var launch = DefaultLaunch.Create(i);
                if ((launch.Verb == "runas") != (i == 2 || i == 4)) throw new Exception("Wrong elevation");
                if (Path.GetFileName(launch.FileName) != (i >= 3 ? "cmd.exe" : "powershell.exe")) throw new Exception("Wrong shell");
            }
            File.WriteAllText(path, "unknown");
            if (DefaultLaunch.Read() != 0) throw new Exception("Invalid settings must show picker");
            if (!DefaultLaunch.ShowSettings(new[] { "--settings" })) throw new Exception("Settings override ignored");
            DefaultLaunch.Save(2);
            Ui.Language = "en";
            DefaultLaunch.Save(DefaultLaunch.Read());
            if (DefaultLaunch.Read() != 2 || File.ReadAllLines(path)[1] != "en") throw new Exception("Language save changed launch mode");
            DefaultLaunch.Save(4);
            if (File.ReadAllLines(path)[1] != "en") throw new Exception("Mode save lost language");
            File.WriteAllText(path, "powershell-admin");
            if (DefaultLaunch.Read() != 2) throw new Exception("Legacy settings no longer load");
            using (var form = new PickerForm()) {
                System.Windows.Forms.ComboBox choice = null;
                foreach (System.Windows.Forms.Control control in form.Controls)
                    if (control is System.Windows.Forms.ComboBox && ((System.Windows.Forms.ComboBox)control).Items.Count == 5) choice = (System.Windows.Forms.ComboBox)control;
                if (choice == null || choice.SelectedIndex != 2 || form.HasUnsavedChanges) throw new Exception("Saved mode not loaded into UI");
                choice.SelectedIndex = 4;
                if (!form.HasUnsavedChanges || DefaultLaunch.Read() != 2) throw new Exception("Pending change tracking failed");
                form.SaveChoice();
                if (form.HasUnsavedChanges || DefaultLaunch.Read() != 4) throw new Exception("UI save failed");
            }
            Console.WriteLine("PASS: default persistence, four launch modes, invalid settings, explicit settings override");
            return 0;
        } finally { if (File.Exists(path)) File.Delete(path); }
    }
}
