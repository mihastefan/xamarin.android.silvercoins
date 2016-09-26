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
using SilverCoins.BusinessLayer.Managers;
using SilverCoins.Droid.Dialogs;
using SilverCoins.BusinessLayer.Models;
using Android.Support.Design.Widget;

namespace SilverCoins.Droid.Activities
{
    [Activity(Label = "Save Transaction", ParentActivity = typeof(HomeActivity),
              ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    [MetaData("android.support.PARENT_ACTIVITY", Value = "silverCoins.droid.activities.HomeActivity")]
    public class SaveTransactionActivity : BaseActivity
    {
        TextView txtIncome,
                 txtExpense,
                 txtTransfer,
                 txtTransactionAmount,
                 txtTransactionAccount,
                 txtTransactionDate,
                 txtTransactionCategory,
                 txtTransactionCurrency,
                 txtTransactionPaymentType;
        EditText edtTransactionName,
                 edtCurrencyRate;
        Spinner spinnerTransactionAccount,
                spinnerTransactionCategory,
                spinnerTransactionPaymentType,
                spinnerTransactionAccountTransfer;
        ArrayAdapter<Account> adapterAccount,
                              adapterAccountTransfer;
        ArrayAdapter<Category> adapterCategory;
        ArrayAdapter<string> adapterPaymentType;
        TextInputLayout txtInputLayoutTranName;

        private string[] paymentTypes = { "Cash", "Debit card", "Credit card", "Bank transfer",
                                          "Voucher", "Mobile payment", "Web payment" };
        protected bool EditMode = false;
        protected Transaction transaction;
        protected string Type = "Income";

        protected List<Account> listOfAccounts = SilverCoinsManager.GetAccounts().ToList();
        protected List<Category> listOfIncomeCategories = SilverCoinsManager.GetIncomeCategories();
        protected List<Category> listOfExpenseCategories = SilverCoinsManager.GetExpenseCategories();

        protected override int LayoutResource { get { return Resource.Layout.activity_save_transaction; } }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Toolbar.SetNavigationIcon(Resource.Drawable.ic_cancel);

            Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#00796B"));

            edtTransactionName = FindViewById<EditText>(Resource.Id.edit_transaction_name);

            txtTransactionAmount = FindViewById<TextView>(Resource.Id.transaction_ammount);
            txtTransactionCategory = FindViewById<TextView>(Resource.Id.txt_transaction_category);
            txtTransactionAccount = FindViewById<TextView>(Resource.Id.txt_transaction_account);
            txtTransactionCurrency = FindViewById<TextView>(Resource.Id.transaction_currency);
            txtTransactionPaymentType = FindViewById<TextView>(Resource.Id.txt_transaction_payment_type);

            txtInputLayoutTranName = FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutTranName);

            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                EditMode = extras.GetBoolean("EDIT_MODE");
                if (EditMode)
                {
                    var transactionId = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(extras.GetString("TRANSACTION_ID"));
                    transaction = SilverCoinsManager.GetTransaction(transactionId);
                    Type = transaction.Type;
                    edtTransactionName.Text = transaction.Name;
                    txtTransactionAmount.Text = transaction.Amount.ToString("N2");
                }
            }
            else
            {
                transaction = new Transaction();
            }

            InitalizeTransactionTypes();
            InitalizeSpinners();
            InitalizeDatePicker();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (EditMode)
                MenuInflater.Inflate(Resource.Menu.menu_save_delete, menu);
            else
                MenuInflater.Inflate(Resource.Menu.menu_save, menu);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_save:
                    if (!string.IsNullOrEmpty(txtInputLayoutTranName.EditText.Text))
                    {
                        var amount = txtTransactionAmount.Text;
                        if (!string.IsNullOrEmpty(amount) && amount != "0")
                        {
                            Account accountFrom = listOfAccounts.ElementAt(spinnerTransactionAccount.SelectedItemPosition);
                            Account accountTo = listOfAccounts.ElementAt(spinnerTransactionAccountTransfer.SelectedItemPosition);
                            if (Type == "Transfer")
                            {
                                if (accountFrom.Id == accountTo.Id)
                                {
                                    Toast.MakeText(this, Utils.Constants.SameAccountTransferWarning, ToastLength.Short).Show();
                                }
                                else if (accountFrom.Currency != accountTo.Currency)
                                {
                                    edtCurrencyRate = new EditText(this);
                                    new AlertDialog.Builder(this)
                                        .SetCancelable(true)
                                        .SetMessage(Utils.Constants.CurrencyRateMessage)
                                        .SetView(edtCurrencyRate)
                                        .SetPositiveButton("OK", (sender, args) =>
                                        {
                                            SaveTransaction();
                                        })
                                        .SetNegativeButton("Cancel", (sender, args) => { })
                                        .Show();
                                }
                                else
                                {
                                    SaveTransaction();
                                }
                            }
                            else
                            {
                                SaveTransaction();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, Utils.Constants.TransactionNullOrZeroAmount, ToastLength.Short).Show();
                        }

                    }
                    else
                    {
                        txtInputLayoutTranName.Error = Utils.Constants.EmptyField;
                    }

                    return true;
                case Resource.Id.action_delete:
                    DeleteTransaction();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void InitalizeDatePicker()
        {
            txtTransactionDate = FindViewById<TextView>(Resource.Id.txt_transaction_date_selected);
            if (EditMode)
            {
                txtTransactionDate.Text = transaction.CreatedDate.ToString("dd/MM/yyyy");
            }
            else
            {
                txtTransactionDate.Text = DateTime.Now.ToShortDateString();
            }

            txtTransactionDate.Click += TxtTransactionDate_Click;
        }

        private void TxtTransactionDate_Click(object sender, EventArgs e)
        {
            DatePickerFragment fragment = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                txtTransactionDate.Text = time.ToShortDateString();
            });

            fragment.Show(SupportFragmentManager, "DatePickerFragment");
        }

        private void InitalizeTransactionTypes()
        {
            txtExpense = FindViewById<TextView>(Resource.Id.txtExpense);
            txtIncome = FindViewById<TextView>(Resource.Id.txtIncome);
            txtTransfer = FindViewById<TextView>(Resource.Id.txtTransfer);

            if (EditMode)
            {
                switch (transaction.Type)
                {
                    case "Income":
                        txtIncome.SetTextColor(Android.Graphics.Color.White);
                        break;
                    case "Expense":
                        txtExpense.SetTextColor(Android.Graphics.Color.White);
                        break;
                    case "Transfer":
                        txtTransfer.SetTextColor(Android.Graphics.Color.White);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                txtIncome.SetTextColor(Android.Graphics.Color.White);
            }

            txtIncome.Click += TxtIncome_Click;
            txtExpense.Click += TxtExpense_Click;
            txtTransfer.Click += TxtTranfer_Click;
        }

        private void InitalizeSpinners()
        {
            adapterAccount = new ArrayAdapter<Account>(this, Resource.Layout.spinner_item, listOfAccounts);
            adapterAccountTransfer = new ArrayAdapter<Account>(this, Resource.Layout.spinner_item, listOfAccounts);
            adapterCategory = new ArrayAdapter<Category>(this, Resource.Layout.spinner_item, EditMode ? (transaction.Type == "Income" ? listOfIncomeCategories : listOfExpenseCategories) : listOfIncomeCategories);
            adapterPaymentType = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, paymentTypes);

            adapterAccount.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            adapterCategory.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            adapterPaymentType.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            adapterAccountTransfer.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            spinnerTransactionAccount = FindViewById<Spinner>(Resource.Id.spinner_transaction_account);
            spinnerTransactionCategory = FindViewById<Spinner>(Resource.Id.spinner_transaction_category);
            spinnerTransactionPaymentType = FindViewById<Spinner>(Resource.Id.spinner_transaction_payment_type);
            spinnerTransactionAccountTransfer = FindViewById<Spinner>(Resource.Id.spinner_transaction_account_transfer);

            spinnerTransactionAccount.Adapter = adapterAccount;
            spinnerTransactionCategory.Adapter = adapterCategory;
            spinnerTransactionPaymentType.Adapter = adapterPaymentType;
            spinnerTransactionAccountTransfer.Adapter = adapterAccountTransfer;

            spinnerTransactionAccount.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinnerTransactionAccount_ItemClick);

            if (EditMode)
            {
                spinnerTransactionAccount.SetSelection(GetIndex(spinnerTransactionAccount, SilverCoinsManager.GetAccount(transaction.Account).Name));
                spinnerTransactionCategory.SetSelection(GetIndex(spinnerTransactionCategory, SilverCoinsManager.GetCategory(transaction.Category).Name));
                spinnerTransactionPaymentType.SetSelection(adapterPaymentType.GetPosition(transaction.PaymentType));
                if (transaction.AccountTransfer != 0)
                {
                    spinnerTransactionAccountTransfer.SetSelection(GetIndex(spinnerTransactionAccountTransfer, SilverCoinsManager.GetAccount(transaction.AccountTransfer).Name));
                }

                switch (transaction.Type)
                {
                    case "Income":
                        spinnerTransactionAccountTransfer.Visibility = ViewStates.Gone;
                        break;
                    case "Expense":
                        spinnerTransactionAccountTransfer.Visibility = ViewStates.Gone;
                        break;
                    case "Transfer":
                        spinnerTransactionCategory.Visibility = ViewStates.Gone;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                spinnerTransactionAccountTransfer.Visibility = ViewStates.Gone;
            }
        }

        private void SpinnerTransactionAccount_ItemClick(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            txtTransactionCurrency.Text = listOfAccounts.ElementAt(e.Position).Currency;
        }

        private void TxtTranfer_Click(object sender, EventArgs e)
        {
            txtIncome.SetTextColor(Android.Graphics.Color.ParseColor("#B2DFDB"));
            txtExpense.SetTextColor(Android.Graphics.Color.ParseColor("#B2DFDB"));
            txtTransfer.SetTextColor(Android.Graphics.Color.White);

            Type = "Transfer";
            spinnerTransactionAccountTransfer.Visibility = ViewStates.Visible;
            spinnerTransactionCategory.Visibility = ViewStates.Gone;

            txtTransactionCategory.Text = "To account";
            txtTransactionAccount.Text = "From account";
        }

        private void TxtExpense_Click(object sender, EventArgs e)
        {
            txtIncome.SetTextColor(Android.Graphics.Color.ParseColor("#B2DFDB"));
            txtExpense.SetTextColor(Android.Graphics.Color.White);
            txtTransfer.SetTextColor(Android.Graphics.Color.ParseColor("#B2DFDB"));

            Type = "Expense";
            spinnerTransactionAccountTransfer.Visibility = ViewStates.Gone;
            adapterCategory.Clear();
            adapterCategory.AddAll(listOfExpenseCategories);
            adapterCategory.NotifyDataSetChanged();
            spinnerTransactionCategory.Visibility = ViewStates.Visible;
            txtTransactionCategory.Text = "Category";
            txtTransactionAccount.Text = "Account";
        }

        private void TxtIncome_Click(object sender, EventArgs e)
        {
            txtIncome.SetTextColor(Android.Graphics.Color.White);
            txtExpense.SetTextColor(Android.Graphics.Color.ParseColor("#B2DFDB"));
            txtTransfer.SetTextColor(Android.Graphics.Color.ParseColor("#B2DFDB"));

            Type = "Income";
            spinnerTransactionAccountTransfer.Visibility = ViewStates.Gone;
            adapterCategory.Clear();
            adapterCategory.AddAll(listOfIncomeCategories);
            adapterCategory.NotifyDataSetChanged();
            spinnerTransactionCategory.Visibility = ViewStates.Visible;
            txtTransactionCategory.Text = "Category";
            txtTransactionAccount.Text = "Account";
        }

        private void SaveTransaction()
        {
            decimal oldAmount = 0;
            if (EditMode)
            {
                oldAmount = transaction.Amount;
            }
            Account accountFrom = listOfAccounts.ElementAt(spinnerTransactionAccount.SelectedItemPosition);
            Account accountTo = listOfAccounts.ElementAt(spinnerTransactionAccountTransfer.SelectedItemPosition);

            transaction.Name = edtTransactionName.Text;
            transaction.Amount = Convert.ToDecimal(txtTransactionAmount.Text);
            transaction.Account = accountFrom.Id;
            transaction.PaymentType = spinnerTransactionPaymentType.SelectedItem.ToString();
            transaction.Type = Type;
            transaction.CurrencyRate = 1;

            var date = Convert.ToDateTime(txtTransactionDate.Text);
            var datetime = new DateTime(date.Year, date.Month, date.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            transaction.CreatedDate = datetime;

            if (Type == "Transfer")
            {
                transaction.AccountTransfer = accountTo.Id;
                transaction.Category = SilverCoinsManager.GetCategoryByName("Transfer").Id;

                if (accountFrom.Currency != accountTo.Currency)
                {
                    transaction.CurrencyRate = Convert.ToDecimal(edtCurrencyRate.Text);
                }
                else
                {
                    transaction.CurrencyRate = 1;
                }
            }
            else if (Type == "Income")
            {
                transaction.Category = listOfIncomeCategories.ElementAt(spinnerTransactionCategory.SelectedItemPosition).Id;
            }
            else
            {
                transaction.Category = listOfExpenseCategories.ElementAt(spinnerTransactionCategory.SelectedItemPosition).Id;
            }

            SilverCoinsManager.SaveTransaction(transaction);

            if (EditMode)
            {
                AccountCalculations.AccountCalculations.UpdateBalanceAfterTransactionUpdate(accountFrom, accountTo, transaction, oldAmount);
            }
            else
            {
                AccountCalculations.AccountCalculations.UpdateBalanceAfterTransactionCreate(accountFrom, accountTo, transaction);
            }
            Finish();
        }
        private void DeleteTransaction()
        {
            new AlertDialog.Builder(this)
                        .SetCancelable(true)
                        .SetMessage("Delete this item?")
                        .SetPositiveButton("Yes", (sender, args) =>
                        {
                            AccountCalculations.AccountCalculations.
                                UpdateBalanceAfterTransactionDelete(SilverCoinsManager.GetAccount(transaction.Account),
                                                                    SilverCoinsManager.GetAccount(transaction.AccountTransfer),
                                                                    transaction);
                            SilverCoinsManager.DeleteTransaction(transaction.Id);
                            OnBackPressed();
                            Finish();
                        })
                        .SetNegativeButton("No", (sender, args) => { })
                        .Show();
        }

        private int GetIndex(Spinner spinner, string myString)
        {
            int index = 0;

            for (int i = 0; i < spinner.Count; i++)
            {
                if (spinner.GetItemAtPosition(i).ToString() == myString)
                {
                    index = i;
                }
            }
            return index;
        }
    }
}