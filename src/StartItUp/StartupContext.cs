using System;
using System.Collections.Generic;
using System.Reflection;

namespace StartItUp
{
    public abstract class StartupContext
    {

        public Assembly[] AppAssemblies { get; internal set; } = Array.Empty<Assembly>();

        public Assembly ConfigAssembly => GetType().Assembly();

        protected StartupContext()
        {
            
        }
        
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, dynamic> ContextData { get; } = new Dictionary<string, dynamic>();

        

        internal void Finished()
        {
            OnFinished?.Invoke(this);
            OnFinished = null;
        }

        /// <summary>
        /// Should be used by various helpers to cleanup etc
        /// </summary>
        public event Action<StartupContext> OnFinished;

    }
}