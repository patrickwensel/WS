using System.IO;
using Autofac;
using NLog;
using Data = WS.Framework.ServicesInterfaceImplementation;
using WS.Framework.ServicesInterface;
using System.Configuration;

namespace WS.ConsoleApp.MWOImageCleanup
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static IContainer Container { get; set; }

        static void Main(string[] args)
        {
            RegisterContainer();
            Run();
        }

        public static void Run()
        {
            string stagingFolder = ConfigurationManager.AppSettings["FeithImageStagingFileLocation"];

            string[] dirs = Directory.GetDirectories(stagingFolder);
            using (ILifetimeScope scope = Container.BeginLifetimeScope())
            {
                var workOrderService = scope.Resolve<IWorkOrderService>();

                foreach (string dir in dirs)
                {
                    workOrderService.ProcessImagesWithoutCheck(dir);
                }
            }
        }

        private static void RegisterContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Data.SecurityService>().As<ISecurityService>();
            builder.RegisterType<Data.UnitService>().As<IUnitService>();
            builder.RegisterType<Data.BusinessUnitService>().As<IBusinessUnitService>();
            builder.RegisterType<Data.ItemService>().As<IItemService>();
            builder.RegisterType<Data.HelperService>().As<IHelperService>();
            builder.RegisterType<Data.JDEService>().As<IJDEService>();
            builder.RegisterType<Data.WorkOrderService>().As<IWorkOrderService>();
            builder.RegisterType<Data.UserDefinedCodeService>().As<IUserDefinedCodeService>();
            builder.RegisterType<Data.EquipmentService>().As<IEquipmentService>();
            builder.RegisterType<Data.EmployeeService>().As<IEmployeeService>();
            builder.RegisterType<Data.OMBWorkOrderService>().As<IOMBWorkOrderService>();
            builder.RegisterType<Data.EmailService>().As<IEmailService>();
            builder.RegisterType<Data.OracleHelperService>().As<IOracleHelperService>();
            Container = builder.Build();
        }
    }
}
