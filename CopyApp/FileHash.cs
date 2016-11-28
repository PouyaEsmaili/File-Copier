using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace CopyApp
{
    class FileHash
    {
        public DirectoryCpoier DC { get; set; }

        public FileHash(DirectoryCpoier DC)
        {
            this.DC = DC;
        }

        public void Main()
        {
            FileHashData[] CurrentSource = GetFiles(DC.SourcePath);
            FileHash_Save(CurrentSource);


            foreach (string Dest in DC.DestnationPathList)
            {
                FileHashData[] DestinaionFiles = GetFiles(Dest);
                foreach (FileHashData cur in CurrentSource)
                {
                    bool IsTheSame = false;
                    foreach (FileHashData destfile in DestinaionFiles)
                    {
                        if (destfile.FileName == cur.FileName &&
                            destfile.FilePath.Remove(0, Dest.Length + 1) == cur.FilePath.Remove(0, DC.SourcePath.Length + 1))
                        {
                            int index = Array.IndexOf(DestinaionFiles, destfile);
                            DestinaionFiles = DestinaionFiles.Where(obj => obj != DestinaionFiles[index]).ToArray();
                            if (destfile.FileHash == cur.FileHash && destfile.Length == cur.Length)
                            {
                                IsTheSame = true;
                                break;
                            }
                        }
                    }

                    if (!IsTheSame)
                    {
                        foreach (string destpath in DC.DestnationPathList)
                        {
                            string filePath = Path.Combine(destpath, cur.FilePath.Remove(0, DC.SourcePath.Length + 1));
                            if (!Directory.Exists(filePath.Remove(filePath.Length - cur.FileName.Length - 1)))
                                Directory.CreateDirectory(filePath.Remove(filePath.Length - cur.FileName.Length - 1));
                            if (File.Exists(filePath))
                                File.Delete(filePath);
                            File.Copy(cur.FilePath, filePath);
                        }
                    }
                }

                if (DestinaionFiles.Length > 0)
                {
                    foreach (FileHashData item in DestinaionFiles)
                    {
                        File.Delete(item.FilePath);
                    }
                }
            }
        }

        public FileHashData[] GetFiles(string DirectoryPath)
        {
            FileHashData[] fhdList = new FileHashData[0];
            if (Directory.Exists(DirectoryPath))
            {
                DirectoryInfo dir = new DirectoryInfo(DirectoryPath);
                DirectoryInfo[] subdirs = dir.GetDirectories();
                FileInfo[] Files = dir.GetFiles();
                foreach (FileInfo item in Files)
                {
                    Array.Resize(ref fhdList, fhdList.Length + 1);
                    FileHashData fhd = new FileHashData(item.Name);
                    fhd.FilePath = item.FullName;
                    fhd.FileHash = Hash(item.FullName);
                    fhd.Length = item.Length;
                    fhdList[fhdList.Length - 1] = fhd;
                }
                foreach (DirectoryInfo subdir in subdirs)
                {
                    FileHashData[] fhd_subdir = GetFiles(subdir.FullName);
                    foreach (FileHashData fhd in fhd_subdir)
                    {
                        Array.Resize(ref fhdList, fhdList.Length + 1);
                        fhdList[fhdList.Length - 1] = fhd;
                    }
                }
            }
            return fhdList;
        }

        public static void FileHash_Save(FileHashData[] fhdList)
        {
            FileStream fs = new FileStream("FileHashData.data", FileMode.OpenOrCreate);
            XmlSerializer xs = XmlSerializer.FromTypes(new[] { typeof(FileHashData[]) })[0];
            xs.Serialize(fs, fhdList);
            fs.Close();
        }

        public static FileHashData[] FileHash_Read()
        {
            if (File.Exists("FileHashData.data"))
            {
                FileStream fs = new FileStream("FileHashData.data", FileMode.OpenOrCreate);
                XmlSerializer xs = XmlSerializer.FromTypes(new[] { typeof(FileHashData[]) })[0];
                FileHashData[] fhdList = (FileHashData[])xs.Deserialize(fs);
                fs.Close();
                return fhdList;
            }
            return null;
        }

        public static string Hash(string FilePath)
        {
            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            byte[] Hashed = null;
            try
            {
                Hashed = MD5.Create().ComputeHash(fs);
            }
            catch (Exception) { }
            fs.Close();
            return BitConverter.ToString(Hashed);
        }
    }
}