using System.Collections.Generic;
using CommandLine;

namespace Client.Options
{
    [Verb("download", HelpText = "Downloads a file (or files) provided the correct password is given")]
    public class DownloadOptions
    {
        [Value(0, MetaName = "files", HelpText = "Unique name(s) of file(s) to be downloaded", Required = true)]
        public IEnumerable<string> FileNames { get; set; }
        
        [Option('p', "password", HelpText = "Password used to access file(s)", Required = true)]
        public string Password { get; set; }
    }
}