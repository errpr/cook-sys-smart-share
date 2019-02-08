using System;
using System.IO;
using Client.Options;
using CommandLine;
using static Core.Constants;

namespace Client
{
    public class Program
    {
        private static readonly bool developmentMode = true;
        
        public static void Main(string[] args)
        {
            if (args.Length <= 0 && developmentMode)
            {
                Tester.RunTestArgs();
            }
            else
            {
                RunCommandArgs(args);
            }
        }

        public static void RunCommandArgs(string[] args)
        {
            Parser.Default.ParseArguments<DownloadOptions, UploadOptions, ViewOptions>(args)
                .MapResult(
                    (DownloadOptions opts) => ExecuteDownloadAndReturnExitCode(opts),
                    (UploadOptions opts) => ExecuteUploadAndReturnExitCode(opts),
                    (ViewOptions opts) => ExecuteViewAndReturnExitCode(opts),
                    errs => 1);
        }

        public static int ExecuteDownloadAndReturnExitCode(DownloadOptions options)
        {
            foreach (var fileName in options.FileNames)
            {
                Console.WriteLine($"Attempting to download file: {fileName}");
                var success = Api.Download(fileName, options.Password);
                if (!success)
                {
                    Console.WriteLine("Operation unsuccessful.");
                    continue;
                }
                Console.WriteLine($"Success! Downloaded {fileName}");
            }
            return 0;
        }


        public static int ExecuteUploadAndReturnExitCode(UploadOptions options)
        {
            if (options.Duration < DURATION_MINIMUM || options.Duration > DURATION_MAXIMUM)
            {
                Console.WriteLine($"Duration value out of range. (Expected {DURATION_MINIMUM} - {DURATION_MAXIMUM}).");
                return 1;
            }

            Console.WriteLine("Uploading...");
            foreach (var path in options.FilePaths)
            {
                var file = new FileInfo(path);

                var success = Api.Upload(file, options.MaxDownloads, options.Duration, options.Password);
                if (!success)
                {
                    Console.WriteLine("Operation unsuccessful.");
                    return 1;
                }
                Console.WriteLine($"Success! File: {file.Name} Password: {options.Password}");
            }
            return 0;
        }

        public static int ExecuteViewAndReturnExitCode(ViewOptions options)
        {
            try
            {
                var fileView = Api.View(options.FileName, options.Password);
                Console.WriteLine($"File:                {fileView.FileName}");
                Console.WriteLine($"Time Remaining:      {fileView.RemainingMinutes} Minutes");
                Console.WriteLine($"Downloads Remaining: {fileView.RemainingDownloads?.ToString() ?? "Infinite"}");
            }
            catch
            {
                Console.WriteLine("Operation unsuccessful.");
                return 1;
            }
            return 0;
        }
    }
}