using System;
using SQLite;
using SilverCoins.BusinessLayer.Models;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SilverCoins.BusinessLayer.Contracts;

namespace SilverCoins.DataLayer
{
    /// <summary>
    /// SilverCoinsDatabase builds on SQLite.Net and represents a specific database, in our case, the SilverCoins DB.
    /// It contains methods for retrieval and persistance as well as db creation, all based on the 
    /// underlying ORM.
    /// </summary>
    public class SilverCoinsDatabase : SQLiteConnection
    {
        static object locker = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="SilverCoinsy.DataLayer.SilverCoinsDatabase"/> SilverCoinsDatabase. 
        /// if the database doesn't exist, it will create the database and all the tables.
        /// </summary>
        /// <param name='path'>
        /// Path.
        /// </param>
        public SilverCoinsDatabase(string path) : base(path)
        {
            // create the tables
            CreateTable<Account>();
            CreateTable<Category>();
            CreateTable<Transaction>();
            CreateTable<Configuration>();
        }

        public IEnumerable<T> GetItems<T>() where T : IBusinessEntity, new()
        {
            lock (locker)
            {
                return Table<T>().ToList();
            }
        }

        public T GetItem<T>(int Id) where T : IBusinessEntity, new()
        {
            lock (locker)
            {
                return Table<T>().FirstOrDefault(x => x.Id == Id);
            }
        }

        public T GetItemByName<T>(string name) where T : IBusinessEntity, new()
        {
            lock(locker)
            {
                return Table<T>().FirstOrDefault(x => x.Name == name);
            }
        }

        public int SaveItem<T>(T item) where T : IBusinessEntity
        {
            lock (locker)
            {
                if (item.Id != 0)
                {
                    Update(item);
                    return item.Id;
                }
                else {
                    return Insert(item);
                }
            }
        }

        public int DeleteItem<T>(int Id) where T : IBusinessEntity, new()
        {
            lock (locker)
            {
                return Delete(new T() { Id = Id });
            }
        }

        public Configuration GetConfigurationRecordByKey(string key)
        {
            lock(locker)
            {
                return Table<Configuration>().FirstOrDefault(x => x.Key == key);
            }
        }
        
    }
}

