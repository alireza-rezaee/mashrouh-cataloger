using System;
using MashrouhShared.Helpers.Enums;
using MashrouhShared.Models;
using FileInfo = MashrouhShared.Models.FileInfo;

namespace MashrouhGrabber.Models;

public record File
{
    public string? Path { get; set; }
    public string? Channel { get; set; }
    public string? Description { get; set; }
    public Uri? Url { get; set; }
    public Uri? UrlMirror1 { get; set; }
    public Uri? UrlMirror2 { get; set; }
    public Uri? SessionPage { get; set; }
}