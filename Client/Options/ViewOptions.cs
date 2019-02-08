using CommandLine;

namespace Client.Options
{
    [Verb("view", HelpText = "View information about an uploaded file, provided the correct password is given")]
    public class ViewOptions
    {
        [Value(0, MetaName = "filename", HelpText = "Unique name of file", Required = true)]
        public string FileName { get; set; }

        [Value(1, MetaName = "password", HelpText = "Password used to access file", Required = true)]
        public string Password { get; set; }
    }
}