using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CavemanTools.Logging;
using StartItUp;

namespace System
{
    public class StartIt
    {
        private readonly dynamic _settings;

        /// <summary>
        /// Executes the config class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static void Up<T>(T instance,Action<StartIt> config=null)
        {
           var boot=new StartIt(instance);
            config?.Invoke(boot);
            boot.Build();
        }

        /// <summary>
        /// Instantiates and executes the config class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Up<T>(Action<StartIt> config = null) where T : new()
        {
            var instance = new T();
            Up(instance,config);
            return instance;
        }

        private StartIt(dynamic settings)
        {
            _settings = settings;
            var type = settings.GetType() as Type;
            AppAssemblies.Add(type.Assembly());
           
        }

        void Build()
        {
            var set = _settings as StartupContext;
            if (set!=null) set.AppAssemblies = AppAssemblies.ToArray();
            ConfigureLog();
            RunTasks();
            _logger = null;
            
            set?.Finished();
        }

        /// <summary>
        /// The name of the class configuring the logger
        /// </summary>
        public static string ConfigureLoggerName = "ConfigureLogging";

        /// <summary>
        /// This searches and invokes the logging configuration class.
        /// Override if you want to configure logging directly
        /// </summary>
        private void ConfigureLog()
        {
            var logConfig = AppAssemblies.SelectMany(a =>a.GetPublicTypes(t => t.Name == ConfigureLoggerName)).FirstOrDefault();
            if (logConfig == null) return;
            dynamic inst = System.TypeExtensions.CreateInstance(logConfig);
            var method = IntrospectionExtensions.GetTypeInfo(logConfig).GetMethod("Run");

            var hasResult = method.ReturnType == typeof(Action<string>);
            if (hasResult)
            {
                _logger=inst.Run(_settings);
                return;
            }
            inst.Run(_settings);
        }

        private Action<string> _logger=s=>"Bootstrapper".LogInfo(s);

        /// <summary>
        /// App assemblies, used to search for config tasks. 
        /// The assembly where the bootstrapper is defined is automatically included
        /// </summary>
        public List<Assembly> AppAssemblies { get; } = new List<Assembly>();



        #region Tasks
        void RunTasks()
        {
            var taskTypes = GetTasksList();
           ExecuteTasks(taskTypes);
            
        }

       

        private void ExecuteTasks(List<TaskInfo> list)
        {
            list.OrderBy<TaskInfo, int>(d => d.Order).ForEach(ti =>
            {
                var pos = ti.Order == int.MaxValue ? "no order" : ti.Order.ToString();
                this._logger($"Executing task: '{ti.Name}'({pos})");
                ti.Invoke(_settings);
                this._logger($"Task '{ti.Name}' completed");
            });
        }

       
        private List<TaskInfo> GetTasksList()
        {
            var types = new List<Type>();
           
            types.AddRange(AppAssemblies.SelectMany(a => a.GetExportedTypes()));
            
            var taskTypes = types.Distinct<Type>().Select(TaskInfo.TaskFromTypeConvention)
                .FilterNulls()
                .OrderBy(t => t.Order).ToList();
            
            return taskTypes;
        }
        #endregion
    }
}