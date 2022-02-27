using System;
using MashrouhCataloger;
using System.CommandLine;
using static System.Console;


// Options:
var bundleOption = new Option<string?>(
    new[] { "--bundle", "-b" },
    "Output file path for bundle catalogue");
var bundleMinOption = new Option<string?>(
    new[] { "--bundle-min", "-c" },
    "Output file path for minified bundle catalogue");

var iransedaOption = new Option<string?>(
    new[] { "--iranseda", "-i" },
    "Output file path for iranseda catalogue");
var iransedaMinOption = new Option<string?>(
    new[] { "--iranseda-min", "-j" },
    "Output file path for minified iranseda catalogue");


// Add the options to a root command:
var rootCommand = new RootCommand
{
    bundleOption,
    bundleMinOption,
    iransedaOption,
    iransedaMinOption
};

rootCommand.Description = "Mashrouh Cataloger";

rootCommand.SetHandler((
    string bundle, string bundleMin,
    string iranseda, string iransedaMin) =>
{
    CatalogueBuilder builder = new();

    if (!string.IsNullOrEmpty(iranseda) || !string.IsNullOrEmpty(iransedaMin))
    {
        builder.Iranseda();
        CatalogueBuilder? iransedaBuilder = builder.IransedaCatalogue();
        if (iransedaBuilder != null)
        {
            iransedaBuilder.Catalogue.ReleaseDate = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(iranseda))
                iransedaBuilder.SaveCatalogue(path: iranseda, minify: false);

            if (!string.IsNullOrEmpty(iransedaMin))
                iransedaBuilder.SaveCatalogue(path: iransedaMin, minify: true);
        }
    }

    if (builder.Catalogue.Channels == null)
        WriteLine("No output was generated, because you did not select any channels! You may have to use the --help.");

    if (string.IsNullOrEmpty(bundle) && string.IsNullOrEmpty(bundleMin))
        return;
    
    builder.Catalogue.ReleaseDate = DateTime.UtcNow;

    if (!string.IsNullOrEmpty(bundle))
        builder.SaveCatalogue(path: bundle, minify: false);

    if (!string.IsNullOrEmpty(bundleMin))
        builder.SaveCatalogue(path: bundleMin, minify: true);

}, bundleOption, bundleMinOption,
    iransedaOption, iransedaMinOption);

return rootCommand.Invoke(args);
