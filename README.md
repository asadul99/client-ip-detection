In old asp.net, we used to use following code to get the client IP address:

```c#
var clientIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
```

In asp.net core 3.1, we can use following code to get the client IP address:

```c#
private string GetClientIP(IHttpContextAccessor contextAccessor)
{
    return  contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
}
```

However, in the situation there is a reverse proxy or load balancer before the server, using above code would only get the IP address of the reverse proxy or load balancer.

To solve this issue, people invented the HTTP header X-Forwarded-For. The X-Forwarded-For (XFF) header is a de-facto standard header for identifying the originating IP address of a client connecting to a web server through an HTTP proxy or a load balancer

![architecture](https://github.com/asadul99/client-ip-detection/blob/master/img/client-ip-detection.png?raw=true)

The Forwarded Headers Middleware (ForwardedHeadersMiddleware) in asp.net core 3.1 reads X-Forwarded-For, X-Forwarded-Proto and X-Forwarded-Host headers and fills in the associated fields on HttpContext.

The middleware updates:

HttpContext.Connection.RemoteIpAddress: Set using the X-Forwarded-For header value.
HttpContext.Request.Scheme: Set using the X-Forwarded-Proto header value.
HttpContext.Request.Host: Set using the X-Forwarded-Host header value.


- ForwardLimit: null. There is no limit on how many entries the middleware would process in the X-Forwarded-For header.
- KnownProxies: [ 10.130.10.77 ]. Assume this is the reverse proxy/load balancer IP address before your web application server.
- Client connects to your web application via your load balancer. There are no other proxies involved except yours (10.130.10.77). Client IP address is 192.168.98.99.


When client sends the request to your web application, it first reaches to the load balancer. Load balancer would create the X-Forwarded-For header and add the value of 192.168.98.99. Load balancer then forwards the request to web server. When web server receives the request, before the execution of the Forwarded Headers Middleware, the HttpContext.Connection.RemoteIpAddress field would be 10.130.10.77 and the X-Forwarded-For header would be 192.168.98.99. Since 10.130.10.77 is in the list of KnownProxies while 192.168.98.99 not, after execution of the Forwarded Headers Middleware, the HttpContext.Connection.RemoteIpAddress field would be 192.168.98.99.

Configure the service to bypass the proxy IP

```c#
//read the known proxy
var knownProxies = builder.Configuration.GetSection("KnownProxies").Value;
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = null;
    options.KnownProxies.Clear();
    foreach (var ip in knownProxies.Split(new char[] { ',' }))
    {
        options.KnownProxies.Add(IPAddress.Parse(ip));
    }
});
```