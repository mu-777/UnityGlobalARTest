﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalAR
{
    public class GlobalARSessionManager
    {
        private static GlobalARSessionManager _instance;
        public static GlobalARSessionManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new GlobalARSessionManager();
                }
                return _instance;
            }
        }

        public static GlobalARSession Session { get { return _instance?._sessionComponent; } }
        private GlobalARSession _sessionComponent;

        internal GlobalARSessionConfig Config { get; private set; }

        private class CoroutineInfo
        {
            public Coroutine Coroutine = null;
            public IEnumerator CoroutineFunc;

            public CoroutineInfo(IEnumerator coroutineFunc)
            {
                CoroutineFunc = coroutineFunc;
            }
        }

        private List<CoroutineInfo> _coroutineInfos = new List<CoroutineInfo>();

        private GlobalARSessionManager()
        {
            _coroutineInfos.Add(new CoroutineInfo(GeoDataUpdateCoroutine()));
            _coroutineInfos.Add(new CoroutineInfo(GeoLocationUpdateCoroutine()));
        }

        internal void CreateSession(GlobalARSession sessionComponent)
        {
            //sessionComponent.StartCoroutine(InstantPreviewManager.InitializeIfNeeded());

            if(_sessionComponent != null)
            {
                Debug.LogError("Multiple session components cannot exist in the scene. Destroying the newest.");
                GameObject.Destroy(sessionComponent);
                return;
            }

            _sessionComponent = sessionComponent;
            Config = _sessionComponent.globalARSessionConfig;

            GeoLocationManager.Instance.Initialize(_sessionComponent.geoLocationEstimatorSystem,
                                                   _sessionComponent.geoLocationEstimatorConfig);
            GeoDataManager.Instance.Initialize(_sessionComponent.geoDataLoaderSystem,
                                               _sessionComponent.geoDataLoaderConfig);
        }

        internal void StartSession()
        {
            foreach(var cinfo in _coroutineInfos)
            {
                if(cinfo.Coroutine == null)
                {
                    cinfo.Coroutine = _sessionComponent.StartCoroutine(cinfo.CoroutineFunc);
                }
            }

            GeoDataManager.Instance.NewGeoDataLoadedEvent += (GeoData result) =>
            {
                if (_sessionComponent.OnNewGeoDataLoaded != null)
                {
                    _sessionComponent.OnNewGeoDataLoaded.Invoke(result);
                }
            };
        }

        internal void StopSession()
        {
            foreach(var cinfo in _coroutineInfos)
            {
                if(cinfo.Coroutine != null)
                {
                    _sessionComponent.StopCoroutine(cinfo.Coroutine);
                    cinfo.Coroutine = null;
                }
            }

            GeoDataManager.Instance.NewGeoDataLoadedEvent = null;
        }

        internal void UpdateSession()
        {

        }

        internal void DestroySession()
        {
            GeoLocationManager.Instance.DestroySelf();
            GeoDataManager.Instance.DestroySelf();
        }

        private IEnumerator GeoDataUpdateCoroutine()
        {
            var yielder = new WaitForSeconds(Config.GeoLocUpdateIntervalSec);
            while(true)
            {
                yield return yielder;
                if(GeoLocationManager.Instance.IsLocalized)
                {
                    GeoDataManager.Instance.Update(GeoLocationManager.Instance.CurrGeoPose.GeoPos);
                }
            }
        }

        private IEnumerator GeoLocationUpdateCoroutine()
        {
            var task = GeoLocationManager.Instance.CoordAlignmentAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            if(!task.Result)
            {
                Debug.LogError("Fail to alignment");
            }

            var yielder = new WaitForEndOfFrame();
            while(true)
            {
                yield return yielder;
                GeoLocationManager.Instance.EstimateGeoLocation(out _, out _);
            }
        }

    }
}
