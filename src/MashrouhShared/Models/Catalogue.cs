using System;
using System.Collections.Generic;

namespace MashrouhShared.Models
{
    public class Catalogue
    {
        public DateTime ReleaseDate { get; set; }

#nullable enable
        public List<Channel>? Channels { get; set; }
#nullable disable
    }
}
