using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TakeHome
{
    public static class MatrixService
    {
        private static readonly HttpClient _httpClient = new HttpClient() { BaseAddress = new Uri("http://100.29.16.196:8080/api/"), Timeout = TimeSpan.FromSeconds(10) };
        private const int maxRetry = 3;

        public static async Task InitializeMatrix(int n)
        {
            var retryCount = 0;
            while (retryCount < maxRetry)
            {
                try
                {
                    var content = new StringContent($"{{\"n\": {n}}}", Encoding.UTF8, "application/json");
                    var result = await _httpClient.PostAsync("matrix/init", content);
                    result.EnsureSuccessStatusCode();
                    return;
                }
                catch
                {
                    retryCount++;
                    await Task.Delay(1000);
                }
            }
            throw new Exception($"Failed to initialize the Matriz after {maxRetry} attempts.");
        }

        public static async Task<int[][]> FetchMatrixRows(string matrixName, int n)
        {
            var matrix = new int[n][];
            var tasks = new Task[n];

            Parallel.For(0, n, i =>
            {
                tasks[i] = FetchMatrixRow(matrixName, i, matrix);
            });

            await Task.WhenAll(tasks);
            return matrix;
        }

        private static async Task FetchMatrixRow(string matrixName, int rowIndex, int[][] matrix)
        {
            var retryCount = 0;
            while (retryCount < maxRetry)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"matrix/{matrixName}/row/{rowIndex}");
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    var rowData = JsonConvert.DeserializeObject<dynamic>(json);
                    matrix[rowIndex] = rowData.row.ToObject<int[]>();
                    return;
                }
                catch
                {
                    retryCount++;
                    await Task.Delay(1000);
                }
            }
            throw new Exception($"Failed to retrieve row {rowIndex} from matrix '{matrixName}' after {maxRetry} attempts.");
        }

        public static int[][] MultiplyMatrices(int[][] matrixA, int[][] matrixB, int n)
        {
            var transposedB = new int[n][];
            for (int i = 0; i < n; i++)
            {
                transposedB[i] = new int[n];
                for (int j = 0; j < n; j++)
                {
                    transposedB[i][j] = matrixB[j][i];
                }
            }

            var resultMatrix = new int[n][];
            Parallel.For(0, n, i =>
            {
                resultMatrix[i] = new int[n];
                for (int j = 0; j < n; j++)
                {
                    int sum = 0;
                    for (int k = 0; k < n; k++)
                    {
                        sum += matrixA[i][k] * transposedB[j][k];
                    }
                    resultMatrix[i][j] = sum;
                }
            });

            return resultMatrix;
        }

        public static string ComputeMD5Hash(int[][] matrix)
        {
            using (var md5 = MD5.Create())
            {
                var flattenedMatrix = string.Join(",", matrix.SelectMany(row => row));
                
                var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(flattenedMatrix));
                
                return Convert.ToBase64String(hashBytes);
            }
        }


        public static async ValueTask<string> ValidateHash(string hash)
        {
            var retryCount = 0;
            while (retryCount < maxRetry)
            {
                try
                {
                    var response = await _httpClient.PostAsync("matrix/validate",
                        new StringContent($"{{\"Hash\": \"{hash}\"}}", Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
                catch
                {
                    retryCount++;
                    await Task.Delay(1000);
                }
            }
            throw new Exception($"Failed to validate the hash after {maxRetry} attempts.");
        }
    }
}
