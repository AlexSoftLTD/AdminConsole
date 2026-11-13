using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
internal static class StarterIntegration {
    internal static readonly string AppId = IdForDirectory(AppDomain.CurrentDomain.BaseDirectory);
    internal static string IdForDirectory(string directory) {
        string normalized = System.IO.Path.GetFullPath(directory).TrimEnd('\\').ToUpperInvariant();
        using (var hash = System.Security.Cryptography.SHA256.Create()) {
            return "AdminConsole.Portable." + BitConverter.ToString(hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(normalized))).Replace("-", "").Substring(0, 24);
        }
    }
    internal static string RelaunchCommand { get { return "\"" + Path + "\""; } }
    internal static string Path { get { return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Start Admin Console.exe"); } }
    [DllImport("shell32.dll",CharSet=CharSet.Unicode)] internal static extern int SetCurrentProcessExplicitAppUserModelID(string id);
    [StructLayout(LayoutKind.Sequential)] private struct Key { internal Guid Format; internal uint Id; }
    [StructLayout(LayoutKind.Explicit,Size=24)] private struct Value { [FieldOffset(0)] internal ushort Type; [FieldOffset(8)] internal IntPtr Text; }
    [ComImport,Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface Store {
        [PreserveSig] int GetCount(out uint count);
        [PreserveSig] int GetAt(uint index,out Key key);
        [PreserveSig] int GetValue(ref Key key,out Value value);
        [PreserveSig] int SetValue(ref Key key,ref Value value);
        [PreserveSig] int Commit();
    }
    [DllImport("shell32.dll")] private static extern int SHGetPropertyStoreForWindow(IntPtr window,ref Guid iid,out Store store);
    internal static void Attach(Form form) {
        Store store=null;
        try {
            Guid iid=typeof(Store).GUID;
            Marshal.ThrowExceptionForHR(SHGetPropertyStoreForWindow(form.Handle,ref iid,out store));
            Set(store,2,RelaunchCommand); Set(store,3,Path+",0"); Set(store,4,AppInfo.Name); Set(store,5,AppId);
            Marshal.ThrowExceptionForHR(store.Commit());
        } finally { if(store!=null) Marshal.ReleaseComObject(store); }
    }
    [DllImport("ole32.dll")] private static extern int PropVariantClear(ref Value value);
    internal static string ReadWindowProperty(Form form, uint id) {
        Store store = null;
        Value value = new Value();
        try {
            Guid iid = typeof(Store).GUID;
            Marshal.ThrowExceptionForHR(SHGetPropertyStoreForWindow(form.Handle, ref iid, out store));
            Key key = new Key { Format = new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), Id = id };
            Marshal.ThrowExceptionForHR(store.GetValue(ref key, out value));
            return value.Type == 31 ? Marshal.PtrToStringUni(value.Text) : null;
        } finally { PropVariantClear(ref value); if (store != null) Marshal.ReleaseComObject(store); }
    }
    private static void Set(Store store,uint id,string text) {
        Key key=new Key { Format=new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"),Id=id };
        Value value=new Value { Type=31,Text=Marshal.StringToCoTaskMemUni(text) };
        try { Marshal.ThrowExceptionForHR(store.SetValue(ref key,ref value)); }
        finally { Marshal.FreeCoTaskMem(value.Text); }
    }
}
