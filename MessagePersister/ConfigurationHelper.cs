using System.Configuration;

namespace MessagePersisterComponent
{
    public static class ConfigurationHelper
    {
        public static string RootFolder => ConfigurationManager.AppSettings["rootFolder"];
        public static string MessageFolder => RootFolder + ConfigurationManager.AppSettings["persistFolder"];

        public static string MessageExtension => ConfigurationManager.AppSettings["fileExtension"];


    }
}
