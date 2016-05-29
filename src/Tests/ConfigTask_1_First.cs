using System;

namespace Tests
{
    public class ConfigTask_2_First
    {
        public void Run(MyConfig cfg)
        {
            TestConfigs.Tasks.Add(GetType());
        }
    }

    public class ConfigTask_1_Second
    {
        public void Run()
        {
            TestConfigs.Tasks.Add(GetType());
        }
    }

    public class ConfigureLogging
    {
        public Action<string> Run(MyConfig c)
        {
            c.Result.Append("log");
            TestConfigs.Tasks.Add(GetType());
            return s =>c.Settings=true;
        }
    }
}