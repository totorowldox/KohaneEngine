using System;
using System.Collections.Generic;

namespace KohaneEngine.Scripts.Utils
{
    public class FunctionCallResolver
    {
        public delegate void FunctionCall(List<Argument> args);

        private Dictionary<string, Function> functions;
        
        public void Register(string call, List<Argument> args, FunctionCall func)
        {
            if (!functions.TryAdd(call, new Function { Args = args, Func = func }))
            {
                throw new InvalidOperationException($"[FunctionCallResolver] Call {call} has been registered");
            }
        }

        public void Resolve(string call, List<object> args)
        {
            if (!functions.TryGetValue(call, out var func))
            {
                throw new InvalidOperationException($"[FunctionCallResolver] Call {call} has not been registered!");
            }
            
            
        }
    }
    
    public class Argument
    {
        public string Name;
        public Type Type;
    }

    internal class Function
    {
        internal List<Argument> Args = new();
        internal FunctionCallResolver.FunctionCall Func;
    }
}