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
using SilverCoins.BusinessLayer.Models;

namespace SilverCoins.Droid.Adapters
{
    class AccountListAdapter : BaseAdapter<Account>
    {
        Activity context;
        IList<Account> accounts = new List<Account>();

        public AccountListAdapter (Activity _context, IList<Account> _accounts)
            : base()
        {
            context = _context;
            accounts = _accounts;
        }

        public override Account this[int position]
        {
            get
            {
                return accounts[position];
            }
        }

        public override int Count
        {
            get
            {
                return accounts.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            if(view == null)
            {
                view = context.LayoutInflater.Inflate(Resource.Layout.list_view_item, parent, false);
            }

            Account account = this[position];
            view.FindViewById<TextView>(Resource.Id.account_name).Text = account.Name;
            view.FindViewById<TextView>(Resource.Id.account_balance).Text = account.Balance.ToString("N2") + " " + account.Currency;
            view.FindViewById<TextView>(Resource.Id.account_description).Text = account.Description;
            view.FindViewById<ImageView>(Resource.Id.account_icon).SetImageResource(account.Icon);

            if (account.Balance >= 0)
            {
                view.FindViewById<TextView>(Resource.Id.account_balance).SetTextColor(Android.Graphics.Color.ForestGreen);
            }
            else
            {
                view.FindViewById<TextView>(Resource.Id.account_balance).SetTextColor(Android.Graphics.Color.OrangeRed);
            }

            return view;
        }
    }
}