using SilverCoins.BusinessLayer.Contracts;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SilverCoins.BusinessLayer.Models
{
    public class Configuration : IBusinessEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        public string StringValue { get; set; }

        public int IntValue { get; set; }

        public bool BooleanValue { get; set; }
        
        public double DoubleValue { get; set; }

        public Configuration() { }

    }
}
