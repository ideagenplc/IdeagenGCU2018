using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace ExampleS3Upload
{
    class Program
    {
        private const string UploadAction = "UPLOAD";
        private const string GetAction = "GET";
        private const string BaseUrl = "https://yx2yxri4le.execute-api.eu-west-1.amazonaws.com/Prod/";
        private const string UploadActionUrl = "TimelineEventAttachment/GenerateUploadPresignedUrl";
        private const string GetActionUrl = "TimelineEventAttachment/GenerateGetPresignedUrl";

        private static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Enter Action(UPLOAD/GET): ");
                    var action = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(action))
                    {
                        Console.WriteLine("Please enter either Upload or Get");
                        continue;
                    }
                    switch (action.ToUpperInvariant())
                    {
                        case UploadAction:
                            UploadObject();
                            break;
                        case GetAction:
                            DownloadObject();
                            break;
                        default:
                            Console.WriteLine("Please enter either Upload or Get");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static void UploadObject()
        {
            Console.WriteLine("Enter path of file to upload to s3: ");
            var filePath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("Must enter filepath to upload object");
                return;
            }
            var url = GenerateUploadPreSignedUrl(filePath);
            var httpRequest = WebRequest.Create(url) as HttpWebRequest;
            httpRequest.Method = "PUT";
            using (var dataStream = httpRequest.GetRequestStream())
            {
                var buffer = new byte[18000];
                using (var fileStream = new FileStream(Path.GetFileName(filePath), FileMode.Open, FileAccess.Read))
                {
                    var bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        dataStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            var response = httpRequest.GetResponse() as HttpWebResponse;
            Console.WriteLine($"Response status code: {response.StatusCode}");
            Console.ReadLine();
        }

        private static void DownloadObject()
        {
            Console.WriteLine("Enter name of file to download from s3: ");
            var fileName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                Console.WriteLine("Must enter filename to download file");
                return;
            }
            using (var wc = new WebClient())
            {
                wc.DownloadProgressChanged += DownloadProgressCallback;
                wc.DownloadFileAsync(new Uri(GenerateGetPreSignedUrl(fileName)), fileName);
            }
        }

        private static void UploadProgressCallback(object sender, UploadProgressChangedEventArgs e)
        {
            // Displays the operation identifier, and the transfer progress.
            Console.WriteLine($"Uploaded {e.BytesSent} of {e.TotalBytesToSend} bytes. {e.ProgressPercentage} % complete...");
        }
        private static void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            // Displays the operation identifier, and the transfer progress.
            Console.WriteLine($"Downloaded {e.BytesReceived} of {e.TotalBytesToReceive} bytes. {e.ProgressPercentage} % complete...");
        }

        private static string GenerateUploadPreSignedUrl(string fileName)
        {
            var request = CreateHttpWebRequest(fileName, UploadActionUrl);
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return JsonConvert.DeserializeObject(reader.ReadToEnd()).ToString();
            }
        }

        private static string GenerateGetPreSignedUrl(string fileName)
        {
            var request = CreateHttpWebRequest(fileName, GetActionUrl);
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return JsonConvert.DeserializeObject(reader.ReadToEnd()).ToString();
            }
        }

        private static HttpWebRequest CreateHttpWebRequest(string fileName, string actionUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create($"{BaseUrl}{actionUrl}");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Headers.Add("TenantId", "IDGDev");
            request.Headers.Add("AuthToken", "1337");
            request.Headers.Add("AttachmentId", fileName);
            return request;
        }
    }
}
