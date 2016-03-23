using Owin;
using OwinProxy;

namespace OwinProxyServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use<HttpProxy>(new RequestBodyBuilder(), new RequestHeaderBuilder());
            app.Run(context =>
            {
                context.Response.ContentType = "text/plain";
                return context.Response.WriteAsync("Proxy bypassed!!!");
            });
        }
    }
}