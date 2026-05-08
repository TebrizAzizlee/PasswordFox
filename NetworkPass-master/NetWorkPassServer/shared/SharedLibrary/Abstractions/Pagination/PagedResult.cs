using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Abstractions.Pagination
{
    public sealed record PagedResult<T>(
     IReadOnlyList<T> Items,
     int TotalCount,
     int Page,
     int PageSize
 )
    {
        public int TotalPages =>
       (int)Math.Ceiling(
           TotalCount / (double)PageSize);

        public bool HasNext =>
            Page < TotalPages;

        public bool HasPrevious =>
            Page > 1;
    }
}
