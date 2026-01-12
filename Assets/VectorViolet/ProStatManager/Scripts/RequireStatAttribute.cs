using System;

namespace VectorViolet.Core.Stats
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequireStatAttribute : Attribute
    {
        public string[] StatNames;

        public RequireStatAttribute(params string[] statNames)
        {
            this.StatNames = statNames;
        }
    }
}