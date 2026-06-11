namespace BcMCPProxy.Auth;

using System.Diagnostics;
using System.Runtime.InteropServices;

/// <summary>
/// Provides cross-platform window handle discovery for MCP host applications.
/// </summary>
public static class MCPHostMainWindowHandleProvider
{
    /// <summary>
    /// Gets the main window handle of the parent MCP host process (Claude, VS Code, etc.).
    /// Cross-platform implementation for Windows and macOS.
    /// </summary>
    /// <returns>Window handle or IntPtr.Zero if not found or not supported on platform.</returns>
    public static IntPtr GetMCPHostWindow()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var process = FindFirstParentWithWindowHandle(Process.GetCurrentProcess());
            return process?.MainWindowHandle ?? IntPtr.Zero;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // On macOS, try to find the parent process with a window
            var process = FindFirstParentWithWindowHandleMacOS(Process.GetCurrentProcess());
            return process?.MainWindowHandle ?? IntPtr.Zero;
        }
        else
        {
            // Linux or other platforms - return zero (not supported for window handle discovery)
            return IntPtr.Zero;
        }
    }

    static Process? FindFirstParentWithWindowHandle(Process current)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return null;

            var visited = new HashSet<int>();

            while (current != null && !visited.Contains(current.Id))
            {
                visited.Add(current.Id);

                var parent = GetParentProcessWindows(current);

                // For Claude we cannot get the parent process so we handle null (this can be improved)
                // For VS Code we can get all the way to explorer
                if (current.MainWindowHandle != IntPtr.Zero && (parent == null || parent.ProcessName == "explorer"))
                    return current;

                current = parent!;
            }

            return null;
        }

        static Process? FindFirstParentWithWindowHandleMacOS(Process current)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return null;

            var visited = new HashSet<int>();

            while (current != null && !visited.Contains(current.Id))
            {
                visited.Add(current.Id);

                var parent = GetParentProcessMacOS(current);

                // Check if current process has a window handle
                // On macOS, MainWindowHandle might return IntPtr.Zero more often
                // We look for common parent processes like Claude, Code, or other GUI apps
                if (current.MainWindowHandle != IntPtr.Zero)
                    return current;

                // If we found a known GUI application by name, return it even without window handle
                if (IsKnownMCPHostProcess(current.ProcessName))
                    return current;

                // Stop if we've reached a system process
                if (parent == null || IsSystemProcess(parent.ProcessName))
                    return current.MainWindowHandle != IntPtr.Zero ? current : null;

                current = parent!;
            }

            return null;
        }

        static bool IsKnownMCPHostProcess(string processName)
        {
            // Common MCP host applications
            return processName.Contains("Claude", StringComparison.OrdinalIgnoreCase) ||
                   processName.Contains("Code", StringComparison.OrdinalIgnoreCase) ||
                   processName.Contains("cursor", StringComparison.OrdinalIgnoreCase) ||
                   processName.Contains("VSCode", StringComparison.OrdinalIgnoreCase);
        }

        static bool IsSystemProcess(string processName)
        {
            // Common macOS system processes we don't want to traverse past
            return processName == "launchd" ||
                   processName == "loginwindow" ||
                   processName == "WindowServer";
        }

        static Process? GetParentProcessWindows(Process process)
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

        static Process? GetParentProcessMacOS(Process process)
        {
            try
            {
                // Use ps command to get parent process ID
                var startInfo = new ProcessStartInfo
                {
                    FileName = "/bin/ps",
                    Arguments = $"-o ppid= -p {process.Id}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var psProcess = Process.Start(startInfo);
                if (psProcess == null)
                    return null;

                var output = psProcess.StandardOutput.ReadToEnd().Trim();
                psProcess.WaitForExit();

                if (int.TryParse(output, out int parentPid) && parentPid > 0)
                {
                    return Process.GetProcessById(parentPid);
                }
            }
            catch (Exception)
            {
                // Process not found or access denied
            }

            return null;
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
