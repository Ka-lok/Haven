

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;


namespace HavenWcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);


        [OperationContract]
        DateTime CheckIn(DateTime DateTime);

        [OperationContract]
        Boolean CheckAndAddUsersNumber(String PhoneNumber);

        [OperationContract]
        List<ContactData> CheckAndAddUsersPhoneContactsForNewUser(List<Contact> ContactList);

        [OperationContract]
        List<ContactData> CheckAndAddUsersPhoneContactsForExistingUser(List<Contact> ContactList, String PhoneNumber);
        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }

    [DataContract]
    public class Contact
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public long LocalId { get; set; }
        [DataMember]
        public string PhotoId { get; set; }
        [DataMember]
        public int HasPhoneNumber { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }

    }

    [DataContract]
    public class ContactData 
    {
        [DataMember]
        public DateTime lastCheckInTime { get; set; }
        [DataMember]
        public bool CheckedIn { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }
  
    }

}
