using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TakeHome
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://100.29.16.196:8080");

            int n = 0;
            bool numberIsValid = false;

            while (!numberIsValid)
            {
                Console.WriteLine("Please provide a number between 1 and 1000: ");
                var clientNumber = Console.ReadLine();
                numberIsValid = int.TryParse(clientNumber, out n) && n >= 1 && n <= 1000;

                if (!numberIsValid)
                {
                    Console.WriteLine("Type a valid Integer number between 1 e 1000.\n");
                }
            }

            Console.WriteLine("\n*** Initialize matrix ***");
            var initStartTime = DateTime.Now;
            await MatrixService.InitializeMatrix(n);
            var initEndTime = DateTime.Now;

            Console.WriteLine("*** Get 'A' matrix rows ***");
            var fetchARowsStartTime = DateTime.Now;
            var matrixARows = await MatrixService.FetchMatrixRows("A", n);
            var fetchARowsEndTime = DateTime.Now;

            Console.WriteLine("*** Get 'B' matrix rows  ***");
            var fetchBRowsStartTime = DateTime.Now;
            var matrixBRows = await MatrixService.FetchMatrixRows("B", n);
            var fetchBRowsEndTime = DateTime.Now;

            Console.WriteLine("*** Multiply matrices ***");
            var multiplyStartTime = DateTime.Now;
            var resultMatrix = MatrixService.MultiplyMatrices(matrixARows, matrixBRows, n);
            var multiplyEndTime = DateTime.Now;

            Console.WriteLine("*** Validate MD5 hash ***");
            var md5StartTime = DateTime.Now;
            var base64Hash = MatrixService.ComputeMD5Hash(resultMatrix);
            Console.WriteLine("### Send hash to validate: " + base64Hash);
            var validateResponse = await MatrixService.ValidateHash(base64Hash);
            var md5EndTime = DateTime.Now;
            Console.WriteLine("MD5 response: " + validateResponse);

            var totalEndTime = DateTime.Now;
            Console.WriteLine($"\n*** Init Message Initialized matrices A & B with size {n} x {n} ***");
            Console.WriteLine($"Get A Rows took: {(fetchARowsEndTime - fetchARowsStartTime).TotalSeconds:F2} seconds");
            Console.WriteLine($"Get B Rows took: {(fetchBRowsEndTime - fetchBRowsStartTime).TotalSeconds:F2} seconds");
            Console.WriteLine($"Multiply took: {(multiplyEndTime - multiplyStartTime).TotalSeconds:F2} seconds");
            Console.WriteLine($"Validation MD5 response: {validateResponse}");
            Console.WriteLine($"Validation MD5 took: {(md5EndTime - md5StartTime).TotalSeconds:F2} seconds");
            Console.WriteLine($"Total took: {(totalEndTime - initStartTime).TotalSeconds:F2} seconds");

        }
    }
}
