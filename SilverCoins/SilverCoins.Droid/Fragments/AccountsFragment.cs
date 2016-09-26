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
using SilverCoins.BusinessLayer.Models;
using SilverCoins.Droid.Adapters;
using Android.Support.V4.Widget;
using SilverCoins.Droid.Activities;

namespace SilverCoins.Droid.Fragments
{
    public class AccountsFragment : Fragment
    {
        private IList<Account> accounts;
        private View view;
        private ListView list;

        public AccountsFragment()
        {
            RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ((HomeActivity)Activity).SupportActionBar.SetDisplayShowTitleEnabled(true);
            ((HomeActivity)Activity).SupportActionBar.SetDisplayShowCustomEnabled(false);
            ((HomeActivity)Activity).SupportActionBar.Title = "Accounts";
            HasOptionsMenu = true;

            view = inflater.Inflate(Resource.Layout.fragment_accounts, null);
            list = view.FindViewById<ListView>(Resource.Id.listViewAccounts);
            
            list.ItemClick += OnListItemClick;
            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            accounts = BusinessLayer.Managers.SilverCoinsManager.GetAccounts();
            list.Adapter = new AccountListAdapter(Activity, accounts);
        }

        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            Account account = accounts[itemClickEventArgs.Position];

            Intent intent = new Intent(Activity, typeof(SaveAccountActivity));
            var accountJson = Newtonsoft.Json.JsonConvert.SerializeObject(account);
            intent.PutExtra("ACCOUNT_JSON", accountJson);
            intent.PutExtra("EDIT_MODE", true);
            StartActivity(intent);

        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.menu_create, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_create:
                    Intent intent = new Intent(Activity, typeof(SaveAccountActivity));
                    StartActivity(intent);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }

        }
    }
}