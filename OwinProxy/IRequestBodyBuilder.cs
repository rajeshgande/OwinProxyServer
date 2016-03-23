using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace OwinProxy
{
    public interface IRequestBodyBuilder
    {
        Task Build(IOwinContext context, WebRequest request);
    }
}