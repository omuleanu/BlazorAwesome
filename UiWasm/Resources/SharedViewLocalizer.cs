using System.Reflection;
using Microsoft.Extensions.Localization;

namespace UiWasm.Resources
{
    public class SharedViewLocalizer
    {
        private readonly IStringLocalizer localizer;

        public SharedViewLocalizer(IStringLocalizerFactory factory)
        {
            var type = typeof(AweMui);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            localizer = factory.Create(nameof(AweMui), assemblyName.Name);
        }

        public LocalizedString this[string key] => localizer[key];

        public LocalizedString GetLocalizedString(string key)
        {
            return localizer[key];
        }
    }
}