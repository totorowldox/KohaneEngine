using System;
using System.Collections.Generic;
using KohaneEngine.Scripts.Structure;

namespace KohaneEngine.Scripts.Story
{
    public abstract class Resolver
    {
        protected readonly Dictionary<string, Func<Block, ResolveResult>> Functions = new();

        public ResolveResult Resolve(Block block)
        {
            return Functions.TryGetValue(block.type, out var function)
                ? function.Invoke(block)
                : ResolveResult.FailResult($"[GenericResolver] Unknown block type {block.type}");
        }
    }
}