using System;

namespace SilverCoins.BusinessLayer.Contracts
{
	public interface IBusinessEntity
    {
		int Id { get; set; }
        string Name { get; set; }
	}
}

