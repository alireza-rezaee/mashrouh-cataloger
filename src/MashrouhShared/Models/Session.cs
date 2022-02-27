using System;
using System.Collections.Generic;
using MD.PersianDateTime.Standard;

namespace MashrouhShared.Models
{
    public class Session
    {
#nullable enable
        public int? No { get; set; }
        public string? Term { get; set; }
        public DateTime? Date { get; set; }
        public string? Description { get; set; }
        public Uri? Url { get; set; }
        public List<FileInfo>? FileInfos { get; set; }
#nullable disable
    }
}
