using System.Threading.Tasks;
using HappyTravel.StdOutLogger.Models;
using Microsoft.AspNetCore.Http;

namespace HappyTravel.StdOutLogger
{
    public interface IHttpContextLogger
    {
        Task AddHttpRequest(HttpRequest httpRequest);
        void AddHttpResponse(HttpResponse httpResponse);
        HttpContextLogEntry GetHttpContextLogModel();
    }
}