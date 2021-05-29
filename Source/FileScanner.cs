using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace svgLibrary
{
    class FileScanner
    {
		
        readonly string RootDirectory = Properties.Settings.Default.defaultRootPath;
		public delegate void ProgressUpdate(int value);
		public event ProgressUpdate OnProgressUpdate;

		static int fileCount = 0;
		static int folderCount = 1; //1 Indicating Root directory
		static int itemsChecked = 0;

		static List<KeyValuePair<string,  string>> fileList = new List<KeyValuePair<string, string>>();
		static List<KeyValuePair<string, string>> fileDeletionQueue = new List<KeyValuePair<string, string>>();
		static List<string> folderDeletionQueue = new List<string>();
		//Properties.Settings.Default.defaultRootPath
		public int InitProgressBar()
		{
			int c = CountFolders(Properties.Settings.Default.defaultRootPath);
			return c;
		}

		public int Update()
		{
			if (OnProgressUpdate != null)
            {
				OnProgressUpdate(itemsChecked);
            }
			
			return itemsChecked;
		}

		//Recursively scan all folders and files in root directory
		public void ScanDirectory()
		{
			Console.WriteLine($"Scanning {RootDirectory}");

			if (IsEmpty(RootDirectory))
			{
				Console.WriteLine("Entire Directory is empty!");
			}
		}

		//Count folders for total
		private static int CountFolders(string folderPath)
        {
			fileCount += CountFiles(folderPath);
			foreach (var subFolder in Directory.EnumerateDirectories(folderPath))
			{
				fileCount += CountFiles(subFolder);
				folderCount++;
				CountFolders(subFolder);
			}
			return fileCount + folderCount;
		}

		//Count files for total
		private static int CountFiles(string folderPath)
		{
			return Directory.EnumerateFiles(folderPath).Count();
		}
		//Does current folder have files
		private static bool HasFiles(string folderPath)
		{
			return Directory.EnumerateFiles(folderPath).Any();
		}
		//Report results back to caller
		public List<KeyValuePair<string, int>> ReportResults()
		{
			List<KeyValuePair<string, int>> results = new List<KeyValuePair<string, int>>()
			{
				new KeyValuePair<string, int>("Total Files: ", fileCount),
				new KeyValuePair<string, int>("\nTotal Folders: ", folderCount),
				new KeyValuePair<string, int>("\nUnused File Types: ", fileDeletionQueue.Count()),
				new KeyValuePair<string, int>("\nEmpty Folders ", folderDeletionQueue.Count())
			};
			return results;
		}
		//Reset results so we don't get false statistics
		public void ResetResults()
		{
			fileCount = 0;
			folderCount = 1;
			itemsChecked = 0;
			fileList.Clear();
			fileDeletionQueue.Clear();
			folderDeletionQueue.Clear();
		}
		//Recursive Search checking for empty folders
		private static bool IsEmpty(string folderPath)
		{
			
			bool allSubFoldersEmpty = true;
			//Recursive search checking files
			FileCheck(folderPath);
			itemsChecked++;
			foreach (var subFolder in Directory.EnumerateDirectories(folderPath))
			{
				FileCheck(subFolder);
				if (IsEmpty(subFolder))
				{
					//Add Folder to deletion queue
					folderDeletionQueue.Add(subFolder.ToString());
				}
				else
				{
					allSubFoldersEmpty = false;
				}
			}
			

			if (allSubFoldersEmpty && !HasFiles(folderPath))
			{
				return true;
			}
			return false;
		}

		private static void FileCheck(string folderPath)
        {
			DirectoryInfo dir = new DirectoryInfo(folderPath);
			foreach (FileInfo file in dir.EnumerateFiles())
            {
				itemsChecked++;
				if (file.Name.EndsWith(".zip"))
                {
					//IsExtracted(file.FullName, folderPath);
                }
				if (UnacceptedType(file))
				{
					//Add File to deletion Queue
					fileDeletionQueue.Add(new KeyValuePair<string, string>(file.Name, file.FullName));
				} else
				{
					//Add file name to list for later usage
					fileList.Add(new KeyValuePair<string, string>(file.Name, file.FullName));
                }
				
            }

		}

		private static bool UnacceptedType(FileInfo file)
		{
			if (file.Name.EndsWith(".png") || file.Name.EndsWith(".svg"))
			{
				return false;
			}
			if (file.Name.EndsWith(".zip") || file.Name.EndsWith(".rar"))
            {
				return false;
            }
			return true;
        }

		//Compressed file check not implemented yet
		private static void IsExtracted(string zipPath, string folderPath)
        {
			int countChecked = 0;
			int entries = 0;
			using (ZipArchive archive = ZipFile.OpenRead(zipPath))
			{
				foreach (ZipArchiveEntry entry in archive.Entries)
				{
					
					if (entry.Name.EndsWith(".png") || entry.Name.EndsWith(".svg"))
                    {
						entries++;
						
						
                    }
						entries++;
					// Check contents against list of approved files
					if (fileList.Any(k => k.Key == entry.Name))
						countChecked++;
				}
			}
			if (countChecked < entries && countChecked >= 1)
				Console.WriteLine($"{zipPath} - Partial Extraction");

			if (countChecked == 0)
				Console.WriteLine($"{zipPath} - Not Extracted");

			if (countChecked == entries)
				Console.WriteLine($"{zipPath} - Fully Extracted");

		}

		public List<KeyValuePair<string, string>> GetFileDeletionQueue()
		{
			return fileDeletionQueue;
		}

		public List<string> GetFolderDeletionQueue()
		{
			return folderDeletionQueue;
		}

	}
}
