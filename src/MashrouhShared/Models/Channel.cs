using System;
using System.Collections.Generic;
using MashrouhShared.Helpers.Enums;
using MashrouhShared.Extensions;

namespace MashrouhShared.Models
{
    public class Channel
    {
        public Channel(ChannelType type, Uri url)
            => (Type, Url) = (type, url);

        public ChannelType Type { get; set; }

        public string Title { get => Type.ToString(); }

        public string Description { get => Type.GetDescription(); }

        public Uri Url { get; set; }

        public DateTime ReleaseDate { get; set; }

#nullable enable
        public List<Session>? Sessions { get; set; }
#nullable disable
    }
}
