using System;
using SQLite;
using SilverCoins.BusinessLayer.Contracts;

namespace SilverCoins.BusinessLayer.Models
{
	public class Category : IBusinessEntity
    {
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public DateTime CreatedDate { get; set; }

		public int Icon { get ; set; }

        public CategoryTypes Type { get; set; }

        public int Visible { get; set; }

        public Category ()
		{
		}

        public enum CategoryTypes
        {
            MoneyTransfer,
            Income,
            Expense
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

