using AuthServer.Domain.Users.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthServer.Application.Dtos;

public sealed record LoginResponseDto
{
  
    public bool RequiresTFA { get; init; }
    //[JsonIgnore]
    //public TokenDto? Token { get; init; }
}