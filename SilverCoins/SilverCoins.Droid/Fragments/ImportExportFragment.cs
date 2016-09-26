using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using LINQtoCSV;
using SilverCoins.BusinessLayer.Managers;
using SilverCoins.BusinessLayer.Models;
using SilverCoins.Droid.Activities;
using SilverCoins.Droid.Dialogs;
using SilverCoins.ImportExport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Fragment = Android.Support.V4.App.Fragment;

namespace SilverCoins.Droid.Fragments
{
    public class ImportExportFragment : Fragment
    {
        private List<Account> listOfAccounts = SilverCoinsManager.GetAccounts().ToList();
        private Spinner spinnerAccount,
                        spinnerType;
        private ArrayAdapter<Account> adapterAccount;
        private ArrayAdapter<string> adapterType;
        private TextView txtFrom,
                         txtTo;
        private View view;
        private Button buttonExport;
        private static readonly int pickFileResultCode = 1;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ((HomeActivity)Activity).SupportActionBar.SetDisplayShowTitleEnabled(true);
            ((HomeActivity)Activity).SupportActionBar.SetDisplayShowCustomEnabled(false);
            ((HomeActivity)Activity).SupportActionBar.Title = "Export";
            HasOptionsMenu = true;

            view = inflater.Inflate(Resource.Layout.fragment_export, null);

            listOfAccounts.Insert(0, new Account() { Name = "All accounts", Id = 0 });

            adapterAccount = new ArrayAdapter<Account>(Activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, listOfAccounts);
            adapterType = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, new string[] { "Both", "Income", "Expense" });
            adapterAccount.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            adapterType.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            spinnerAccount = view.FindViewById<Spinner>(Resource.Id.spinner_export_account);
            spinnerType = view.FindViewById<Spinner>(Resource.Id.spinner_export_type);
            spinnerAccount.Adapter = adapterAccount;
            spinnerType.Adapter = adapterType;

            InitalizeDatePickers();

            buttonExport = view.FindViewById<Button>(Resource.Id.buttonExport);
            buttonExport.Click += ButtonExport_Click;

            return view;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.menu_import, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_import:
                    FileChooser();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void FileChooser()
        {
            Intent intent = new Intent();
            intent.SetType("text/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Select file"), pickFileResultCode);
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if ((requestCode == pickFileResultCode) && (resultCode == (int)Result.Ok) && (data != null))
            {
                Android.Net.Uri fileUri = data.Data;
                ImportTransactionsFromFile(fileUri);
            }
        }

        private void ImportTransactionsFromFile(Android.Net.Uri fileUri)
        {
            Java.IO.File myFile = new Java.IO.File(fileUri.Path);
            var path = myFile.AbsolutePath;

            CsvFileDescription inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true
            };
            CsvContext cc = new CsvContext();

            try
            {
                IEnumerable<TransactionImportExport> transactionsForImport = cc.Read<TransactionImportExport>(path, inputFileDescription);
                ImportExport.ImportExport.ImportTransactionsFromList(transactionsForImport);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Activity, Utils.Constants.GeneralError, ToastLength.Short).Show();
            }

        }

        private void ButtonExport_Click(object sender, EventArgs e)
        {
            Account account = listOfAccounts[spinnerAccount.SelectedItemPosition];
            string type = spinnerType.SelectedItem.ToString();
            DateTime from = Convert.ToDateTime(txtFrom.Text);
            DateTime to = Convert.ToDateTime(txtTo.Text);

            to = new DateTime(to.Year, to.Month, to.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            var transactionsForExport = ImportExport.ImportExport.GetTransactionForExport(account, type, from, to);
            string filename = Path.Combine(Context.ExternalCacheDir.AbsolutePath, "SilverCoins_Export_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");

            CsvFileDescription outputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                TextEncoding = Encoding.UTF8
            };

            CsvContext cc = new CsvContext();
            cc.Write(transactionsForExport, filename, outputFileDescription);

            var email = new Intent(Intent.ActionSend);
            var file = new Java.IO.File(filename);
            var uri = Android.Net.Uri.FromFile(file);

            file.SetReadable(true, true);
            email.PutExtra(Intent.ExtraSubject, "Export from SilverCoins");
            email.PutExtra(Intent.ExtraStream, uri);
            email.SetType("message/rfc822");

            StartActivity(Intent.CreateChooser(email, "Export file..."));
        }

        private void InitalizeDatePickers()
        {
            txtFrom = view.FindViewById<TextView>(Resource.Id.date_export_from);
            txtFrom.Text = DateTime.Today.AddMonths(-1).ToShortDateString();
            txtFrom.Click += TxtExportFrom_Click;

            txtTo = view.FindViewById<TextView>(Resource.Id.date_export_to);
            txtTo.Text = DateTime.Today.ToShortDateString();
            txtTo.Click += TxtExportTo_Click;
        }

        private void TxtExportTo_Click(object sender, EventArgs e)
        {
            DatePickerFragment fragment = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                txtTo.Text = time.ToShortDateString();
            });

            fragment.Show(Activity.SupportFragmentManager, "DatePickerFragment");
        }

        private void TxtExportFrom_Click(object sender, EventArgs e)
        {
            DatePickerFragment fragment = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                txtFrom.Text = time.ToShortDateString();
            });

            fragment.Show(Activity.SupportFragmentManager, "DatePickerFragment");
        }
    }
}