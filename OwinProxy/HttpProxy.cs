using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace OwinProxy
{
    public class HttpProxy : OwinMiddleware
    {
        private readonly IRequestBodyBuilder _bodyBuilder;
        private readonly IRequestHeaderBuilder _headerBuilder;

        public HttpProxy(OwinMiddleware next, IRequestBodyBuilder bodyBuilder, IRequestHeaderBuilder headerBuilder) : base(next)
        {
            _bodyBuilder = bodyBuilder;
            _headerBuilder = headerBuilder;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Path.Value.ToLower().StartsWith("/proxy"))
            {
                string targeturl = context.Request.Headers.FirstOrDefault(x => x.Key == "x-targeturl").Value.FirstOrDefault();
                WebRequest request = WebRequest.Create(targeturl);
                request.Method = context.Request.Method;
                request.ContentType = context.Request.Headers.FirstOrDefault(x => x.Key == "Content-Type").Value.FirstOrDefault();

                _headerBuilder.Build(context, request);
                await _bodyBuilder.Build(context, request);
                try
                {
                    using (var responsex = (HttpWebResponse)request.GetResponse())
                    {
                        context.Response.StatusCode = (int)responsex.StatusCode;
                        if (responsex.ContentLength != 0)
                        {
                            await responsex.GetResponseStream().CopyToAsync(context.Response.Body);
                        }
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        context.Response.StatusCode = (int)(ex.Response as HttpWebResponse).StatusCode;
                        await ex.Response.GetResponseStream().CopyToAsync(context.Response.Body);
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                await Next.Invoke(context);
            }
        }
        
    }
}
