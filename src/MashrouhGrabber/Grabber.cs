using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CsvHelper;
using MashrouhShared.Models;
using File = MashrouhGrabber.Models.File;
using FileInfo = MashrouhShared.Models.FileInfo;

namespace MashrouhGrabber;

public class Grabber
{
    private Catalogue _catalogue;
    private readonly string _catalogueUrl;
    private readonly HttpClient _httpClient;

    public Grabber()
    {
        _catalogue = new();
        _httpClient = new();
        _catalogueUrl = "https://raw.githubusercontent.com/" +
                        "alireza-rezaee/mashrouh-cataloger/main/catalog.bundle.latest.min.json";
    }

    public Grabber(string catalogueUrl)
    {
        _catalogue = new();
        _httpClient = new();
        _catalogueUrl = catalogueUrl;
    }

    public async Task<bool> ReadCatalogue()
    {
        Catalogue? catalog;
        try
        {
            catalog = await _httpClient.GetFromJsonAsync<Catalogue>(requestUri: _catalogueUrl);
        }
        catch (Exception)
        {
            return false;
        }

        if (catalog == null)
            return false;

        _catalogue = catalog;

        return true;
    }

    public async Task Grab(string outputDirectory)
    {
        if (string.IsNullOrEmpty(outputDirectory))
            throw new ArgumentException("Directory argument is required!");

        if (!Directory.Exists(outputDirectory))
            throw new DirectoryNotFoundException("The entered directory does not exist on the disk!");

        string jsonString = JsonSerializer.Serialize(_catalogue);
        await System.IO.File.WriteAllTextAsync(
            path: Path.Combine(outputDirectory, "catalog.json"),
            contents: jsonString,
            encoding: Encoding.UTF8);

        if (_catalogue.Channels == null || !_catalogue.Channels.Any())
            return;

        foreach (var channel in _catalogue.Channels)
        {
            if (string.IsNullOrEmpty(channel.Title))
                throw new InvalidOperationException("Channel Title not specified.");

            if (channel.Sessions == null || !channel.Sessions.Any())
                continue;

            foreach (var session in channel.Sessions)
                if (session.Url == null)
                    throw new Exception("One or more Session URLs are Null.");
        }

        List<File> files = new();

        foreach (var channel in _catalogue.Channels)
        {
            Directory.CreateDirectory(Path.Combine(outputDirectory, channel.Title));

            if (channel.Sessions == null || !channel.Sessions.Any())
                continue;

            foreach (var session in channel.Sessions)
                if (session.Url == null)
                    throw new Exception("One or more Session URLs are Null.");

            foreach (var session in channel.Sessions)
            {
                if (session.FileInfos == null || !session.FileInfos.Any())
                    continue;

                foreach (var fileInfo in session.FileInfos)
                {
                    if (fileInfo.FileUrl == null
                        || fileInfo.FileUrlMirrors == null
                        || !fileInfo.FileUrlMirrors.Any())
                        continue;

                    List<Uri> mirrorUris = fileInfo.FileUrlMirrors.SkipWhile(uri => uri == fileInfo.FileUrl).ToList();

                    var file = new File()
                    {
                        Path = Path.Combine(outputDirectory, channel.Title, $"{Guid.NewGuid()}.mp4"),
                        Channel = channel.Type.ToString(),
                        Description = fileInfo.Description,
                        Url = fileInfo.FileUrl,
                        SessionPage = session.Url,
                        UrlMirror1 = mirrorUris.Any() ? mirrorUris.ElementAt(0) : null,
                        UrlMirror2 = mirrorUris.Count >= 2 ? mirrorUris.ElementAt(1) : null
                    };

                    await DownloadFile(
                        fileInfo: fileInfo,
                        path: file.Path);

                    files.Add(file);
                }
            }
        }

        await using var writer = new StreamWriter(Path.Combine(outputDirectory, "./data.csv"));
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(files);
    }

    private async Task DownloadFile(FileInfo fileInfo, string path)
    {
        if (fileInfo?.FileUrl == null)
            throw new ArgumentNullException($"The {nameof(fileInfo)} must not be null!");

        List<Uri> fileMirrorUrls =
            fileInfo.FileUrlMirrors != null ? new List<Uri>(fileInfo.FileUrlMirrors) : new List<Uri>();
        fileMirrorUrls.Add(fileInfo.FileUrl);

        foreach (var fileUri in fileMirrorUrls)
        {
            try
            {
                byte[] fileBytes = await _httpClient.GetByteArrayAsync(fileUri);
                await System.IO.File.WriteAllBytesAsync(path, fileBytes);
            }
            catch
            {
                //
            }
        }
    }
}