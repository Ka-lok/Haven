using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Provider;
using Android.Views;
using Android.Widget;
using Java.Lang;
using static Android.Provider.ContactsContract.CommonDataKinds;

using System.Diagnostics;
using Android.Database;
using HavenWcfService;

namespace HavenAndroidApp
{
    internal class ContactsAdapter : BaseAdapter
    {
       public List<Contact> _contactList;
        Activity activity;

        public ContactsAdapter(Activity activity)
        {
            this.activity = activity;
            FillContacts();
        }

        public override int Count
        {
            get { return _contactList.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            // could wrap a Contact in a Java.Lang.Object
            // to return it here if needed
            return null;
        }

        public override long GetItemId(int position)
        {
            return _contactList[position].LocalId;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(
                Resource.Layout.ContactListItem, parent, false);
            var contactName = view.FindViewById<TextView>(Resource.Id.ContactName);
            var contactImage = view.FindViewById<ImageView>(Resource.Id.ContactImage);
            contactName.Text = _contactList[position].Name + " Id:" + _contactList[position].LocalId;

            if (_contactList[position].PhotoId == null)
            {
                contactImage = view.FindViewById<ImageView>(Resource.Id.ContactImage);
                contactImage.SetImageResource(Resource.Drawable.ContactImage);
            }
            else
            {
                var contactUri = ContentUris.WithAppendedId(
                    ContactsContract.Contacts.ContentUri, _contactList[position].LocalId);
                var contactPhotoUri = Android.Net.Uri.WithAppendedPath(contactUri,
                    Contacts.Photos.ContentDirectory);
                contactImage.SetImageURI(contactPhotoUri);
            }
            return view;
        }

        void FillContacts()
        {
            var uri = ContactsContract.Contacts.ContentUri;

            string[] projection = {
                ContactsContract.Contacts.InterfaceConsts.Id,
                ContactsContract.Contacts.InterfaceConsts.DisplayName,
                ContactsContract.Contacts.InterfaceConsts.PhotoId,
                ContactsContract.Contacts.InterfaceConsts.HasPhoneNumber,

            };

            var cursor = activity.ManagedQuery(uri, projection, null, null, null);


            var uri1 = ContactsContract.CommonDataKinds.Phone.ContentUri;
            var cursor1 = activity.ManagedQuery(uri1, null, null, null, null);

            Dictionary<string, string> DictionaryIdToPhoneNumber = new Dictionary<string, string>();
            

            while (cursor1.MoveToNext())
            {
                //  Debug.WriteLine("Phone number : " + cursor1.GetString(cursor1.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number)));
                DictionaryIdToPhoneNumber.Add(cursor1.GetString(cursor1.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName)),cursor1.GetString(cursor1.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number)));
                //Debug.WriteLine("Phone number : " + (cursor1.GetString(cursor1.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName))) + " " + cursor1.GetString(cursor1.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number)));
            }

            _contactList = new List<HavenWcfService.Contact>();
            
            if (cursor.MoveToFirst())
            {

                do
                {
                    int hasPhoneNumber = cursor.GetInt(cursor.GetColumnIndex(projection[3]));
                    if (!(hasPhoneNumber == 0))
                    {

                        //    var phones = activity.ManagedQuery(ContactsContract.CommonDataKinds.Phone.ContentUri, null, null, null, null);
                        //    while (phones.MoveToNext())
                        //    {
                        //        string name = phones.GetString(phones.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.DisplayName));
                        //        string phoneNumber = phones.GetString(phones.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number));
                        //        Debug.WriteLine("Contact with : " + name + " " + phoneNumber);
                        //    }
                        //    phones.Close();

                        _contactList.Add(new HavenWcfService.Contact
                        {
                            LocalId = cursor.GetLong(
                        cursor.GetColumnIndex(projection[0])),
                            Name = cursor.GetString(
                        cursor.GetColumnIndex(projection[1])),
                            PhotoId = cursor.GetString(
                        cursor.GetColumnIndex(projection[2])),
                            HasPhoneNumber = cursor.GetInt(
                        cursor.GetColumnIndex(projection[3])),
                            PhoneNumber = DictionaryIdToPhoneNumber[cursor.GetString(
                        cursor.GetColumnIndex(projection[1]))]
                        
                        });
                    }

                } while (cursor.MoveToNext());

            }
        }
    }
}