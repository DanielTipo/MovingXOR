using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diff
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: diff \"file1\" \"file2\"");
                return;
            }

            var file1 = args[0];
            if (!File.Exists(file1))
            {
                Console.Error.WriteLine("File1 not found");
                return;
            }
            var file2 = args[1];
            if (!File.Exists(file2))
            {
                Console.Error.WriteLine("File2 not found");
                return;
            }

            int numofdiffs = 0;
            int length = 0;
            try {
                using (var f1 = new FileStream(file1, FileMode.Open))
                using (var f2 = new FileStream(file2, FileMode.Open))
                using (var o = new FileStream(file1 + "_" + file2 + ".diff", FileMode.OpenOrCreate))
                {

                    length = (int)Math.Min(f1.Length, f2.Length);
                    int read1 = 0;
                    int read2 = 0;
                    var r1 = new byte[512];
                    var r2 = new byte[512];
                    while (0 < (read1 = f1.Read(r1, 0, 512))
                        && 0 < (read2 = f2.Read(r2, 0, 512)))
                    {
                        var ob = new byte[Math.Min(read1, read2)];
                        for (int i = 0; i < read1 && i < read2; i++)
                        {
                            if (r1[i] == r2[i])
                            {
                                ob[i] = (byte)r1[i];
                                numofdiffs++;
                            }
                            else
                                ob[i] = 0;
                        }
                        o.Write(ob, 0, ob.Length);
                    }
                }


                Console.WriteLine("Difference: {0:N2}%, {1}", ((double)(length - numofdiffs) / length * 100), numofdiffs);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Something happened: " + e.Message);
                return;
            }
        }
    }
}
