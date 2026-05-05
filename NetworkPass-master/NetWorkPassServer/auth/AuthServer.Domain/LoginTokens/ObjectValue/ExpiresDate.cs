using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Domain.LoginTokens.ObjectValue;
public sealed record ExpiresDate(DateTimeOffset Value);
