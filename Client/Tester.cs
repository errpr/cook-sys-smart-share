using System;
using System.Collections.Generic;

namespace Client
{
    public sealed class Tester
    {
        public static string TESTFILENAME = "bitmap.bmp";
        public static string TESTFILENAME2 = "testfile2.bmp";
        public static string TESTFILENAME3 = "testfile3.bmp";
        public static string TESTFILEPATH = $"c:\\Code\\testfiles\\{TESTFILENAME}";
        public static string TESTFILEPATH2 = $"c:\\Code\\testfiles\\{TESTFILENAME2}";
        public static string TESTFILEPATH3 = $"c:\\Code\\testfiles\\{TESTFILENAME3}";

        private static readonly List<string[]> TestArgs = new List<string[]>
        {
            new[] {"upload", "c:\\Code\\testfiles\\hello.txt", "-p", "hellotest"},
            new[] {"download", "hello.txt", "-p", "hellotest"},
            new[] {"upload", TESTFILEPATH, "-p", "pwpwpw" },
            new[] {"download", TESTFILENAME, "-p", "pwpwpw" },
            new[] {"upload", TESTFILEPATH2, TESTFILEPATH3, "-p", "pazzwerd"},
            new[] {"download", TESTFILENAME2, TESTFILENAME3, "-p", "pazzwerd"},
            new[] {"view", TESTFILENAME, "pwpwpw"}
        };

        public static void RunTestArgs()
        {
            int runCount = 1;
            foreach (var args in TestArgs)
            {
                Console.WriteLine($"\n*** RUN #{runCount++}, args: { String.Join(" ", args)}");
                Program.RunCommandArgs(args);
            }
        }
    }
}