namespace IQGROUP_test_task.Middleware
{
    public class AuthorizeHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizeHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Сохраните заголовок Authorization в HttpContext.Items
            if (context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                context.Items["Authorization"] = authorizationHeader.ToString();
            }

            await _next(context);
        }
    }
}
