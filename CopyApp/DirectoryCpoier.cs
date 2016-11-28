using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CopyApp
{
    [Serializable]
    public class DirectoryCpoier
    {
        public struct Time
        {
            public int Hour;
            public int Minute;
        }

        public string SourcePath { get; set; }
        public string[] DestnationPathList { get; set; }
        private string[] DestPathList;

        public enum DaysOfWeek
        {
            Saturday,
            Sunday,
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday
        }

        public DayOfWeek DayOfWeek { get; set; }

        public int DayOfMonth { get; set; }

        public enum ScheduledCopyTypes
        {
            Hourly,
            Daily,
            Weekly,
            Monthly,
            Watching
        }

        public ScheduledCopyTypes ScheduledCopyType { get; set; }

        public Time CopyTime { get; set; }

        public DirectoryCpoier()
        {

        }
        public DirectoryCpoier(string SourcePath)
        {
            this.SourcePath = SourcePath;
            DestPathList = new string[0];
        }

        public void AddDestination(string DestinationPath)
        {
            Array.Resize(ref DestPathList, DestPathList.Length + 1);
            DestPathList[DestPathList.Length - 1] = DestinationPath;
            DestnationPathList = DestPathList;
        }

        public bool StartCopy()
        {
            foreach (string item in DestnationPathList)
            {
                bool result = DirectoryCopy(SourcePath, item);
                if (!result)
                    return false;
            }

            return true;
        }
        private bool DirectoryCopy(string sourceDirPath, string destDirPath)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirPath);

            if (!dir.Exists)
            {
                return false;
            }

            try
            {
                DirectoryInfo[] dirs = dir.GetDirectories();
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(destDirPath))
                {
                    Directory.CreateDirectory(destDirPath);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(destDirPath, file.Name);
                    if (File.Exists(temppath))
                    {
                        string SourceHash = FileHash.Hash(file.FullName);
                        string DestHash = FileHash.Hash(temppath);
                        if (SourceHash == DestHash && file.Length == new FileInfo(temppath).Length)
                            continue;
                        else
                            File.Delete(temppath);
                    }
                    file.CopyTo(temppath, false);
                }


                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirPath, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath);
                }

            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}