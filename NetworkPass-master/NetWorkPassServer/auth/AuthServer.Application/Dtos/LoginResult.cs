using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Application.Dtos
{
    public sealed record LoginResult
    {
        public bool RequiresTFA { get; init; }
        
        public TokenDto? Token { get; init; }
    }
}
    

