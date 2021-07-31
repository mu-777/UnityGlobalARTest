using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalAR
{
    public class GlobalARManager : MonoBehaviour
    {
        public GeoLocationEstimatorSystem geoLocationEstimatorSystem;
        public ScriptableObject geoLocationEstimatorConfig;

        public GeoDataLoaderSystem geoDataLoaderSystem;
        public ScriptableObject geoDataLoaderConfig;

        public float geoLocUpdateIntervalSec = 3.0f;

        private GeoDataManager _geoDataManager;
        private Coroutine _geoLocUpdateCoroutine = null;

        void Start()
        {
            GeoLocationManager.Instance.Initialize(geoLocationEstimatorSystem, geoLocationEstimatorConfig);
            GeoDataManager.Instance.Initialize(geoDataLoaderSystem, geoDataLoaderConfig);
        }

        void OnEnable()
        {
            StartGeoLocationUpdateCoroutine();
        }

        void OnDisable()
        {
            StopGeoLocationUpdateCoroutine();
        }

        void Update()
        {
            GeoLocationManager.Instance.EstimateGeoLocation(out var geoPose);
        }

        private void StartGeoLocationUpdateCoroutine()
        {
            if (_geoLocUpdateCoroutine == null)
            {
                _geoLocUpdateCoroutine = StartCoroutine(GeoLocationUpdateCoroutine());
            }
        }

        private void StopGeoLocationUpdateCoroutine()
        {
            if (_geoLocUpdateCoroutine != null)
            {
                StopCoroutine(_geoLocUpdateCoroutine);
                _geoLocUpdateCoroutine = null;
            }
        }

        private IEnumerator GeoLocationUpdateCoroutine()
        {
            var yielder = new WaitForSeconds(geoLocUpdateIntervalSec);
            while (true)
            {
                yield return yielder;
                GeoDataManager.Instance.Update(GeoLocationManager.Instance.CurrGeoPose);
            }
        }
    }
}
