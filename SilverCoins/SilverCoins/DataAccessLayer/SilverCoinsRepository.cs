using System;
using System.Collections.Generic;
using System.Text;
using SilverCoins.DataLayer;
using SilverCoins.BusinessLayer.Models;
using System.IO;

namespace SilverCoins.DataAccessLayer
{
    public class SilverCoinsRepository
    {
        SilverCoinsDatabase db = null;
        protected static string dbLocation;
        protected static SilverCoinsRepository scr;

        static SilverCoinsRepository()
        {
            scr = new SilverCoinsRepository();
        }

        protected SilverCoinsRepository()
        {
            // set the db location
            dbLocation = DatabaseFilePath;

            // instantiate the database	
            db = new SilverCoinsDatabase(dbLocation);
        }

        public static string DatabaseFilePath
        {
            get
            {
                var sqliteFilename = "SilverCoins.db3";

#if NETFX_CORE
                var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sqliteFilename);
#else

#if SILVERLIGHT
				// Windows Phone expects a local path, not absolute
	            var path = sqliteFilename;
#else

#if __ANDROID__
				// Just use whatever directory SpecialFolder.Personal returns
	            string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
#else
                // we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
                // (they don't want non-user-generated data in Documents)
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
                string libraryPath = Path.Combine(documentsPath, "../Library/"); // Library folder
#endif
                var path = Path.Combine(libraryPath, sqliteFilename);
#endif

#endif
                return path;
            }
        }

        #region Accounts

        public static Account GetAccount(int id)
        {
            return scr.db.GetItem<Account>(id);
        }

        public static Account GetAccountByName(string name)
        {
            return scr.db.GetItemByName<Account>(name);
        }

        public static IEnumerable<Account> GetAccounts()
        {
            return scr.db.GetItems<Account>();
        }

        public static int SaveAccount(Account item)
        {
            return scr.db.SaveItem<Account>(item);
        }

        public static int DeleteAccount(int id)
        {
            return scr.db.DeleteItem<Account>(id);
        }

        #endregion

        #region Category

        public static Category GetCategory(int id)
        {
            return scr.db.GetItem<Category>(id);
        }

        public static Category GetCategoryByName(string name)
        {
            return scr.db.GetItemByName<Category>(name);
        }

        public static IEnumerable<Category> GetCategories()
        {
            return scr.db.GetItems<Category>();
        }

        public static int SaveCategory(Category item)
        {
            return scr.db.SaveItem<Category>(item);
        }

        public static int DeleteCategory(int id)
        {
            return scr.db.DeleteItem<Category>(id);
        }

        #endregion

        #region Transaction

        public static Transaction GetTransaction(int id)
        {
            return scr.db.GetItem<Transaction>(id);
        }

        public static IEnumerable<Transaction> GetTransactions()
        {
            return scr.db.GetItems<Transaction>();
        }

        public static int SaveTransaction(Transaction item)
        {
            return scr.db.SaveItem<Transaction>(item);
        }

        public static int DeleteTransaction(int id)
        {
            return scr.db.DeleteItem<Transaction>(id);
        }

        #endregion

        #region Configuration

        public static Configuration GetConfigurationRecordByKey(string key)
        {
            return scr.db.GetConfigurationRecordByKey(key);
        }

        public static Configuration GetConfiguration(int id)
        {
            return scr.db.GetItem<Configuration>(id);
        }

        public static int SaveConfiguration(Configuration item)
        {
            return scr.db.SaveItem<Configuration>(item);
        }

        public static int DeleteConfiguration(int id)
        {
            return scr.db.DeleteItem<Configuration>(id);
        }

        #endregion
    }

}
