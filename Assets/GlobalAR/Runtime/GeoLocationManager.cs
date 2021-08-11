using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private GeoLocation _currGeoPose;
        public GeoLocation CurrGeoPose { get { return _currGeoPose; } }
        private Pose _currLocalPose;
        public Pose CurrLocalPose { get { return _currLocalPose; } }
        public bool IsLocalized { get; private set; }

        private GlobalARSessionConfig _config;

        private GeoLocationManager()
        {
            _config = GlobalARSessionManager.Instance.Config;
            _currGeoPose = new GeoLocation();
            _currGeoPose.HorizontalError = _config.GeoLocConvergenceErrThreshold + 1f;
            _currGeoPose.VerticalError = _config.GeoLocConvergenceErrThreshold + 1f;

            IsLocalized = false;
        }

        public void Initialize(GeoLocationEstimatorSystem system, ScriptableObject config)
        {
            _geoLocEstimator = GeoLocationEstimatorFactory.Create(system, config);
        }

        public bool EstimateGeoLocation(out GeoLocation geoPose, out Pose localPose)
        {
            if(_geoLocEstimator.EstimateGeoLocation(out var tempGeoPose, out var tempLocalPose) != GARResult.SUCCESS)
            {
                geoPose = _currGeoPose;
                localPose = _currLocalPose;
                return false;
            };
            geoPose = _currGeoPose = tempGeoPose;
            localPose = _currLocalPose = tempLocalPose;
            return true;
        }

        async public Task<bool> CoordAlignmentAsync()
        {
            IsLocalized = await WaitForConvergence();
            if(!IsLocalized)
            {
                return IsLocalized;
            }
            return IsLocalized;
        }

        async private Task<bool> WaitForConvergence()
        {
            var isSuccess = false;
            var cnt = 0;
            var intervalMSec = 1000;
            while(cnt * intervalMSec * 0.001f < _config.CoordAlignmentTimeoutSec)
            {
                EstimateGeoLocation(out var geoPose, out var _);
                if((geoPose.HorizontalError < _config.GeoLocConvergenceErrThreshold)
                        && (geoPose.VerticalError < _config.GeoLocConvergenceErrThreshold))
                {
                    isSuccess = true;
                    break;
                }
                await Task.Delay(intervalMSec);
                cnt++;
            }
            if(!isSuccess)
            {
                Debug.LogWarning("fail to align coordinates");
                return isSuccess;
            }
            return isSuccess;
        }
    }

    public static class GeoLocationUtils
    {
        public const float EarthRadius = 6356752f;

        /// <summary>
        /// The diff should be small sufficiently.
        /// X = west to east(lon), Y = to top, Z = south to north(lot)
        /// TODO: http://www.thothchildren.com/chapter/5bd87bc651d930518903aa14
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector3 GeoLocationDiffToPos(GeoLocation origin, GeoLocation target)
        {
            var lon1 = origin.Longtitude * Math.PI / 180.0;
            var lon2 = target.Longtitude * Math.PI / 180.0;
            var lat1 = origin.Latitude * Math.PI / 180.0;
            var lat2 = target.Latitude * Math.PI / 180.0;
            var dlon = lon2 - lon1;
            var dlat = lat2 - lat1;
            return new Vector3(
                       EarthRadius * (float)Math.Sin(dlon),
                       target.Altitude - origin.Altitude,
                       EarthRadius * (float)Math.Sin(dlat));
        }
    }
}
