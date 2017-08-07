using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertSyncPhotosManual
{
    class Program
    {
        static void Main(string[] args)
        {
            Converter converter = new Converter();
            string sourceDirectory = converter.SourceDirectoryName;

            Console.WriteLine("{0} - Start manual convert.", DateTime.Now.ToString());
            var allFiles = Directory.EnumerateFiles(sourceDirectory, "*.jpg", SearchOption.AllDirectories);
            Console.WriteLine("Total file(s): {0}", allFiles.Count());
            foreach (var item in allFiles)
            {
              converter.ResizeAsync(item);
            }
            Console.WriteLine("{0} - Finish manual convert.", DateTime.Now.ToString());

            Console.ReadKey();
        }
    }
}
