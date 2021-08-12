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
        public Action<int> GeoDataRemovedEvent;
        public List<GeoData> AllGeoData { get { return _geoDataCache.Values.ToList(); } }

        private IGeoDataLoader _geoLoader;
        private int _currGeoMeshCode = 0;
        private Dictionary<int, GeoData> _geoDataCache;

        private GeoDataManager()
        {
            _geoDataCache = new Dictionary<int, GeoData>();
        }

        public void Initialize(GeoDataLoaderSystem system, ScriptableObject config)
        {
            _geoLoader = GeoDataLoaderFactory.Create(system, config);
        }

        public void Update()
        {

        }

        public void LoadGeoDataIfNeeded(GeoPosition currGeoPos)
        {
            var geoMeshCode = GeoDataUtils.GeoPositionToMeshCode3rd(currGeoPos);

            if(_currGeoMeshCode == geoMeshCode)
            {
                return;
            }
            _currGeoMeshCode = geoMeshCode;
            LoadGeoData(_currGeoMeshCode);

            var loadingCodes = new List<int>();
            for(var i = -1; i < 2; i++)
            {
                for(var j = -1; j < 2; j++)
                {
                    if(GeoDataUtils.OffsetMeshCode(_currGeoMeshCode, i, j, out var offsetedCode))
                    {
                        loadingCodes.Add(offsetedCode);
                        LoadGeoData(offsetedCode);
                    }
                }
            }
            foreach(var code in _geoDataCache.Keys)
            {
                if (!loadingCodes.Contains(code))
                {
                    UnloadGeoData(code);
                }
            }
        }

        private void LoadGeoData(int geoMeshCode)
        {
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

        private void UnloadGeoData(int geoMeshCode)
        {
            if(!_geoDataCache.ContainsKey(geoMeshCode))
            {
                return;
            }
            _geoDataCache.Remove(geoMeshCode);
            if(GeoDataRemovedEvent != null)
            {
                GeoDataRemovedEvent.Invoke(geoMeshCode);
            }
        }

        public void DestroySelf()
        {
            NewGeoDataLoadedEvent = null;
            GeoDataRemovedEvent = null;
            _instance = null;
        }
    }
}
