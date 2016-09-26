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
using Java.Lang;
using SilverCoins.BusinessLayer.Models;
using SilverCoins.BusinessLayer.Managers;

namespace SilverCoins.Droid.Adapters
{
    class TransactionListAdapter : BaseAdapter<Transaction>
    {
        Activity context;
        IList<Transaction> transactions = new List<Transaction>();
        private Account account;

        public TransactionListAdapter(Activity _context, IList<Transaction> transactions, Account account)
            : base()
        {
            context = _context;
            this.transactions = transactions;
            this.account = account;
        }

        public override int Count
        {
            get
            {
                return transactions.Count;
            }
        }

        public override Transaction this[int position]
        {
            get
            {
                return transactions[position];
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View itemView = convertView;

            if (itemView == null)
            {
                itemView = context.LayoutInflater.Inflate(Resource.Layout.list_view_item_transaction, parent, false);
            }

            Transaction transaction = this[position];
            var category = SilverCoinsManager.GetCategory(transaction.Category);
            var account = SilverCoinsManager.GetAccount(transaction.Account);

            itemView.FindViewById<ImageView>(Resource.Id.tran_category_icon).SetImageResource(category.Icon != 0 ? category.Icon : Resource.Drawable.ic_launcher);
            itemView.FindViewById<TextView>(Resource.Id.tran_category_name).Text = category.Name;
            itemView.FindViewById<TextView>(Resource.Id.transaction_name).Text = transaction.Name;
            itemView.FindViewById<TextView>(Resource.Id.tran_payment_type).Text = transaction.PaymentType;
            itemView.FindViewById<TextView>(Resource.Id.tran_account).Text = account.Name;
            itemView.FindViewById<TextView>(Resource.Id.transaction_amount).Text = transaction.Amount.ToString("N2");
            itemView.FindViewById<TextView>(Resource.Id.transaction_crated_date).Text = transaction.CreatedDate.ToString("dd/MM/yyyy");
            itemView.FindViewById<TextView>(Resource.Id.transaction_currency).Text = " " + account.Currency;

            if (transaction.Type == "Transfer" && transaction.AccountTransfer == this.account.Id)
            {
                transaction.Type = "Income";
            }

            switch (transaction.Type)
            {
                case "Income":
                    itemView.FindViewById<TextView>(Resource.Id.transaction_amount).SetTextColor(Android.Graphics.Color.ForestGreen);
                    break;
                case "Expense":
                    itemView.FindViewById<TextView>(Resource.Id.transaction_amount).SetTextColor(Android.Graphics.Color.OrangeRed);
                    itemView.FindViewById<TextView>(Resource.Id.transaction_amount).Text = "-" + transaction.Amount.ToString("N2");
                    break;
                case "Transfer":
                    itemView.FindViewById<TextView>(Resource.Id.transaction_amount).SetTextColor(Android.Graphics.Color.OrangeRed);
                    itemView.FindViewById<TextView>(Resource.Id.transaction_amount).Text = "-" + transaction.Amount.ToString("N2");
                    break;
                default:
                    break;
            }

            return itemView;
        }
    }
}
