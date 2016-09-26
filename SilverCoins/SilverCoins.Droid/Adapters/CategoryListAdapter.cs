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

namespace SilverCoins.Droid.Adapters
{
    class CategoryListAdapter : BaseAdapter<Category>
    {
        Activity context;
        IList<Category> categories = new List<Category>();

        public CategoryListAdapter(Activity _context, IList<Category> _categories)
            :base()
        {
            context = _context;
            categories = _categories;
        }

        public override int Count
        {
            get
            {
                return categories.Count;
            }
        }

        public override Category this[int position]
        {
            get
            {
                return categories[position];
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            if (view == null)
            {
                view = context.LayoutInflater.Inflate(Resource.Layout.list_view_item, parent, false);
            }

            Category category = this[position];

            view.FindViewById<ImageView>(Resource.Id.account_icon).SetImageResource(category.Icon != 0 ? category.Icon : Resource.Drawable.ic_launcher);
            view.FindViewById<TextView>(Resource.Id.account_name).Text = category.Name;
            view.FindViewById<TextView>(Resource.Id.account_description).Text = category.Type.ToString();

            return view;
        }
    }
}