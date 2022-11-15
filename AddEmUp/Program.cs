using AddEmUp;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using static System.Formats.Asn1.AsnWriter;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                try
                {
                    StringBuilder results = new StringBuilder("");

                    Winner winner = new Winner(args[1]);

                    // Check if an input file is provided.
                    if (args.Contains("--in"))
                    {
                        results = winner.ProcessUserScores(winner: winner);
                    }

                    // Check if the output file is expected.
                    if (args.Contains("--out"))
                    {
                        winner.WriteToOutputFile(filename: args[3], results: results.ToString());
                    }

                }
                catch (Exception e)
                {
                    Winner winner = new Winner(args[1]);
                    StringBuilder errorMessage = new StringBuilder("ERROR:\n");
                    errorMessage.Append(e.Message);
                    winner.WriteToOutputFile(filename: args[3], results: errorMessage.ToString());
                }
            }
            else
            {
                Console.WriteLine("ERROR: No argurments provided");
            }

        }
    }
}