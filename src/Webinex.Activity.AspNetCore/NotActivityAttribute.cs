using System;

namespace Webinex.Activity.AspNetCore
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NotActivityAttribute : Attribute
    {
    }
}