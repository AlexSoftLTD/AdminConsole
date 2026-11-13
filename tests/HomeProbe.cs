using System;
using System.Diagnostics;
using System.IO;
internal static class HomeProbe {
    static int Main() {
        string system = Environment.GetFolderPath(Environment.SpecialFolder.System);
        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        Check(Path.Combine(system,"cmd.exe"),"/d /c cd",home);
        Check(Path.Combine(system,"WindowsPowerShell\\v1.0\\powershell.exe"),"-NoLogo -NoProfile -NonInteractive -Command \"(Get-Location).Path\"",home);
        return 0;
    }
    static void Check(string file,string args,string home) {
        var info = ShellLaunch.Create(file,args,false);
        if(info.WorkingDirectory != home || info.Verb == "runas") throw new Exception("Invalid normal launch settings");
        var admin = ShellLaunch.Create(file,args,true);
        if(admin.WorkingDirectory != home || admin.Verb != "runas") throw new Exception("Invalid elevated launch settings");
        info.UseShellExecute=false; info.RedirectStandardOutput=true; info.CreateNoWindow=true;
        using(var process=Process.Start(info)) {
            string actual=process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
            if(process.ExitCode != 0 || !String.Equals(actual,home,StringComparison.OrdinalIgnoreCase)) throw new Exception("Wrong directory: "+actual);
            Console.WriteLine("PASS "+Path.GetFileName(file)+": "+actual);
        }
    }
}
