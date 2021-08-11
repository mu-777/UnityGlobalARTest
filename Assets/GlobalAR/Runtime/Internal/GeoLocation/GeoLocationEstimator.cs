using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalAR
{
    public enum GeoLocationEstimatorSystem
    {
        SimpleGPS, SLAMFusion, Mock
    }


    public interface IGeoLocationEstimator
    {
        GARResult EstimateGeoLocation(out GeoLocation geoPose, out Pose localPose);
    }

    public class GeoLocationEstimatorFactory
    {
        public static IGeoLocationEstimator Create(GeoLocationEstimatorSystem system, ScriptableObject config)
        {
            var switcher = new Dictionary<GeoLocationEstimatorSystem, Func<IGeoLocationEstimator>>()
            {
                { GeoLocationEstimatorSystem.Mock, () => { return new MockGeoLocationEstimator(config); } }
            };
            return switcher[system]();
        }
    }

}
