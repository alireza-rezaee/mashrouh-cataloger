using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MashrouhCataloger.Channels;
using Newtonsoft.Json;
using MashrouhShared.Models;
using MashrouhShared.Helpers.Enums;

namespace MashrouhCataloger
{
    public class CatalogueBuilder
    {
        private readonly Catalogue _catalogue;

        public Catalogue Catalogue
        {
            get => _catalogue;
        }

        public CatalogueBuilder()
        {
            _catalogue = new Catalogue();
        }

        public Channel Iranseda()
        {
            if (_catalogue.Channels == null)
                _catalogue.Channels = new();

            IransedaChannel iransedaChannel = new();
            Channel channel = iransedaChannel.Retrieve();
            _catalogue.Channels.Add(channel);

            return channel;
        }

        public CatalogueBuilder? IransedaCatalogue()
        {
            Channel? iranseda = this._catalogue.Channels?.FirstOrDefault(c => c.Type == ChannelType.Iranseda);
            if (iranseda == null)
                return null;

            CatalogueBuilder builder = new();
            builder._catalogue.Channels = new();
            builder._catalogue.Channels.Add(iranseda);
            return builder;
        }

        public void SaveCatalogue(string path, bool minify = false)
        {
            JsonSerializerSettings jsonSetting = new()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = minify ? Formatting.None : Formatting.Indented,
            };

            File.WriteAllText(
                path: path,
                contents: JsonConvert.SerializeObject(_catalogue, jsonSetting));
        }
    }
}