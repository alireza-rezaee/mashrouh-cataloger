using System;
using System.Collections.Generic;
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

        List<File> files = new();

        foreach (var channel in _catalogue.Channels)
        {
            if (channel.Sessions == null || !channel.Sessions.Any())
                continue;

            foreach (var session in channel.Sessions)
            {
                if (session.FileInfos == null || !session.FileInfos.Any())
                    continue;

                var sessionId = Guid.NewGuid();

                Directory.CreateDirectory(Path.Combine(outputDirectory, sessionId.ToString()));

                foreach (var fileInfo in session.FileInfos)
                {
                    if (fileInfo.FileUrl == null
                        || fileInfo.FileUrlMirrors == null
                        || !fileInfo.FileUrlMirrors.Any())
                        continue;

                    var file = new File()
                    {
                        Id = Guid.NewGuid(),
                        Channel = channel.Type,
                        Description = fileInfo.Description,
                        SessionMirrorUrl = fileInfo.FileUrlMirrors,
                        SessionId = sessionId,
                        SessionUrl = session.Url
                    };

                    await DownloadFile(
                        fileInfo: fileInfo,
                        path: Path.Combine(outputDirectory, sessionId.ToString(), $"{file.Id.ToString()}.mp4"));

                    files.Add(file);
                }
            }
        }

        await using var writer = new StreamWriter("dataset.csv");
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