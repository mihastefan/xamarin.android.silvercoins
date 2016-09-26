using SilverCoins.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using SilverCoins.BusinessLayer.Managers;

namespace SilverCoins.AccountCalculations
{
    public static class AccountCalculations
    {
        public static void UpdateBalanceAfterTransactionCreate(Account accountFrom, Account accountTo, Transaction transaction)
        {
            if (transaction.Type == "Transfer")
            {
                accountFrom.Balance = accountFrom.Balance - transaction.Amount;
                SilverCoinsManager.SaveAccount(accountFrom);

                accountTo.Balance = accountTo.Balance + (transaction.Amount * transaction.CurrencyRate);
                SilverCoinsManager.SaveAccount(accountTo);
            }
            else if (transaction.Type == "Income")
            {
                accountFrom.Balance = accountFrom.Balance + transaction.Amount;
                SilverCoinsManager.SaveAccount(accountFrom);
            }
            else
            {
                accountFrom.Balance = accountFrom.Balance - transaction.Amount;
                SilverCoinsManager.SaveAccount(accountFrom);
            }
        }

        public static void UpdateBalanceAfterTransactionDelete(Account accountFrom, Account accountTo, Transaction transaction)
        {
            if (transaction.Type == "Transfer")
            {
                accountFrom.Balance = accountFrom.Balance + transaction.Amount;
                SilverCoinsManager.SaveAccount(accountFrom);

                accountTo.Balance = accountTo.Balance - (transaction.Amount * transaction.CurrencyRate);
                SilverCoinsManager.SaveAccount(accountTo);
            }
            else if (transaction.Type == "Income")
            {
                accountFrom.Balance = accountFrom.Balance - transaction.Amount;
                SilverCoinsManager.SaveAccount(accountFrom);
            }
            else
            {
                accountFrom.Balance = accountFrom.Balance + transaction.Amount;
                SilverCoinsManager.SaveAccount(accountFrom);
            }
        }

        public static void UpdateBalanceAfterTransactionUpdate(Account accountFrom, Account accountTo, Transaction transaction, decimal oldAmount)
        {
            var difference = oldAmount - transaction.Amount;

            if (transaction.Type == "Transfer")
            {
                accountTo.Balance = accountTo.Balance - (difference * transaction.CurrencyRate);
                SilverCoinsManager.SaveAccount(accountTo);
            }

            accountFrom.Balance = accountFrom.Balance + difference;
            SilverCoinsManager.SaveAccount(accountFrom);
        }
    }
}
