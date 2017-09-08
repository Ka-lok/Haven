using HavenWcfService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HavenConsoleClient
{
    class Program
    {
        public static readonly EndpointAddress EndPoint = new EndpointAddress("http://192.168.1.66:60235/Service1.svc");
        Service1Client client;

        static void Main(string[] args)
        {

            Program p = new Program();

            p.InitializeServiceClient();


        }


        private void InitializeServiceClient()
        {
            BasicHttpBinding binding = CreateBasicHttp();

            client = new Service1Client(binding, EndPoint);
            CompositeType a = new CompositeType();
            a.StringValue = "banana";
            Console.WriteLine(a.StringValue);
            //Console.WriteLine(client.GetDataUsingDataContract(a).StringValue);
            Console.WriteLine(client.GetData(1));
            client.Close();
            Console.ReadLine();
        }

        private static BasicHttpBinding CreateBasicHttp()
        {
            BasicHttpBinding binding = new BasicHttpBinding
            {
                Name = "basicHttpBinding",
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647
            };
            TimeSpan timeout = new TimeSpan(0, 0, 30);
            binding.SendTimeout = timeout;
            binding.OpenTimeout = timeout;
            binding.ReceiveTimeout = timeout;
            return binding;
        }
    }
}
