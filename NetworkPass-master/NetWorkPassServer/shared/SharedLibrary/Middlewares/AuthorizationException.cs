using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Middlewares;
public class AuthorizationException : Exception
{
    public AuthorizationException(string message = "Unauthorized") : base(message) { }
}
