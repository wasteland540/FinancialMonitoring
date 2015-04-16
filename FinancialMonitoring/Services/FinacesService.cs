using System;
using System.Collections.Generic;
using System.Linq;
using FinancialMonitoring.DataAccessLayer;
using FinancialMonitoring.Model;

namespace FinancialMonitoring.Services
{
    public class FinacesService : IFinacesService
    {
        private readonly IDataAccessLayer _database;

        public FinacesService(IDataAccessLayer database)
        {
            _database = database;
        }

        public List<PaymentRecord> GetGroupedPaymentRecords(Months month)
        {
            var groupedRecords = new List<PaymentRecord>();

            List<PaymentRecord> records = _database.GetPaymentRecord<PaymentRecord>(month);
            GroupRecords(records, groupedRecords);

            return groupedRecords;
        }
        
        public List<PaymentRecord> GetGroupedPaymentRecords(DateTime startDate, DateTime endDate)
        {
            var groupedRecords = new List<PaymentRecord>();

            List<PaymentRecord> records = _database.GetPaymentRecord<PaymentRecord>(startDate, endDate);
            GroupRecords(records, groupedRecords);

            return groupedRecords;
        }

        private static void GroupRecords(IEnumerable<PaymentRecord> records, List<PaymentRecord> groupedRecords)
        {
            foreach (PaymentRecord record in records)
            {
                PaymentRecord groupedRecord = groupedRecords.FirstOrDefault(gr => gr.Category == record.Category);

                if (groupedRecord == null)
                {
                    var newGroupedRecord = new PaymentRecord(record.Date, record.Amount, string.Empty, record.Category);

                    groupedRecords.Add(newGroupedRecord);
                }
                else
                {
                    groupedRecord.Amount += record.Amount;
                }
            }
        }
    }
}