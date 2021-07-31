using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalAR
{    
    public class MockGeoLocationEstimator : IGeoLocationEstimator
    {
        private MockGeoLocationEstimatorConfig _config;

        public MockGeoLocationEstimator(ScriptableObject config)
        {
            _config = config as MockGeoLocationEstimatorConfig;
        }

        public GARResult EstimateGeoLocation(out GeoLocation geoPose)
        {
            geoPose = _config.MockData;
            return GARResult.SUCCESS;
        }
    }
}
