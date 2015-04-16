using System.Windows;
using FinancialMonitoring.DataAccessLayer;
using FinancialMonitoring.DataAccessLayer.NDatabase;
using FinancialMonitoring.Properties;
using FinancialMonitoring.Services;
using FinancialMonitoring.Views;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity;

namespace FinancialMonitoring
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public IUnityContainer Container = new UnityContainer();

        protected override void OnStartup(StartupEventArgs e)
        {
            //service registrations
            Container.RegisterType<IFinacesService, FinacesService>();
            var nDatabaseConnector = new NDatabaseConnector();
            Container.RegisterInstance(typeof (IDataAccessLayer), nDatabaseConnector);

            //registraions utils
            //only one instance from messenger can exists! (recipient problems..)
            var messenger = new Messenger();
            Container.RegisterInstance(typeof (IMessenger), messenger);

            var mainWindow = Container.Resolve<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Default.Save();

            //object databases have always a open connection while the application is running!
            var dataAccessLayer = Container.Resolve<IDataAccessLayer>();
            dataAccessLayer.Close();

            base.OnExit(e);
        }
    }
}