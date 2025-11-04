namespace BcMCPProxy.Auth
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public static class MCPHostMainWindowHandleProvider
    {
        /// <summary>
        /// This method is used to get the main window handle of the parent process. (Claude or vs code or any MCP host running on windows)
        /// This needs to be tested with:
        /// - Claude
        /// - vscode
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetMCPHostWindow()
        {
            return FindFirstParentWithWindowHandle(Process.GetCurrentProcess()).MainWindowHandle;
        }

        static Process FindFirstParentWithWindowHandle(Process current)
        {
            var visited = new HashSet<int>();

            while (current != null && !visited.Contains(current.Id))
            {
                visited.Add(current.Id);

                var parent = GetParentProcess(current);

                //for claude we cannot get the parent process so we handle null (this can be improved)
                //for vs code we can get all the way to explorer
                if (current.MainWindowHandle != IntPtr.Zero && (parent == null || parent.ProcessName == "explorer"))
                    return current;

                current = parent;
            }

            return null;
        }

        static Process GetParentProcess(Process process)
        {
            try
            {
                PROCESS_BASIC_INFORMATION pbi = new();
                int status = NtQueryInformationProcess(process.Handle, 0, ref pbi, Marshal.SizeOf<PROCESS_BASIC_INFORMATION>(), out _);
                if (status != 0)
                    return null;

                int parentPid = pbi.InheritedFromUniqueProcessId.ToInt32();
                return Process.GetProcessById(parentPid);
            }
            catch (Exception)
            {
                // We can get an incorrect processId which is not going to be found by GetProcessById
                return null;
            }
        }

        // Native structs and imports for Windows
        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr Reserved1;
            public IntPtr PebBaseAddress;
            public IntPtr Reserved2_0;
            public IntPtr Reserved2_1;
            public IntPtr UniqueProcessId;
            public IntPtr InheritedFromUniqueProcessId;
        }

        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass,
            ref PROCESS_BASIC_INFORMATION processInformation, int processInformationLength, out int returnLength);
    }
}
