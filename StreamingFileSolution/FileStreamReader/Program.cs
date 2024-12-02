using Microsoft.VisualBasic;
using System.Diagnostics;

namespace FileStreamReader
{
    internal class Program
    {
        static string filePath = @"F:\Projects\200MB_text_file.txt";
        static  void Main(string[] args)
        {
            Task.Factory.StartNew(()=> Run());
            Task.Factory.StartNew(()=> RunChunk());
            Task.Factory.StartNew(()=> RunBuffered());
            Console.ReadLine();
        }


        private static async Task Run()
        {
            int counter = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            await foreach (var data in ReadFileAsync())
            {
                if (counter % 10000 == 0)
                {
                    Console.Write(data + "  " + counter);
                }
                counter++;
            }
            sw.Stop();
            Console.Write($"Total time taken to read 200 MB file {sw.Elapsed.TotalSeconds}, no of lines: {counter}");
        }

        private static async Task RunChunk()
        {
            int counter = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            await foreach (var data in ReadFileInChunksAsync(10000))
            {
                if (counter % 10000 == 0)
                {
                    Console.Write("ChunkData " + "  " + counter + " "+ System.Text.Encoding.Default.GetString(data)) ;
                }
                counter++;
            }
            sw.Stop();
            Console.Write($"Total time taken to read 200 MB file with Chunk {sw.Elapsed.TotalSeconds}, no of lines: {counter}");
        }

        private static async Task RunBuffered()
        {
            int counter = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            await foreach (var data in ReadBufferedLinesAsync())
            {
                if (counter % 10000 == 0)
                {
                    Console.Write("Buffered " + "  " + counter + " " + data);
                }
                counter++;
            }
            sw.Stop();
            Console.Write($"Total time taken to read 200 MB file with Buffered {sw.Elapsed.TotalSeconds}, no of lines: {counter}");
        }

        public static async IAsyncEnumerable<byte[]> ReadFileInChunksAsync(int chunkSize)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var buffer = new byte[chunkSize];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    yield return buffer[..bytesRead];
                }
            }
        }

        public static async IAsyncEnumerable<string> ReadBufferedLinesAsync()
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var bufferedStream = new BufferedStream(fileStream))
            using (var reader = new StreamReader(bufferedStream))
            {
                while (!reader.EndOfStream)
                {
                    yield return await reader.ReadLineAsync();
                }
            }
        }

        private static async IAsyncEnumerable<string> ReadFileAsync()
        {
            
            long counter = 0;
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    counter++;
                     yield return await reader.ReadLineAsync() ;
                }
            }
           
        }

        private static Task ReadFile()
        {
            long counter = 0;
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    reader.ReadLine();
                    counter++;
                }
            }
            return Task.CompletedTask;
        }
    }
}
