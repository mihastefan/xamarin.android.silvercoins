using Android.Content;
using Android.Views;
using Android.Widget;
using System;

namespace SilverCoins.Droid.Adapters
{
    class IconsAdapter : BaseAdapter
    {
        Context context;

        public IconsAdapter(Context _context, int[] icons)
        {
            context = _context;
            Icons = icons;
        }

        public int[] Icons { get; set; }

        public override int Count
        {
            get
            {
                return Icons.Length;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return Icons[position];
        }

        public override long GetItemId(int position)
        {
            return Icons[position];
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ImageView imageView;

            if(convertView == null)
            {
                imageView = new ImageView(context);
                imageView.LayoutParameters = new GridView.LayoutParams(85, 85);
                imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
                imageView.SetPadding(8, 8, 8, 8);
            }
            else
            {
                imageView = (ImageView)convertView;
            }
            // Sets the background for the first item that should be pre-selected.
            int iconPosition = GetPosition((int)parent.Tag);
            if (position == iconPosition)
                imageView.SetBackgroundResource(Resource.Color.accent);

            imageView.SetImageResource(Icons[position]);

            return imageView;
        }

        public int GetPosition(int iconId)
        {
            return Array.IndexOf(Icons, iconId);
        }
    }
}