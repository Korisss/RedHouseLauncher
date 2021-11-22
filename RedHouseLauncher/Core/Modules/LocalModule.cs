using Newtonsoft.Json;

namespace RedHouseLauncher.Core.Modules
{
    internal class LocalModule
    {
        internal LocalModule()
        {
            Name = null;
            Version = -1;
        }
        internal LocalModule(string name, short version)
        {
            Name = name;
            Version = version;
        }

        [JsonProperty("name")] internal string? Name { get; private set; }
        [JsonProperty("version")] internal short Version { get; private set; }
    }
}
