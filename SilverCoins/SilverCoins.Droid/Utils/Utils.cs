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
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using SilverCoins.BusinessLayer.Models;

namespace SilverCoins.Droid.Utils
{
    public static class Utils
    {
        public static Dictionary<string, string> currencies = new Dictionary<string, string>()
        {
            { "RSD", "Serbian Dinar (RSD)"},
            { "EUR", "Euro (EUR)"},
            { "GBP", "British Pound (GBP)"},
            { "USD", "US Dollar (USD)"},
            { "CHF", "Swiss Franc (CHF)"}
        };

        public static void ShowSnackbar(View parentView, string messageText, string actionText)
        {
            Snackbar.Make(parentView, messageText, 3000)
                    .SetAction(actionText, action => { })
                    .Show();
        }

        public static string GetCurrencyFullName(string currencyCode)
        {
            return currencies[currencyCode];
        }

        internal static string GetCurrencyCode(string currency)
        {
            return currencies.FirstOrDefault(x => x.Value == currency).Key;
        }

        internal static void ShowAlertDialog(Context context, string message)
        {
            new AlertDialog.Builder(context)
                        .SetCancelable(true)
                        .SetMessage(message)
                        .SetNegativeButton("OK", (sender, args) => { })
                        .Show();
        }
    }
}