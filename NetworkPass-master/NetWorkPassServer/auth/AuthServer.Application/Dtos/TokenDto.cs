using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthServer.Application.Dtos;
public sealed record TokenDto(
   string? AccessToken ,
   string? RefreshToken ,
  DateTimeOffset? AccessTokenExpiration,
   DateTimeOffset? RefreshTokenExpiration 
   



);
