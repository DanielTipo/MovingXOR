using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingXor
{
    class Program
    {
        static void Main(string[] args)
        {   
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: xor2 \"file\" \"code\"");
                return;
            }
            var code = args[1];
            if (code.Length < 8)
            {
                Console.Error.WriteLine("Code is shorter than 8 characters");
                return;
            }
            var file = args[0];
            if (!File.Exists(file))
            {
                Console.Error.WriteLine("File not exists");
                return;
            }
            var file2 = file + ".xor2";
            Console.Write("Working.");
            try
            {
                using (var sr = new FileStream(file, FileMode.Open))
                using (var sw = new FileStream(file2, FileMode.OpenOrCreate))
                {
                    var cpos = 0;
                    var rpos = 1;
                    var read = 0;
                    var rbuffer = new byte[512];
                    while (0 < (read = sr.Read(rbuffer, 0, 512)))
                    {
                        var rnd = new Random(getcode(cpos, rpos, code));
                        var wbuffer = new byte[read];
                        var xbuffer = new byte[read];
                        rnd.NextBytes(xbuffer);
                        for (int i = 0; i < read; ++i)
                            wbuffer[i] = (byte)((byte)rbuffer[i] ^ xbuffer[i]);
                        sw.Write(wbuffer, 0, read);
                        cpos++;
                        rpos++;
                        if (cpos == code.Length) cpos = 0;
                        if (rpos % 1000 == 0) Console.Write('.');
                    }
                }
                Console.WriteLine("Done.");
                Console.WriteLine("Output: " + file2);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Something happened: " + e.Message);
                return;
            }
        }

        private static int getcode(int cpos, int rpos, string code)
        {
            int result = 0;
            int pos = 0;
            int rposs = 128 - rpos % 255;
            while (pos++ < code.Length)
            {
                result += (int)Math.Pow(1.4142135, pos) * (int)code[cpos] * rposs;
                if (cpos >= code.Length - 1) cpos = 0;
                cpos++;
            }
            return result;
        }
    }
}
