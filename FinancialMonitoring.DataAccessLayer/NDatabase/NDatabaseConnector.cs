using System;
using System.Collections.Generic;
using System.Linq;
using FinancialMonitoring.Model;
using NDatabase;
using NDatabase.Api;
using NDatabase.Api.Query;

namespace FinancialMonitoring.DataAccessLayer.NDatabase
{
    public class NDatabaseConnector : IDataAccessLayer
    {
        private const string DatabaseName = "financialMonitoring.db";
        private readonly IOdb _odb;

        public NDatabaseConnector()
        {
            _odb = OdbFactory.Open(DatabaseName);
        }

        public void Insert<T>(T value) where T : class, IDbObject
        {
            _odb.Store(value);
        }

        public void Delete<T>(T value) where T : class, IDbObject
        {
            _odb.Delete(value);
        }

        public void Update<T>(T value) where T : class, IDbObject
        {
            _odb.Store(value);
        }

        public List<T> GetAll<T>() where T : class, IDbObject
        {
            var resultList = new List<T>();

            IQuery query = _odb.Query<T>();
            resultList.AddRange(query.Execute<T>());

            return resultList;
        }

        public List<T> GetPaymentRecord<T>(Category category) where T : PaymentRecord
        {
            var resultList = new List<T>();

            IQuery query = _odb.Query<T>();
            resultList.AddRange(query.Execute<T>().Where(record => record.Category == category));
            
            return resultList;
        }

        public List<T> GetPaymentRecord<T>(Months month) where T : PaymentRecord
        {
            var resultList = new List<T>();
            
            IQuery query = _odb.Query<T>();
            resultList.AddRange(query.Execute<T>().Where(record => record.Date.Month == (int) month));
            
            return resultList;
        }

        public List<T> GetPaymentRecord<T>(DateTime startDate, DateTime endDate) where T : PaymentRecord
        {
            var resultList = new List<T>();
            
            IQuery query = _odb.Query<T>();
            resultList.AddRange(
                query.Execute<T>().Where(record => record.Date >= startDate && record.Date <= endDate));
            
            return resultList;
        }

        public void Close()
        {
            _odb.Close();
        }
    }
}