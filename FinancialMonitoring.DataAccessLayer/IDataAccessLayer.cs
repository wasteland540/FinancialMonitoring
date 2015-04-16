using System;
using System.Collections.Generic;
using FinancialMonitoring.Model;

namespace FinancialMonitoring.DataAccessLayer
{
    public interface IDataAccessLayer
    {
        void Insert<T>(T value) where T : class, IDbObject;

        void Delete<T>(T value) where T : class, IDbObject;

        void Update<T>(T value) where T : class, IDbObject;

        List<T> GetAll<T>() where T : class, IDbObject;

        List<T> GetPaymentRecord<T>(Category category) where T : PaymentRecord;

        List<T> GetPaymentRecord<T>(Months month) where T : PaymentRecord;

        List<T> GetPaymentRecord<T>(DateTime startDate, DateTime endDate) where T : PaymentRecord;

        void Close();
    }
}