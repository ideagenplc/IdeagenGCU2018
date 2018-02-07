using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExampleS3Upload
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter path of file to upload to s3: ");
            var filePath = Console.ReadLine();
            UploadObject(filePath);
        }

        private static void UploadObject(string filePath)
        {
            var url = GeneratePreSignedUrl(filePath);
            var httpRequest = WebRequest.Create(url) as HttpWebRequest;
            httpRequest.Method = "PUT";
            using (var dataStream = httpRequest.GetRequestStream())
            {
                var buffer = new byte[18000];
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        dataStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            var response = httpRequest.GetResponse() as HttpWebResponse;
            Console.WriteLine(response);
        }

        private static string GeneratePreSignedUrl(string filePath)
        {
            var request = CreateHttpWebRequest(filePath);
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return JsonConvert.DeserializeObject(reader.ReadToEnd()).ToString();
            }
        }

        private static HttpWebRequest CreateHttpWebRequest(string filePath)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://g8g3qxyqxl.execute-api.eu-west-1.amazonaws.com/Stage/TimelineEventAttachment/GenerateAttachmentPresignedUrl");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Headers.Add("TenantId", "IDGDev");
            request.Headers.Add("AuthToken", "1337");
            request.Headers.Add("AttachmentId", Path.GetFileName(filePath));
            return request;
        }
    }
}
