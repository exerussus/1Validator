using System;
using System.Diagnostics;

namespace Exerussus._1Validator
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class ValidationMethodAttribute : Attribute
    {
        public ValidationMethodAttribute() {}
    }
}