using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PathCli;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
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
                OpenZshrcInVSCode();
                break;

            case "--help":
                Console.WriteLine("  path-cli add <path>     Add a new path to the .zshrc file.");
                Console.WriteLine("  path-cli add current    Add the current directory to the .zshrc file.");
                Console.WriteLine("  path-cli remove <path>  Remove a path from the .zshrc file.");
                Console.WriteLine("  path-cli list           List all paths in the .zshrc file.");
                Console.WriteLine("  path-cli code           Open the .zshrc file with Visual Studio Code.");
                break;

            default:
                Console.WriteLine("Unknown command. Use --help for available commands.");
                break;
        }
    }

    static void AddPath(string path)
    {
        var zshrcPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".zshrc");
        var lines = File.ReadAllLines(zshrcPath).ToList();

        // Resolve the path to its absolute form
        var absolutePath = Path.GetFullPath(path);

        // Format the path to match the existing entries
        var formattedPath = $"export PATH=\"$PATH:{absolutePath}\"";
        Console.WriteLine($"Formatted path to add: {formattedPath}");

        if (!lines.Any(line => line.Contains(formattedPath)))
        {
            lines.Add(formattedPath);
            File.WriteAllLines(zshrcPath, lines);
            Console.WriteLine($"{absolutePath} added to PATH in .zshrc.");
        }
        else
        {
            Console.WriteLine($"{absolutePath} is already in PATH.");
        }
    }


    static void RemovePath(string path)
    {
        var zshrcPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".zshrc");
        var lines = File.ReadAllLines(zshrcPath).ToList();

        // Ensure the path is formatted correctly for comparison
        var formattedPath = $"export PATH=\"$PATH:{path}\"";
        Console.WriteLine($"Formatted path to remove: {formattedPath}");

        // Remove lines containing the exact formatted path
        var updatedLines = lines.Where(line => 
        {
            var trimmedLine = line.Trim();
            // Match line if it starts with 'export PATH=' and contains the path
            return !(trimmedLine.StartsWith("export PATH=") && trimmedLine.Contains(path));
        }).ToList();

        if (updatedLines.Count != lines.Count)
        {
            File.WriteAllLines(zshrcPath, updatedLines);
            Console.WriteLine($"{path} removed from PATH in .zshrc.");
        }
        else
        {
            Console.WriteLine($"{path} not found in .zshrc.");
        }
    }




    static void ListPaths()
    {
        var zshrcPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".zshrc");
        var lines = File.ReadAllLines(zshrcPath)
                        .Where(line => !line.TrimStart().StartsWith("#") && line.Contains("export PATH="))
                        .ToList();

        if (lines.Any())
        {
            Console.WriteLine("Paths in .zshrc:");
            foreach (var line in lines)
            {
                Console.WriteLine(line.Replace("export PATH=\"$PATH:", "").Replace("\"", ""));
            }
        }
        else
        {
            Console.WriteLine("No paths found in .zshrc.");
        }
    }

    static void OpenZshrcInVSCode()
    {
        var zshrcPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".zshrc");

        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "code",
                Arguments = zshrcPath,
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
            Console.WriteLine($"An error occurred while trying to open .zshrc in Visual Studio Code: {ex.Message}");
        }
    }
}

