using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Abstractions.Entity
{
    public class EntityDto
    {
        public string Id { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; }
        public string CreatedBy { get; set; } = default!;
        public string CreatedByName { get; set; } = default!;// 🔥 ƏSAS
    }
}
