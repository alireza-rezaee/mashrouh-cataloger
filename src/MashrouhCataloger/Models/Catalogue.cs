using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MashrouhCataloger.Channels;

namespace MashrouhCataloger.Models
{
    public class Catalogue
    {
        public DateTime ReleaseDate { get; set; }

        public List<Channel>? Channels { get; set; }
    }
}