#if FULLNET
using System.Configuration;

namespace StartItUp
{
    /// <summary>
    /// For reading settings from app/web.config
    /// </summary>
    public class ConfigReader
    {
        /// <summary>
        /// Tries to read AppSettings then ConnectionStrings
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static string Get(string keyName)
        {
            var set = ConfigurationManager.AppSettings[keyName];
            if (set != null) return set;
            var cnx= ConfigurationManager.ConnectionStrings[keyName];
            return cnx?.ConnectionString;
        }
    }
}

#endif