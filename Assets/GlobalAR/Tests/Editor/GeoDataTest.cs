using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using GlobalAR;

namespace Tests
{
    public class GeoDataTest
    {
        private readonly float acceptableError = 0.000001f;

        [TestCase(35.529166f, 139.69375f,
                  35.52455616868929f, "14130-bldg-245827",
                  139.69557329102642f, 139.69557329102642f,
                  TestName = "Kawasaki-eki")]
        public void GeoDataLoadTest(float lat, float lon,
                                    float expectedLowerCornerLat, string expectedFirstBuildingId,
                                    float expectedFirstBuildingLod0Lon, float expectedFirstBuildingLod1Lon)
        {
            var config = new LocalGeoDataLoaderConfig();
            config.GmlDirPath = Path.GetFullPath("Assets/GlobalAR/Tests/Resources/GMLDir");

            var geoDataLoader = new LocalGeoDataLoader(config);

            var geoPos = new GeoPosition(lat, lon, 0f);
            Assert.AreEqual(geoDataLoader.LoadGeoData(GeoDataUtils.GeoPositionToMeshCode3rd(geoPos),
                                                      out var geoData), GARResult.SUCCESS);
            Assert.AreEqual(expectedLowerCornerLat, geoData.LowerCorner.Latitude, acceptableError);
            Assert.AreEqual(expectedFirstBuildingId, geoData.Buildings[0].BuildingId);
            Assert.AreEqual(expectedFirstBuildingLod0Lon, geoData.Buildings[0].Lod0FootPrint[0].Longtitude, acceptableError);
            Assert.AreEqual(expectedFirstBuildingLod0Lon, geoData.Buildings[0].Lod1Solid[0][0].Longtitude, acceptableError);
        }

        [TestCase(35.529166f, 139.69375f, TestName = "Kawasaki-eki")]
        public void CachedGeoDataLoadTest(float lat, float lon)
        {
            var sw = new System.Diagnostics.Stopwatch();
            var config = new LocalGeoDataLoaderConfig();
            config.GmlDirPath = Path.GetFullPath("Assets/GlobalAR/Tests/Resources/GMLDir");

            GeoDataManager.Instance.Initialize(GeoDataLoaderSystem.Local, config);

            var geoPos = new GeoPosition(lat, lon, 0f);

            sw.Start();
            GeoDataManager.Instance.LoadGeoDataIfNeeded(geoPos);
            sw.Stop();
            var loadTime1 = sw.Elapsed;

            sw.Restart();
            GeoDataManager.Instance.LoadGeoDataIfNeeded(geoPos);
            sw.Stop();
            var loadTime2 = sw.Elapsed;
            Assert.Less(loadTime2.Seconds, loadTime1.Seconds * 0.1f);

            GeoDataManager.Instance.DestroySelf();
        }

        [TestCase(35.529166f, 139.69375f, 35.5229939f, 139.6914585f, TestName = "Kawasaki-eki_Hachonawate-eki")]
        public void NewGeoDataLoadedTest(float lat1, float lon1, float lat2, float lon2)
        {
            var cnt = 0;

            var config = new LocalGeoDataLoaderConfig();
            config.GmlDirPath = Path.GetFullPath("Assets/GlobalAR/Tests/Resources/GMLDir");

            GeoDataManager.Instance.Initialize(GeoDataLoaderSystem.Local, config);
            GeoDataManager.Instance.NewGeoDataLoadedEvent += (GeoData data) =>
            {
                Debug.Log($"NewGeoDataLoadedEvent: {++cnt}");
            };

            var geoPos = new GeoPosition(lat1, lon1, 0f);
            GeoDataManager.Instance.LoadGeoDataIfNeeded(geoPos);

            geoPos.Latitude = lat2;
            geoPos.Longtitude = lon2;
            GeoDataManager.Instance.LoadGeoDataIfNeeded(geoPos);
            Assert.AreEqual(2, cnt);
            GeoDataManager.Instance.DestroySelf();
        }

        [TestCase(35.666863f, 139.74954f, 53394509, TestName = "Tranomon")]
        [TestCase(35.529166f, 139.69375f, 53392535, TestName = "Kawasaki-eki")]
        public void MeshCodeConvertTest(float lat, float lon, int expected3rd)
        {
            var geoPos = new GeoPosition(lat, lon, 0f);

            Assert.AreEqual(GeoDataUtils.GeoPositionToMeshCode1st(geoPos), Mathf.FloorToInt(expected3rd * 0.0001f));
            Assert.AreEqual(GeoDataUtils.GeoPositionToMeshCode2nd(geoPos), Mathf.FloorToInt(expected3rd * 0.01f));
            Assert.AreEqual(GeoDataUtils.GeoPositionToMeshCode3rd(geoPos), expected3rd);
        }

        [TestCase(533925, 1, 0, 533915)]
        [TestCase(533925, -1, 0, 533935)]
        [TestCase(533925, 10, 0, 523905)]
        [TestCase(533925, -10, 0, 543945)]
        [TestCase(533925, 15, 0, 513935)]
        [TestCase(533925, -15, 0, 553915)]
        [TestCase(533925, 0, 1, 533926)]
        [TestCase(533925, 0, -1, 533924)]
        [TestCase(533925, 0, 10, 534027)]
        [TestCase(533925, 0, -10, 533823)]
        [TestCase(533925, 0, 15, 534124)]
        [TestCase(533925, 0, -15, 533726)]
        [TestCase(53392535, 1, 0, 53392525)]
        [TestCase(53392535, -1, 0, 53392545)]
        [TestCase(53392535, 0, 1, 53392536)]
        [TestCase(53392535, 0, -1, 53392534)]
        [TestCase(53392535, 10, 0, 53391535)]
        [TestCase(53392535, -10, 0, 53393535)]
        [TestCase(53392535, 0, 10, 53392635)]
        [TestCase(53392535, 0, -10, 53392435)]
        [TestCase(53392535, 15, 0, 53390585)]
        [TestCase(53392535, -15, 0, 53393585)]
        [TestCase(53392535, 0, 15, 53392730)]
        [TestCase(53392535, 0, -15, 53392430)]
        public void MeshCodeOffsetTest(int baseCode, int offsetDown, int offsetRight, int expectedCode)
        {
            Assert.IsTrue(GeoDataUtils.OffsetMeshCode(baseCode, offsetDown, offsetRight, out var actualCode));
            Assert.AreEqual(expectedCode, actualCode);
        }



        [Test]
        public void GetPathTest()
        {
            Debug.Log(Path.GetFullPath("Assets/GlobalAR/Tests/Resources/GMLDir"));
            Debug.Log(Path.GetFullPath(@"C:\Users\Public\Desktop"));
        }

        //[UnityTest]
        //public IEnumerator GeoDataLoaderTestWithEnumeratorPasses()
        //{
        //    yield return null;
        //}
    }
}
