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

        public async Task<string> Run(string pluginName, string buttonName)
        {
            var result = await _api.GetInstruction(pluginName, buttonName);

            if (!result.Success || String.IsNullOrWhiteSpace(result.Data))
                return standartUrl;

            return result.Data;
        }
    }
}
