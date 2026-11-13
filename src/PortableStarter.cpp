#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <shellapi.h>
#include <shobjidl.h>
#include <unknwn.h>
struct __declspec(uuid("D332DB9E-B9B3-4125-8207-A14884F53216")) ICLRMetaHost : IUnknown {
    virtual HRESULT STDMETHODCALLTYPE GetRuntime(LPCWSTR,REFIID,LPVOID*)=0;
};
struct __declspec(uuid("BD39D1D2-BA2F-486A-89B0-B4B0CB466891")) ICLRRuntimeInfo : IUnknown {
    virtual HRESULT STDMETHODCALLTYPE GetVersionString(LPWSTR,DWORD*)=0;
    virtual HRESULT STDMETHODCALLTYPE GetRuntimeDirectory(LPWSTR,DWORD*)=0;
    virtual HRESULT STDMETHODCALLTYPE IsLoaded(HANDLE,BOOL*)=0;
    virtual HRESULT STDMETHODCALLTYPE LoadErrorString(UINT,LPWSTR,DWORD*,LONG)=0;
    virtual HRESULT STDMETHODCALLTYPE LoadLibrary(LPCWSTR,HMODULE*)=0;
    virtual HRESULT STDMETHODCALLTYPE GetProcAddress(LPCSTR,LPVOID*)=0;
    virtual HRESULT STDMETHODCALLTYPE GetInterface(REFCLSID,REFIID,LPVOID*)=0;
    virtual HRESULT STDMETHODCALLTYPE IsLoadable(BOOL*)=0;
};
static const CLSID CLSID_CLRMetaHost={0x9280188d,0x0e8e,0x4867,{0xb3,0x0c,0x7f,0xa8,0x38,0x84,0xe8,0xde}};
#include <string>
#pragma comment(lib,"user32.lib")
#pragma comment(lib,"shell32.lib")
#pragma comment(lib,"advapi32.lib")
static bool RuntimeReady() {
    DWORD release=0, size=sizeof(release);
    if(RegGetValueW(HKEY_LOCAL_MACHINE,L"SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full",L"Release",RRF_RT_REG_DWORD,0,&release,&size)!=ERROR_SUCCESS || release<378389) return false;
    WCHAR sys[MAX_PATH]; GetSystemDirectoryW(sys,MAX_PATH);
    std::wstring shim=std::wstring(sys)+L"\\mscoree.dll";
    HMODULE library=LoadLibraryW(shim.c_str()); if(!library) return false;
    typedef HRESULT (STDAPICALLTYPE *CreateFn)(REFCLSID,REFIID,LPVOID*);
    CreateFn create=(CreateFn)GetProcAddress(library,"CLRCreateInstance");
    ICLRMetaHost* host=0; ICLRRuntimeInfo* info=0; BOOL loadable=FALSE;
    bool ok=create && SUCCEEDED(create(CLSID_CLRMetaHost,__uuidof(ICLRMetaHost),(void**)&host));
    if(ok) ok=SUCCEEDED(host->GetRuntime(L"v4.0.30319",__uuidof(ICLRRuntimeInfo),(void**)&info));
    if(ok) ok=SUCCEEDED(info->IsLoadable(&loadable)) && loadable;
    if(ok) {
        WCHAR directory[MAX_PATH]; DWORD length=MAX_PATH;
        ok=SUCCEEDED(info->GetRuntimeDirectory(directory,&length));
        if(ok) { const WCHAR* files[]={L"clr.dll",L"mscorlib.dll",L"System.Windows.Forms.dll",L"System.Drawing.dll",L"WPF\\PresentationFramework.dll"};
            for(auto file:files) if(GetFileAttributesW((std::wstring(directory)+file).c_str())==INVALID_FILE_ATTRIBUTES) ok=false;
        }
    }
    if(info) info->Release(); if(host) host->Release(); FreeLibrary(library); return ok;
}
int WINAPI wWinMain(HINSTANCE,HINSTANCE,LPWSTR arguments,int) {
    bool ready=RuntimeReady();
    if(wcscmp(arguments,L"--check-runtime")==0) return ready?0:10;
    if(!ready) {
        if(MessageBoxW(0,L"Не удалось обнаружить доступный .NET Framework 4.5 или новее.\n\nВ Windows 8 и новее он входит в систему. Можно запустить стандартное восстановление компонентов Windows: DISM, затем SFC. Потребуются права администратора; операция может занять длительное время и использовать Центр обновления Windows.\n\nЗапустить восстановление?",L"Командная консоль — восстановление .NET",MB_YESNO|MB_ICONWARNING|MB_DEFBUTTON2)==IDYES) {
            WCHAR sys[MAX_PATH]; GetSystemDirectoryW(sys,MAX_PATH);
            std::wstring cmd=std::wstring(sys)+L"\\cmd.exe";
            auto result=ShellExecuteW(0,L"runas",cmd.c_str(),L"/k DISM.exe /Online /Cleanup-Image /RestoreHealth && sfc.exe /scannow",sys,SW_SHOWNORMAL);
            if((INT_PTR)result<=32 && (INT_PTR)result!=SE_ERR_ACCESSDENIED) MessageBoxW(0,L"Не удалось запустить восстановление Windows.",L"Командная консоль",MB_OK|MB_ICONERROR);
        }
        return 10;
    }
    WCHAR path[32768]; DWORD count=GetModuleFileNameW(0,path,32768);
    if(count==0 || count>=32768) return 11;
    std::wstring folder(path); folder=folder.substr(0,folder.find_last_of(L"\\"));
    std::wstring executable=folder+L"\\Admin Console APP.exe";
    std::wstring command=L"\""+executable+L"\" "+arguments;
    STARTUPINFOW startup={sizeof(startup)}; PROCESS_INFORMATION process={};
    if(!CreateProcessW(executable.c_str(),&command[0],0,0,FALSE,0,0,folder.c_str(),&startup,&process)) {
        MessageBoxW(0,L"Не удалось запустить приложение. Распакуйте архив целиком: оба файла EXE должны находиться рядом.",L"Командная консоль",MB_OK|MB_ICONERROR); return 11;
    }
    CloseHandle(process.hThread); CloseHandle(process.hProcess); return 0;
}
