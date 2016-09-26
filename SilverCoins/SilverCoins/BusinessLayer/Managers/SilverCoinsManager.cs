using System;
using System.Collections.Generic;
using System.Text;
using SilverCoins.BusinessLayer.Models;
using SilverCoins.DataAccessLayer;
using System.Linq;

namespace SilverCoins.BusinessLayer.Managers
{
    public static class SilverCoinsManager
    {
        static SilverCoinsManager()
        {
        }

        #region Account

        public static Account GetAccount(int id)
        {
            return SilverCoinsRepository.GetAccount(id);
        }

        public static Account GetAccountByName(string name)
        {
            return SilverCoinsRepository.GetAccountByName(name);
        }

        public static IList<Account> GetAccounts()
        {
            return new List<Account>(SilverCoinsRepository.GetAccounts());
        }

        public static int SaveAccount(Account item)
        {
            return SilverCoinsRepository.SaveAccount(item);
        }

        public static int DeleteAccount(int id)
        {
            var transactions = GetAllTransactionsForAccount(id);
            foreach (var transaction in transactions)
            {
                DeleteTransaction(transaction.Id);
            }
            return SilverCoinsRepository.DeleteAccount(id);
        }

        #endregion

        #region Category

        public static List<Category> GetIncomeCategories()
        {
            return GetCategories().Where(x => x.Visible == 1 && x.Type == Category.CategoryTypes.Income).ToList();
        }

        public static List<Category> GetExpenseCategories()
        {
            return GetCategories().Where(x => x.Visible == 1 && x.Type == Category.CategoryTypes.Expense).ToList();
        }

        public static List<Category> GetVisibleCategories()
        {
            return GetCategories().Where(x => x.Visible == 1).ToList();
        }

        public static Category GetCategory(int id)
        {
            return SilverCoinsRepository.GetCategory(id);
        }

        public static Category GetCategoryByName(string name)
        {
            return SilverCoinsRepository.GetCategoryByName(name);
        }

        public static IList<Category> GetCategories()
        {
            return new List<Category>(SilverCoinsRepository.GetCategories());
        }

        public static int SaveCategory(Category item)
        {
            return SilverCoinsRepository.SaveCategory(item);
        }

        public static int DeleteCategory(int id)
        {
            return SilverCoinsRepository.DeleteCategory(id);
        }

        #endregion

        #region Transaction

        public static List<Transaction> GetTransactionsByCategory(int categoryId)
        {
            return GetTransactions().Where(x => x.Category == categoryId).ToList();
        }

        public static List<Transaction> GetTransactionsByDate(int day, int month, int year, int accountId)
        {
            var list = new List<Transaction>();
            if (month == 0)
            {
                list = GetTransactions().Where(x => x.CreatedDate.Year == year).ToList();
            }
            else if (day == 0)
            {
                list = GetTransactions().Where(x => x.CreatedDate.Year == year &&
                                               x.CreatedDate.Month == month).ToList();
            }
            else
            {
                list = GetTransactions().Where(x => x.CreatedDate.Year == year &&
                                               x.CreatedDate.Month == month &&
                                               x.CreatedDate.Day == day).ToList();
            }

            if (accountId != 0)
            {
                list.Where(x => x.Account == accountId);
            }

            return list;
        }

        public static List<Transaction> GetAllTransactionsForAccount(int accountId)
        {
            return GetTransactions()
                    .Where(x => x.Account == accountId || x.AccountTransfer == accountId)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList();
        }

        public static Transaction GetTransaction(int id)
        {
            return SilverCoinsRepository.GetTransaction(id);
        }

        public static IList<Transaction> GetTransactions()
        {
            return new List<Transaction>(SilverCoinsRepository.GetTransactions()
                .OrderByDescending(x => x.CreatedDate));
        }

        public static int SaveTransaction(Transaction item)
        {
            return SilverCoinsRepository.SaveTransaction(item);
        }

        public static int DeleteTransaction(int id)
        {
            return SilverCoinsRepository.DeleteTransaction(id);
        }

        #endregion

        #region Configuration

        public static Configuration GetConfigurationRecordByKey(string key)
        {
            return SilverCoinsRepository.GetConfigurationRecordByKey(key);
        }

        public static Configuration GetConfiguration(int id)
        {
            return SilverCoinsRepository.GetConfiguration(id);
        }

        public static int SaveConfiguration(Configuration item)
        {
            return SilverCoinsRepository.SaveConfiguration(item);
        }

        public static int DeleteConfiguration(int id)
        {
            return SilverCoinsRepository.DeleteConfiguration(id);
        }

        #endregion

    }
}
