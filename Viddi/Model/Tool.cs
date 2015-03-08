using System.Collections.Generic;

namespace Viddi.Model
{
    public class Tool
    {
        public Tool(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public string Name { get; set; }
        public string Url { get; set; }

        public static List<Tool> GetTools()
        {
            var result = new List<Tool>
            {
                new Tool("Cimbalino", "http://cimbalino.org"),
                new Tool("MVVM Light", "http://www.mvvmlight.net/"),
                new Tool("Coding4Fun", "http://coding4fun.codeplex.com"),
                new Tool("Fody", "https://github.com/Fody"),
                new Tool("PropertyChanged.Fody", "https://github.com/Fody/PropertyChanged"),
                new Tool("Player Framework", "http://playerframework.codeplex.com"),
                new Tool("Json.net", "http://www.newtonsoft.com/json"),
                new Tool("ThemeManagerRt", "https://github.com/scottisafool/thememanagerrt"),
                new Tool("VidMePortable", "https://github.com/scottisafool/vidmeportable"),
                new Tool("WinRTXamlToolkit", "http://WinRTXamlToolkit.codeplex.com")
            };

            return result;
        } 
    }
}
