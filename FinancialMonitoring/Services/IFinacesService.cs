using System;
using System.Collections.Generic;
using FinancialMonitoring.Model;

namespace FinancialMonitoring.Services
{
    public interface IFinacesService
    {
        List<PaymentRecord> GetGroupedPaymentRecords(Months month);

        List<PaymentRecord> GetGroupedPaymentRecords(DateTime startDate, DateTime endDate);
    }
}