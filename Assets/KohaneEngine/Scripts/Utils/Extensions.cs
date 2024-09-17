using System.Collections.Generic;
using KohaneEngine.Scripts.Structure;

namespace KohaneEngine.Scripts.Utils
{
    public static class Extensions
    {
        public static T GetArg<T>(this Block block, int index)
        {
            return (T)block?.args[index] ?? default(T);
        }
    }
}