﻿using System.Collections;
using System.Collections.Generic;
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
                if (_instance == null)
                {
                    _instance = new GeoDataManager();
                }
                return _instance;
            }
        }

        private IGeoDataLoader _geoLoader;

        public void Initialize(GeoDataLoaderSystem system, ScriptableObject config)
        {
            _geoLoader = GeoDataLoaderFactory.Create(system, config);
        }

        public void Update(GeoLocation currGeoPose)
        {
            if (!IsGeoDataLoaded())
            {
                _geoLoader.LoadGeoData(currGeoPose, out var data);
            }
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
