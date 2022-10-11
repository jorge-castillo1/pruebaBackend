using Microsoft.AspNetCore.Mvc;
using System;

namespace customerportalapi.Loggers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomLogAttribute : TypeFilterAttribute
    {
        public CustomLogAttribute() : base(typeof(CustomLogFilter))
        {

        }
    }
}