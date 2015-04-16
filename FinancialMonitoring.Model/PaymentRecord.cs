using System;

namespace FinancialMonitoring.Model
{
    public class PaymentRecord : IDbObject
    {
        public PaymentRecord()
        {
            Date = DateTime.Now;
        }

        public PaymentRecord(DateTime date, double amount, string subject, Category category)
        {
            Date = date;
            Amount = amount;
            Subject = subject;
            Category = category;
        }

        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string Subject { get; set; }
        public Category Category { get; set; }
    }
}