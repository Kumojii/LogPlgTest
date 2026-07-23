using System;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using STPLib.General.Base;
using STPLib.WebLog.Api;
using STPLib.WebLog.Enums;
using STPLib.WebLog.Pipelines;

namespace LogPlgTest
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class App : BaseApp, IExternalApplication
    {
        private const string _tabName = "WebApiTab"; // Отдел
        private const string _panelName = "WebApiPanel"; // Произвольно
        public const string PlgName = "Тест"; // tor_db: Plugins."Name"
        public const string PlgBtnName = "Кнопка Тест"; // tor_db: Plugins."Button"
        public const Department PlgDepartment = Department.STP; // tor_db: Plugins."Department"

        // ПРОАДАКШЕН
        // - таймаут всегда 5 секунд, адрес — из share/fallback
        // -  swagger address http://192.168.149.20:5261/swagger/index.html
        //public static ApiClient STPWebApi { get; set; } = new ApiClient();

        // ТЕСТИРОВАНИЕ
        // - таймаут не ограничен, адрес из перегрузки
        // -  swagger address http://localhost:5261/swagger/index.html
        public static ApiClient STPWebApi = new ApiClient(new Uri("http://localhost:5261/"));

        // ТЕСТИРОВАНИЕ
        // - настройка таймаута, адрес из перегрузки
        // -  swagger address http://localhost:5261/swagger/index.html
        //public static ApiClient STPWebApi = new ApiClient(new Uri("http://localhost:5261/"), TimeSpan.FromSeconds(300));

        public Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel panel = CreateOrGetPanel(application, _tabName/*Department.STP.ToString()*/, _panelName);

            AddButton(
                buttonType: RibbonItemType.PushButton,
                ribbonPanel: panel,
                image: null,
                buttonName: PlgName,
                buttonText: PlgBtnName,
                urlInstr: Task.Run(async () => await new InitPlg(STPWebApi).Run(PlgName, PlgBtnName, PlgDepartment.ToString())).GetAwaiter().GetResult(),
                commandPath: typeof(Command).FullName);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
