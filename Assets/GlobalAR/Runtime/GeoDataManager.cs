using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalAR
{
    public class GeoDataManager
    {
        private IGeoLocationEstimator _geoLocEstimator;
        private IGeoDataLoader _geoLoader;

        public GeoDataManager(IGeoLocationEstimator geoLocEstimator, IGeoDataLoader geoLoader)
        {
            _geoLocEstimator = geoLocEstimator;
            _geoLoader = geoLoader;
        }

        public void Update()
        {
            _geoLocEstimator.EstimateGeoLocation(out var geoPose);
            Debug.Log(string.Format("Altitude: {0}", geoPose.Altitude));

        }

        private bool IsGeoDataLoaded()
        {
            return false;
        }
    }

    //ref: https://tech.atware.co.jp/mesh-system/
    //ref: https://qiita.com/yuusei/items/549402a80efd7e7192ef
    public static class GeoDataLoaderUtils
    {
        public static int GeoLocationToMeshCode1st(GeoLocation geoPose)
        {
            return Mathf.FloorToInt(geoPose.Latitude * 1.5f) * 100 + Mathf.FloorToInt(geoPose.Longtitude - 100);
        }

        public static int GeoLocationToMeshCode2nd(GeoLocation geoPose)
        {
            return GeoLocationToMeshCode1st(geoPose) * 100
                   + Mathf.FloorToInt(geoPose.Latitude * 12f % 8) * 10 + Mathf.FloorToInt((geoPose.Longtitude - 100f) * 8) % 8;
        }

        public static int GeoLocationToMeshCode3rd(GeoLocation geoPose)
        {
            return GeoLocationToMeshCode2nd(geoPose) * 100
                   + Mathf.FloorToInt(geoPose.Latitude * 120f % 10) * 10 + Mathf.FloorToInt((geoPose.Longtitude - 100f) * 80) % 10;
        }

    }
}
