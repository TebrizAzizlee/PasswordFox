using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Application.Dtos;
public sealed record BranchDto(
  Guid Id,
  string Name,
  string City,
  string FullAddress,
  string PhoneNumber,
  string Email,
  bool IsActive
);
