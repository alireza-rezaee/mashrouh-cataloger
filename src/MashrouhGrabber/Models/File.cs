using System;
using MashrouhShared.Helpers.Enums;
using MashrouhShared.Models;
using FileInfo = MashrouhShared.Models.FileInfo;

namespace MashrouhGrabber.Models;

public record File
{
    public Guid Id { get; set; }
    public ChannelType Channel { get; set; }
    public string? Description { get; set; }
    public Guid SessionId { get; set; }
    public Uri? SessionUrl { get; set; }
    public List<Uri>? SessionMirrorUrl { get; set; }
}