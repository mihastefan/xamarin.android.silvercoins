using SilverCoins.BusinessLayer.Managers;
using SilverCoins.BusinessLayer.Models;
using SilverCoins.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilverCoins.ImportExport
{
    internal static class ImportExport
    {
        internal static List<TransactionImportExport> GetTransactionForExport(Account account, string type, DateTime from, DateTime to)
        {
            var transactionsExport = new List<TransactionImportExport>();
            var transactions = SilverCoinsManager.GetTransactions().Where(x => x.CreatedDate >= from && x.CreatedDate <= to).ToList();

            if (account.Id != 0)
            {
                transactions.Where(x => x.Account == account.Id).ToList();
            }

            if (type != "Both")
            {
                transactions.Where(x => x.Type == type).ToList();
            }

            foreach (var transaction in transactions)
            {
                var category = SilverCoinsManager.GetCategory(transaction.Category);
                var accountAc = SilverCoinsManager.GetAccount(transaction.Account);
                transactionsExport.Add(new TransactionImportExport
                {
                    Account = accountAc.Name,
                    Category = category.Name,
                    Type = transaction.Type,
                    Amount = transaction.Amount.ToString("N2"),
                    Currency = accountAc.Currency,
                    Date = transaction.CreatedDate.ToString("dd/MM/yyyy"),
                    PaymentType = transaction.PaymentType,
                    Name = transaction.Name,
                    Balance = accountAc.Balance.ToString("N2")
                });
            }

            return transactionsExport;
        }

        internal static void ImportTransactionsFromList(IEnumerable<TransactionImportExport> transactionsForImport)
        {
            if(transactionsForImport.Any())
            {
                // Get list of accounts and categories from document
                var accountsForImport = transactionsForImport.Select(x => new Account
                {
                    Name = x.Account,
                    Currency = x.Currency,
                    Balance = Convert.ToDecimal(x.Balance),
                    CreatedDate = DateTime.Today,
                    Description = string.Empty,
                    Icon = Resource.Drawable.cash
                });

                var categoriesForImport = transactionsForImport.Select(x => new Category
                {
                    Name = x.Category,
                    Type = x.Type == Category.CategoryTypes.Expense.ToString() ? Category.CategoryTypes.Expense : Category.CategoryTypes.Income,
                    Description = string.Empty,
                    CreatedDate = DateTime.Today,
                    Visible = 1,
                    Icon = Resource.Drawable.salary
                });

                var accounts = SilverCoinsManager.GetAccounts();
                var categories = SilverCoinsManager.GetCategories();

                // Find accounts and categories that do not exist in database
                var newAccounts = accountsForImport.Where(x => !accounts.Any(s => s.Name == x.Name));
                var newCategories = categoriesForImport.Where(x => !categories.Any(s => s.Name == x.Name));

                // Insert missing accounts and categories
                if (newAccounts.Any())
                {
                    foreach (var account in newAccounts)
                    {
                        SilverCoinsManager.SaveAccount(account);
                    }
                }

                if (newCategories.Any())
                {
                    foreach (var category in newCategories)
                    {
                        SilverCoinsManager.SaveCategory(category);
                    }
                }

                foreach (var transaction in transactionsForImport)
                {
                    SilverCoinsManager.SaveTransaction(new Transaction
                    {
                        Name = transaction.Name,
                        Account = SilverCoinsManager.GetAccountByName(transaction.Account).Id,
                        Category = SilverCoinsManager.GetCategoryByName(transaction.Category).Id,
                        Amount = Convert.ToDecimal(transaction.Amount),
                        PaymentType = transaction.PaymentType,
                        Type = transaction.Type,
                        CreatedDate = DateTime.Now
                    });
                }
            }
        }
    }
}
