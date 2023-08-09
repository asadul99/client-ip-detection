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





The Forwarded Headers Middleware (ForwardedHeadersMiddleware) in asp.net core 3.1 reads X-Forwarded-For, X-Forwarded-Proto and X-Forwarded-Host headers and fills in the associated fields on HttpContext.

The middleware updates:

HttpContext.Connection.RemoteIpAddress: Set using the X-Forwarded-For header value.
HttpContext.Request.Scheme: Set using the X-Forwarded-Proto header value.
HttpContext.Request.Host: Set using the X-Forwarded-Host header value.