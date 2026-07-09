using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.UI;
using LogPlgTest.Api;
using LogPlgTest.Dto;
using LogPlgTest.Exceptions;
using LogPlgTest.Models;


namespace LogPlgTest.Pipelines
{
    public class StartPlg
    {
        private readonly ApiClient _api;

        public EmployeeResult EmpResult = new EmployeeResult();
        public PluginResult PlgResult = new PluginResult();

        public StartPlg(ApiClient api)
        {
            _api = api;
        }

        public async Task<VerifyResult> Run(string plgName, string btnName, string machineName, string userName, string plgVersion)
        {
            try
            {
                // Верификация пользователя
                EmpResult = (await _api.VerifyEmployee(machineName, userName))
                    .GetDataOrThrow();

                if (!EmpResult.Allowed)
                    return ParseRequests(EmpResult, PlgResult);

                // Регистрация пользователя
                if (EmpResult.NeedRegistr)
                {
                    MessageBox.Show(EmpResult.Message);
                    EmployeeRequest empReq = new EmployeeRequest()
                    {
                        Name = "Имя",
                        Surname = "Фамилия",
                        Lastname = "Отчество",
                        Department = null,

                        CompName = Environment.MachineName,
                        WindowName = Environment.UserName,
                    };

                    // TODO: ui регистрации c редактированием empReq

                    (await _api.Register(empReq)).ThrowIfFailed();
                    EmpResult = (await _api.VerifyEmployee(machineName, userName))
                        .GetDataOrThrow();
                }

                // Обновление данных пользователя
                if (EmpResult.NeedUpdate)
                {
                    EmployeeRequest empReq = new EmployeeRequest()
                    {
                        Name = "Имя",
                        Surname = "Фамилия",
                        Lastname = "Отчество",
                        Department = null,

                        CompName = Environment.MachineName,
                        WindowName = Environment.UserName,
                    };

                    // TODO: ui обновление данных c редактированием empReq


                    (await _api.Update(empReq)).ThrowIfFailed();
                    EmpResult = (await _api.VerifyEmployee(machineName, userName))
                        .GetDataOrThrow();
                }

                // Проверка существования плагина
                PlgResult = (await _api.VerifyPlugin(plgName, btnName))
                    .GetDataOrThrow();

                if (!PlgResult.Allowed)
                    return ParseRequests(EmpResult, PlgResult);

                // Проверка доступа
                PlgResult.HasAccess = (await _api.HasAccessToThePlugin(plgName, btnName, machineName, userName))
                    .GetDataOrThrow();

                if (!PlgResult.HasAccess)
                {
                    PlgResult.Message = "Доступ на плагин запрещён.";
                    return ParseRequests(EmpResult, PlgResult);
                }

                // Проверка версии
                var plgVerRes = (await _api.CheckPluginVersion(plgName, btnName, plgVersion))
                    .GetDataOrThrow();

                PlgResult.IsLatest = plgVerRes.IsLatest;

                if (!PlgResult.IsLatest)
                {
                    PlgResult.Message = plgVerRes.Message;
                    return ParseRequests(EmpResult, PlgResult);
                }

                return ParseRequests(EmpResult, PlgResult);
            }
            catch (ServerUnavailableException)
            {
                return AllowOffline();
            }

        }

        private VerifyResult ParseRequests(EmployeeResult empRes, PluginResult plgRes)
        {
            string message = string.Empty;
            if (empRes != null && !String.IsNullOrEmpty(empRes.Message))
                message = empRes.Message;
            else if (plgRes != null && !String.IsNullOrEmpty(plgRes.Message))
                message = plgRes.Message;

            return new VerifyResult()
            {
                EmpAllowed = empRes.Allowed,
                EmpNeedRegistr = empRes.NeedRegistr,
                EmpNeedUpdate = empRes.NeedUpdate,
                PlgAllowed = plgRes.Allowed,
                HasAccess = plgRes.HasAccess,

                Result = empRes.Allowed && plgRes.Allowed,
                Message = message
            };
        }

        private VerifyResult AllowOffline()
        {
            return new VerifyResult
            {
                EmpAllowed = true,
                EmpNeedRegistr = false,
                EmpNeedUpdate = false,

                PlgAllowed = true,
                HasAccess = true,

                ServerUnavailable = true,
                AllowRun = true,

                Result = true,
                Message = "Сервер недоступен. Плагин запущен в автономном режиме."
            };
        }
    }
}
