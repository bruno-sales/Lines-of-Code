using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LinesOfCode
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the file extensions (use ',' to split): ");

            var extensions = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(extensions))
            {
                Console.Write("Extensions were not given. The application is exiting.");
                Environment.Exit(0);
            }

            var extensionList = new List<string>();
            var extensionSplited = extensions.Split(',');

            foreach (var ext in extensionSplited.Select(extension => extension.Trim().ToLower()))
            {
                if (ext.StartsWith(".") == false) extensionList.Add("." + ext);
                else extensionList.Add(ext);
            }

            Console.Write("Enter the root path: ");

            var path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.Write("Path was not given. The application is exiting.");
                Environment.Exit(0);
            }

            Console.Write("Do you want to count empty lines (Y/N) (Default 'N'): ");
            
            var countEmptyAnswer = Console.ReadLine();

            var countEmpty = countEmptyAnswer?.ToLower() == "y";

            var folderBlackList = FolderBlackList();
            var allFiles = Directory
                .GetFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(file => extensionList.Any(file.ToLower().EndsWith) &&
                               folderBlackList.All(i=> file.ToLower().Contains(i) == false))
                .ToList();

            Console.WriteLine();
            Console.WriteLine("========================================================================");

            var totalLineCount = 0;
            var mostLinesFile = 0;
            var mostLinesFileName = string.Empty;

            var resumeList = new Dictionary<string, int>();

            foreach (var file in allFiles)
            {
                var lineCount = countEmpty ? File.ReadLines(file).Count() : File.ReadLines(file).Count(i => string.IsNullOrWhiteSpace(i.Trim()) == false);
                totalLineCount += lineCount;

                var fileName = file.Substring(file.LastIndexOf('\\') + 1);
                Console.WriteLine(fileName + " has " + lineCount + " lines.");

                if (lineCount > mostLinesFile)
                {
                    mostLinesFile = lineCount;
                    mostLinesFileName = file;
                }

                resumeList.Add(file, lineCount);
            }

            Console.WriteLine("========================================================================");
            Console.WriteLine();
            
            Console.Write("There are ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{totalLineCount} lines ");
            Console.ResetColor();
            Console.Write("in all the directory.");
            Console.WriteLine();

            Console.Write($"The file with most lines is: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(mostLinesFileName);
            Console.ResetColor();
            Console.WriteLine();

            CreateLog(resumeList, totalLineCount);

            Console.WriteLine($"A txt with all files ordered by number of lines was created in the path of this application.");
            Console.WriteLine();
            
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        private static List<string> FolderBlackList()
        {
            return new List<string>()
            {
                "\\bin", "\\debug"
            };
        }

        private static void CreateLog(Dictionary<string, int> values, int total)
        {
            var path = $"Resume_File-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
            var writer = new StreamWriter(path,true);

            var ordered = values.OrderByDescending(i => i.Value);

            writer.WriteLine($"Total of lines: {total}");
            writer.WriteLine($"Number of files checked: {values.Count}");
            writer.WriteLine();
            writer.WriteLine("All entries");
            writer.WriteLine("========================================================");

            foreach (var pair in ordered)
            {
                writer.WriteLine($"{pair.Value} - {pair.Key}");
            }

            writer.Close();
        }
    }
}
