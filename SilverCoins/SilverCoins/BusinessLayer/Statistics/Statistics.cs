using Com.Syncfusion.Charts;
using SilverCoins.BusinessLayer.Managers;
using SilverCoins.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SilverCoins.Statistics
{
    public static class Statistics
    {
        internal static ObservableArrayList PieChartData(Account account)
        {
            List<Transaction> transactions = new List<Transaction>();

            if (account.Id != 0)
            {
                transactions = SilverCoinsManager.GetTransactions().Where(x => x.Type != "Transfer" && x.Account == account.Id).ToList();
            }
            else
            {
                transactions = SilverCoinsManager.GetTransactions().Where(x => x.Type != "Transfer").ToList();
            }

            List<ResultLine> resultList = transactions.GroupBy(x => x.Category)
                                                      .Select(r => new ResultLine
                                                      {
                                                          Name = SilverCoinsManager.GetCategory(r.First().Category).Name,
                                                          Value = r.Sum(s => s.Amount)
                                                      })
                                                      .ToList();

            ObservableArrayList list = new ObservableArrayList();
            foreach (var item in resultList)
            {
                list.Add(new ChartDataPoint(item.Name, Convert.ToDouble(item.Value)));
            }

            return list;
        }

        internal static List<ObservableArrayList> BarChartData(Account account)
        {
            var transactions = SilverCoinsManager.GetTransactions()
                                                 .Where(x => x.Type != "Transfer" && x.CreatedDate >= DateTime.Today.AddYears(-1))
                                                 .OrderBy(x => x.CreatedDate)
                                                 .ToList();

            if (account.Id != 0)
            {
                transactions = transactions.Where(x => x.Account == account.Id).ToList();
            }

            List<ResultLine> resultListIncome = GetDataForTransactionsGroupByMonth(transactions.Where(x => x.Type == "Income").ToList());
            List<ResultLine> resultListExpense = GetDataForTransactionsGroupByMonth(transactions.Where(x => x.Type == "Expense").ToList());

            ObservableArrayList listIncome = new ObservableArrayList();
            ObservableArrayList listExpense = new ObservableArrayList();

            foreach (var item in resultListIncome)
            {
                listIncome.Add(new ChartDataPoint(item.Name, Convert.ToDouble(item.Value)));
            }

            foreach (var item in resultListExpense)
            {
                listExpense.Add(new ChartDataPoint(item.Name, Convert.ToDouble(item.Value)));
            }

            List<ObservableArrayList> dataModel = new List<ObservableArrayList>();
            dataModel.Add(listIncome);
            dataModel.Add(listExpense);

            return dataModel;
        }

        internal static List<ObservableArrayList> ColumnChartData(Account account)
        {
            List<ObservableArrayList> dataModel = new List<ObservableArrayList>();

            if (account.Id == 0)
            {
                var accounts = SilverCoinsManager.GetAccounts().ToList();

                foreach (var item in accounts)
                {
                    dataModel.Add(GetBalanceGroupByMonthPerAccount(item));
                }
            }
            else
            {
                dataModel.Add(GetBalanceGroupByMonthPerAccount(account));
            }

            return dataModel;
        }

        private static ObservableArrayList GetBalanceGroupByMonthPerAccount(Account account)
        {
            var transactions = SilverCoinsManager.GetTransactions()
                                                 .Where(x => x.Type != "Transfer" &&
                                                             x.CreatedDate >= DateTime.Today.AddYears(-1) &&
                                                             x.Account == account.Id)
                                                 .OrderBy(x => x.CreatedDate)
                                                 .ToList();

            List<ResultLine> resultList = transactions.GroupBy(x => x.CreatedDate.Month)
                                                      .Select(r => new ResultLine
                                                      {
                                                          Name = r.First().CreatedDate.ToString("MMM"),
                                                          Value = r.Sum(x => (x.Type == "Income" ? x.Amount : -x.Amount))
                                                      })
                                                      .ToList();

            ObservableArrayList list = new ObservableArrayList();
            decimal balance = 0;

            foreach (var item in resultList)
            {
                balance += item.Value;
                list.Add(new ChartDataPoint(item.Name, Convert.ToDouble(balance)));
            }

            return list;
        }

        private static List<ResultLine> GetDataForTransactionsGroupByMonth(List<Transaction> transactions)
        {
            List<ResultLine> resultList = transactions.GroupBy(x => x.CreatedDate.Month)
                                                      .Select(r => new ResultLine
                                                      {
                                                          Name = r.First().CreatedDate.ToString("MMM"),
                                                          Value = r.Sum(s => s.Amount)
                                                      })
                                                      .ToList();
            return resultList;
        }
    }

    internal class ResultLine
    {
        public ResultLine() { }

        public string Name { get; set; }

        public decimal Value { get; set; }
    }
}


