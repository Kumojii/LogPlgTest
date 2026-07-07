using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.UI;
using LogPlgTest.Api;
using LogPlgTest.Dto;
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
            // Верификация
            EmpResult = await _api.VerifyEmployee(machineName, userName);

            if (EmpResult == null && !EmpResult.Allowed)
                return ParseRequests(EmpResult, PlgResult);

            // Регистрация
            if (EmpResult != null && EmpResult.NeedRegistr)
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

                // ui регистрации c редактированием empReq

                await _api.Register(empReq);
                EmpResult = await _api.VerifyEmployee(machineName, userName);
            }

            // Обновление данных пользователя
            if (EmpResult != null && EmpResult.NeedUpdate)
            {
                MessageBox.Show(EmpResult.Message);

                // ui обновление данных c редактированием empReq

                EmpResult = await _api.VerifyEmployee(machineName, userName);
            }

            // Проверка наличия плагина 
            PlgResult = await _api.VerifyPlugin(plgName, btnName);
            if (PlgResult != null && !PlgResult.Allowed)
                return ParseRequests(EmpResult, PlgResult);

            // Проверка доступа плагина 
            PlgResult.HasAccess = await _api.HasAccessToThePlugin(plgName, btnName, machineName, userName);
            if (PlgResult.HasAccess == null || !PlgResult.HasAccess)
            {
                PlgResult.Message = "Доступ на плагин запрещён.";
                return ParseRequests(EmpResult, PlgResult);
            }

            // Проверка версии плагина
            var response = await _api.CheckPluginVersion(plgName, btnName, plgVersion);
            PlgResult.IsLatest = response.IsLatest;
            if (PlgResult.IsLatest != null && !PlgResult.IsLatest)
            {
                PlgResult.Message = response.Message;
                return ParseRequests(EmpResult, PlgResult);
            }

            return ParseRequests(EmpResult, PlgResult);
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

    }
}
