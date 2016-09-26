using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Fragment = Android.Support.V4.App.Fragment;
using SilverCoins.Droid.Activities;
using Refractored.Controls;
using Android.Support.Design.Widget;

namespace SilverCoins.Droid.Fragments
{
    public class ProfileFragment : Fragment
    {
        
        EditText edtProfileName,
                 edtProfileEmail;
        CircleImageView imgProfileImage;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ((HomeActivity)Activity).SupportActionBar.SetDisplayShowTitleEnabled(true);
            ((HomeActivity)Activity).SupportActionBar.SetDisplayShowCustomEnabled(false);
            ((HomeActivity)Activity).SupportActionBar.Title = "Profile";
            HasOptionsMenu = true;

            var prefs = Activity.GetSharedPreferences(Utils.Constants.SilverCoinsPreferences, FileCreationMode.Private);
            var profileName = prefs.GetString(Utils.Constants.ProfileName, "John Doe");
            var profileEmail = prefs.GetString(Utils.Constants.ProfileEmail, "example@email.com");
            var profileImageUri = prefs.GetString(Utils.Constants.ProfileImageUri, null);

            var view = inflater.Inflate(Resource.Layout.fragment_profile, null);

            edtProfileName = view.FindViewById<EditText>(Resource.Id.profile_name);
            edtProfileEmail = view.FindViewById<EditText>(Resource.Id.profile_email);
            imgProfileImage = view.FindViewById<CircleImageView>(Resource.Id.profile_imageView);

            view.FindViewById<EditText>(Resource.Id.profile_name).Text = profileName;
            view.FindViewById<EditText>(Resource.Id.profile_email).Text = profileEmail;

            if (profileImageUri != null)
            {
                view.FindViewById<CircleImageView>(Resource.Id.profile_imageView).SetImageURI(Android.Net.Uri.Parse(profileImageUri));
            }
            else
            {
                view.FindViewById<CircleImageView>(Resource.Id.profile_imageView).SetImageResource(Resource.Drawable.nav_header_profile);
            }

            imgProfileImage.Click += ImgProfileImage_Click;

            return view;
        }

        private void ImgProfileImage_Click(object sender, EventArgs e)
        {
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 0);
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == (int)Result.Ok)
            {
                imgProfileImage.SetImageURI(data.Data);
                var prefs = Activity.GetSharedPreferences(Utils.Constants.SilverCoinsPreferences, FileCreationMode.Private);
                var editor = prefs.Edit();
                editor.PutString(Utils.Constants.ProfileImageUri, data.Data.ToString());
                editor.Apply();

                var headerView = Activity.FindViewById<NavigationView>(Resource.Id.nav_view).GetHeaderView(0);
                headerView.FindViewById<CircleImageView>(Resource.Id.profile_image).SetImageURI(data.Data);

            }
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.menu_save, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_save:

                    var prefs = Activity.GetSharedPreferences(Utils.Constants.SilverCoinsPreferences, FileCreationMode.Private);
                    var editor = prefs.Edit();
                    editor.PutString(Utils.Constants.ProfileName, edtProfileName.Text);
                    editor.PutString(Utils.Constants.ProfileEmail, edtProfileEmail.Text);
                    editor.Apply();

                    var headerView = Activity.FindViewById<NavigationView>(Resource.Id.nav_view).GetHeaderView(0);
                    headerView.FindViewById<TextView>(Resource.Id.username).Text = edtProfileName.Text;
                    headerView.FindViewById<TextView>(Resource.Id.email).Text = edtProfileEmail.Text;

                    ((HomeActivity)Activity).SupportFragmentManager.BeginTransaction()
                                  .Replace(Resource.Id.content_frame, new OverviewFragment())
                                  .Commit();

                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }

        }
    }
}