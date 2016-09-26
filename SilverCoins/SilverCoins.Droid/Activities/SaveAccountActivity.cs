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
using SilverCoins.BusinessLayer.Managers;
using System.Text.RegularExpressions;
using SilverCoins.Droid.Adapters;

namespace SilverCoins.Droid.Activities
{
    [Activity(Label = "Save Account", ParentActivity = typeof(HomeActivity),
              ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    [MetaData("android.support.PARENT_ACTIVITY", Value = "silverCoins.droid.activities.HomeActivity")]
    public class SaveAccountActivity : BaseActivity
    {
        protected string[] currencies = new string[]
                                        {
                                            "Serbian Dinar (RSD)", "Euro (EUR)", "British Pound (GBP)", "US Dollar (USD)", "Swiss Franc (CHF)"
                                        };

        protected Account account;
        protected EditText editName;
        protected EditText editDescription;
        protected Spinner spinnerCurrency;
        protected GridView gridIcons;
        private IconsAdapter iconsAdapter;
        protected int iconDrawable = Resource.Drawable.cash;
        protected bool editMode = false;
        protected int[] accountIcons =
        {
            Resource.Drawable.cash, Resource.Drawable.paypal,
            Resource.Drawable.diners, Resource.Drawable.mastercard,
            Resource.Drawable.visa, Resource.Drawable.deposits,
            Resource.Drawable.swiss_frank, Resource.Drawable.pound,
            Resource.Drawable.euro, Resource.Drawable.dollar
        };

        protected override int LayoutResource { get { return Resource.Layout.activity_save_account; } }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Toolbar.SetNavigationIcon(Resource.Drawable.ic_cancel);

            Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#00796B"));

            // Initalize text fields
            editName = FindViewById<EditText>(Resource.Id.edit_account_name);
            editDescription = FindViewById<EditText>(Resource.Id.edit_account_description);

            //Initalize spinner
            ArrayAdapter ad = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, currencies);
            ad.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerCurrency = FindViewById<Spinner>(Resource.Id.spinner_account_currency);
            spinnerCurrency.Adapter = ad;

            // Initialize gridview and handle click events
            gridIcons = FindViewById<GridView>(Resource.Id.gridview);
            iconsAdapter = new IconsAdapter(this, accountIcons);
            gridIcons.Adapter = iconsAdapter;

            View previousGridItem = null; // The previous selected item
            bool flag = true;

            gridIcons.ItemClick += (sender, args) =>
            {
                if (flag)
                {
                    previousGridItem = gridIcons.GetChildAt(0);
                    flag = false;
                }
                if (previousGridItem != args.View)
                {
                    args.View.SetBackgroundResource(Resource.Color.accent);
                    previousGridItem.SetBackgroundResource(0);
                    previousGridItem = args.View;
                }
                iconDrawable = (int)gridIcons.GetItemIdAtPosition(args.Position);
            };

            // If edit mode -> pre-fill fields
            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                editMode = extras.GetBoolean("EDIT_MODE");
                if (editMode)
                {
                    account = Newtonsoft.Json.JsonConvert.DeserializeObject<Account>(extras.GetString("ACCOUNT_JSON"));
                    editName.Text = account.Name;
                    editDescription.Text = account.Description;
                    spinnerCurrency.SetSelection(ad.GetPosition(Utils.Utils.GetCurrencyFullName(account.Currency)));
                    gridIcons.Tag = account.Icon;
                }
            }
            else
            {
                account = new Account();
                gridIcons.Tag = iconDrawable;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (editMode)
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
                    SaveAccount();
                    return true;
                case Resource.Id.action_delete:
                    DeleteAccount();
                    return true;
                default:
                    Finish();
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void SaveAccount()
        {
            account.Name = editName.Text;
            account.Description = editDescription.Text;
            account.Currency = Regex.Match((string)spinnerCurrency.SelectedItem, @"(?<=\().+?(?=\))").ToString();
            account.Icon = iconDrawable;

            account.CreatedDate = DateTime.Today;

            SilverCoinsManager.SaveAccount(account);
            Finish();
        }

        private void DeleteAccount()
        {
            new AlertDialog.Builder(this)
                        .SetCancelable(true)
                        .SetMessage("Delete this item?")
                        .SetPositiveButton("Yes", (sender, args) =>
                        {
                            SilverCoinsManager.DeleteAccount(account.Id);
                            OnBackPressed();
                            Finish();
                        })
                        .SetNegativeButton("No", (sender, args) => { })
                        .Show();

        }

    }

}