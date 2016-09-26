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
using SilverCoins.BusinessLayer.Models;
using SilverCoins.Droid.Adapters;

namespace SilverCoins.Droid.Fragments
{
    public class CategoriesFragment : Fragment
    {
        private View view;
        private ListView listView;
        private IList<Category> categories;

        public CategoriesFragment()
        {
            RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ((HomeActivity)Activity).SupportActionBar.SetDisplayShowTitleEnabled(true);
            ((HomeActivity)Activity).SupportActionBar.SetDisplayShowCustomEnabled(false);
            ((HomeActivity)Activity).SupportActionBar.Title = "Categories";
            HasOptionsMenu = true;

            view = inflater.Inflate(Resource.Layout.fragment_categories, null);
            listView = view.FindViewById<ListView>(Resource.Id.listViewCategories);

            listView.ItemClick += OnListItemClick;
            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            categories = SilverCoins.BusinessLayer.Managers.SilverCoinsManager.GetVisibleCategories();
            listView.Adapter = new CategoryListAdapter(Activity, categories);
        }

        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs args)
        {
            Category category = categories[args.Position];
            string categoryJson = Newtonsoft.Json.JsonConvert.SerializeObject(category);

            Intent intent = new Intent(Activity, typeof(SaveCategoryActivity));
            intent.PutExtra("CATEGORY_OBJECT", categoryJson);
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
                    Intent intent = new Intent(Activity, typeof(SaveCategoryActivity));
                    StartActivity(intent);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}