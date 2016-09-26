using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Views;
using SilverCoins.BusinessLayer.Models;
using SilverCoins.Droid.Fragments;
using System.Threading.Tasks;
using System;
using SilverCoins.BusinessLayer.Managers;
using System.Collections.Generic;
using System.Linq;
using Android.Widget;
using Refractored.Controls;

namespace SilverCoins.Droid.Activities
{
    [Activity(Label = "Silver Coins", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@drawable/ic_launcher",
              ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class HomeActivity : BaseActivity
    {
        DrawerLayout drawerLayout;
        NavigationView navigationView;

        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.activity_home;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            PrepareCategories();
            PrepareAccounts();

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_overview:
                        ListItemClicked(0);
                        break;
                    case Resource.Id.nav_account:
                        ListItemClicked(1);
                        break;
                    case Resource.Id.nav_category:
                        ListItemClicked(2);
                        break;
                    case Resource.Id.nav_statistics:
                        ListItemClicked(3);
                        break;
                    case Resource.Id.nav_export:
                        ListItemClicked(4);
                        break;
                    case Resource.Id.nav_settings:
                        ListItemClicked(5);
                        break;
                }

                drawerLayout.CloseDrawers();
            };

            if (savedInstanceState == null)
            {
                ListItemClicked(0);
            }
            var prefs = this.GetSharedPreferences(Utils.Constants.SilverCoinsPreferences, FileCreationMode.Private);
            var profileName = prefs.GetString(Utils.Constants.ProfileName, "John Doe");
            var profileEmail = prefs.GetString(Utils.Constants.ProfileEmail, "example@email.com");
            var profileImageUri = prefs.GetString(Utils.Constants.ProfileImageUri, null);

            View headerView = navigationView.GetHeaderView(0);
            headerView.FindViewById<TextView>(Resource.Id.username).Text = profileName;
            headerView.FindViewById<TextView>(Resource.Id.email).Text = profileEmail;

            if (profileImageUri != null)
            {
                headerView.FindViewById<CircleImageView>(Resource.Id.profile_image).SetImageURI(Android.Net.Uri.Parse(profileImageUri));
            }
            else
            {
                headerView.FindViewById<CircleImageView>(Resource.Id.profile_image).SetImageResource(Resource.Drawable.nav_header_profile);
            }
        }

        private void ListItemClicked(int position)
        {
            var parentView = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            Android.Support.V4.App.Fragment fragment = null;
            switch (position)
            {
                case 0:
                    fragment = new OverviewFragment();
                    break;
                case 1:
                    fragment = new AccountsFragment();
                    break;
                case 2:
                    fragment = new CategoriesFragment();
                    break;
                case 3:
                    fragment = new StatisticsFragment();
                    break;
                case 4:
                    fragment = new ImportExportFragment();
                    break;
                case 5:
                    fragment = new ProfileFragment();
                    break;
            }

            SupportFragmentManager.BeginTransaction()
                                  .Replace(Resource.Id.content_frame, fragment)
                                  .Commit();

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            if (SupportFragmentManager.BackStackEntryCount > 0)
            {
                SupportFragmentManager.PopBackStack();
                return;
            }
            base.OnBackPressed();
        }

        private void PrepareCategories()
        {
            Configuration categoryConfig = SilverCoinsManager.GetConfigurationRecordByKey("CATINIT");

            if (categoryConfig == null || categoryConfig.BooleanValue == false)
            {
                Category[] categories =
                {
                new Category { Name = "Salary",
                               Description = "",
                               Type = Category.CategoryTypes.Income,
                               CreatedDate = DateTime.Today,
                               Visible = 1,
                               Icon = Resource.Drawable.salary,},
                new Category { Name = "Groceries",
                               Description = "",
                               Type = Category.CategoryTypes.Expense,
                               CreatedDate = DateTime.Today,
                               Visible = 1,
                               Icon = Resource.Drawable.groceries,},
                new Category { Name = "Eating out",
                               Description = "",
                               Type = Category.CategoryTypes.Expense,
                               CreatedDate = DateTime.Today,
                               Visible = 1,
                               Icon = Resource.Drawable.eating_out,},
                new Category { Name = "Holidays",
                               Description = "",
                               Type = Category.CategoryTypes.Expense,
                               CreatedDate = DateTime.Today,
                               Visible = 1,
                               Icon = Resource.Drawable.vacation,},
                new Category { Name = "Sport",
                               Description = "",
                               Type = Category.CategoryTypes.Expense,
                               CreatedDate = DateTime.Today,
                               Visible = 1,
                               Icon = Resource.Drawable.sports,},
                new Category { Name = "Car",
                               Description = "",
                               Type = Category.CategoryTypes.Expense,
                               CreatedDate = DateTime.Today,
                               Visible = 1,
                               Icon = Resource.Drawable.car, },
                new Category { Name = "Shopping",
                               Description = "",
                               Type = Category.CategoryTypes.Expense,
                               CreatedDate = DateTime.Today,
                               Visible = 1,
                               Icon = Resource.Drawable.shopping,},
                new Category { Name = "Entertainment",
                               Description = "",
                               Type = Category.CategoryTypes.Expense,
                               CreatedDate = DateTime.Today,
                               Visible = 1,
                               Icon = Resource.Drawable.entertainment,},
                new Category { Name = "Pets",
                               Description = "",
                               Type = Category.CategoryTypes.Expense,
                               CreatedDate = DateTime.Today,
                               Visible = 1,
                               Icon = Resource.Drawable.pets,},
                new Category { Name = "Kids",
                               Description = "",
                               Type = Category.CategoryTypes.Expense,
                               CreatedDate = DateTime.Today,
                               Visible = 1,
                               Icon = Resource.Drawable.kids,},
                new Category { Name = "Electronics",
                               Description = "",
                               Type = Category.CategoryTypes.Expense,
                               CreatedDate = DateTime.Today,
                               Visible = 1,
                               Icon = Resource.Drawable.electronics,},
                new Category { Name = "Transfer",
                               Description = "",
                               Type = Category.CategoryTypes.MoneyTransfer,
                               CreatedDate = DateTime.Today,
                               Visible = 0,
                               Icon = Resource.Drawable.transfer},
                };

                foreach (var category in categories)
                {
                    SilverCoinsManager.SaveCategory(category);
                }

                SilverCoinsManager.SaveConfiguration(new Configuration()
                {
                    Name = "Categories initalized",
                    Key = "CATINIT",
                    BooleanValue = true
                });
            }
        }

        private void PrepareAccounts()
        {
            Configuration accountConfig = SilverCoinsManager.GetConfigurationRecordByKey("ACCINIT");
            if (accountConfig == null || accountConfig.BooleanValue == false)
            {
                Account[] accounts =
                {
                    new Account()
                    {
                        Name = "Checking account",
                        Currency = "EUR",
                        CreatedDate = DateTime.Today,
                        Icon = Resource.Drawable.euro
                    },
                    new Account()
                    {
                        Name = "Savings account",
                        Currency = "EUR",
                        CreatedDate = DateTime.Today,
                        Icon = Resource.Drawable.deposits
                    },
                    new Account()
                    {
                        Name = "Cash",
                        Currency = "EUR",
                        CreatedDate = DateTime.Today,
                        Icon = Resource.Drawable.cash
                    },
                };

                foreach (var account in accounts)
                {
                    SilverCoinsManager.SaveAccount(account);
                }

                SilverCoinsManager.SaveConfiguration(new Configuration()
                {
                    Name = "Accounts initalized",
                    Key = "ACCINIT",
                    BooleanValue = true
                });
            }
        }
    }
}