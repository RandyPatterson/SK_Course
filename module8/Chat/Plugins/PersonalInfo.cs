using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.VisualBasic;

namespace Chat.Plugins
{
    public class PersonalInfo
    {
        [KernelFunction("get_my_information")]
        [Description("call this when my information is needed including name, address, location or birthdate")]
        [return: Description("returns my name, address, location and birthdate formated as JSON")]
        public Info GetInfo() => new Info();
    }


    public class Info
    {
        public string Name { get => "Udemy Class"; }
        public DateTime Birthdate { get => new DateTime(1990,1,1); }
        public string Address { get => "Dallas, TX"; }


    }
}
