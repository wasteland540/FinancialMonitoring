using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Commands.Commands;
using FinancialMonitoring.DataAccessLayer;
using FinancialMonitoring.Model;
using FinancialMonitoring.Services;

namespace FinancialMonitoring.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IDataAccessLayer _database;
        private readonly IFinacesService _finacesService;
        private ICommand _addCategoryCommand;
        private ICommand _addPaymentRecordCommand;

        private List<Category> _categories;
        private bool? _filterByDaterange = false;
        private bool? _filterByMonth = true;
        private ICommand _filterCommand;
        private DateTime _filterEndDate = DateTime.Now;
        private DateTime _filterStartDate = DateTime.Now;
        private string _newCategory;
        private List<PaymentRecord> _overViewPaymentRecords;
        private PaymentRecord _paymentRecordToAdd;
        private List<PaymentRecord> _paymentRecords;
        private ICommand _removeCategoryCommand;
        private ICommand _removePaymentRecordCommand;
        private Category _selectedCategory;
        private Months _selectedMonth = InitCurrentMonth();
        private PaymentRecord _selectedPaymentRecord;
        private bool _showDaterangeFilter;
        private bool _showMonthFilter = true;

        public MainWindowViewModel(IDataAccessLayer dataAccessLayer, IFinacesService finacesService)
        {
            _database = dataAccessLayer;
            _finacesService = finacesService;
        }

        #region Properties

        public Category SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                if (value != null && value != _selectedCategory)
                {
                    _selectedCategory = value;
                    RaisePropertyChanged("SelectedCategory");

                    _newCategory = _selectedCategory.Name;
                    RaisePropertyChanged("NewCategory");
                }
            }
        }

        public List<Category> Categories
        {
            get
            {
                if (_categories == null)
                {
                    RefreshCategories();
                }

                return _categories;
            }

            set
            {
                if (value != null && value != _categories)
                {
                    _categories = value;
                    RaisePropertyChanged("Categories");
                }
            }
        }

        public string NewCategory
        {
            get { return _newCategory; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _newCategory = value;
                    RaisePropertyChanged("NewCategory");
                }
            }
        }

        public ICommand AddCategoryCommand
        {
            get { return _addCategoryCommand = _addCategoryCommand ?? new DelegateCommand(AddCategory); }
        }

        public ICommand RemoveCategoryCommand
        {
            get { return _removeCategoryCommand = _removeCategoryCommand ?? new DelegateCommand(RemoveCategory); }
        }

        public PaymentRecord PaymentRecordToAdd
        {
            get { return _paymentRecordToAdd ?? (_paymentRecordToAdd = new PaymentRecord()); }
            set
            {
                if (value != null && value != _paymentRecordToAdd)
                {
                    _paymentRecordToAdd = value;
                    RaisePropertyChanged("PaymentRecordToAdd");
                }
            }
        }

        public ICommand AddPaymentRecordCommand
        {
            get { return _addPaymentRecordCommand = _addPaymentRecordCommand ?? new DelegateCommand(AddPaymentRecord); }
        }

        public List<PaymentRecord> PaymentRecords
        {
            get
            {
                if (_paymentRecords == null)
                {
                    RefreshPaymentRecords();
                }

                return _paymentRecords;
            }
            set
            {
                if (value != null && value != _paymentRecords)
                {
                    _paymentRecords = value;
                    RaisePropertyChanged("PaymentRecords");
                }
            }
        }

        public PaymentRecord SelectedPaymentRecord
        {
            get { return _selectedPaymentRecord; }
            set
            {
                if (value != null && value != _selectedPaymentRecord)
                {
                    _selectedPaymentRecord = value;
                    RaisePropertyChanged("SelectedPaymentRecord");
                }
            }
        }

        public ICommand RemovePaymentRecordCommand
        {
            get
            {
                return
                    _removePaymentRecordCommand =
                        _removePaymentRecordCommand ?? new DelegateCommand(RemovePaymentRecord);
            }
        }

        public bool? FilterByMonth
        {
            get { return _filterByMonth; }
            set
            {
                _filterByMonth = value;
                RaisePropertyChanged("FilterByMonth");

                if (value != null)
                {
                    ShowMonthFilter = value.Value;
                    ShowDaterangeFilter = ! value.Value;
                }
            }
        }

        public bool? FilterByDateRange
        {
            get { return _filterByDaterange; }
            set
            {
                _filterByDaterange = value;
                RaisePropertyChanged("FilterByDateRange");

                if (value != null)
                {
                    ShowMonthFilter = ! value.Value;
                    ShowDaterangeFilter = value.Value;
                }
            }
        }

        public bool ShowMonthFilter
        {
            get { return _showMonthFilter; }
            set
            {
                _showMonthFilter = value;
                RaisePropertyChanged("ShowMonthFilter");
            }
        }

        public bool ShowDaterangeFilter
        {
            get { return _showDaterangeFilter; }
            set
            {
                _showDaterangeFilter = value;
                RaisePropertyChanged("ShowDaterangeFilter");
            }
        }

        public IEnumerable<Months> Monthses
        {
            get
            {
                return Enum.GetValues(typeof (Months))
                    .Cast<Months>();
            }
        }

        public Months SelectedMonth
        {
            get { return _selectedMonth; }
            set
            {
                if (value != _selectedMonth)
                {
                    _selectedMonth = value;
                    RaisePropertyChanged("SelectedMonth");
                }
            }
        }

        public ICommand FilterCommand
        {
            get { return _filterCommand = _filterCommand ?? new DelegateCommand(FilterRecords); }
        }

        public List<PaymentRecord> OverViewPaymentRecords
        {
            get
            {
                if (_overViewPaymentRecords == null)
                {
                    RefreshPaymentRecords();
                }

                return _overViewPaymentRecords;
            }
            set
            {
                if (value != null && value != _overViewPaymentRecords)
                {
                    _overViewPaymentRecords = value;
                    RaisePropertyChanged("OverViewPaymentRecords");
                }
            }
        }

        public DateTime FilterStartDate
        {
            get { return _filterStartDate; }
            set
            {
                if (value != _filterStartDate)
                {
                    _filterStartDate = value;
                    RaisePropertyChanged("FilterStartDate");
                }
            }
        }

        public DateTime FilterEndDate
        {
            get { return _filterEndDate; }
            set
            {
                if (value != _filterEndDate)
                {
                    _filterEndDate = value;
                    RaisePropertyChanged("FilterEndDate");
                }
            }
        }

        #endregion Properties

        #region privat methods

        private void RefreshCategories()
        {
            Categories = _database.GetAll<Category>();
        }

        private void AddCategory(object obj)
        {
            if (!string.IsNullOrEmpty(_newCategory))
            {
                var newCategory = new Category(_newCategory);

                _database.Insert(newCategory);

                RefreshCategories();
            }
        }

        private void RemoveCategory(object obj)
        {
            if (_selectedCategory != null)
            {
                if (CategoryNotLinked())
                {
                    _database.Delete(_selectedCategory);

                    NewCategory = string.Empty;
                    SelectedCategory = null;
                    RefreshCategories();
                }
                else
                {
                    MessageBox.Show("You can not delete a category which is linked to a payment record!", "Waring",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void AddPaymentRecord(object obj)
        {
            if (_paymentRecordToAdd.Amount > 0 && !string.IsNullOrEmpty(_paymentRecordToAdd.Subject) &&
                _paymentRecordToAdd.Category != null)
            {
                _database.Insert(_paymentRecordToAdd);

                PaymentRecordToAdd = new PaymentRecord();
                RefreshPaymentRecords();
            }
        }

        private void RefreshPaymentRecords()
        {
            PaymentRecords = _database.GetAll<PaymentRecord>();
            OverViewPaymentRecords = _finacesService.GetGroupedPaymentRecords(_selectedMonth);
        }

        private void RemovePaymentRecord(object obj)
        {
            if (_selectedPaymentRecord != null)
            {
                _database.Delete(_selectedPaymentRecord);

                _selectedPaymentRecord = null;
                RefreshPaymentRecords();
            }
        }

        private bool CategoryNotLinked()
        {
            List<PaymentRecord> resultList = _database.GetPaymentRecord<PaymentRecord>(_selectedCategory);

            return resultList.Count == 0;
        }

        private void FilterRecords(object obj)
        {
            if (_filterByMonth != null && _filterByDaterange != null)
            {
                if (_filterByMonth.Value)
                {
                    if (_selectedMonth != 0)
                    {
                        OverViewPaymentRecords = _finacesService.GetGroupedPaymentRecords(_selectedMonth);
                    }
                }
                else if (_filterByDaterange.Value)
                {
                    OverViewPaymentRecords = _finacesService.GetGroupedPaymentRecords(_filterStartDate, _filterEndDate);
                }
            }
        }

        private static Months InitCurrentMonth()
        {
            int month = DateTime.Now.Month;

            foreach (Months m in Enum.GetValues(typeof (Months)).Cast<Months>())
            {
                if ((int) m == month)
                {
                    return m;
                }
            }

            return Months.January;
        }

        #endregion privat methods
    }
}