using System;
using System.IO;
using System.Reflection;

namespace BasicDemos
{
    class Program
    {
        static void Main(string[] args)
        {
            string? exePath = Assembly.GetExecutingAssembly().Location;
            // Get the directory containing the .exe file
            if (exePath != null)
            {
                string? exeDirectory = Path.GetDirectoryName(exePath);
                // Set the current working directory to that folder
                if (exeDirectory != null)
                    Directory.SetCurrentDirectory(exeDirectory);
            }

            if (!Directory.Exists("pdfs"))
                Directory.CreateDirectory("pdfs");


            Console.WriteLine("Running Haru PDF Library Basic Demos...\n");

            Console.WriteLine("Running FontDemo...");
            FontDemo.Run();
            Console.WriteLine("FontDemo completed.\n");

            Console.WriteLine("Running LineDemo...");
            LineDemo.Run();
            Console.WriteLine("LineDemo completed.\n");

            Console.WriteLine("Running TextDemo...");
            TextDemo.Run();
            Console.WriteLine("TextDemo completed.\n");

            Console.WriteLine("Running ImageDemo...");
            ImageDemo.Run();
            Console.WriteLine("ImageDemo completed.\n");

            Console.WriteLine("Running OutlineDemo...");
            OutlineDemo.Run();
            Console.WriteLine("OutlineDemo completed.\n");

            Console.WriteLine("Running SlideShowDemo...");
            SlideShowDemo.Run();
            Console.WriteLine("SlideShowDemo completed.\n");

            Console.WriteLine("Running EncryptedDemo...");
            EncryptedDemo.Run();
            Console.WriteLine("EncryptedDemo completed.\n");

            Console.WriteLine("Running InternationalDemo...");
            InternationalDemo.Run();
            Console.WriteLine("InternationalDemo completed.\n");

            Console.WriteLine("Running Type1FontDemo...");
            Type1FontDemo.Run();
            Console.WriteLine("Type1FontDemo completed.\n");

            Console.WriteLine("Running CJKDemo...");
            CJKDemo.Run();
            Console.WriteLine("CJKDemo completed.\n");

            Console.WriteLine("Running PageLabelAndBoundaryDemo...");
            PageLabelAndBoundaryDemo.Run();
            Console.WriteLine("PageLabelAndBoundaryDemo completed.\n");

            Console.WriteLine("All demos completed successfully!");
            Console.WriteLine("PDF files have been generated in the current directory.");
        }
    }
}
