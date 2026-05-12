using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public abstract class ServiceResultBase
    {
        public ProblemDetails? Error { get; init; }
        [JsonIgnore]
        public bool IsSuccess => Error is null;

        protected static ProblemDetails CreateProblem(string code, string detail, HttpStatusCode status)
        {
            var problem = new ProblemDetails
            {
                Title = code,
                Detail = detail,
                Status = (int)status
            };

            problem.Extensions["code"] = code;
            return problem;
        }
    }
}
