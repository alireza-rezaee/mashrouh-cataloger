using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MD.PersianDateTime.Standard;

namespace MashrouhCataloger.Models
{
    public class Session
    {
        public int? No { get; set; }
        public string? Term { get; set; }
        public PersianDateTime? Date { get; set; }
        public string? Description { get; set; }
        public Uri? Url { get; set; }
        public List<FileInfo>? FileInfos { get; set; }
    }
}