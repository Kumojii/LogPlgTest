using System;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using STPLib.CustomExceptions;
using STPLib.General.Base;
using STPLib.General;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using LogPlgTest;
using STPLib.WebLog.Enums;
using STPLib.Utils.LogTimers;
using STPLib.WebLog.Pipelines;
using System.Threading.Tasks;
using STPLib.WebLog.Models;

namespace LogPlgTest
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class CommandTest : BaseWebCommand, IExternalCommand
    {
        private const string _plgName = "Тест"; //tor_db: Plugins."Name"
        private const string _plgBtn = "Кнопка Тест"; //tor_db: Plugins."Button"
        private readonly string _machineName = Environment.MachineName;
        private VerifyResult _verifyRes;
        public CommandTest()
            : base(
                  pluginName: _plgName,
                  pluginButton: _plgBtn,
                  
#if DEBUG
                isLogEnabled: false)
#else
                isLogEnabled: true)
#endif
        { }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                PlgTimers.RefreshTimers();
                HardLoads.LoadAssemblyResolveConfig();

                InitializeFields(commandData);
                InitializeLogger();

                _verifyRes = Task.Run(async () => await new StartPlg(App.STPWebApi)
                    .Run(_plgName, _plgBtn, _machineName, _userName, _pluginVersion))
                    .GetAwaiter()
                    .GetResult();

                if(!_verifyRes.Result)
                    throw new PluginCanceledException($"{_verifyRes.Message}");

                PlgTimers.StartTimer(Timer.Work);

                #region Логика плагина
                #endregion

                Logger.Log.Elements = 0;
                HandleSuccess();
                return Result.Succeeded;
            }
            catch (PluginCanceledException pce)
            {
                HandleCancel(pce);
                return Result.Cancelled;
            }
            catch (PluginErrorException pee)
            {
                HandleError(pee);
                return Result.Failed;
            }
            catch (Exception ex)
            {
                HandleCrash(ex);
                return Result.Failed;
            }
            finally
            {
                FinalizeLogger();
                Task.Run(() => new SendLog(App.STPWebApi).Run(Logger.Log, _verifyRes)).GetAwaiter();
            }
        }
    }
}
