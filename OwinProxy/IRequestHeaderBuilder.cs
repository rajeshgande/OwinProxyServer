using System.Net;
using Microsoft.Owin;

namespace OwinProxy
{
    public interface IRequestHeaderBuilder
    {
        void Build(IOwinContext context, WebRequest request);
    }
}