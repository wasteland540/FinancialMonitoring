using System;
using System.Collections.Generic;
using FinancialMonitoring.DataAccessLayer;
using FinancialMonitoring.DataAccessLayer.NDatabase;
using FinancialMonitoring.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccessLayerUnitTest
{
    /// <summary>
    ///     Summary description for NDatabaseConnectorUnitTest
    /// </summary>
    [TestClass]
    public class NDatabaseConnectorUnitTest
    {
        private static IDataAccessLayer _dataAccess;

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            _dataAccess = new NDatabaseConnector();
        }

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            _dataAccess.Close();
        }

        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        [TestCleanup]
        public void MyTestCleanup()
        {
            List<Category> categories = _dataAccess.GetAll<Category>();

            foreach (Category category in categories)
            {
                _dataAccess.Delete(category);
            }

            List<PaymentRecord> records = _dataAccess.GetAll<PaymentRecord>();

            foreach (PaymentRecord record in records)
            {
                _dataAccess.Delete(record);
            }
        }

        #endregion

        [TestMethod]
        public void Insert()
        {
            List<Category> categories = _dataAccess.GetAll<Category>();
            Assert.IsTrue(categories.Count == 0);

            List<PaymentRecord> records = _dataAccess.GetAll<PaymentRecord>();
            Assert.IsTrue(records.Count == 0);

            var category = new Category("Essen");
            _dataAccess.Insert(category);

            categories = _dataAccess.GetAll<Category>();
            Assert.IsTrue(categories.Count == 1);

            var paymentRecord = new PaymentRecord(DateTime.Now, 12.33, "Blubb", category);
            _dataAccess.Insert(paymentRecord);

            records = _dataAccess.GetAll<PaymentRecord>();
            Assert.IsTrue(records.Count == 1);
        }

        [TestMethod]
        public void Update()
        {
            List<Category> categories = _dataAccess.GetAll<Category>();
            Assert.IsTrue(categories.Count == 0);

            var category = new Category("Essen");
            _dataAccess.Insert(category);

            categories = _dataAccess.GetAll<Category>();
            Assert.IsTrue(categories.Count == 1);

            Category cat = categories[0];
            cat.Name = "trinken";
            _dataAccess.Update(cat);

            categories = _dataAccess.GetAll<Category>();
            Assert.IsTrue(categories.Count == 1);
        }

        [TestMethod]
        public void GetPaymentRecordByCategory()
        {
            List<Category> categories = _dataAccess.GetAll<Category>();
            Assert.IsTrue(categories.Count == 0);

            List<PaymentRecord> records = _dataAccess.GetAll<PaymentRecord>();
            Assert.IsTrue(records.Count == 0);

            var category1 = new Category("Essen");
            _dataAccess.Insert(category1);
            var paymentRecord1 = new PaymentRecord(DateTime.Now, 12.33, "Blubb", category1);

            var category2 = new Category("Versicherungen");
            _dataAccess.Insert(category2);
            var paymentRecord2 = new PaymentRecord(DateTime.Now, 1.33, "Blubb2", category2);

            categories = _dataAccess.GetAll<Category>();
            Assert.IsTrue(categories.Count == 2);


            _dataAccess.Insert(paymentRecord1);
            _dataAccess.Insert(paymentRecord2);

            categories = _dataAccess.GetAll<Category>();
            Assert.IsTrue(categories.Count == 2);

            records = _dataAccess.GetAll<PaymentRecord>();
            Assert.IsTrue(records.Count == 2);

            List<PaymentRecord> foodRecords = _dataAccess.GetPaymentRecord<PaymentRecord>(category1);
            Assert.IsTrue(foodRecords.Count == 1);

            List<PaymentRecord> insuranceRecords = _dataAccess.GetPaymentRecord<PaymentRecord>(category2);
            Assert.IsTrue(insuranceRecords.Count == 1);
        }

        [TestMethod]
        public void GetPaymentRecordByMonth()
        {
            List<Category> categories = _dataAccess.GetAll<Category>();
            Assert.IsTrue(categories.Count == 0);

            List<PaymentRecord> records = _dataAccess.GetAll<PaymentRecord>();
            Assert.IsTrue(records.Count == 0);

            var category1 = new Category("Essen");
            _dataAccess.Insert(category1);
            var paymentRecord1 = new PaymentRecord(new DateTime(2015, 1, 14), 12.33, "Blubb", category1);

            _dataAccess.Insert(category1);
            var paymentRecord2 = new PaymentRecord(new DateTime(2015, 10, 16), 1.33, "Blubb2", category1);

            categories = _dataAccess.GetAll<Category>();
            Assert.IsTrue(categories.Count == 1);


            _dataAccess.Insert(paymentRecord1);
            _dataAccess.Insert(paymentRecord2);

            categories = _dataAccess.GetAll<Category>();
            Assert.IsTrue(categories.Count == 1);

            records = _dataAccess.GetAll<PaymentRecord>();
            Assert.IsTrue(records.Count == 2);

            List<PaymentRecord> januaryRecords = _dataAccess.GetPaymentRecord<PaymentRecord>(Months.January);
            Assert.IsTrue(januaryRecords.Count == 1);

            List<PaymentRecord> octoberRecords = _dataAccess.GetPaymentRecord<PaymentRecord>(Months.October);
            Assert.IsTrue(octoberRecords.Count == 1);
        }

        [TestMethod]
        public void GetPaymentRecordByTimespan()
        {
            List<Category> categories = _dataAccess.GetAll<Category>();
            Assert.IsTrue(categories.Count == 0);

            List<PaymentRecord> records = _dataAccess.GetAll<PaymentRecord>();
            Assert.IsTrue(records.Count == 0);

            var category1 = new Category("Essen");
            _dataAccess.Insert(category1);

            categories = _dataAccess.GetAll<Category>();
            Assert.IsTrue(categories.Count == 1);


            var paymentRecord1 = new PaymentRecord(new DateTime(2015, 1, 1), 12.33, "Blubb", category1);
            var paymentRecord2 = new PaymentRecord(new DateTime(2015, 1, 14), 13.33, "Blubb2", category1);
            var paymentRecord3 = new PaymentRecord(new DateTime(2015, 1, 19), 14.33, "Blubb3", category1);
            var paymentRecord4 = new PaymentRecord(new DateTime(2015, 1, 31), 15.33, "Blubb4", category1);
            var paymentRecord5 = new PaymentRecord(new DateTime(2015, 2, 1), 16.33, "Blubb5", category1);

            _dataAccess.Insert(paymentRecord1);
            _dataAccess.Insert(paymentRecord2);
            _dataAccess.Insert(paymentRecord3);
            _dataAccess.Insert(paymentRecord4);
            _dataAccess.Insert(paymentRecord5);

            categories = _dataAccess.GetAll<Category>();
            Assert.IsTrue(categories.Count == 1);

            records = _dataAccess.GetAll<PaymentRecord>();
            Assert.IsTrue(records.Count == 5);

            List<PaymentRecord> januaryRecords = _dataAccess.GetPaymentRecord<PaymentRecord>(new DateTime(2015, 1, 1),
                new DateTime(2015, 1, 31));

            Assert.IsTrue(januaryRecords.Count == 4);
        }
    }
}