using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace AotForms
{
    internal class Adb
    {
        string _path;
        ConcurrentQueue<string> _outputQueue;
        Process _process;

        internal Adb(string path)
        {
            _path = path;
            _outputQueue = new ConcurrentQueue<string>();
        }

        internal async Task Kill()
        {
            await Task.Run(() =>
            {
                ExecuteAdbCommand("kill-server");

                var adbProcesses = Process.GetProcessesByName("HD-Adb");
                foreach (var adbProcess in adbProcesses)
                {
                    adbProcess.Kill();
                    adbProcess.WaitForExit();
                }

                return Task.CompletedTask;
            });
        }

        internal async Task<bool> Start()
        {
            return await Task.Run(async () =>
            {
                ExecuteAdbCommand("kill-server");
                ExecuteAdbCommand("devices");

                _outputQueue = new ConcurrentQueue<string>();

                _process = new Process();
                _process.StartInfo.FileName = _path;
                _process.StartInfo.Arguments = "shell \"getprop ro.secure ; /boot/android/android/system/xbin/bstk/su\"";
                _process.StartInfo.RedirectStandardOutput = true;
                _process.StartInfo.RedirectStandardError = true;
                _process.StartInfo.RedirectStandardInput = true;
                _process.StartInfo.UseShellExecute = false;
                _process.StartInfo.CreateNoWindow = true;
                _process.EnableRaisingEvents = true;
                _process.OutputDataReceived += Receiver;
                _process.ErrorDataReceived += Receiver;

                _process.Start();
                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();

                _process.StandardInput.AutoFlush = true;

                // Wait for first output with 5-second timeout
                for (int i = 0; i < 5000; i++)
                {
                    if (!_outputQueue.IsEmpty) return true;
                    await Task.Delay(1);
                }

                return false;
            });
        }

        void Receiver(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
                _outputQueue.Enqueue(e.Data);
        }

        internal async Task<uint> FindModule(string process, string module)
        {
            return await Task.Run(async () =>
            {
                _outputQueue = new ConcurrentQueue<string>();

                _process.StandardInput.WriteLine("ps");

                string pid = await WaitForOutputAndExtractPID(process);
                if (string.IsNullOrEmpty(pid)) return 0U;

                _outputQueue = new ConcurrentQueue<string>();
                _process.StandardInput.WriteLine($"cat proc/{pid}/maps | grep {module}");

                string address = await WaitForValidAddress();
                if (string.IsNullOrEmpty(address)) return 0U;

                _process.Kill();
                await _process.WaitForExitAsync();
                await Kill();

                // address is like "7f8a3c0000-7f8a3c1000 r-xp ..."
                string start = address.Split('-')[0].Trim();

                if (!uint.TryParse(start, NumberStyles.HexNumber,
                        CultureInfo.InvariantCulture, out uint result))
                    return 0U;

                return result;
            });
        }

        // Waits up to 5 seconds for any output line, returns first line received
        async Task<string> WaitForOutput()
        {
            for (int i = 0; i < 5000; i++)
            {
                if (_outputQueue.TryDequeue(out string result))
                    return result;
                await Task.Delay(1);
            }
            return null;
        }

        // Waits up to 5 seconds, scanning all output lines for a valid hex memory address range.
        async Task<string> WaitForValidAddress()
        {
            var accumulated = new List<string>();

            for (int i = 0; i < 5000; i++)
            {
                while (_outputQueue.TryDequeue(out string line))
                    accumulated.Add(line);

                foreach (var line in accumulated)
                {
                    // Memory map lines start with "hexaddr-hexaddr", e.g. "7f8a3c0000-7f8a3c1000 r-xp ..."
                    var firstToken = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                                         .FirstOrDefault();

                    if (firstToken == null) continue;

                    var dashIndex = firstToken.IndexOf('-');
                    if (dashIndex <= 0) continue;

                    var startHex = firstToken.Substring(0, dashIndex);

                    if (uint.TryParse(startHex, NumberStyles.HexNumber,
                            CultureInfo.InvariantCulture, out _))
                        return line;
                }

                await Task.Delay(1);
            }

            return null;
        }

        // Waits up to 5 seconds, scans all output lines for the target process and extracts its PID
        async Task<string> WaitForOutputAndExtractPID(string process)
        {
            var accumulated = new List<string>();

            for (int i = 0; i < 5000; i++)
            {
                while (_outputQueue.TryDequeue(out string line))
                    accumulated.Add(line);

                foreach (var procLine in accumulated)
                {
                    if (!procLine.Contains(process)) continue;

                    var parts = procLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 1)
                    {
                        // Prefer the first purely-numeric token as PID (handles different ps formats)
                        var pidToken = parts.FirstOrDefault(p => p.All(char.IsDigit));
                        if (pidToken != null) return pidToken;

                        // Fallback to standard "USER PID PPID ..." layout
                        return parts[1];
                    }
                }

                await Task.Delay(1);
            }

            return null;
        }

        void ExecuteAdbCommand(string command)
        {
            using var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _path,
                    Arguments = command,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            proc.WaitForExit();
        }

        internal async Task PatchMemory(string package, ulong address, byte[] patch)
        {
            await Task.Run(() =>
            {
                string tmpFile = "/data/local/tmp/patch.hex";

                var hexBytes = string.Join(" ", patch.Select(b => $"\\x{b:X2}"));
                // printf is more portable than `echo -ne` across Android shells
                string echoCommand = $"printf '{hexBytes}' > {tmpFile}";

                _process.StandardInput.WriteLine(echoCommand);
                Thread.Sleep(200);

                string patchCommand = $"dd if={tmpFile} of=/proc/$(pidof {package})/mem bs=1 seek={address} conv=notrunc";
                _process.StandardInput.WriteLine(patchCommand);
                Thread.Sleep(200);
            });
        }
    }
}