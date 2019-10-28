using System;

namespace Harmony
{
    public static class TypeExtensions
    {
        public static bool IsNot(this Object obj, Type type)
        {
            return !(obj.GetType() == type);
        }
    }
}