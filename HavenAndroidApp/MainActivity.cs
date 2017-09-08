using Android.App;
using Android.Widget;
using Android.OS;
using System.ServiceModel;
using System;
using Android.Telephony;
using HavenWcfService;

using System.Diagnostics;
using System.Collections.Generic;
using Android.Graphics;
using Android.Gms.Common;
using Firebase.Messaging;
using Firebase.Iid;
using Android.Util;
using Firebase;

namespace HavenAndroidApp
{
    [Activity(Label = "HavenAndroidApp", Icon = "@drawable/icon",MainLauncher = true)]
    public class MainActivity : Activity
    {
        public static readonly EndpointAddress EndPoint = new EndpointAddress("http://192.168.1.66:60236/Service1.svc");

     Contact [] contactListArray;
        Service1Client client;
        Button GetDataButton;
        ListView ContactList;
        ContactsAdapter contactsAdapter;
        ArrayAdapter<String> ListAdapter;
        TelephonyManager mTelephonyMgr;
        String Number;
        List<ContactDataClient> ContactDataClientList;

        ProgressBar ProgressBar1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            
          //  mTelephonyMgr = (TelephonyManager)GetSystemService(TelephonyService);
            //Number = mTelephonyMgr.Line1Number;
            
            SetContentView (Resource.Layout.Main);

            ContactList = FindViewById<ListView>(Resource.Id.ContactList);
            GetDataButton = FindViewById<Button>(Resource.Id.GetDataButton);
            GetDataButton.Click += GetDataButton_Click;

            ProgressBar1 = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            
            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    var value = Intent.Extras.GetString(key);
                    Log.Debug(FirebaseInstanceId.Instance.Token, "Key: {0} Value: {1}", key, value);
                }

            }
            
            GetDataButton.Text = FirebaseInstanceId.Instance.Id;

             LoadContacts();
            //  IsPlayServicesAvailable();
            //  InitializeServiceClient();


        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    GetDataButton.Text = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                else
                {
                    GetDataButton.Text = "This device is not supported";
                    Finish();
                }
                return false;
            }
            else
            {
                GetDataButton.Text = "Google Play Services is available.";
                return true;
            }
        }

        private void GetDataButton_Click(object sender, System.EventArgs e)
        {

            //  GetDataButton.Text =  (client.CheckIn(DateTime.Now)).ToString();
            GetDataButton.Text = client.GetData(1);
            //GetDataButton.Text = client.CheckOrAddNumber("2222").ToString();
          
        }

        private void LoadContacts()
        {
            
           //ContactList.Adapter = ListAdapter;
           
            contactsAdapter = new ContactsAdapter(this);
            
            contactListArray = (contactsAdapter._contactList).ToArray();
            
            ContactList.Adapter = contactsAdapter;
           InitializeServiceClient();
        }

        private void InitializeServiceClient()
        {
            BasicHttpBinding binding = CreateBasicHttp();
            client = new Service1Client(binding, EndPoint);
            
            Boolean numberAlreadyExists = false;


            //numberAlreadyExists = client.CheckOrAddNumber(Number);
            numberAlreadyExists = client.CheckAndAddUsersNumber("222222"); //check or add the phone user number NOT THEIR CONTACTS

            ContactData[] contactDataArray;

            if (numberAlreadyExists)
            {
                GetDataButton.Text = "The Number already Exists";

                contactDataArray = client.CheckAndAddUsersPhoneContactsForExistingUser(contactListArray, "222222");

                //check for new and then get data for all other contacts
            }
            else
            {
                
                GetDataButton.Text = "Number does not exist";

                contactDataArray = client.CheckAndAddUsersPhoneContactsForNewUser(contactListArray);
                
            }

            
            client.Close(); 

            List<ContactData> contactDataList = new List<ContactData>(contactDataArray);

            //GetDataButton.Text = "Returned list :" + (contactDataList.Count).ToString() + " Returned Array :" + contactDataArray.Length;
            DisplayContactDataInContactDataList(contactDataList);


            ProgressBar1.SetProgress(100,true);
            
            ProgressBar1.Visibility = Android.Views.ViewStates.Gone;
        }

        private void DisplayContactDataInContactDataList(List<ContactData> contactDataList)
        {
            
            contactsAdapter.UpdateContactListWithContactData(contactDataList);
            //ListAdapter.NotifyDataSetChanged();
             
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

