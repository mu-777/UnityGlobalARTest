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
        public class GeoLocationUpdatedEvent : UnityEvent<Vector3> { };
        public GeoLocationUpdatedEvent OnGeoLocationUpdated;

        [System.Serializable]
        public class NewGeoDataLoadedEvent : UnityEvent<GeoData> { };
        public NewGeoDataLoadedEvent OnNewGeoDataLoaded;

        [System.Serializable]
        public class GeoDataRemovedEvent : UnityEvent<int> { };
        public GeoDataRemovedEvent OnGeoDataRemoved;

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
