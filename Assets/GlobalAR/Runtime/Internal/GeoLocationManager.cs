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

        public GeoPosition OriginInGeoCoord { get { return _originInGeoCoord; } }
        private GeoPosition _originInGeoCoord = new GeoPosition(35.531664234319436, 139.69824465760138, 2.07f); // TODO: This is temp(14130-bldg-245435)

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

        public void DestroySelf()
        {
            _instance = null;
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
            return isSuccess;
        }
    }
}
