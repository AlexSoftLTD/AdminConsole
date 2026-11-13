using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// Always calculate from the original 96-DPI geometry, never from rounded scaled bounds.
internal class DpiForm : Form {
    private sealed class Item {
        internal Control Control;
        internal Rectangle Bounds;
        internal Padding Padding;
        internal string Family;
        internal float Points;
        internal FontStyle Style;
        internal Font OwnedFont;
    }
    private readonly List<Item> items = new List<Item>();
    private bool captured, applying;
    private Size logicalClient;
    private int currentDpi;
    internal DpiForm() { AutoScaleMode = AutoScaleMode.None; }

    [DllImport("user32.dll")] private static extern uint GetDpiForWindow(IntPtr window);
    [StructLayout(LayoutKind.Sequential)] private struct NativeRect { public int Left, Top, Right, Bottom; }

    private int ReadDpi() {
        try { uint dpi = GetDpiForWindow(Handle); if (dpi != 0) return (int)dpi; }
        catch (EntryPointNotFoundException) { }
        using (Graphics graphics = Graphics.FromHwnd(Handle)) return (int)Math.Round(graphics.DpiX);
    }
    protected override void OnLoad(EventArgs e) {
        ApplyDpi(ReadDpi());
        base.OnLoad(e);
    }
    private void CaptureLayout(Control parent) {
        foreach (Control control in parent.Controls) {
            items.Add(new Item { Control = control, Bounds = control.Bounds, Padding = control.Padding,
                Family = control.Font.FontFamily.Name, Points = control.Font.SizeInPoints, Style = control.Font.Style });
            CaptureLayout(control);
        }
    }
    private static int ScaleValue(int value, int dpi) { return (int)Math.Round(value * dpi / 96.0); }
    internal void ApplyDpi(int dpi) {
        if (applying || dpi <= 0) return;
        if (!captured) { logicalClient = ClientSize; CaptureLayout(this); captured = true; }
        if (currentDpi == dpi) return;
        applying = true;
        SuspendLayout();
        foreach (Item item in items) item.Control.SuspendLayout();
        try {
            ClientSize = new Size(ScaleValue(logicalClient.Width, dpi), ScaleValue(logicalClient.Height, dpi));
            foreach (Item item in items) {
                Control c = item.Control;
                Font previous = item.OwnedFont;
                // Pixel fonts avoid the system-DPI font cache used by older WinForms versions.
                item.OwnedFont = new Font(item.Family, item.Points * dpi / 72f, item.Style, GraphicsUnit.Pixel);
                c.Font = item.OwnedFont;
                if (previous != null) previous.Dispose();
                Padding p = item.Padding;
                c.Padding = new Padding(ScaleValue(p.Left,dpi),ScaleValue(p.Top,dpi),ScaleValue(p.Right,dpi),ScaleValue(p.Bottom,dpi));
                Rectangle b = item.Bounds;
                c.SetBounds(ScaleValue(b.X,dpi),ScaleValue(b.Y,dpi),ScaleValue(b.Width,dpi),ScaleValue(b.Height,dpi));
            }
            OnDpiImagesChanged(dpi);
            currentDpi = dpi;
        } finally {
            foreach (Item item in items) item.Control.ResumeLayout(false);
            ResumeLayout(false);
            applying = false;
        }
        Invalidate(true);
    }
    protected virtual void OnDpiImagesChanged(int dpi) { }
    protected override void WndProc(ref Message m) {
        if (m.Msg == 0x02E0 && captured) { // WM_DPICHANGED
            int dpi = (int)(m.WParam.ToInt64() & 0xffff);
            NativeRect suggested = (NativeRect)Marshal.PtrToStructure(m.LParam, typeof(NativeRect));
            ApplyDpi(dpi);
            Location = new Point(suggested.Left, suggested.Top);
            m.Result = IntPtr.Zero;
            return;
        }
        base.WndProc(ref m);
    }
    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);
        if (disposing) foreach (Item item in items) if (item.OwnedFont != null) item.OwnedFont.Dispose();
    }
}
