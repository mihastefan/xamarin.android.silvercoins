using SQLite;
using System;
using SilverCoins.BusinessLayer.Contracts;

namespace SilverCoins.BusinessLayer.Models
{
	public class Account : IBusinessEntity
    {
		[PrimaryKey, AutoIncrement]
		public int Id { get ; set; }

		public string Name { get; set; }

		public string Description { get; set;}

		public DateTime CreatedDate { get; set; }

		public string Currency { get; set; }

		public decimal Balance { get; set; }

		public int Icon { get; set; }

		public Account ()
		{
		}

        public override string ToString()
        {
            return Name;
        }
    }
}

