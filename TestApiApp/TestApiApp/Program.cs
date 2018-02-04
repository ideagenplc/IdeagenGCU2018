using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestApiApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Http Method:");
            var httpMethod = Console.ReadLine();
            Console.WriteLine("Enter API path");
            var path = Console.ReadLine();
            var request =
                "{\r\n    \"TenantId\" : \"IDGDev\",\r\n    \"AuthToken\" : \"1337\",\r\n    \"TimelineId\":\"e38743fa-4914-4f5a-ad6e-7c478cb74860\",\r\n    \"Title\":\"Grounded Flight: G13GB\"\r\n} ";
            using (var client = new WebClient())
            {
                client.UploadData("https://dspvtlvupl.execute-api.eu-west-1.amazonaws.com/Stage/" + path, httpMethod, Encoding.ASCII.GetBytes(request));
            } 
            Console.Read();
        }
    }
}
