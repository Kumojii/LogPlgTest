using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using LogPlgTest.Api;
using LogPlgTest.Dto;
using LogPlgTest.Enums;
using LogPlgTest.Models;
using LogPlgTest.Models.LibTimer;
using LogPlgTest.Pipelines;

//review2
namespace LogPlgTest
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    //REVIEW: Program (одна m)
    public class Programm : IExternalCommand
    {
        public Document Doc { get; set; }

        private readonly string _plgName = "Тест"; //tor_db: Plugins."Name"
        private readonly string _plgBtn = "Кнопка Тест"; //tor_db: Plugins."Button"
        private readonly string _machineName = Environment.MachineName;
        private string _userName;
        private string _plgVersion;
        private VerifyResult _verifyRes;

        private LogCreateRequest _log;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                PlgTimers.RefreshTimers();
                PlgTimers.StartTimer(Timer.Work);
                string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                _plgVersion = string.Join(".", assemblyVersion.Split('.').Take(3));

                Doc = commandData.Application.ActiveUIDocument.Document;

                string filePath = string.Empty;
                if (Doc.IsWorkshared)
                {
                    ModelPath worksharingCentralModelPath = Doc.GetWorksharingCentralModelPath();
                    filePath = ModelPathUtils.ConvertModelPathToUserVisiblePath(worksharingCentralModelPath);
                }
                else
                    filePath = Doc.PathName;

                _userName = Doc.Application.Username;
                _log = new LogCreateRequest
                {
                    PluginName = _plgName,
                    ButtonName = _plgBtn,
                    Version = _plgVersion,

                    UserName = _userName,
                    MachineName = _machineName,

                    FileName = Doc.Title,
                    //REVIEW: Заглушка ? Просто выше есть filePath
                    FilePath = "ExempleFilePath",
                    Date = DateTime.Now,
                };

                // Пайплайн верификации 
                var startPlg = new StartPlg(App.STPWebApi);
                _verifyRes = Task.Run(async () =>
                {
                    return await startPlg.Run(_plgName, _plgBtn, _machineName, _userName, _plgVersion);
                }).GetAwaiter().GetResult();


                PlgTimers.StartTimer(Timer.Notification);
                PlgTimers.StartTimer(Timer.Interface);
                PlgTimers.StartTimer(Timer.Work);

                ///
                // Логика плагина 
                ///

                _log.Result = LogResult.Succeeded.ToString();
                PlgTimers.StopTimers();

                return Result.Succeeded;
            }
            catch (TaskCanceledException tce)
            {
                PlgTimers.StartTimer(Timer.Notification);
                MessageBox.Show(tce.Message);
                PlgTimers.StopTimers();

                _log.Result = LogResult.Cancelled.ToString();
                if (tce.Message != "Отмена")
                    _log.Message = tce.Message;

                return Result.Cancelled;
            }
            catch (Exception exception)
            {
                PlgTimers.StartTimer(Timer.Notification);
                MessageBox.Show(exception.Message);
                PlgTimers.StopTimers();

                _log.Message = "Критическая ошибка.";
                _log.FullMessage = exception.Message;
                _log.ErrorPath = exception.TargetSite.Name;
                _log.ErrorDetails = exception.StackTrace;
                _log.Result = LogResult.Failed.ToString();

                return Result.Failed;
            }
            finally
            {
                _log.WorkTime = PlgTimers.WorkTime;
                _log.InterfaceTime = PlgTimers.InterfaceTime;
                _log.InformationTime = PlgTimers.NotifyTime;

                // пайплайн логирования 
                var sendLog = new SendLog(App.STPWebApi);

                // REVIEW: В SendLog описал, что метод async void Run не дождётся реального результат отправки
                Task.Run(async () => sendLog.Run(_log, _verifyRes)).GetAwaiter().GetResult();
            }
        }
    }
}
