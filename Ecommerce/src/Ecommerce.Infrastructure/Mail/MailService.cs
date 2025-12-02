using Ecommerce.Application.Abstractions;
using Ecommerce.Infrastructure.DependencyInjection.Options;
using Microsoft.Extensions.Configuration;
using Resend;

namespace Ecommerce.Infrastructure.Mail;
public class MailService : IMailService
{
    private readonly IResend _resend;

    public MailService(IResend resend)
    {
        _resend = resend;
    }

    public async Task Execute()
    {
        var resp = await _resend.EmailSendAsync(new EmailMessage()
        {
            From = "onboarding@resend.dev",
            To = "nhatspam1999@gmail.com",
            Subject = "Hello World",
            HtmlBody = "<p>Nhat Nguyen New Congrats on sending your <strong>first email</strong>!</p>",
        });
        Console.WriteLine(resp.Content);
        Console.WriteLine(resp.Exception);
    }

}
