using System;
using System.IO;
using System.Linq;
using System.Text;
using FastCdcFs.Net;

namespace Benchmark
{
    class Program
    {
        private const string TestDataRoot = "TestData";
        private const string BinaryDataDir = "Binary";
        private const string TextDataDir = "Text";

        static void Main(string[] args)
        {
            Console.WriteLine("Generating test data...");
            GenerateBinaryFiles();
            GenerateTextFiles();
            Console.WriteLine("Test data generation complete.");

            Console.WriteLine("\nRunning benchmarks...");
            RunBenchmark(Path.Combine(TestDataRoot, BinaryDataDir), "Binary Data");
            RunBenchmark(Path.Combine(TestDataRoot, TextDataDir), "Text Data");
        }

        private static void RunBenchmark(string directoryPath, string benchmarkName)
        {
            Console.WriteLine($"\n--- {benchmarkName} Benchmark ---");

            var sourceDirectory = new DirectoryInfo(directoryPath);
            var totalSize = sourceDirectory.GetFiles().Sum(f => f.Length);

            var archivePath = Path.GetTempFileName();
            try
            {
                var writer = new FastCdcFsWriter(o => o
                    .WithChunkSizes(4 * 1024, 32 * 1024, 128 * 1024)
                    .WithSolidBlockSize(4 * 1024));
                writer.AddDirectory(directoryPath);
                writer.Build(archivePath);

                var archiveSize = new FileInfo(archivePath).Length;
                var ratio = (double)archiveSize / totalSize;

                Console.WriteLine($"Original Size: {totalSize / 1024 / 1024} MB");
                Console.WriteLine($"Archive Size: {archiveSize / 1024 / 1024} MB");
                Console.WriteLine($"Compression Ratio: {ratio:P2}");
            }
            finally
            {
                if (File.Exists(archivePath))
                {
                    File.Delete(archivePath);
                }
            }
        }

        private static void GenerateBinaryFiles()
        {
            var binaryDir = Path.Combine(TestDataRoot, BinaryDataDir);
            Directory.CreateDirectory(binaryDir);

            var random = new Random();
            var commonData = new byte[4 * 1024 * 1024]; // 4MB common data
            random.NextBytes(commonData);

            for (int i = 0; i < 20; i++)
            {
                var filePath = Path.Combine(binaryDir, $"file{i}.bin");
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    fs.Write(commonData, 0, commonData.Length);
                    var remainingData = new byte[1 * 1024 * 1024]; // 1MB unique data
                    random.NextBytes(remainingData);
                    fs.Write(remainingData, 0, remainingData.Length);
                }
            }
        }

        private static void GenerateTextFiles()
        {
            var textDir = Path.Combine(TestDataRoot, TextDataDir);
            Directory.CreateDirectory(textDir);

            var random = new Random();
            var baseHtml = "<html><body><h1>Hello, {0}!</h1><svg>{1}</svg></body></html>";
            var baseSvg = "<circle cx='50' cy='50' r='{0}' stroke='black' stroke-width='3' fill='red' />";

            for (int i = 0; i < 100; i++)
            {
                var svgContent = string.Format(baseSvg, random.Next(10, 40));
                var htmlContent = string.Format(baseHtml, $"User{i}", svgContent);
                var filePath = Path.Combine(textDir, $"file{i}.html");
                File.WriteAllText(filePath, htmlContent, Encoding.UTF8);
            }
        }
    }
}
