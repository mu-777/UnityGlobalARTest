using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalAR
{
    public class MockGeoLocationEstimator : IGeoLocationEstimator
    {
        private GeoLocation _currGeoPose;
        private Vector3 _offsetFromKeyInput;

        public MockGeoLocationEstimator(ScriptableObject config)
        {
            var conf = config as MockGeoLocationEstimatorConfig;
            _currGeoPose = conf.MockData;
            _offsetFromKeyInput = Vector3.zero;
        }

        public GARResult EstimateGeoLocation(out GeoLocation geoPose, out Pose localPose)
        {
            _currGeoPose.GeoPos.Translate(_offsetFromKeyInput);
            _offsetFromKeyInput = Vector3.zero;
            geoPose = _currGeoPose;
            localPose.position = _currGeoPose.GeoPos.ToVector3(GeoLocationManager.Instance.OriginInGeoCoord);
            localPose.rotation = Quaternion.identity;
            return GARResult.SUCCESS;
        }

        public GARResult GetGPSPose(out GeoLocation geoPose)
        {
            geoPose = _currGeoPose;
            return GARResult.SUCCESS;
        }

        public void Update()
        {
            var offset = 1.0f;
            if(Input.GetKey(KeyCode.UpArrow))
            {
                _offsetFromKeyInput.z += offset;
            }
            if(Input.GetKey(KeyCode.DownArrow))
            {
                _offsetFromKeyInput.z -= offset;
            }
            if(Input.GetKey(KeyCode.RightArrow))
            {
                _offsetFromKeyInput.x += offset;
            }
            if(Input.GetKey(KeyCode.LeftArrow))
            {
                _offsetFromKeyInput.x -= offset;
            }
            if(Input.GetKey(KeyCode.PageUp))
            {
                _offsetFromKeyInput.y += offset;
            }
            if(Input.GetKey(KeyCode.PageDown))
            {
                _offsetFromKeyInput.y -= offset;
            }
        }
    }
}
