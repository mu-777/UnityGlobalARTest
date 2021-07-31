using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalAR
{
    public enum GeoDataLoaderSystem
    {
        Web, Local, Mock
    }

    public interface IGeoDataLoader
    {
        GARResult LoadGeoData(GeoLocation geoPose, out GeoData data);
    }

    public class GeoDataLoaderFactory
    {
        public static IGeoDataLoader Create(GeoDataLoaderSystem system, ScriptableObject config)
        {
            var switcher = new Dictionary<GeoDataLoaderSystem, Func<IGeoDataLoader>>()
            {
                { GeoDataLoaderSystem.Local, () => { return new LocalGeoDataLoader(config); } }
            };
            return switcher[system]();
        }
    }
}
