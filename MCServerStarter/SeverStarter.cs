using System.Diagnostics;

namespace MCServerStarter;

public class SeverStarter
{
    private readonly string _serverPath; // Server Core Path
    private readonly string _javaPath; // Java Path
    private readonly string _memoryArgs;
    private Process? _serverProcess;

    public SeverStarter(string serverPath, string javaPath, string xmax, string xms)
    {
        this._serverPath = serverPath;
        this._javaPath = javaPath;
        this._memoryArgs = $"-Xmax{xmax} -Xms{xms}";
    }

    public void StartServer()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = this._javaPath,
            Arguments = $"{this._memoryArgs} -jar {this._serverPath} nogui",
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        this._serverProcess = new Process
        {
            StartInfo = startInfo,
        };
        _serverProcess.OutputDataReceived += serverProcess_OutputDataReceived;

        _serverProcess.Start();
        _serverProcess.BeginOutputReadLine();
    }

    private static void serverProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null)
        {
            Console.WriteLine(e.Data);
        }
    }

    private async Task<string?> ReadOutputAsync()
    {
        StreamReader render = this._serverProcess.StandardOutput;
        string? line = await render.ReadLineAsync();
        return line;
    }

    private async Task WriteCommandsAsync(string cmd)
    {
        StreamWriter writer = this._serverProcess.StandardInput;
        await writer.WriteLineAsync(cmd);

        if (cmd == "exit" || cmd == "quit")
        {
            await this._serverProcess.WaitForExitAsync();
        }
    }
}