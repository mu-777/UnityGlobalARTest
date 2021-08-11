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
        }
    }
}
