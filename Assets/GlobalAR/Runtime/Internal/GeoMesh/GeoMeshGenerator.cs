using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalAR
{
    public static class GeoMeshGenerator
    {
        // https://edom18.hateblo.jp/entry/2018/03/25/100234
        public static bool GeoSurfaceToMesh(GeoSurface surface, GeoPosition origin,
                                            out List<Vector3> vertices, out List<int> indices, out List<Vector3> normals)
        {
            vertices = new List<Vector3>();
            indices = new List<int>();
            normals = new List<Vector3>();

            var leftVerts = surface.Points.Select(pt => pt.ToVector3(origin)).ToList();
            var targetIdx = 0;
            var isLooped = false;
            while(leftVerts.Count > 3)
            {
                targetIdx = FindFarthestIdx(leftVerts);
                var prevIdx = PrevIdx(targetIdx, leftVerts);
                var nextIdx = NextIdx(targetIdx, leftVerts);
                var dir = GetSerfDirection(prevIdx, targetIdx, nextIdx, leftVerts);

                if(!CheckPointInTriangle(prevIdx, targetIdx, nextIdx, leftVerts))
                {
                    UpdateMeshData(prevIdx, targetIdx, nextIdx, leftVerts,
                                   ref vertices, ref indices, ref normals);
                    leftVerts.RemoveAt(targetIdx);
                    continue;
                }

                var startIdx = targetIdx;
                while(true)
                {
                    targetIdx = nextIdx;
                    if(targetIdx == startIdx)
                    {
                        isLooped = true;
                        break;
                    }
                    prevIdx = PrevIdx(targetIdx, leftVerts);
                    nextIdx = NextIdx(targetIdx, leftVerts);
                    var dir2 = GetSerfDirection(prevIdx, targetIdx, nextIdx, leftVerts);
                    if((Vector3.Dot(dir, dir2) > 0f) && !CheckPointInTriangle(prevIdx, targetIdx, nextIdx, leftVerts))
                    {
                        UpdateMeshData(prevIdx, targetIdx, nextIdx, leftVerts,
                                       ref vertices, ref indices, ref normals);
                        leftVerts.RemoveAt(targetIdx);
                        break;
                    }
                }
                if(isLooped)
                {
                    break;
                }
            }
            if(leftVerts.Count != 3)
            {
                return false;
            }
            UpdateMeshData(0, 1, 2, leftVerts, ref vertices, ref indices, ref normals);
            return true;
        }

        private static void UpdateMeshData(int prevIdx, int targetIdx, int nextIdx, List<Vector3> leftVerts,
                                           ref List<Vector3> vertices, ref List<int> indices, ref List<Vector3> normals)
        {
            vertices.AddRange(new Vector3[] { leftVerts[targetIdx], leftVerts[prevIdx], leftVerts[nextIdx] });
            indices.AddRange(new int[] { vertices.Count - 3, vertices.Count - 2, vertices.Count - 1 });

            var normal = GetSerfDirection(prevIdx, targetIdx, nextIdx, leftVerts);
            normals.AddRange(new Vector3[] { normal, normal, normal });
        }

        private static int NextIdx(int targetIdx, List<Vector3> leftVerts)
        {
            return (targetIdx + 1) % leftVerts.Count;
        }

        private static int PrevIdx(int targetIdx, List<Vector3> leftVerts)
        {
            var ret = (targetIdx - 1) % leftVerts.Count;
            return (ret < 0) ? (leftVerts.Count + ret) % leftVerts.Count : ret;
        }

        private static int FindFarthestIdx(List<Vector3> leftVerts)
        {
            var retIdx = 0;
            var maxDist = float.MinValue;
            foreach(var(vert, idx) in leftVerts.Select((vert, index) => (vert, index)))
            {
                var dist = Vector3.Distance(vert, Vector3.zero);
                if(dist > maxDist)
                {
                    retIdx = idx;
                    maxDist = dist;
                }
            }
            return retIdx;
        }

        private static Vector3 GetSerfDirection(int prevIdx, int targetIdx, int nextIdx, List<Vector3> leftVerts)
        {
            return Vector3.Cross(leftVerts[prevIdx] - leftVerts[targetIdx], leftVerts[nextIdx] - leftVerts[targetIdx]).normalized;
        }

        private static bool CheckPointInTriangle(int prevIdx, int targetIdx, int nextIdx, List<Vector3> leftVerts)
        {
            for(var checkIdx = 0; checkIdx < leftVerts.Count; checkIdx++)
            {
                if((checkIdx == prevIdx) || (checkIdx == targetIdx) || (checkIdx == nextIdx))
                {
                    continue;
                }
                var dir1 = GetSerfDirection(prevIdx, targetIdx, checkIdx, leftVerts);
                var dir2 = GetSerfDirection(targetIdx, nextIdx, checkIdx, leftVerts);
                var dir3 = GetSerfDirection(nextIdx, prevIdx, checkIdx, leftVerts);

                var isSameDir12 = (Vector3.Dot(dir1, dir2) > 0.0f);
                var isSameDir23 = (Vector3.Dot(dir2, dir3) > 0.0f);

                if(isSameDir12 && isSameDir23)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
