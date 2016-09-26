using System;
using SQLite;
using SilverCoins.BusinessLayer.Contracts;

namespace SilverCoins.BusinessLayer.Models
{
	public class Transaction : IBusinessEntity
    {
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string Name { get; set; }

		public decimal Amount { get; set; }

		public DateTime CreatedDate { get; set; }

		public string Type { get; set; }

        public int Account { get; set; }

		public int AccountTransfer { get; set; }

		public int Category { get; set; }

        public string PaymentType { get; set; }

        public decimal CurrencyRate { get; set; }

		public Transaction ()
		{
		}

	}
}

