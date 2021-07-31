using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GlobalAR
{
    public class GeoLocationManager
    {
        private static GeoLocationManager _instance;
        public static GeoLocationManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new GeoLocationManager();
                }
                return _instance;
            }
        }

        private IGeoLocationEstimator _geoLocEstimator;
        public GeoLocation CurrGeoPose { get; private set; }

        private GeoLocationManager()
        {
            CurrGeoPose = new GeoLocation();
        }

        public void Initialize(GeoLocationEstimatorSystem system, ScriptableObject config)
        {
            _geoLocEstimator = GeoLocationEstimatorFactory.Create(system, config);
        }

        public bool EstimateGeoLocation(out GeoLocation geoPose)
        {
            if(_geoLocEstimator.EstimateGeoLocation(out var tempGeoPose) == GARResult.SUCCESS)
            {
                geoPose = CurrGeoPose = tempGeoPose;
                return true;
            };
            geoPose = CurrGeoPose;
            return false;
        }
    }
}
