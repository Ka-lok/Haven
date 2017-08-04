using Android.App;
using Android.Widget;
using Android.OS;
using System.ServiceModel;
using System;
using Android.Telephony;
using HavenWcfService;

using System.Diagnostics;
using System.Collections.Generic;

namespace HavenAndroidApp
{
    [Activity(Label = "HavenAndroidApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public static readonly EndpointAddress EndPoint = new EndpointAddress("http://192.168.1.71:60235/Service1.svc");

     Contact [] contactListArray;
        Service1Client client;
        Button GetDataButton;
        ListView ContactList;
        ArrayAdapter<String> ListAdapter;
        TelephonyManager mTelephonyMgr;
        String Number;
 
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            
          //  mTelephonyMgr = (TelephonyManager)GetSystemService(TelephonyService);
            //Number = mTelephonyMgr.Line1Number;
            
            SetContentView (Resource.Layout.Main);

            ContactList = FindViewById<ListView>(Resource.Id.ContactList);

            GetDataButton = FindViewById<Button>(Resource.Id.GetDataButton);
            GetDataButton.Click += GetDataButton_Click;
            LoadContacts();
            //InitializeServiceClient();
            
        }

        private void GetDataButton_Click(object sender, System.EventArgs e)
        {
           
           
            GetDataButton.Text =  (client.CheckIn(DateTime.Now)).ToString();
            //GetDataButton.Text = client.CheckOrAddNumber("2222").ToString();
         
        }

        private void LoadContacts()
        {
            
           ContactList.Adapter = ListAdapter;
           
            ContactsAdapter contactsAdapter = new ContactsAdapter(this);


            System.Diagnostics.Debug.WriteLine (" Contact list from contact adapter is "  + contactsAdapter._contactList.Count);
            contactListArray = (contactsAdapter._contactList).ToArray();
            
            ContactList.Adapter = contactsAdapter;

        }

        private void InitializeServiceClient()
        {
            BasicHttpBinding binding = CreateBasicHttp();
            client = new Service1Client(binding, EndPoint);
            System.Diagnostics.Debug.WriteLine("client test please work " + client.GetData(1));
            Boolean numberAlreadyExists = false;
            
           

            ////numberAlreadyExists = client.CheckOrAddNumber(Number);
            //numberAlreadyExists = client.CheckAndAddUsersNumber("222222"); //check or add the phone user number NOT THEIR CONTACTS

            //ContactData[] contactDataArray;

            //if (numberAlreadyExists)
            //{
            //    GetDataButton.Text = "The Number already Exists";

            //    contactDataArray = client.CheckAndAddUsersPhoneContactsForExistingUser(contactListArray, "222222");

            //    //check for new and then get data for all other contacts
            //}
            //else
            //{
            //    //


            //    GetDataButton.Text = "Number does not exist";

            //    contactDataArray = client.CheckAndAddUsersPhoneContactsForNewUser(contactListArray);



            //}

            //List<ContactData> contactDataList = new List<ContactData>(contactDataArray);

            //GetDataButton.Text = "Returned list :" + (contactDataList.Count).ToString() + " Returned Array :" + contactDataArray.Length;

            //client.Close();
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

