

using System;
using Android.OS;
using Android.Runtime;
using HavenWcfService;
using Java.Interop;
using Java.Lang;

namespace HavenAndroidApp
{
    public class ContactDataClient : Java.Lang.Object, IParcelable
        
    {

       public DateTime lastCheckInTime { get; set; }
        public string PhoneNumber { get; set; }
        public bool CheckedIn { get; set; }

        public ContactDataClient()
        {

        }

        public ContactDataClient(DateTime lastCheckInTime, string PhoneNumber, bool CheckedIn)
        {
            this.lastCheckInTime = lastCheckInTime;
            this.PhoneNumber = PhoneNumber;
            this.CheckedIn = CheckedIn;
        }

        public ContactDataClient(Parcel parcel)
        {
            this.lastCheckInTime = DateTime.Parse(parcel.ReadString());
            this.PhoneNumber = parcel.ReadString();
            
        }


       // public IntPtr Handle => throw new NotImplementedException();

        public int DescribeContents()
        {
            return 0;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(lastCheckInTime.ToString());
            dest.WriteString(PhoneNumber);
            dest.WriteByte((sbyte)(CheckedIn ? 1 : 0));// if true = 1
        }

        [ExportField("CREATOR")]
        public static ContactDataClientCreator InititalizeCreator()
        {
            return new ContactDataClientCreator();
        }

        public class ContactDataClientCreator : Java.Lang.Object, IParcelableCreator
        {
            //public IntPtr Handle => throw new NotImplementedException();

            public Java.Lang.Object CreateFromParcel(Parcel source)
            {
                //return new ContactDataClient()
                //{
                //    lastCheckInTime = DateTime.Parse(source.ReadString()),
                //    PhoneNumber = source.ReadString(),
                //    CheckedIn = source.ReadByte() != 0 //true if byte !=0
                //}
                //;

                ContactDataClient c = new ContactDataClient();
                c.lastCheckInTime = DateTime.Parse(source.ReadString());
                    c.PhoneNumber = source.ReadString();
                c.CheckedIn = source.ReadByte() != 0; //true if byte !=0
                return c;
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public Java.Lang.Object[] NewArray(int size)
            {
                return new Java.Lang.Object[size];
            }

            
        }

    }
}