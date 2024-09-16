using System;
using KohaneEngine.Scripts.Structure;
using UnityEngine;

namespace KohaneEngine.Scripts.Story
{
    public abstract class Resolver
    {
        public static Resolver Instance;

        public abstract ResolveResult Resolve(Block block);
    }
}