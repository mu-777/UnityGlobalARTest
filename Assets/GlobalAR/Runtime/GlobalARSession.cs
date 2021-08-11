using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GlobalAR
{
    public class GlobalARSession : MonoBehaviour
    {
        public GlobalARSessionConfig globalARSessionConfig;

        public GeoLocationEstimatorSystem geoLocationEstimatorSystem;
        public ScriptableObject geoLocationEstimatorConfig;

        public GeoDataLoaderSystem geoDataLoaderSystem;
        public ScriptableObject geoDataLoaderConfig;
        
        [System.Serializable]
        public class NewGeoDataLoadedEvent : UnityEvent<GeoData> { };
        public NewGeoDataLoadedEvent OnNewGeoDataLoaded;

        private GeoDataManager _geoDataManager;

        void Awake()
        {
            GlobalARSessionManager.Instance.CreateSession(this);

            GeoLocationManager.Instance.Initialize(geoLocationEstimatorSystem, geoLocationEstimatorConfig);

            GeoDataManager.Instance.Initialize(geoDataLoaderSystem, geoDataLoaderConfig);
            GeoDataManager.Instance.NewGeoDataLoadedEvent += (GeoData result) =>
            {
                if(OnNewGeoDataLoaded != null)
                {
                    OnNewGeoDataLoaded.Invoke(result);
                }
            };
        }

        void OnEnable()
        {
            GlobalARSessionManager.Instance.StartSession();
        }

        void OnDisable()
        {
            GlobalARSessionManager.Instance.StopSession();
        }

        void Update()
        {
            GlobalARSessionManager.Instance.UpdateSession();
        }

        void OnDestroy()
        {
            GlobalARSessionManager.Instance.DestroySession();
            GeoLocationManager.Instance.DestroySelf();
            GeoDataManager.Instance.DestroySelf();
        }
    }
}
