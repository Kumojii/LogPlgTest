using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using STPLib.General.Base;
using STPLib.WebLog.Api;
using STPLib.WebLog.Enums;
using STPLib.WebLog.Pipelines;

namespace LogPlgTest
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class AppTest : BaseApp, IExternalApplication
    {
        private const string _tabName = "tttt"; // Отдел
        private const string _panelName = "WebLogPlg123"; // Произвольно
        private const string _btnName = "Тест"; // tor_db: Plugins."Name"
        private const string _btnText = "Кнопка Тест"; // tor_db: Plugins."Button"
        public static ApiClient STPWebApi { get; set; } = new ApiClient(new Uri("http://192.168.149.20:5261/"));

        public Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel panel = CreateOrGetPanel(application, _tabName/*Department.STP.ToString()*/, _panelName);

            AddButton(
                buttonType: RibbonItemType.PushButton,
                ribbonPanel: panel,
                image: null,
                buttonName: _btnName,
                buttonText: _btnText,
                urlInstr: Task.Run(async () => await new InitPlg(STPWebApi).Run(_btnName, _btnText)).Result,
                commandPath: typeof(CommandTest).FullName);

            panel.AddSeparator();

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

    }
}
