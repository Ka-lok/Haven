using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.ServiceModel;
using HavenWcfService;
using Android.Telephony;

namespace HavenAndroidApp

{
    [Activity(Label = "HomeActivity")]
    public class LoadingScreenActivity : Activity
    {
        public static readonly EndpointAddress EndPoint = new EndpointAddress("http://192.168.1.66:60236/Service1.svc");

        Contact[] contactListArray;
        Service1Client client;
       
        
        ContactsAdapter contactsAdapter;
        ArrayAdapter<String> ListAdapter;
        TelephonyManager mTelephonyMgr;
        String Number;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LoadingScreen);

            GetContacts();
            InitializeServiceClient();
            // Create your application here
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
               
                contactDataArray = client.CheckAndAddUsersPhoneContactsForExistingUser(contactListArray, "222222");

                //check for new haven users and then get data for all other contacts
            }
            else
            {
                
                contactDataArray = client.CheckAndAddUsersPhoneContactsForNewUser(contactListArray);

            }
            client.Close();

            List<ContactData> contactDataList = new List<ContactData>(contactDataArray);

            var MainActivity = new Intent(this, typeof(MainActivity));


           // MainActivity.PutExtra("something", new ContactDataClient() { PhoneNumber = "69" });
            MainActivity.PutParcelableArrayListExtra("ContactDataClientList",  new List<IParcelable>(ConvertContactDataToContactDataClient(contactDataList)));
          
           // MainActivity.PutExtra("ContactAdapterStringList", contactsAdapter);

            StartActivity(MainActivity);    
        }

        private List<ContactDataClient> ConvertContactDataToContactDataClient(List<ContactData> ContactDataList)
        {
            List<ContactDataClient> ContactDataClientList = new List<ContactDataClient>();

            foreach (var ContactData in ContactDataList)
            {
                ContactDataClientList.Add(new ContactDataClient() {

                    PhoneNumber = ContactData.PhoneNumber,
                    CheckedIn = ContactData.CheckedIn,
                    lastCheckInTime = ContactData.lastCheckInTime

                });
            }
            return ContactDataClientList;
        }




        private void GetContacts()
        {
            
            contactsAdapter = new ContactsAdapter(this);
            contactListArray = (contactsAdapter._contactList).ToArray();
            
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