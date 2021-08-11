using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace GlobalAR
{
    public class CityGMLParser
    {
        // posListString = "35.4719702241799 139.72718154727625 3.15 35.47199811382851 139.72712035483926 3.15 35.471962847883304 139.72709637967515 3.15 35.47193504838676 139.72715757197514 3.15 35.4719702241799 139.72718154727625 3.15"
        private static List<GeoLocation> PosListStrToGeoLocList(string posListString, double timestamp)
        {
            return posListString.Split(' ')
            .Select((str, idx) => new { str, idx })
            .GroupBy(pair => pair.idx / 3)
            .Select(group => group.Select(pair => float.Parse(pair.str)))
            .Select(point => new GeoLocation(timestamp,
                                             point.ElementAt(0), 0.0f,
                                             point.ElementAt(1), 0.0f,
                                             point.ElementAt(2))).ToList();
        }

        public static GARResult Parse(XElement gml, out GeoData data)
        {
            var timestamp = 0.0;

            data = new GeoData();
            var envelope = gml.Element(CityGMLNamespaces.gml + "boundedBy").Element(CityGMLNamespaces.gml + "Envelope");
            data.lowerCorner = PosListStrToGeoLocList(envelope.Element(CityGMLNamespaces.gml + "lowerCorner").Value,
                                                      timestamp)[0];
            data.upperCorner = PosListStrToGeoLocList(envelope.Element(CityGMLNamespaces.gml + "upperCorner").Value,
                                                      timestamp)[0];
            data.buildings = new List<GeoBuilding>();
            var buildings = gml.Elements(CityGMLNamespaces.core + "cityObjectMember").Select(el => el.Element(CityGMLNamespaces.bldg + "Building"));
            foreach(var bldg in buildings)
            {
                var buildingObj = new GeoBuilding();
                buildingObj.gmlId = bldg.Attribute(CityGMLNamespaces.gml + "id").Value;
                buildingObj.buildingId = bldg.Element(CityGMLNamespaces.gen + "stringAttribute")
                                         .Element(CityGMLNamespaces.gen + "value").Value;

                buildingObj.lod0FootPrint = PosListStrToGeoLocList(bldg.Element(CityGMLNamespaces.bldg + "lod0FootPrint")
                                                                   .Descendants()
                                                                   .Where(el => (el.Name == CityGMLNamespaces.gml + "posList"))
                                                                   .ElementAt(0).Value,
                                                                   timestamp);

                buildingObj.lod1Solid = bldg.Element(CityGMLNamespaces.bldg + "lod1Solid")
                                        .Descendants()
                                        .Where(el => (el.Name == CityGMLNamespaces.gml + "posList"))
                                        .Select(el => PosListStrToGeoLocList(el.Value, timestamp)).ToList();
                data.buildings.Add(buildingObj);
            }
            return GARResult.SUCCESS;
        }
    }

    public class CityGMLNamespaces
    {
        public static readonly XNamespace grp = "http://www.opengis.net/citygml/cityobjectgroup/2.0";
        public static readonly XNamespace core = "http://www.opengis.net/citygml/2.0";
        public static readonly XNamespace pbase = "http://www.opengis.net/citygml/profiles/base/2.0";
        public static readonly XNamespace smil20lang = "http://www.w3.org/2001/SMIL20/Language";
        public static readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        public static readonly XNamespace smil20 = "http://www.w3.org/2001/SMIL20/";
        public static readonly XNamespace bldg = "http://www.opengis.net/citygml/building/2.0";
        public static readonly XNamespace xAL = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0";
        public static readonly XNamespace uro = "http://www.kantei.go.jp/jp/singi/tiiki/toshisaisei/itoshisaisei/iur/uro/1.4";
        public static readonly XNamespace luse = "http://www.opengis.net/citygml/landuse/2.0";
        public static readonly XNamespace app = "http://www.opengis.net/citygml/appearance/2.0";
        public static readonly XNamespace gen = "http://www.opengis.net/citygml/generics/2.0";
        public static readonly XNamespace dem = "http://www.opengis.net/citygml/relief/2.0";
        public static readonly XNamespace tex = "http://www.opengis.net/citygml/texturedsurface/2.0";
        public static readonly XNamespace tun = "http://www.opengis.net/citygml/tunnel/2.0";
        public static readonly XNamespace xlink = "http://www.w3.org/1999/xlink";
        public static readonly XNamespace sch = "http://www.ascc.net/xml/schematron";
        public static readonly XNamespace veg = "http://www.opengis.net/citygml/vegetation/2.0";
        public static readonly XNamespace frn = "http://www.opengis.net/citygml/cityfurniture/2.0";
        public static readonly XNamespace gml = "http://www.opengis.net/gml";
        public static readonly XNamespace tran = "http://www.opengis.net/citygml/transportation/2.0";
        public static readonly XNamespace wtr = "http://www.opengis.net/citygml/waterbody/2.0";
        public static readonly XNamespace brid = "http://www.opengis.net/citygml/bridge/2.0";

    }
}
