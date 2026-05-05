using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Middlewares;
public class RateLimitExceededException : Exception
{
    public int RetryAfterSeconds { get; }

    public RateLimitExceededException(int retryAfterSeconds = 5)
        : base("Çox tez sorğu göndərdiniz. Zəhmət olmasa bir müddət gözləyin.")
    {
        RetryAfterSeconds = retryAfterSeconds;
    }
}
