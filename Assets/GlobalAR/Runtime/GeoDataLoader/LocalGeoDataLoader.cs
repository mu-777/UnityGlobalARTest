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
        private Dictionary<int, GeoData> _geoDataCache;


        public LocalGeoDataLoader(ScriptableObject config)
        {
            _config = config as LocalGeoDataLoaderConfig;
            _geoDataCache = new Dictionary<int, GeoData>();
        }

        public GARResult LoadGeoData(GeoLocation geoPose, out GeoData data)
        {
            var meshCode3rd = GeoDataUtils.GeoLocationToMeshCode3rd(geoPose);
            if (_geoDataCache.ContainsKey(meshCode3rd))
            {
                data = _geoDataCache[meshCode3rd];
                return GARResult.SUCCESS;
            }            
            var gml = XElement.Load(FormatGMLFilePath(meshCode3rd));
            var res = CityGMLParser.Parse(gml, out data);
            _geoDataCache.Add(meshCode3rd, data);
            return res;
        }

        private string FormatGMLFilePath(int meshCode3rd)
        {
            return $"{Path.GetFullPath(_config.GmlDirPath)}/{meshCode3rd}_bldg_6697_op.gml";
        }
    }
}
