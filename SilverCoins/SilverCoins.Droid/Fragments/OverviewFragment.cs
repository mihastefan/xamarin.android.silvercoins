using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using SilverCoins.BusinessLayer.Managers;
using SilverCoins.BusinessLayer.Models;
using SilverCoins.Droid.Activities;
using SilverCoins.Droid.Adapters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Fragment = Android.Support.V4.App.Fragment;

namespace SilverCoins.Droid.Fragments
{
    public class OverviewFragment : Fragment
    {
        private View view;
        private ListView listView;
        private List<Transaction> transactions;
        private ArrayAdapter<Account> adapterAccounts;
        private ArrayAdapter<string> adapterDay,
                                     adapterMonth;
        private ArrayAdapter<int> adapterYear;
        private List<Account> listOfAccounts = SilverCoinsManager.GetAccounts().ToList();
        private Spinner spinnerAccounts,
                        spinnerDay,
                        spinnerMonth,
                        spinnerYear;
        private TransactionListAdapter transactionAdapter;
        private Android.Support.V7.App.AlertDialog showIntervalDialog;

        private List<int> listOfYears
        {
            get
            {
                return Enumerable.Range(2000, DateTime.Now.Year - 2000 + 1).OrderByDescending(n => n).ToList();
            }
        }
        private List<string> listOfDays
        {
            get
            {
                var list = Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).Select(n => n.ToString()).ToList();
                list.Insert(0, "Select day");
                return list;
            }
        }
        private List<string> listOfMonths
        {
            get
            {
                var list = DateTimeFormatInfo.CurrentInfo.MonthNames.ToList();
                list.Insert(0, "Select month");
                return list;
            }
        }

        public OverviewFragment()
        {
            RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ((HomeActivity)Activity).SupportActionBar.SetDisplayShowTitleEnabled(false);
            ((HomeActivity)Activity).SupportActionBar.SetDisplayShowCustomEnabled(true);
            ((HomeActivity)Activity).SupportActionBar.SetCustomView(Resource.Layout.layout_spinnerAccounts);
            HasOptionsMenu = true;

            view = inflater.Inflate(Resource.Layout.fragment_overview, null);
            listView = view.FindViewById<ListView>(Resource.Id.listViewTransactions);
            listView.ItemClick += OnListItemClick;

            listOfAccounts.Insert(0, new Account() { Name = "All accounts", Id = 0 });

            spinnerAccounts = Activity.FindViewById<Spinner>(Resource.Id.spinnerAccounts);
            adapterAccounts = new ArrayAdapter<Account>(Activity, Resource.Layout.spinner_item, listOfAccounts);
            adapterAccounts.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerAccounts.Adapter = adapterAccounts;

            spinnerAccounts.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinnerTransactionAccount_ItemClick);

            FloatingActionButton fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += (o, e) =>
            {
                if (SilverCoinsManager.GetAccounts().ToList().Any())
                {
                    Intent intent = new Intent(fab.Context, typeof(SaveTransactionActivity));
                    StartActivity(intent);
                }
                else
                {
                    new Android.Support.V7.App.AlertDialog.Builder(Activity)
                        .SetCancelable(true)
                        .SetMessage("You need to create account first!")
                        .SetPositiveButton("OK", (sender, args) =>
                        {
                            Intent intent = new Intent(fab.Context, typeof(SaveAccountActivity));
                            StartActivity(intent);
                        })
                        .SetNegativeButton("Cancel", (sender, args) => { })
                        .Show();
                }
            };

            return view;
        }

        private void SpinnerTransactionAccount_ItemClick(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Account account = listOfAccounts[e.Position];
            PopulateListView(account);
        }

        public override void OnResume()
        {
            base.OnResume();

            Account account = listOfAccounts[spinnerAccounts.SelectedItemPosition];
            PopulateListView(account);
        }

        private void PopulateListView(Account account)
        {
            if (account.Id == 0)
            {
                var list = SilverCoinsManager.GetTransactions().ToList();
                transactions = list.ToList();
                foreach (var item in list.Where(x => x.Type == "Transfer"))
                {
                    transactions.Insert(transactions.IndexOf(item) + 1, new Transaction()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Category = item.Category,
                        Amount = item.Amount * item.CurrencyRate,
                        Account = item.AccountTransfer,
                        CreatedDate = item.CreatedDate,
                        PaymentType = item.PaymentType,
                        Type = "Income"
                    });
                }
            }
            else
            {
                transactions = SilverCoinsManager.GetAllTransactionsForAccount(account.Id);
            }

            transactionAdapter = new TransactionListAdapter(Activity, transactions, account);
            listView.Adapter = transactionAdapter;
        }

        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs args)
        {
            var transactionId = transactions[args.Position].Id;
            string transactionIdJson = Newtonsoft.Json.JsonConvert.SerializeObject(transactionId);

            Intent intent = new Intent(Activity, typeof(SaveTransactionActivity));
            intent.PutExtra("TRANSACTION_ID", transactionIdJson);
            intent.PutExtra("EDIT_MODE", true);
            StartActivity(intent);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.menu_overview, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_interval:
                    ShowIntervalDialog();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void ShowIntervalDialog()
        {
            var builder = new Android.Support.V7.App.AlertDialog.Builder(Activity);
            var dialogView = Activity.LayoutInflater.Inflate(Resource.Layout.interval_selection, null);

            spinnerDay = dialogView.FindViewById<Spinner>(Resource.Id.spinner_day);
            spinnerMonth = dialogView.FindViewById<Spinner>(Resource.Id.spinner_month);
            spinnerYear = dialogView.FindViewById<Spinner>(Resource.Id.spinner_year);

            adapterDay = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, listOfDays);
            adapterMonth = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, listOfMonths);
            adapterYear = new ArrayAdapter<int>(Activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, listOfYears);

            adapterDay.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            adapterMonth.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            adapterYear.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            spinnerDay.Adapter = adapterDay;
            spinnerMonth.Adapter = adapterMonth;
            spinnerYear.Adapter = adapterYear;

            spinnerDay.SetSelection(DateTime.Now.Day);
            spinnerMonth.SetSelection(DateTime.Now.Month);
            spinnerYear.SetSelection(listOfYears.IndexOf(DateTime.Now.Year));

            builder.SetView(dialogView);
            builder.SetCancelable(false);
            builder.SetTitle(Utils.Constants.IntervalTitle);
            builder.SetView(dialogView);
            builder.SetPositiveButton("OK", (EventHandler<DialogClickEventArgs>)null);
            builder.SetNegativeButton("Cancel", (sender, args) => { });
            showIntervalDialog = builder.Create();
            showIntervalDialog.Show();
            Button button = showIntervalDialog.GetButton((int)DialogButtonType.Positive);
            button.Click += (sender, args) =>
            {
                FilterTransactions();
            };
        }

        private void FilterTransactions()
        {
            var day = spinnerDay.SelectedItemPosition;
            var month = spinnerMonth.SelectedItemPosition;
            var year = listOfYears[spinnerYear.SelectedItemPosition];
            var account = listOfAccounts[spinnerAccounts.SelectedItemPosition];

            if (day != 0 && month == 0)
            {
                Toast.MakeText(Activity, "Month must be selected!", ToastLength.Short).Show();
            }
            else
            {
                transactions = SilverCoinsManager.GetTransactionsByDate(day, month, year, account.Id);
                transactionAdapter = new TransactionListAdapter(Activity, transactions, account);
                listView.Adapter = transactionAdapter;
                showIntervalDialog.Dismiss();
            }
        }
    }
}