using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Abstractions.Pagination
{
    public sealed record PagedResult<T>
    {
        public PagedResult(
            IReadOnlyList<T> items,
            int totalCount,
            int page,
            int pageSize)
        {
            if (page <= 0)
            {
                throw new ArgumentException(
                    "Page 0-dan böyük olmalıdır");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException(
                    "PageSize 0-dan böyük olmalıdır");
            }

            if (totalCount < 0)
            {
                throw new ArgumentException(
                    "TotalCount mənfi ola bilməz");
            }

            Items = items;

            TotalCount = totalCount;

            Page = page;

            PageSize = pageSize;
        }

        public IReadOnlyList<T> Items { get; }

        public int TotalCount { get; }

        public int Page { get; }

        public int PageSize { get; }

        public int TotalPages =>
            (int)Math.Ceiling(
                TotalCount /
                (double)PageSize);

        public bool HasNext =>
            Page < TotalPages;

        public bool HasPrevious =>
            Page > 1;
    }
}
