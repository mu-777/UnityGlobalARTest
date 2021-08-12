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

        public Action<Vector3> GeoLocationUpdatedEvent;

        private IGeoLocationEstimator _geoLocEstimator;
        private GeoLocation _currGeoPose;

        public GeoLocation CurrGeoPose { get { return _currGeoPose; } }
        private Pose _currLocalPose;

        public GeoPosition OriginInGeoCoord { get { return _originInGeoCoord; } }
        private GeoPosition _originInGeoCoord;

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

        public void Update()
        {
            _geoLocEstimator.Update();
        }

        public void DestroySelf()
        {
            _instance = null;
        }

        public bool EstimateGeoLocation(out GeoLocation geoPose, out Pose localPose)
        {
            if(!IsLocalized)
            {
                geoPose = _currGeoPose;
                localPose = _currLocalPose;
                return false;
            }
            if(_geoLocEstimator.EstimateGeoLocation(out var tempGeoPose, out var tempLocalPose) != GARResult.SUCCESS)
            {
                geoPose = _currGeoPose;
                localPose = _currLocalPose;
                return false;
            };
            geoPose = _currGeoPose = tempGeoPose;
            localPose = _currLocalPose = tempLocalPose;
            if(GeoLocationUpdatedEvent != null)
            {
                GeoLocationUpdatedEvent.Invoke(localPose.position);
            }
            return true;
        }

        async public Task<bool> CoordAlignmentAsync()
        {
            var resTuple = await WaitForConvergence();
            IsLocalized = resTuple.Item1;
            _currGeoPose = resTuple.Item2;
            if(IsLocalized)
            {
                _originInGeoCoord = new GeoPosition(_currGeoPose.GeoPos.Latitude,
                                                    _currGeoPose.GeoPos.Longtitude,
                                                    _currGeoPose.GeoPos.Altitude);
            }
            return IsLocalized;
        }

        async private Task<Tuple<bool, GeoLocation>> WaitForConvergence()
        {
            var isSuccess = false;
            var geoPose = new GeoLocation();
            var cnt = 0;
            var intervalMSec = 1000;
            while(cnt * intervalMSec * 0.001f < _config.CoordAlignmentTimeoutSec)
            {
                if(_geoLocEstimator.GetGPSPose(out geoPose) != GARResult.SUCCESS)
                {
                    await Task.Delay(intervalMSec);
                    cnt++;
                    continue;
                }
                if((geoPose.HorizontalError < _config.GeoLocConvergenceErrThreshold)
                        && (geoPose.VerticalError < _config.GeoLocConvergenceErrThreshold))
                {
                    isSuccess = true;
                    break;
                }
                await Task.Delay(intervalMSec);
                cnt++;
            }
            return new Tuple<bool, GeoLocation>(isSuccess, geoPose);
        }
    }
}
