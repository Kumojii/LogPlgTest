using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LogPlgTest.Api;
using LogPlgTest.Dto;
using LogPlgTest.Exceptions;
using LogPlgTest.Models;


namespace LogPlgTest.Pipelines
{
    public class SendLog
    {
        private readonly ApiClient _api;
        private LogCreateRequest _logRequest { get; set; }

        public SendLog(ApiClient api)
        {
            _api = api;
        }

        public async Task Run(LogCreateRequest log, VerifyResult verifyResult)
        {
            if (log == null || verifyResult == null)
            {
                // TODO: локальное логирование
                return;
            }

            CheckVerifyResult(ref log, verifyResult);

            _logRequest = log;

            try
            {
                (await _api.SendLog(_logRequest)).ThrowIfFailed();
            }
            catch (ServerUnavailableException)
            {
                // TODO локальное логирование
            }
        }

        private bool CheckVerifyResult(ref LogCreateRequest log, VerifyResult verifyResult)
        {
            if (!verifyResult.Result)
            {
                log.Result = "Cancelled";
                log.Message = verifyResult.Message;
            }

            return verifyResult.Result;
        }
    }
}
