using System;
using static Core.Constants;

namespace Core.Dto
{
    [Serializable]
    public class FileRequestInfo
    {
        public RequestType RequestType { get; set; }
        public string FileName { get; set; }
        public int DurationMinutes { get; set; } = DURATION_DEFAULT;
        public int? MaxDownloadCount { get; set; }
        public string Password { get; set; }
    }
    
    public enum RequestType
    {
        Upload,
        Download,
        View
    }
}
