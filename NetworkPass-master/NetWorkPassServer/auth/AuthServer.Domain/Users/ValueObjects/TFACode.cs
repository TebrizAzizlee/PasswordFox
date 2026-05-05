using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Domain.Users.ValueObjects;
public sealed record TFACode(string Value);
