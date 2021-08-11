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

        public GARResult EstimateGeoLocation(out GeoLocation geoPose, out Pose localPose)
        {
            geoPose = _config.MockData;
            localPose = new Pose(Camera.main.transform.position, Camera.main.transform.rotation);
            return GARResult.SUCCESS;
        }
    }
}
