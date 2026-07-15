using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using STPLib.WebLog.Api;
using STPLib.WebLog.Pipelines;
using static System.Net.Mime.MediaTypeNames;

namespace LogPlgTest
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class App : IExternalApplication
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
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            RibbonPanel panel = GetRibbonPanel(application, tabName, panelName);

            PushButtonData testBtn = new PushButtonData(testBtnName, testBtnText, assemblyPath, typeof(Program).FullName);

            // пайплайн инициализации
            var initPlg = new InitPlg(STPWebApi);
            var testBtnInstructionUrl = Task.Run(async () =>
            {
                return await initPlg.Run(testBtnName, testBtnText);
            });


            testBtn.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, testBtnInstructionUrl.Result));

            panel.AddItem(testBtn);
            return Result.Succeeded;
        }


        public RibbonPanel GetRibbonPanel(UIControlledApplication a, string tab, string namePanel)
        {
            try
            {
                a.CreateRibbonTab(tab);
            }
            catch { }

            try
            {
                return a.CreateRibbonPanel(tab, namePanel);
            }
            catch
            {
                foreach (RibbonPanel RPanel in a.GetRibbonPanels(tab))
                {
                    if (RPanel.Name == namePanel)
                    {
                        return RPanel;
                    }
                }
            }

            return null;
        }
    }
}
