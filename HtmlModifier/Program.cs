using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlModifier
{
    class Program
    {
        private const string inputFolder = @"";
        private const string outputFolder = @"";

        private const string pattern = @"<p id='preview'>.*<script .*>.*</script><script.*>.*file=(.*mp4).*</script>";
        private static Regex regex;

        private const int playerWidth = 1280;
        private const int playerHeight = 740;

        static void Main(string[] args)
        {
            var noFolders = inputFolder == null || inputFolder == "" || outputFolder == null || outputFolder == "";

            if (noFolders)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Set folders properly!");
                Console.ResetColor();

                return;
            }

            regex = new Regex(pattern, RegexOptions.Singleline);

            ProcessHtmlFiles();
        }

        private static void ProcessHtmlFiles()
        {
            var htmlFiles = Directory.GetFiles(inputFolder, "*.html");

            Console.WriteLine(String.Format("Found {0} files.", htmlFiles.Length));

            for (int i = 0; i < htmlFiles.Length; i++)
            {
                Console.WriteLine(String.Format("Processing #{0}: {1}", i + 1, Path.GetFileName(htmlFiles[i])));
                ModifyHtml(htmlFiles[i], outputFolder);
            }
        }

        private static void ModifyHtml(string filePath, string outputFolder)
        {
            var text = File.ReadAllText(filePath);
            
            if (regex.IsMatch(text))
            {
                var m = regex.Match(text);
                var outputFilePath = Path.Combine(outputFolder, Path.GetFileName(filePath));

                if (m.Groups.Count > 1)
                {
                    var extractedPath = m.Groups[1].Value;

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(String.Format("Extracted path->{0}", extractedPath));
                    Console.ResetColor();

                    var replacementText = GenerateReplacementText(extractedPath);
                    var modText = regex.Replace(text, replacementText);

                    File.WriteAllText(outputFilePath, modText);
                    Console.WriteLine("Saved.");
                }
                else
                {
                    File.WriteAllText(outputFilePath, text);
                    Console.WriteLine("Copied as is.");
                }
                
            }
            else
            {
                Console.WriteLine("Doesn't match.");
            }
        }

        private static string GenerateReplacementText(string extractedPath)
        {
            var playerStr = new StringBuilder();

            playerStr.AppendLine(String.Format("<video width=\"{0}\" height=\"{1}\" controls>", playerWidth, playerHeight));
            playerStr.AppendLine(String.Format("<source \tsrc=\"{0}\" />", extractedPath));
            playerStr.AppendLine("\tYour player doesn't support video tag.");
            playerStr.AppendLine("</video>");

            return playerStr.ToString();
        }
    }
}
