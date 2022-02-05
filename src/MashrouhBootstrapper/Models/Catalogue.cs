using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MashrouhBootstrapper.Channels;

namespace MashrouhBootstrapper.Models
{
    public class Catalogue
    {
        public DateTime ReleaseDate { get; set; }

        public List<Channel>? Channels { get; set; }
    }
}