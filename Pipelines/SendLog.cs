using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LogPlgTest.Api;
using LogPlgTest.Dto;
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

        // REVIEW: В запросе на этот метод есть GetResult(), но async void сразу же успешно завершится на await
        public async void Run(LogCreateRequest log, VerifyResult verifyResult)
        {
            if (log != null && verifyResult != null)
            {
                CheckVerifyResult(ref log, verifyResult);

                try
                {
                    _logRequest = log;
                    await _api.SendLog(_logRequest);
                }
                catch (Exception ex)
                {
                    // локальное логирование
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                // локальное логирование
            }
        }

        private bool CheckVerifyResult(ref LogCreateRequest log, VerifyResult verifyResult)
        {
            if (!verifyResult.Result)
            {
                log.Result = "Cancelled";
                log.Message = verifyResult.Message;
                MessageBox.Show(verifyResult.Message);
            }

            return verifyResult.Result;
        }
    }
}
