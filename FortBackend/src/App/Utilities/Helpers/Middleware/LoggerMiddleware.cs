using System.Diagnostics;

namespace FortBackend.src.App.Utilities.Helpers.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var currentTime = DateTime.Now.TimeOfDay;
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            
            _logger.LogInformation($"[{currentTime:hh\\:mm\\:ss}-{elapsedMilliseconds}ms] Request: {context.Request.Method} {context.Request.PathBase + context.Request.Path}");
        }
    }
}
