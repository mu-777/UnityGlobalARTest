using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GlobalAR
{
    public class GeoDataManager
    {
        private static GeoDataManager _instance;
        public static GeoDataManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new GeoDataManager();
                }
                return _instance;
            }
        }

        public Action<GeoData> NewGeoDataLoadedEvent;
        public List<GeoData> AllGeoData { get { return _geoDataCache.Values.ToList(); } }

        private IGeoDataLoader _geoLoader;
        private Dictionary<int, GeoData> _geoDataCache;

        private GeoDataManager()
        {
            _geoDataCache = new Dictionary<int, GeoData>();
        }

        public void Initialize(GeoDataLoaderSystem system, ScriptableObject config)
        {
            _geoLoader = GeoDataLoaderFactory.Create(system, config);
        }

        public void Update(GeoLocation currGeoPose)
        {
            var geoMeshCode = GeoDataUtils.GeoLocationToMeshCode3rd(currGeoPose);

            if(_geoDataCache.ContainsKey(geoMeshCode))
            {
                return;
            }

            if(_geoLoader.LoadGeoData(geoMeshCode, out var data) == GARResult.SUCCESS)
            {
                _geoDataCache.Add(geoMeshCode, data);
                if(NewGeoDataLoadedEvent != null)
                {
                    NewGeoDataLoadedEvent.Invoke(data);
                }
            }
        }
        public void DestroySelf()
        {
            NewGeoDataLoadedEvent = null;
            _instance = null;
        }
    }

    //ref: https://tech.atware.co.jp/mesh-system/
    //ref: https://qiita.com/yuusei/items/549402a80efd7e7192ef
    public static class GeoDataUtils
    {
        private static int FloorToInt(double x)
        {
            return (int)(Math.Floor(x));
        }

        public static int GeoLocationToMeshCode1st(GeoLocation geoPose)
        {
            return FloorToInt(geoPose.Latitude * 1.5) * 100 + FloorToInt(geoPose.Longtitude - 100);
        }

        public static int GeoLocationToMeshCode2nd(GeoLocation geoPose)
        {
            return GeoLocationToMeshCode1st(geoPose) * 100
                   + FloorToInt(geoPose.Latitude * 12f % 8) * 10 + FloorToInt((geoPose.Longtitude - 100f) * 8) % 8;
        }

        public static int GeoLocationToMeshCode3rd(GeoLocation geoPose)
        {
            return GeoLocationToMeshCode2nd(geoPose) * 100
                   + FloorToInt(geoPose.Latitude * 120f % 10) * 10 + FloorToInt((geoPose.Longtitude - 100f) * 80) % 10;
        }

    }
}
