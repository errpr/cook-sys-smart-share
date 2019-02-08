using System.Collections.Generic;
using Client.Utils;
using CommandLine;
using static Core.Constants;

namespace Client.Options
{
    [Verb("upload", HelpText = "Uploads a file or files.")]
    public class UploadOptions
    {   
        [Value(0, MetaName = "files", HelpText = "Path(s) to the file(s) to be uploaded.", Required = true)]
        public IEnumerable<string> FilePaths { get; set; }

        [Option('p', "password", HelpText = "Password for the file(s).", Required = false)]
        public string Password { get; set; } = PasswordGenerator.Generate();

        [Option('m', "maxDownloads", HelpText = "Maximum allowed downloads", Required = false)]
        public int? MaxDownloads { get; set; }

        [Option('d', "duration", Default = DURATION_DEFAULT, HelpText = "How long (in minutes) the file(s) should be available.", Required = false)]
        public int Duration { get; set; }
    }
}