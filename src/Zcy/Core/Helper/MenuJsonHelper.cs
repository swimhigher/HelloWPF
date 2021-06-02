
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Core.Helper
{
    public static class MenuJsonHelper
    {
        public static List<MenuModel> Menus { get; set; }


        public static void Init()
        {

            var option = new JsonSerializerOptions()
            {
                ReadCommentHandling = JsonCommentHandling.Skip,

            };
            var BasePath = System.AppDomain.CurrentDomain.BaseDirectory;
            var text = File.ReadAllText(BasePath + "/Json/Menu.json", Encoding.UTF8);
            Menus = JsonSerializer.Deserialize<MenuRoot>(text, option).Menus;



        }
    }

    public class MenuRoot
    {
        public List<MenuModel> Menus { get; set; }
    }
    public class MenuModel
    {

        public string Name { get; set; }
        public string Code { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        

        public List<MenuModel> Children { get; set; }
    }
}
