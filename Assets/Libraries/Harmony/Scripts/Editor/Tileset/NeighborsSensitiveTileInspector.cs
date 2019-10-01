﻿using UnityEditor;

namespace Harmony
{
    [CustomEditor(typeof(NeighborsSensitiveTile), true)]
    public class NeighborsSensitiveTileInspector : BaseInspector
    {
        private GridProperty sprites;

        protected override void Initialize()
        {
            sprites = GetGridProperty("sprites");
        }

        protected override void Draw()
        {
            Initialize();

            DrawPropertyWithLabel(sprites);
        }
    }
}