using System;
using System.Reflection;

namespace StartItUp
{
    /// <summary>
    /// Default implementation of <see cref="IInvokeTask"/>
    /// </summary>
    public class ExecuteRunMethod : IInvokeTask
    {
        /// <summary>
        /// Creates a function that will be invoked by <see cref="TaskInfo"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Action<dynamic> CreateAction(Type type)
        {
            dynamic inst = type.CreateInstance();
            var meth = type.GetTypeInfo().GetMethod("Run");
            if (meth==null) throw new InvalidOperationException("There is no Run public method");
            var args = meth.GetParameters();
            if (args.Length>1 || (args.Length==1 && !args[0].ParameterType.DerivesFrom<StartupContext>())) throw new InvalidOperationException("Only methods without arguments or having one argument deriving from `StartupContext`");
            return (d) =>
            {
                if (args.Length == 0) inst.Run();
                else
                {
                    inst.Run(d);
                }
            };

        }
    }
}