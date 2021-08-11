using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalAR
{
    public class GlobalARSession : MonoBehaviour
    {
        public GlobalARSessionConfig globalARSessionConfig;

        public GeoLocationEstimatorSystem geoLocationEstimatorSystem;
        public ScriptableObject geoLocationEstimatorConfig;

        public GeoDataLoaderSystem geoDataLoaderSystem;
        public ScriptableObject geoDataLoaderConfig;
        
        private GeoDataManager _geoDataManager;

        void Awake()
        {
            GlobalARSessionManager.Instance.CreateSession(this);
            GeoLocationManager.Instance.Initialize(geoLocationEstimatorSystem, geoLocationEstimatorConfig);
            GeoDataManager.Instance.Initialize(geoDataLoaderSystem, geoDataLoaderConfig);
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
    }
}
