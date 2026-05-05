using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Security
{
    
        public static class TokenHashHelper
        {
            public static string Hash(string token)
            {
                var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
                return Convert.ToBase64String(bytes);
            }
        }
    }

