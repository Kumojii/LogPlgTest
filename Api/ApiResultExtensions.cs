using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogPlgTest.Exceptions;

namespace LogPlgTest.Api
{
    public static class ApiResultExtensions
    {
        public static T GetDataOrThrow<T>(this ApiResult<T> result)
        {
            if (!result.Success)
                throw new ServerUnavailableException(result.Error);

            if (result.Data == null)
                throw new ServerUnavailableException("Сервер вернул пустой ответ.");

            return result.Data;
        }

        public static void ThrowIfFailed(this ApiResult result)
        {
            if (!result.Success)
                throw new ServerUnavailableException(result.Error);
        }

        public static void ThrowIfFailed<T>(this ApiResult<T> result)
        {
            if (!result.Success)
                throw new ServerUnavailableException(result.Error);
        }
    }
}
