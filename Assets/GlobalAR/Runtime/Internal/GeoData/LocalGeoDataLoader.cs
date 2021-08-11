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

        public GARResult LoadGeoData(int geoMeshCode3rd, out GeoData data)
        {
            var gml = XElement.Load(FormatGMLFilePath(geoMeshCode3rd));
            var res = CityGMLParser.Parse(gml,
                                          out GeoPosition lowerCorner, out GeoPosition upperCorner,
                                          out List<GeoBuilding> buildings);
            data = new GeoData(geoMeshCode3rd, lowerCorner, upperCorner, buildings);
            return res;
        }

        private string FormatGMLFilePath(int meshCode3rd)
        {
            return $"{Path.GetFullPath(_config.GmlDirPath)}/{meshCode3rd}_bldg_6697_op.gml";
        }
    }
}
