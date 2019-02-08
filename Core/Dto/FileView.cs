using System;

namespace Core.Dto
{
    public class FileView
    {
        public string FileName { get; set; }
        public Nullable<int> RemainingDownloads { get; set; }
        public int RemainingMinutes { get; set; }
    }
}
