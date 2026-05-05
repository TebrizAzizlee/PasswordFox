using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Application.Abstractions;
public sealed class EmailSettings
{
    public string FromEmail { get; set; } = default!;
    public string SmtpServer { get; set; } = default!;
    public int Port { get; set; }
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool UseSsl { get; set; }
}
