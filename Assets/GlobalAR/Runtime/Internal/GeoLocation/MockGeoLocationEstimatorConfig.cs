using UnityEngine;
using UnityEditor;

namespace GlobalAR
{
    public class MockGeoLocationEstimatorConfig : ScriptableObject
    {
        public GeoLocation MockData;

        [MenuItem("ScriptableObjects/GeoLocation/MockGeoLocationEstimatorConfig")]
        private static void Create()
        {
            var confData = CreateInstance<MockGeoLocationEstimatorConfig>();
            var assetName = $"{GlobalARCommon.AssetPath}/MockGeoLocationEstimatorConfig.asset";
            AssetDatabase.CreateAsset(confData, assetName);
            AssetDatabase.Refresh();
        }
    }
}
