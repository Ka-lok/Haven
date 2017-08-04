using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using System.Diagnostics;



namespace HavenWcf
{
     //NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
     //NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
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

//cmd.CommandText = "UPDATE \"User\" SET LastCheckIn = @DateNow WHERE Id = 1;";
            //cmd.Parameters.AddWithValue("@DateNow", DateTime.Now);

            //con.Open();
            //reader = cmd.ExecuteReader();
            //con.Close();
        }

        //public List<ContactData> AddContacts(List<Contact> ContactList)
        //{
        //    List<Contact> HavenContacts;

        //    HavenContacts = CheckForHavenContacts(ContactList);

        //    List<ContactData> list = new List<ContactData>();
        //    return list;
        //}

        //private List<Contact> CheckForHavenContacts(List<Contact> ContactList)
        //{
        //    //check the numbers given to us by the phone are haven contacts or not.

        //    string SearchContactString = MakeSearchContactString(ContactList) + " ;";

        //    cmd.CommandText = "SELECT PhoneNumber From \"User\" WHERE PhoneNumber = " + SearchContactString;

        //    //cmd.Parameters.AddWithValue("@ContactNumber", ContactList[0].PhoneNumber);// need to add an entire list.

        //    List<Contact> HavenContacts;
        //    con.Open();
        //    reader = cmd.ExecuteReader();
        //    HavenContacts = CheckIfPhoneNumberPresentInReader(reader, ContactList);

        //    con.Close();

        //    return HavenContacts;

        //}

        //public List<Contact> CheckIfPhoneNumberPresentInReader(SqlDataReader reader, List<Contact> ContactList)
        //{
        //    List<Contact> HavenContacts = new List<Contact>();

        //    Dictionary<string, int> dictonaryPhoneNumberToContact = new Dictionary<string, int>();

        //    int counter = 0;

        //    foreach (var contact in ContactList)
        //    {
        //        dictonaryPhoneNumberToContact.Add(contact.PhoneNumber, counter);
        //        counter = counter + 1;
        //    }


        //    while (reader.Read())
        //    {
        //        if (dictonaryPhoneNumberToContact.ContainsKey(reader.GetString(0)))
        //        {
        //            HavenContacts.Add(ContactList[dictonaryPhoneNumberToContact[reader.GetString(0)]]);
        //        }
        //    }

        //    return HavenContacts;

        //}

        //private string MakeSearchContactString(List<Contact> ContactList)
        //{
        //    string SearchContactString = "";

        //    foreach (var contact in ContactList)
        //    {
        //        SearchContactString = SearchContactString + " OR " + contact.PhoneNumber;
        //    }

        //    return SearchContactString;
        //}

        //public Boolean CheckOrAddNumber(String PhoneNumberValue)
        //{
        //    Debug.WriteLine("STARTING FROM SERVER NOW!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

        //    if (!(CheckIfNumberIsAlreadyInUse(PhoneNumberValue)))
        //    {
        //        //new number, add number to DB
        //        int id = NextIdCountValue();

        //        cmd.CommandText = "INSERT INTO \"User\" (Id,PhoneNumber) VALUES (@IdValue,@PhoneNumber)";
        //        cmd.Parameters.AddWithValue("@IdValue", id + 1);
        //        cmd.Parameters.AddWithValue("@PhoneNumberValue", PhoneNumberValue);
        //        con.Open();
        //        reader = cmd.ExecuteReader();
        //        con.Close();

        //        return false;

        //    }
        //    else
        //    {
        //      //  get their contacts updates
        //        return true;
        //    }
        //}

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
                Debug.WriteLine("Found PhoneNumber : " + reader["PhoneNumber"].ToString());
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

        public bool CheckOrAddNumber(string PhoneNumber)
        {
            throw new NotImplementedException();
        }
    }
}
