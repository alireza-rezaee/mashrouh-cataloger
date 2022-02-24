using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MashrouhCataloger.Models
{
    public class FileInfo
    {
        private Uri? _fileUrl;

        public string? Description { get; set; }
        public Uri? FileUrl
        {
            get => _fileUrl != null ? _fileUrl : FileUrlMirrors?.FirstOrDefault();
            set => _fileUrl = value;
        }
        public List<Uri>? FileUrlMirrors { get; set; }
    }
}