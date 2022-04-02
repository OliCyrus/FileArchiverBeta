using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace FileArchiverTest
{
    internal class FileProcessing
    {
        string[] foldersPaths { get; set; }
        double days { get; set; }
        
        string archivePath { get; set; }

        public FileProcessing()
        {
            foldersPaths = ConfigurationManager.AppSettings["FoldersPaths"].Split(',').Select(s => s.Trim()).ToArray();
            days = Convert.ToDouble(ConfigurationManager.AppSettings["DaysToSet"]);
            archivePath = ConfigurationManager.AppSettings["PutArchivesTo"];
            foreach (string str in foldersPaths)
            {
                Console.WriteLine(str);
            }
            Console.WriteLine(days);
            Console.WriteLine(archivePath);
        }

        static string[] GetFilesSortedByCreationTime(string path, double days)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            FileInfo[] files = info.GetFiles().Where(p => p.CreationTime < DateTime.Now.AddDays(days)).ToArray();
            string[] result = files.Select(p => p.FullName).ToArray();
            if(result.Length != 0)
            {
                File.AppendAllText("log.txt", String.Concat(DateTime.Now.ToString(), " - Files list gained for directory ", path) + Environment.NewLine);
                return result;

            }
            else
            {
                File.AppendAllText("log.txt", String.Concat(DateTime.Now.ToString(), " - No files with specific creation date in directory ", path) + Environment.NewLine);
                return null;
            }
            

        }

        public void ArchiveAndRemoveFiles()
        {
            try
            {
                foreach(string pathsIterator in foldersPaths)
                {
                    string[] sortedFiles = GetFilesSortedByCreationTime(pathsIterator, days);
                    if (sortedFiles != null)
                    {
                        using (FileStream zipFile = File.Open(String.Concat(@"D:\", pathsIterator.Substring(2).Replace("AntaresVision", "").Replace(@"\", ""), "_", DateTime.Now.ToString("dd.MM.yyyy"), ".zip"), FileMode.Create))
                        {
                            using (ZipArchive archive = new ZipArchive(zipFile, ZipArchiveMode.Update))
                            {
                                foreach (string str in sortedFiles)
                                {
                                    FileInfo fi = new FileInfo(str);
                                    archive.CreateEntryFromFile(fi.FullName, fi.Name);
                                }
                            }
                        }
                        File.AppendAllText("log.txt", String.Concat(DateTime.Now.ToString(), " - Created archive for directory ", pathsIterator) + Environment.NewLine);
                        foreach (string str in sortedFiles)
                        {
                            FileInfo fi = new FileInfo(str);
                            fi.Delete();
                        }
                        File.AppendAllText("log.txt", String.Concat(DateTime.Now.ToString(), " - Removed archived files from directory ", pathsIterator) + Environment.NewLine);
                    }
                        
                    
                }
                
            }
            catch (IOException e)
            {
                if (e.Source != null)
                {
                    File.AppendAllText("log.txt", e.Source + Environment.NewLine);
                    File.AppendAllText("log.txt", e.Message + Environment.NewLine);
                }
                throw;
            }
            catch (OutOfMemoryException e)
            {
                File.AppendAllText("log.txt", e.Source + Environment.NewLine);
                File.AppendAllText("log.txt", e.Message + Environment.NewLine);
            }
            Console.WriteLine("See log.txt for more details");
            Console.ReadKey();
        }

        
    }
}
