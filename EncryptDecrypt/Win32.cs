using System.Runtime.InteropServices;

namespace EncryptDecrypt
{
    class Win32
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();
    }
}
