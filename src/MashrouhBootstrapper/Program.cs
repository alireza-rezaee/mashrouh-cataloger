using MashrouhBootstrapper;
using System.CommandLine;
using MashrouhBootstrapper.Helpers.Enums;

// Create some options:
var minifyOption = new Option<bool>(
    new[] { "--minify", "-m" },
    getDefaultValue: () => true,
    "To avoid minification, set the value to false.");
var bundleOption = new Option<string>(
    new[] { "--bundle", "-b" },
    getDefaultValue: () => "./catalogue.json",
    "Output file path for bundled catalogue.");
var iransedaOption = new Option<string?>(
    new[] { "--iranseda", "-i" },
    getDefaultValue: () => null,
    "An option whose argument is parsed as a string array");

// Add the options to a root command:
var rootCommand = new RootCommand
{
    minifyOption,
    bundleOption,
    iransedaOption
};

rootCommand.Description = "Mashrouh Bootstrapper";

rootCommand.SetHandler((bool minify, string bundle, string iranseda) =>
{
    CatalogueBuilder builder = new();

    if (!string.IsNullOrEmpty(iranseda))
    {
        builder.Iranseda();
        CatalogueBuilder? iransedaBuilder = builder.IransedaCatalogue();
        if (iransedaBuilder != null)
        {
            iransedaBuilder.Catalogue.ReleaseDate = DateTime.UtcNow;
            iransedaBuilder.SaveCatalogue(iranseda, minify);
        }
    }

    if (!string.IsNullOrEmpty(bundle))
    {
        if (builder.Catalogue.Channels == null)
        {
            builder.Catalogue.Channels = new();
            builder.Iranseda();
        }
        if (!builder.Catalogue.Channels.Any(c => c.Type == ChannelType.Iranseda))
            builder.Iranseda();

        builder.Catalogue.ReleaseDate = DateTime.UtcNow;
        builder.SaveCatalogue(bundle, minify);
    }

}, minifyOption, bundleOption, iransedaOption);

// Parse the incoming args and invoke the handler
return rootCommand.Invoke(args);