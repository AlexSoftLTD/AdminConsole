using System;
using System.Windows.Forms;
internal static class StarterProbe {
    [STAThread] static void Main() {
        StarterIntegration.SetCurrentProcessExplicitAppUserModelID(StarterIntegration.AppId);
        using(var form=new PickerForm()) {
            IntPtr handle = form.Handle;
            if (form.Text != AppInfo.Name + " v" + AppInfo.Version) throw new Exception("Version missing from title");
            if (StarterIntegration.ReadWindowProperty(form, 2) != StarterIntegration.RelaunchCommand) throw new Exception("Wrong relaunch command");
            if (StarterIntegration.ReadWindowProperty(form, 2).Contains("--settings")) throw new Exception("Settings leaked into pinned launch");
            if (StarterIntegration.ReadWindowProperty(form, 5) != StarterIntegration.AppId) throw new Exception("Wrong window ID");
            foreach (string language in new[] { "ru", "en", "ru", "en" }) {
                Ui.Language = language;
                StarterIntegration.Attach(form);
                if (StarterIntegration.ReadWindowProperty(form, 4) != AppInfo.Name) throw new Exception("Stale taskbar title");
                if (StarterIntegration.ReadWindowProperty(form, 5) != StarterIntegration.AppId ||
                    StarterIntegration.ReadWindowProperty(form, 2) != StarterIntegration.RelaunchCommand) throw new Exception("Language change altered relaunch identity or command");
            }
        }
        if (StarterIntegration.IdForDirectory(@"C:\App1") == StarterIntegration.IdForDirectory(@"C:\App2")) throw new Exception("Different copies share ID");
        if (StarterIntegration.IdForDirectory(@"C:\App1\") != StarterIntegration.IdForDirectory(@"c:\app1")) throw new Exception("Unstable ID");
        Console.WriteLine("PASS: actual window relaunch excludes settings; different folders have independent identities");
    }
}
