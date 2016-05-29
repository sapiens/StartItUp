using System;

namespace StartItUp
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInvokeTask
    {
        /// <summary>
        /// Creates a function that will be invoked by <see cref="TaskInfo"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Action<dynamic> CreateAction(Type type);
    }
}