using System;

namespace BasicDemos
{
    class Program
    {
        static void Main(string[] args)
        {
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

            Console.WriteLine("All demos completed successfully!");
            Console.WriteLine("PDF files have been generated in the current directory.");
        }
    }
}
