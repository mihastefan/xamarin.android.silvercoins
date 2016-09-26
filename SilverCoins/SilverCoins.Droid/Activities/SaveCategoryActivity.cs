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
using SilverCoins.Droid.Adapters;

namespace SilverCoins.Droid.Activities
{
    [Activity(Label = "Save Category", ParentActivity = typeof(HomeActivity),
              ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    [MetaData("android.support.PARENT_ACTIVITY", Value = "silverCoins.droid.activities.HomeActivity")]
    public class SaveCategoryActivity : BaseActivity
    {
        protected Category category;
        protected EditText editName;
        protected EditText editDescription;
        protected Spinner spinnerCategoryType;
        protected GridView gridIcons;
        private IconsAdapter iconsAdapter;
        protected string[] types = new string[] { "Income", "Expense" };
        protected bool editMode = false;
        protected int iconDrawable = Resource.Drawable.salary;
        protected int[] categoryIcons =
        {
            Resource.Drawable.salary, Resource.Drawable.pets,
            Resource.Drawable.groceries, Resource.Drawable.electronics,
            Resource.Drawable.eating_out, Resource.Drawable.entertainment,
            Resource.Drawable.car, Resource.Drawable.kids,
            Resource.Drawable.shopping, Resource.Drawable.sports,
            Resource.Drawable.vacation,
        };

        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.activity_save_category;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Toolbar.SetNavigationIcon(Resource.Drawable.ic_cancel);

            Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#00796B"));

            editName = FindViewById<EditText>(Resource.Id.edit_category_name);
            editDescription = FindViewById<EditText>(Resource.Id.edit_category_description);

            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, types);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerCategoryType = FindViewById<Spinner>(Resource.Id.spinner_category_type);
            spinnerCategoryType.Adapter = adapter;

            // Initialize gridview and handle click events
            gridIcons = FindViewById<GridView>(Resource.Id.gridviewCategory);
            iconsAdapter = new IconsAdapter(this, categoryIcons);
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

            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                editMode = extras.GetBoolean("EDIT_MODE");
                if (editMode)
                {
                    category = Newtonsoft.Json.JsonConvert.DeserializeObject<Category>(extras.GetString("CATEGORY_OBJECT"));
                    editName.Text = category.Name;
                    editDescription.Text = category.Description;
                    spinnerCategoryType.SetSelection(adapter.GetPosition(category.Type.ToString()));
                    gridIcons.Tag = category.Icon;
                }
            }
            else
            {
                category = new Category();
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
                    SaveCategory();
                    return true;
                case Resource.Id.action_delete:
                    DeleteCategory();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void SaveCategory()
        {
            category.Name = editName.Text;
            category.Description = editDescription.Text;
            category.Type = Category.CategoryTypes.Income.ToString() == spinnerCategoryType.SelectedItem.ToString() ? Category.CategoryTypes.Income : Category.CategoryTypes.Expense;
            category.Visible = 1;
            category.Icon = iconDrawable;

            if (!editMode)
                category.CreatedDate = DateTime.Today;

            SilverCoinsManager.SaveCategory(category);
            Finish();
        }

        private void DeleteCategory()
        {

            if (category.Id != 0)
            {
                new AlertDialog.Builder(this)
                        .SetCancelable(true)
                        .SetMessage("Delete this item?")
                        .SetPositiveButton("Yes", (sender, args) =>
                        {
                            CheckIfCategoryCanBeDeleted(category.Id);
                        })
                        .SetNegativeButton("No", (sender, args) => { })
                        .Show();
            }
        }

        private void CheckIfCategoryCanBeDeleted(int categoryId)
        {
            if (SilverCoinsManager.GetTransactionsByCategory(categoryId).Any())
            {
                Toast.MakeText(this, Utils.Constants.CategoryDeletionWarning, ToastLength.Short).Show();
            }
            else
            {
                SilverCoinsManager.DeleteCategory(categoryId);
                OnBackPressed();
            }
            
        }
    }
}