using System;
using KohaneEngine.Scripts.Structure;

namespace KohaneEngine.Scripts.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Get an argument
        /// </summary>
        public static T GetArg<T>(this Block block, int index)
        {
            if (typeof(T) == typeof(float))
            {
                object value = block.GetFloatArg(index);
                return (T) value;
            }
            
            return (T)block?.args[index] ?? default(T);
        }

        private static float GetFloatArg(this Block block, int index)
        {
            return Convert.ToSingle(block?.args[index]);
        }

        /// <summary>
        /// Return the same path, but without file extension
        /// </summary>
        public static string WithoutExtension(this string s)
        {
            var dotIndex = s.LastIndexOf(".", StringComparison.Ordinal);
            return s.Substring(0, dotIndex == -1 ? s.Length : dotIndex);
        }
    }
}