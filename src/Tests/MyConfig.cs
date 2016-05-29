using System.Reflection;
using System.Text;
using System;
using StartItUp;

namespace Tests
{
    public class MyConfig :StartupContext
    {
        private StringBuilder _sb;
       
        
        public bool Settings { get; set; }
        public MyConfig()
        {
            _sb=new StringBuilder();           
        }

        public StringBuilder Result
        {
            get { return _sb; }
        }
        
       
    }
}