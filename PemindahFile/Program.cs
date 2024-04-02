using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Console = System.Console;

namespace PemindahFile
{
    internal class Program
    {
        static StreamWriter writer;
        static DateTime dateTimeLeast;
        static List<string> Tahuns = new List<string>();
        static void Main(string[] args)
        {
            
            for (int i = 2001; i<2050; i++)
            {
                Tahuns.Add(i.ToString());
            }
            Console.Write("press enter to start");
            while (Console.ReadLine() != "exit")
            {
                string From = "";
                string To = "";
                string InputTahun = "";
                string fileOutput = "";
                Console.Write("File Output: ");
                fileOutput = Console.ReadLine();
                while (File.Exists(fileOutput))
                {
                    Console.Write("File already exists, input new file name: ");
                    fileOutput = Console.ReadLine();
                }
                writer = new StreamWriter(fileOutput);
                Console.Write("Tahun termuda yang tidak mau dipindahkan: ");
                InputTahun = Console.ReadLine();
                try
                {
                    dateTimeLeast = Convert.ToDateTime($"{InputTahun}-01-01");
                }
                catch
                {
                    Console.WriteLine("input tahun yang benar!");
                    continue;
                }
                Console.Write("Copy file From: ");
                From = Console.ReadLine();
                if (!Directory.Exists(From))
                {
                    Console.WriteLine("Directory not exists");
                    continue;
                }
                Console.Write("Copy file To: ");
                To = Console.ReadLine();
                if (To[To.Length-1] != '\\' || To[To.Length - 1] != '/')
                {
                    To = To + "\\";
                }


                List<string> Dirs = new List<string>();
                List<string> subfolders = Directory.GetDirectories(From, "*", SearchOption.AllDirectories).ToList();
                Console.WriteLine("Directory Found :");
                writer.WriteLine("Directory Found :");
                subfolders.ForEach(a => Console.WriteLine(a));
                subfolders.ForEach(a => writer.WriteLine(a));

                List<string> dirFiltered =
                dirFiltered = subfolders.Where(a => folderValidator(Path.GetFileName(a))).ToList();
                dirFiltered = RemoveOverlappingDirectories(dirFiltered);
                writer.WriteLine("Filtered Directory :");
                Console.WriteLine("Filtered Directory :");
                dirFiltered.ForEach(a => Console.WriteLine(a));
                dirFiltered.ForEach(a => writer.WriteLine(a));
                Console.Write("Press enter to Continue");
                Console.ReadLine();
                dirFiltered.ForEach(from => Movefile(from,To));
                writer.Close();
                Console.WriteLine("Type 'exit' to exit");
            }
        }
        public static void Movefile(string from, string to)
        {
            try
            {
                Console.WriteLine("=============================================================");
                Console.WriteLine($"Start copy Directory {from}");
                writer.WriteLine("=============================================================");
                writer.WriteLine($"Start copy Directory {from}");
                string destination = to + getPathWithoutdrive(from);
                if (!Directory.Exists(destination))
                {
                    Directory.CreateDirectory(destination); // Create destination directory if it doesn't exist
                }
                List<string> directory = Directory.EnumerateFiles(from, "*",SearchOption.AllDirectories).ToList();
                List<string> directories = Directory.EnumerateDirectories(from, "*", SearchOption.AllDirectories).ToList();

                foreach (string createDir in directories)
                {
                    string createTargetDir = Path.Combine(to, getPathWithoutdrive(createDir));
                    if (!Directory.Exists(createTargetDir))
                    {
                        Directory.CreateDirectory(createTargetDir);
                    }
                }
                string origin = directory.Count().ToString();
                if (directory.Count() <= 0)
                {
                    writer.WriteLine($"pemindahan folder {from} Kosong");
                    Console.WriteLine($"pemindahan folder {from} Kosong");
                }
                while ( directory.Count > 0)
                {
                    string filePath = directory[0];
                    destination = to + getPathWithoutdrive(Path.GetDirectoryName(filePath));
                    string fileName = Path.GetFileName(filePath);
                    string destinationFilePath = Path.Combine(destination, fileName); 
                    if (!Directory.Exists(Path.GetDirectoryName(destinationFilePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath)); // Create destination directory if it doesn't exist
                    }
                    writer.WriteLine($"pemindahan folder {from} Kosong");
                    Console.WriteLine($"Copying {filePath}");
                    File.Copy(filePath, destinationFilePath);
                    FileInfo fromInfo = new FileInfo(filePath);
                    FileInfo toInfo = new FileInfo(destinationFilePath);
                    if (fromInfo.Length == toInfo.Length)
                    {
                        Console.WriteLine($"Copied {destinationFilePath} Success");
                        writer.WriteLine($"Copied {destinationFilePath} Success");
                        File.Delete(filePath);
                        directory.Remove(filePath);
                        Console.WriteLine($"Remove Source: {filePath} Success");
                        writer.WriteLine($"Remove Source: {filePath} Success");
                    }
                    else
                    {
                        Console.Write($"Copied {destinationFilePath} Failed, need retry");
                        writer.Write($"Copied {destinationFilePath} Failed, need retry");
                        Console.ReadLine();
                        File.Delete(destinationFilePath);
                        Console.WriteLine($"Remove Destination: {destinationFilePath} Success");
                        writer.WriteLine($"Remove Destination: {destinationFilePath} Success");
                        
                        directory.Remove(filePath);
                        directory.Insert(directory.Count, filePath);
                    }
                    Console.WriteLine($"Remaining File to move : {directory.Count} / {origin}");
                    writer.WriteLine($"Remaining File to move : {directory.Count} / {origin}");
                }
                Directory.Delete(from,true);
                Console.WriteLine($"Directory {from} moved to {to} successfully!");
                writer.WriteLine($"Directory {from} moved to {to} successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                writer.WriteLine($"Error: {ex.Message}");
            }
        }

        static string getPathWithoutdrive(string Dir)
        {
            string fullPath = Path.GetFullPath(Dir);

            // Extract components without the drive letter
            string remainingPath = fullPath.Substring(fullPath.IndexOf('\\') + 1);
            return remainingPath;
        }
        static bool folderValidator(string folder)
        {
            if (Tahuns.Contains(folder)){
                try
                {
                    var date = Convert.ToDateTime(folder + "-01-01");
                    if (date < dateTimeLeast) {
                        return true;
                    }
                    else return false;
                }
                catch
                {
                    return false;
                }
            }
            else {  return false; }
        }
        public static List<string> RemoveOverlappingDirectories(List<string> directories)
        {
            // Sort directories with parent folders coming first (longest path first)
            directories.OrderBy(a => a.Length);

            List<String> filtered = new List<string>();
            foreach(string dir in directories)
            {
                if(filtered.Where(a => dir.Contains(a) && a != dir).Count() <= 0)
                { 
                    filtered.Add(dir);
                }
            }

            return filtered;
        }
    }
}
