using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Middlewares;
public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Unauthorized") : base(message) { }
}
