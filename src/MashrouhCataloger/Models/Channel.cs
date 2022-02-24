using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MashrouhCataloger.Helpers.Enums;
using MashrouhCataloger.Extensions;

namespace MashrouhCataloger.Models
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

        public List<Session>? Sessions { get; set; }
    }
}