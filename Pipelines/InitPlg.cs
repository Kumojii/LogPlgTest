using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogPlgTest.Api;

namespace LogPlgTest.Pipelines
{
    public class InitPlg
    {
        private ApiClient _api;

        private readonly string standartUrl = "https://main.sibtehproekt.com/info";
        public InitPlg(ApiClient api)
        {
            _api = api;
        }

        //public async Task<plgDto> Run(string )

        public async Task<string> RunInit(string pluginName, string buttonName)
        {
            try
            {
                return await _api.GetInstruction(pluginName, buttonName);
            }
            catch (Exception ex)
            {
                return standartUrl;
            }
        }

    }
}
