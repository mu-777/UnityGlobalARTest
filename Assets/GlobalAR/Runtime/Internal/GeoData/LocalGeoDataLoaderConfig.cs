
using UnityEngine;
using UnityEditor;

namespace GlobalAR
{
    public class LocalGeoDataLoaderConfig : ScriptableObject
    {
        public string GmlDirPath;

        [MenuItem("ScriptableObjects/GeoData/LocalGeoDataLoaderConfig")]
        private static void Create()
        {
            var confData = CreateInstance<LocalGeoDataLoaderConfig>();
            var assetName = $"{GARCommon.AssetPath}/LocalGeoDataLoaderConfig.asset";
            AssetDatabase.CreateAsset(confData, assetName);
            AssetDatabase.Refresh();
        }
    }
}
