using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

namespace GlobalAR
{
    public class LocalGeoDataLoader : IGeoDataLoader
    {
        private LocalGeoDataLoaderConfig _config;

        public LocalGeoDataLoader(ScriptableObject config)
        {
            _config = config as LocalGeoDataLoaderConfig;
        }

        public GARResult LoadGeoData(GeoLocation geoPose, out GeoData data)
        {
            var fileName = FormatGMLFileName(geoPose);
            var gml = XElement.Load($"{Path.GetFullPath(_config.GmlDirPath)}/{fileName}");
            return CityGMLParser.Parse(gml, out data);
        }

        private string FormatGMLFileName(GeoLocation geoPose)
        {
            var meshCode3rd = GeoDataLoaderUtils.GeoLocationToMeshCode3rd(geoPose);
            return $"{meshCode3rd}_bldg_6697_op.gml";
        }
    }
}
