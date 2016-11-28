using System;

namespace CopyApp
{
    [Serializable]
    public class FileHashData
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileHash { get; set; }
        public long Length { get; set; }

        public FileHashData() {  }
        public FileHashData(string FileName)
        {
            this.FileName = FileName;
        }
    }
}
