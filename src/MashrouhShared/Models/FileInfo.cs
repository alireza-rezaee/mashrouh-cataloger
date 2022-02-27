using System;
using System.Linq;
using System.Collections.Generic;

namespace MashrouhShared.Models
{
    public class FileInfo
    {
#nullable enable
        private Uri? _fileUrl;
        public string? Description { get; set; }
        public Uri? FileUrl
        {
            get => _fileUrl != null ? _fileUrl : FileUrlMirrors?.FirstOrDefault();
            set => _fileUrl = value;
        }
        public List<Uri>? FileUrlMirrors { get; set; }
#nullable disable
    }
}
