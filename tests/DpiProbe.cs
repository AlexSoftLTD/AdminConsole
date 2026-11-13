using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
internal static class DpiProbe {
    [STAThread] private static int Main(string[] args) {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Directory.CreateDirectory(args[0]);
        foreach (string language in new[] { "ru", "en" }) {
        Ui.Language = language;
        using (var form = new PickerForm()) {
            if (form.Icon == null) throw new Exception("Embedded window icon missing");
            form.ShowInTaskbar = false;
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(-30000,-30000);
            form.Show();
            IntPtr handle = form.Handle;
            foreach (int dpi in new int[] {96,120,144,192,240,288,96}) {
                form.ApplyDpi(dpi);
                Size expected = new Size((int)Math.Round(570*dpi/96.0), (int)Math.Round(381*dpi/96.0));
                if (form.ClientSize != expected) throw new Exception("Wrong client size at " + dpi);
                Check(form, dpi);
                using (var bitmap = new Bitmap(form.Width,form.Height)) {
                    form.DrawToBitmap(bitmap, new Rectangle(Point.Empty,bitmap.Size));
                    bitmap.Save(Path.Combine(args[0],"dpi-"+language+"-"+dpi+".png"));
                }
                Console.WriteLine("PASS " + dpi + " DPI: " + form.ClientSize + ", text fits");
            }
        }
        }
        return 0;
    }
    private static void Check(Control parent, int dpi) {
        foreach (Control c in parent.Controls) {
            if (c.Right > parent.ClientSize.Width+1 || c.Bottom > parent.ClientSize.Height+1) throw new Exception("Clipped control: "+c.Text+" at "+dpi);
            if (c is Label || c is Button) {
                Size text = Size.Empty;
                foreach (string line in c.Text.Split('\n')) {
                    Size measured = TextRenderer.MeasureText(line.TrimEnd('\r'),c.Font,Size.Empty,TextFormatFlags.SingleLine);
                    text.Width = Math.Max(text.Width, measured.Width);
                    text.Height += measured.Height;
                }
                int width = c.ClientSize.Width-c.Padding.Horizontal;
                Button button = c as Button;
                if (button != null && button.Image != null) width -= button.Image.Width+4;
                if (text.Width > width || text.Height > c.ClientSize.Height-c.Padding.Vertical) throw new Exception("Text does not fit: "+c.Text+" at "+dpi+" requires "+text+" available width "+width);
            }
            Check(c,dpi);
        }
    }
}
