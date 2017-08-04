using Android.App;
using Android.Widget;
using Android.OS;
using System.ServiceModel;
using System;

namespace WcfAndroidApp
{
    [Activity(Label = "WcfAndroidApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public static readonly EndpointAddress EndPoint = new EndpointAddress("http://192.168.1.71:60235/HelloWorldService.svc");

        Button GetDataButton;
        Service1Client client;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
             SetContentView (Resource.Layout.Main);
            InitializeHelloWorldServiceClient();

            GetDataButton = FindViewById<Button>(Resource.Id.GetDataButton);

            GetDataButton.Click += GetDataButton_Click;

        }

        private void GetDataButton_Click(object sender, System.EventArgs e)
        {
            GetDataButton.Text = client.GetData(1);
        }

        private void InitializeHelloWorldServiceClient()
        {
            BasicHttpBinding binding = CreateBasicHttp();

            client = new Service1Client(binding, EndPoint);
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

