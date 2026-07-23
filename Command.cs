using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using STPLib.CustomExceptions;
using STPLib.General.Base;
using STPLib.General;
using STPLib.Utils.LogTimers;
using STPLib.WebLog.Pipelines;
using System.Threading.Tasks;
using STPLib.WebLog.Models;
using System.Windows;
using STPLib.WebLog.Enums;
using STPLib.Utils.Messanger;

namespace LogPlgTest
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Command : BaseCommand, IExternalCommand
    {
        private const string _plgName = App.PlgName; //tor_db: Plugins."Name"
        private const string _plgBtn = App.PlgBtnName; //tor_db: Plugins."Button"
        private const Department _plgDepartment = App.PlgDepartment;
        private readonly string _machineName = Environment.MachineName;
        private VerifyResult _verifyRes;
        public Command()
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
                    .Run(_plgName, _plgBtn, _plgDepartment.ToString(), _machineName, _userName, _pluginVersion, UiApp))
                    .GetAwaiter()
                    .GetResult();

                if (!_verifyRes.Result)
                    throw new PluginCanceledException($"{_verifyRes.Message}");

                PlgTimers.StartTimer(Timer.Work);
                
                /// Логика плагина

                Logger.Log.Elements = 55555;
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
