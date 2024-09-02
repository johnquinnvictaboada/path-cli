using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PathCli;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("\n");
            Console.WriteLine("  path-cli --help     list of available commands");
            return;
        }

        switch (args[0])
        {
            case "add":
                if (args.Length < 2)
                {
                    Console.WriteLine("Please provide a path to add.");
                    return;
                }
                AddPath(args[1]); // Handle path directly
                break;

            case "remove":
                if (args.Length < 2)
                {
                    Console.WriteLine("Please provide a path to remove.");
                    return;
                }
                RemovePath(args[1]);
                break;

            case "list":
                ListPaths();
                break;

            case "code":
                OpenShellConfigInVSCode();
                break;

            case "--help":
                Console.WriteLine("\n");
                Console.WriteLine("  path-cli add <path>     Add a new path to the shell config file.");
                Console.WriteLine("  path-cli remove <path>  Remove a path from the shell config file.");
                Console.WriteLine("  path-cli list           List all paths in the shell config file.");
                Console.WriteLine("  path-cli code           Open the shell config file with Visual Studio Code.");
                break;

            default:
                Console.WriteLine("Unknown command. Use --help for available commands.");
                break;
        }
    }

    static string GetShellConfigFile()
    {
        var shell = Environment.GetEnvironmentVariable("SHELL");

        if (shell != null && shell.Contains("bash"))
        {
            return ".bashrc";
        }
        else if (shell != null && shell.Contains("zsh"))
        {
            return ".zshrc";
        }
        else
        {
            // Default to .zshrc if shell is not recognized
            return ".zshrc";
        }
    }

    static void AddPath(string path)
    {
        var shellConfigFile = GetShellConfigFile();
        var configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), shellConfigFile);
        var lines = File.ReadAllLines(configFilePath).ToList();

        var absolutePath = Path.GetFullPath(path);

        var formattedPath = $"export PATH=\"$PATH:{absolutePath}\"";
        Console.WriteLine($"Formatted path to add: {formattedPath}");

        if (!lines.Any(line => line.Contains(formattedPath)))
        {
            lines.Add(formattedPath);
            File.WriteAllLines(configFilePath, lines);
            Console.WriteLine($"{absolutePath} added to PATH in {shellConfigFile}.");
        }
        else
        {
            Console.WriteLine($"{absolutePath} is already in PATH.");
        }
    }

    static void RemovePath(string path)
    {
        var shellConfigFile = GetShellConfigFile();
        var configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), shellConfigFile);
        var lines = File.ReadAllLines(configFilePath).ToList();

        var formattedPath = $"export PATH=\"$PATH:{path}\"";
        Console.WriteLine($"Formatted path to remove: {formattedPath}");

        var updatedLines = lines.Where(line => 
        {
            var trimmedLine = line.Trim();
            return !(trimmedLine.StartsWith("export PATH=") && trimmedLine.Contains(path));
        }).ToList();

        if (updatedLines.Count != lines.Count)
        {
            File.WriteAllLines(configFilePath, updatedLines);
            Console.WriteLine($"{path} removed from PATH in {shellConfigFile}.");
        }
        else
        {
            Console.WriteLine($"{path} not found in {shellConfigFile}.");
        }
    }

    static void ListPaths()
    {
        var shellConfigFile = GetShellConfigFile();
        var configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), shellConfigFile);
        var lines = File.ReadAllLines(configFilePath)
                        .Where(line => !line.TrimStart().StartsWith("#") && line.Contains("export PATH="))
                        .ToList();

        if (lines.Any())
        {
            Console.WriteLine($"Paths in {shellConfigFile}:");
            foreach (var line in lines)
            {
                Console.WriteLine(line.Replace("export PATH=\"$PATH:", "").Replace("\"", ""));
            }
        }
        else
        {
            Console.WriteLine($"No paths found in {shellConfigFile}.");
        }
    }

    static void OpenShellConfigInVSCode()
    {
        var shellConfigFile = GetShellConfigFile();
        var configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), shellConfigFile);

        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "code",
                Arguments = configFilePath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // Start the process and check if it was successful
            using (var process = Process.Start(processStartInfo))
            {
                if (process == null)
                {
                    Console.WriteLine("Failed to start Visual Studio Code.");
                    return;
                }

                // Read output and error streams
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                {
                    Console.WriteLine("Output: " + output);
                }
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("Error: " + error);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while trying to open {shellConfigFile} in Visual Studio Code: {ex.Message}");
        }
    }
}
