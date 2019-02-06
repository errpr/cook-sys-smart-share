﻿using System;
using System.Collections.Generic;
using Client.Verbs;
using CommandLine;
using static Client.Verbs.DownloadOptions;
using static Client.Verbs.UploadOptions;

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
            Parser.Default.ParseArguments<DownloadOptions, UploadOptions>(args)
                .MapResult(
                    (DownloadOptions opts) => ExecuteDownloadAndReturnExitCode(opts),
                    (UploadOptions opts) => ExecuteUploadAndReturnExitCode(opts),
                    errs => 1);
        }
    }
}