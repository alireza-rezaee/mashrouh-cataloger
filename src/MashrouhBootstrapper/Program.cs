using Newtonsoft.Json;
using MashrouhBootstrapper.Channels;
using MashrouhBootstrapper.Models;

Catalogue catalogue = new();
catalogue.Channels = new();

IransedaChannel iransedaChannel = new();
var channel = iransedaChannel.Retrieve();
catalogue.Channels.Add(channel);

catalogue.ReleaseDate = DateTime.UtcNow;

JsonSerializerSettings jsonSetting = new()
{
    NullValueHandling = NullValueHandling.Ignore,
    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    Formatting = Formatting.Indented,
};

JsonSerializerSettings jsonMinifiedSetting = new()
{
    NullValueHandling = NullValueHandling.Ignore,
    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    Formatting = Formatting.None,
};

File.WriteAllText(
    path: Path.Combine(Environment.CurrentDirectory, "catalogue.json"),
    contents: JsonConvert.SerializeObject(catalogue, jsonSetting));
File.WriteAllText(
    path: Path.Combine(Environment.CurrentDirectory, "catalogue.min.json"),
    contents: JsonConvert.SerializeObject(catalogue, jsonMinifiedSetting));