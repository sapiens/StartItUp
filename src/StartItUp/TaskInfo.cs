using System;
using CavemanTools;

namespace StartItUp
{
    /// <summary>
    /// Information about a config task class
    /// </summary>
    public class TaskInfo
    {
        /// <summary>
        /// You can change this to detect if a type is a config task
        /// </summary>
        public static Func<Type, TaskInfo> TaskFromTypeConvention = t =>
        {
            var name = t.Name;
            if (!name.StartsWith(Prefix)) return null;
            var parts = name.Split('_');
            var info = new TaskInfo(t);
            var pos = 1;
            int order;
            if (int.TryParse(parts[1], out order))
            {
                pos++;
                info.Order = order;
            }

            
            pos++;

            if (parts.Length == pos)
            {
                info.Name = parts[pos-1];
            }
            return info;
        };

       
        /// <summary>
        /// Config class name prefix
        /// </summary>
        public static string Prefix = "ConfigTask_";
        private Type _type;

        /// <summary>
        /// Replace with your own invoker. By default, it executes the 'Run' method
        /// </summary>
        public static IInvokeTask Invoker=new ExecuteRunMethod();


        public TaskInfo(Type type,int order=int.MaxValue)
        {
            Order = order;
            Type = type;
        }

        /// <summary>
        /// Config task invoker
        /// </summary>
        public Action<dynamic> Invoke { get; set; } = Empty.ActionOf<dynamic>();

        /// <summary>
        /// 
        /// </summary>
        public Type Type
        {
            get { return _type; }
            private set
            {
                _type = value;
                Name = value.Name;
                Invoke = Invoker.CreateAction(value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Order { get; set; }
      
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }      
    }
}