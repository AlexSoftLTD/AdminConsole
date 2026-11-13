using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
internal static class AboutProbe {
    [STAThread] static int Main(string[] args) {
        Application.EnableVisualStyles();
        foreach (string language in new[] { "ru", "en" }) {
            Ui.Language = language;
            using (var form = new AboutForm()) {
                form.ShowInTaskbar = false;
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(-30000,-30000);
                form.Show();
                bool found = false;
                foreach (Control panel in form.Controls) foreach (Control child in panel.Controls) {
                    var text = child as TextBox;
                    if (text != null) {
                        if (!text.Text.Contains(language == "ru" ? "применяется русский текст" : "the Russian text prevails")) throw new Exception("Incorrect governing language");
                        found = text.ReadOnly && text.Text.Contains("AlexSoft LTD") && (language == "ru" ? (text.Text.Contains("РУССКИЙ") && !text.Text.Contains("ENGLISH")) : (text.Text.Contains("ENGLISH") && !text.Text.Contains("РУССКИЙ")));
                    }
                }
                if (!found) throw new Exception("Selected-language license missing or mixed");
                using (var bitmap = new Bitmap(form.Width, form.Height)) {
                    form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                    bitmap.Save(Path.Combine(args[0], "about-" + language + ".png"));
                }
            }
        }
        if (AboutForm.FeedbackUrl != "https://github.com/AlexSoftLTD/AdminConsole/issues") throw new Exception("Wrong feedback destination");
        Console.WriteLine("PASS: both About languages, selected-language full license and single feedback destination");
        return 0;
    }
}
