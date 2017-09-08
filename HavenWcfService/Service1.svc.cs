using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;

namespace HavenWcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        string str = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=WcfServiceDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader reader;

        String PhoneNumber;

        public Service1()
        {


            con = new SqlConnection(str);
            cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            Debug.WriteLine("Service has now been set up");
            SendPushNotification("dTsm6mZp1fw:APA91bGCgyCA-Ut1vpj6CtGs5nEfLNx14GaTDGTYGMDWu71_z64OfcBqCgzKILqdNe4yH_GGEDON8IxOymyRks04XwFKcJ9FZORFPYGfQAwORm5G8hpStKEirs2t1G2C9T6BOrU62Kox");
            //cmd.CommandText = "UPDATE \"User\" SET LastCheckIn = @DateNow WHERE Id = 1;";
            //cmd.Parameters.AddWithValue("@DateNow", DateTime.Now);

            //con.Open();
            //reader = cmd.ExecuteReader();
            //con.Close();
        }

        public List<ContactData> CheckAndAddUsersPhoneContactsForNewUser(List<Contact> ContactList)
        { //checks if anyone in the users contacts have have Haven and then creates the relationship between them.
            List<Contact> HavenContacts;
            Debug.WriteLine("number of contacts passed from client is " + ContactList.Count);
            List<ContactData> list = new List<ContactData>();
            HavenContacts = CheckForHavenContacts(ContactList);

            if (HavenContacts.Count > 0)
            {
                string commandText = "INSERT INTO \"Relationships\" (Id,UserId1,UserId2) VALUES (@RelationId,@UserId1,@UserId2);";


                cmd.Parameters.AddWithValue("@RelationId", NextRelationId());
                cmd.Parameters.AddWithValue("@UserId1", PhoneNumber);
                cmd.Parameters.AddWithValue("@UserId2", HavenContacts[0].PhoneNumber);


                foreach (var HavenContact in HavenContacts)
                {
                    commandText = commandText + "INSERT INTO \"Relationships\"(Id,UserId1,UserId2) VALUES (" + NextRelationId() + "," +PhoneNumber + "," + HavenContact.PhoneNumber +");";
                }

                //cmd.CommandText = "INSERT INTO \"Relationships\" (Id,UserId1,UserId2) VALUES (@RelationId,@UserId1,@UserId2);";

                cmd.CommandText = commandText;
                con.Open();
                reader = cmd.ExecuteReader();
                con.Close();

            }
            
                return list;
        }

        private List<Contact> CheckForHavenContacts(List<Contact> ContactList)
        {
            //check the numbers given to us by the phone are haven contacts or not.



            string SearchContactString = MakeSearchContactString(ContactList) + ";";

          
            cmd.CommandText = "SELECT PhoneNumber From \"User\" WHERE " + SearchContactString;
            Debug.WriteLine(cmd.CommandText);
            //cmd.Parameters.AddWithValue("@ContactNumber", ContactList[0].PhoneNumber);// need to add an entire list.

            List<Contact> HavenContacts;
            con.Open();
            reader = cmd.ExecuteReader();
            HavenContacts = CheckIfPhoneNumberPresentInReader(reader, ContactList);

            con.Close();

            return HavenContacts;

        }

        private int NextRelationId()
        {

            cmd.CommandText = "SELECT Id FROM \"Relationships\";";

            con.Open();
            int count = 0;

            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                count = count + 1;
            }

            con.Close();

            return count + 1;
        }

        public List<Contact> CheckIfPhoneNumberPresentInReader(SqlDataReader reader, List<Contact> ContactList)
        {
            List<Contact> HavenContacts = new List<Contact>();

            Dictionary<string, int> dictonaryPhoneNumberToContact = new Dictionary<string, int>();

            int counter = 0;

            foreach (var contact in ContactList)
            {
                dictonaryPhoneNumberToContact.Add(contact.PhoneNumber, counter);
                counter = counter + 1;
            }


            while (reader.Read())
            {
                if (dictonaryPhoneNumberToContact.ContainsKey(reader.GetString(0)))
                {
                    HavenContacts.Add(ContactList[dictonaryPhoneNumberToContact[reader.GetString(0)]]);
                }
            }

            return HavenContacts;

        }

        private string MakeSearchContactString(List<Contact> ContactList)
        {
            string SearchContactString = "";

            Contact lastContact = ContactList.Last();
            foreach (var contact in ContactList)
            {
                SearchContactString = SearchContactString   + " PhoneNumber = "+  contact.PhoneNumber ;

                if (!(contact.Equals(lastContact)))
                {
                    SearchContactString = SearchContactString + " Or ";
                }
            }

            return SearchContactString;
        }

        public Boolean CheckAndAddUsersNumber(String PhoneNumberValue)
        {
            Debug.WriteLine("STARTING FROM SERVER NOW!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            PhoneNumber = PhoneNumberValue;
            if (!(CheckIfNumberIsAlreadyInUse(PhoneNumberValue)))
            {
                Debug.WriteLine("Number was not found");
                //new number, add number to DB
                int id = NextIdCountValue();

                cmd.CommandText = "INSERT INTO \"User\" (Id,PhoneNumber) VALUES (@IdValue,@PhoneNumber)";
                cmd.Parameters.AddWithValue("@IdValue", id + 1);
                cmd.Parameters.AddWithValue("@PhoneNumberValue", PhoneNumberValue);
                con.Open();
                reader = cmd.ExecuteReader();
                con.Close();

                return false;

            }
            else
            {
                Debug.WriteLine("Number was  found");
                //  get their contacts updates
                return true;
            }
        }

        private int NextIdCountValue()
        {

            cmd.CommandText = "SELECT Id FROM \"User\";";

            con.Open();
            int count = 0;

            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                count = count + 1;
            }

            con.Close();

            return count;

        }

        public Boolean CheckIfNumberIsAlreadyInUse(String PhoneNumber)
        {

            cmd.CommandText = "SELECT PhoneNumber From \"User\" WHERE PhoneNumber = @PhoneNumber";
            cmd.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
            con.Open();
            reader = cmd.ExecuteReader();

            int readerCounter = 0;

            while (reader.Read())
            {
                readerCounter = readerCounter + 1;
                //Debug.WriteLine("Found PhoneNumber : " + reader["PhoneNumber"].ToString());
            }

            con.Close();

            if (readerCounter == 1)
            {
                
                return true;
            }
            else
            {
              
                return false;
            }

        }

        public DateTime CheckIn(DateTime DateTime)
        {
            cmd.CommandText = "UPDATE \"User\" SET LastCheckIn = @DateNow WHERE Id = 1;";
            cmd.Parameters.AddWithValue("@DateNow", DateTime);
            con.Open();
            reader = cmd.ExecuteReader();
            con.Close();
            return DateTime;
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public List<ContactData> CheckAndAddUsersPhoneContactsForExistingUser(List<Contact> ContactList, String UsersPhoneNumber)
        {
            List<Contact> HavenUsers;
            List<Contact> HavenUsersWithExistingRelationshipWithUser = new List<Contact>();
            List<Contact> HavenUsersWithOutExistingRelationshipWithUser = new List<Contact>();
            HavenUsers = CheckForHavenContacts(ContactList);
            

            Dictionary<string, int> HavenPositionDictionary = new Dictionary<string, int>();
            int counter = 0;

            foreach (var HavenUser in HavenUsers)
            {
                HavenPositionDictionary.Add(HavenUser.PhoneNumber, counter);
                counter = counter + 1;
            }
            Debug.WriteLine("HavenContactDictonary List count :" + HavenPositionDictionary.Count);
            Debug.WriteLine("HavenContact List count :" + HavenUsers.Count);

            cmd.CommandText = "SELECT User1Id, User2Id From Relationships Where User1Id = @UserPhoneNumber1 Or User2Id = @UserPhoneNumber2";
            cmd.Parameters.AddWithValue("@UserPhoneNumber1", UsersPhoneNumber);
            cmd.Parameters.AddWithValue("@UserPhoneNumber2", UsersPhoneNumber);

            con.Open();
            reader = cmd.ExecuteReader();

            HashSet<string> phoneNumberContactsSet = new HashSet<string>();

            while (reader.Read())
            {
                //from DB
                if (reader.GetString(0).Equals(UsersPhoneNumber))
                {
                    phoneNumberContactsSet.Add(reader.GetString(1));
                } else
                {
                    phoneNumberContactsSet.Add(reader.GetString(0));
                }
            
            }

            foreach (var contact in HavenUsers)
            {
                if (phoneNumberContactsSet.Contains(contact.PhoneNumber))
                {
                    HavenUsersWithExistingRelationshipWithUser.Add(HavenUsers[HavenPositionDictionary[contact.PhoneNumber]]);
                }
                else
                {
                    HavenUsersWithOutExistingRelationshipWithUser.Add(HavenUsers[HavenPositionDictionary[contact.PhoneNumber]]);
                }
                
            }




            Debug.WriteLine("Haven user with existing relationship count : " + HavenUsersWithExistingRelationshipWithUser.Count);
            Debug.WriteLine("HAven user without existing relationship count" + HavenUsersWithOutExistingRelationshipWithUser.Count);

            con.Close();

           List<ContactData> HavenUsersContactData =  (ReturnContactDataListFromContactList(HavenUsersWithExistingRelationshipWithUser).Concat(CreateRelation(HavenUsersWithOutExistingRelationshipWithUser, UsersPhoneNumber)).ToList());

            Debug.WriteLine("Server is returning ContactData with count " + HavenUsersContactData.Count);
            return HavenUsersContactData;


            //return CreateRelation(HavenUsersWithOutExistingRelationshipWithUser, UsersPhoneNumber);
           
            throw new NotImplementedException();
        }

        private List<ContactData> ReturnContactDataListFromContactList(List<Contact> contactList)
        {
            List<ContactData> contactDataList = new List<ContactData>();
            
            foreach (var contact in contactList)
            {
                cmd.CommandText = "Select LastCheckIn, CheckInPostive From \"User\" WHERE PhoneNumber = " + contact.PhoneNumber;
                //cmd.Parameters.AddWithValue("@ContactPhoneNumber",contact.PhoneNumber);
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    DateTime lastCheckIn = reader.GetDateTime(0);
                    Boolean CheckedIn = reader.GetBoolean(1);
                    contactDataList.Add(new ContactData
                    {

                        PhoneNumber = contact.PhoneNumber,
                        lastCheckInTime = lastCheckIn,
                        CheckedIn = CheckedIn
                        
                    }

                    );

                }
                con.Close();
          
            }

            return contactDataList;
        }

        private List<ContactData> CreateRelation(List<Contact> ContactList, string PhoneNumber)
        {

            if (ContactList.Count > 0)
            {
                string commandText = "INSERT INTO \"Relationships\" (Id,User1Id,User2Id) VALUES (@RelationId,@UserId1,@UserId2);";


                cmd.Parameters.AddWithValue("@RelationId", NextRelationId());
                cmd.Parameters.AddWithValue("@UserId1", PhoneNumber);
                cmd.Parameters.AddWithValue("@UserId2",ContactList[0].PhoneNumber);


                foreach (var HavenContact in ContactList)
                {
                    commandText = commandText + "INSERT INTO \"Relationships\"(Id,User1Id,User2Id) VALUES ("+ (NextRelationId() + 1) + "," + PhoneNumber + "," + HavenContact.PhoneNumber + ");";
                }

                //cmd.CommandText = "INSERT INTO \"Relationships\" (Id,UserId1,UserId2) VALUES (@RelationId,@UserId1,@UserId2);";

                cmd.CommandText = commandText;
                con.Open();
                reader = cmd.ExecuteReader();
                con.Close();

            }

            return ReturnContactDataListFromContactList(ContactList);
        }

        public static void SendPushNotification(String deviceid)
        {

            try
            {

                string applicationID = "1:1058311636460:android:b7c781c493b1abc6";

                string senderId = "1058311636460";

                string deviceId = deviceid;

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    notification = new
                    {
                        body = "Osama",
                        title = "AlBaami",
                        sound = "Enabled"

                    }
                };
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }
    }
}
