using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Application.Dtos
{

    public sealed class CheckTokenResponse
    {
        public bool Valid { get; set; }
    }
}
