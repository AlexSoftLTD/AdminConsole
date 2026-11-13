using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
public static class IconResources {
    delegate bool EnumLang(IntPtr module, IntPtr type, IntPtr name, ushort language, IntPtr data);
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode)] static extern bool EnumResourceLanguages(IntPtr h,IntPtr type,IntPtr name,EnumLang callback,IntPtr data);
    delegate bool EnumName(IntPtr module, IntPtr type, IntPtr name, IntPtr data);
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode,SetLastError=true)] static extern IntPtr LoadLibraryEx(string path,IntPtr file,uint flags);
    [DllImport("kernel32.dll")] static extern bool FreeLibrary(IntPtr h);
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode)] static extern bool EnumResourceNames(IntPtr h,IntPtr type,EnumName callback,IntPtr data);
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode,SetLastError=true)] static extern IntPtr BeginUpdateResource(string file,bool delete);
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode,SetLastError=true)] static extern bool UpdateResource(IntPtr h,IntPtr type,IntPtr name,ushort language,byte[] data,uint size);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool EndUpdateResource(IntPtr h,bool discard);
    public static void Replace(string exe,string ico) {
        var names=new List<Tuple<int,int,ushort>>();
        IntPtr module=LoadLibraryEx(exe,IntPtr.Zero,2);
        if(module==IntPtr.Zero)throw new System.ComponentModel.Win32Exception();
        try { foreach(int type in new[]{3,14}) {
            EnumName callback=delegate(IntPtr m,IntPtr t,IntPtr n,IntPtr d) {
                if(n.ToInt64()>65535)throw new Exception("Named icon resource is unexpected");
                EnumLang languages = delegate(IntPtr lm,IntPtr lt,IntPtr ln,ushort language,IntPtr ld) { names.Add(Tuple.Create(type,n.ToInt32(),language));return true; }; EnumResourceLanguages(m,t,n,languages,IntPtr.Zero);return true;
            };
            EnumResourceNames(module,(IntPtr)type,callback,IntPtr.Zero);
        }}finally{FreeLibrary(module);}
        byte[] bytes=File.ReadAllBytes(ico);
        IntPtr h=BeginUpdateResource(exe,false);if(h==IntPtr.Zero)throw new System.ComponentModel.Win32Exception();
        bool success=false;
        try {
            // Remove all old icon resources using their actual language IDs.
            foreach(var n in names)
                if(!UpdateResource(h,(IntPtr)n.Item1,(IntPtr)n.Item2,n.Item3,null,0))throw new System.ComponentModel.Win32Exception();
            int count=BitConverter.ToUInt16(bytes,4);
            using(var group=new MemoryStream())using(var writer=new BinaryWriter(group)) {
                writer.Write((ushort)0);writer.Write((ushort)1);writer.Write((ushort)count);
                for(int i=0;i<count;i++) {
                    int entry=6+16*i;int size=BitConverter.ToInt32(bytes,entry+8);int offset=BitConverter.ToInt32(bytes,entry+12);
                    var frame=new byte[size];Array.Copy(bytes,offset,frame,0,size);
                    if(!UpdateResource(h,(IntPtr)3,(IntPtr)(i+1),0,frame,(uint)size))throw new System.ComponentModel.Win32Exception();
                    writer.Write(bytes,entry,12);writer.Write((ushort)(i+1));
                }
                var data=group.ToArray();if(!UpdateResource(h,(IntPtr)14,(IntPtr)101,0,data,(uint)data.Length))throw new System.ComponentModel.Win32Exception();
            }
            success=true;
        }finally{if(!EndUpdateResource(h,!success))throw new System.ComponentModel.Win32Exception();}
    }
}
