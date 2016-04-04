using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace WunderBraSolver
{
    class Program
    {
        static void Main()
        {
            var words = DownloadWordList();

            var sw = new Stopwatch();
            sw.Start();

            var solver = new Solver(words);
            var wordCount = solver.Solve();

            sw.Stop();

            Console.WriteLine($"Found {wordCount} in {sw.ElapsedMilliseconds} ms");
        }

        static IReadOnlyList<string> DownloadWordList()
        {
            var list = new List<string>();
            using (var wc = new WebClient())
            {
                var data = wc.DownloadString(@"https://wunder-draft.firebaseapp.com/dict.txt");
                var lines = data.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    list.Add(line.Trim().ToUpper());
                }
                return list;
            }
        }
    }
}
