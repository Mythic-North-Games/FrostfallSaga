using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrostfallSaga.Procedural
{
    [Serializable]
    public class VoronoiGenerator
    {
        private int _gridHeight;
        private int _gridWidth;
        private List<Vector2> _sites;

        public VoronoiGenerator(List<Vector2> sites, int width, int height)
        {
            _sites = sites;
            _gridWidth = width;
            _gridHeight = height;
        }

        public int GetCloserSite(float x, float y)
        {
            float minDistance = float.MaxValue;
            int closestSiteIndex = -1;

            for (int i = 0; i < _sites.Count; i++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), _sites[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestSiteIndex = i;
                }
            }

            return closestSiteIndex;
        }
    }
}