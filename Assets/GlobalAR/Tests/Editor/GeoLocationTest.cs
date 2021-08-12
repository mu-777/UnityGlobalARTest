using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using GlobalAR;

namespace Tests
{
    public class GeoLocationTest
    {
        private readonly float acceptableError = 0.0001f;

        [Test]
        public void GeoPosDistanceTest()
        {
            var lat = 35.0;
            var lon = 135.0;
            var geoPos = new GeoPosition(lat, lon, 0f);
            Assert.AreEqual(0.0f, geoPos.Distance(geoPos));

            var diff = 1.0 / 60.0;

            var expectedLon = 1518.094667f;
            var geoPos_lonDiff = new GeoPosition(lat, lon + diff, 0f);
            Assert.AreEqual(expectedLon, geoPos.Distance(geoPos_lonDiff), acceptableError);

            var expectedLat = 1853.251395f;
            var geoPos_latDiff = new GeoPosition(lat + diff, lon, 0f);
            Assert.AreEqual(expectedLat, geoPos.Distance(geoPos_latDiff), acceptableError);
        }

        [Test]
        public void GeoPosToVectorTest()
        {
            var lat = 35.0;
            var lon = 135.0;
            var origin = new GeoPosition(lat, lon, 0f);
            Assert.AreEqual(Vector3.zero, origin.ToVector3(origin));

            var diff = 1.0 / 60.0;

            var expectedLon = 1518.094667f;
            var geoPos_lonDiff = new GeoPosition(lat, lon + diff, 0f);
            Assert.AreEqual(+expectedLon, geoPos_lonDiff.ToVector3(origin).x, acceptableError);

            var expectedLat = 1853.251395f;
            var geoPos_latDiff = new GeoPosition(lat + diff, lon, 0f);
            Assert.AreEqual(+expectedLat, geoPos_latDiff.ToVector3(origin).z, acceptableError);
        }

        [Test]
        public void GeoPosTranslateTest()
        {
            var diff = 1.0 / 60.0;
            var lat = 35.0;
            var lon = 135.0;

            var origin1 = new GeoPosition(lat, lon, 0f);
            var translatedX = new GeoPosition(lat, lon + diff, 0f);
            var distX = translatedX.Distance(origin1);            
            origin1.Translate(new Vector3((float)distX, 0.0f, 0.0f));
            Debug.Log($"lat: {origin1.Latitude}, Lon: {origin1.Longtitude}, Lon_expected: {lon + diff}");


            var origin2 = new GeoPosition(lat, lon, 0f);
            var translatedZ = new GeoPosition(lat + diff, lon, 0f);
            var distZ = translatedZ.Distance(origin2);
            origin2.Translate(new Vector3(0.0f, 0.0f, (float)distZ));
            Debug.Log($"lat: {origin2.Latitude}, lat_expected: {lat + diff}, Lon: {origin2.Longtitude}");
        }
    }
}
