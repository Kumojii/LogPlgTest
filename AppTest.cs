using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LogPlgTest.Api;
using LogPlgTest.Pipelines;
using STPLib.General.Base;
using STPLib.WebLog.Enums;

namespace LogPlgTest
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class AppTest : BaseApp, IExternalApplication
    {
        public static ApiClient STPWebApi { get; set; } = new ApiClient(new Uri("http://192.168.149.20:5261/"));

        private readonly string tabName = "tttt"; // Отдел
        private readonly string panelName = "WebLogPlg123"; // Произвольно
        private readonly string testBtnName = "Тест"; // tor_db: Plugins."Name"
        private readonly string testBtnText = "Кнопка Тест"; // tor_db: Plugins."Button"

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel panel = CreateOrGetPanel(application, tabName/*Department.STP.ToString()*/, panelName);

            var initPlg = new InitPlg(STPWebApi);
            var instrRequest = Task.Run(async () =>
            {
                return await initPlg.Run(testBtnName, testBtnText);
            });



            AddButton(
                buttonType: RibbonItemType.PushButton,
                ribbonPanel: panel,
                image: null,
                buttonName: testBtnName,
                buttonText: testBtnText,
                urlInstr: instrRequest.Result,
                commandPath: typeof(CommandTest).FullName);

            panel.AddSeparator();

            return Result.Succeeded;
        }
    }
}
