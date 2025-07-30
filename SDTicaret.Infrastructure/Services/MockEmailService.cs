using SDTicaret.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace SDTicaret.Infrastructure.Services
{
    public class MockEmailService : IEmailService
    {
        private readonly ILogger<MockEmailService> _logger;
        public MockEmailService(ILogger<MockEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            _logger.LogInformation($"[MOCK EMAIL] To: {to}\nSubject: {subject}\nBody: {body}");
            return Task.CompletedTask;
        }
    }
}