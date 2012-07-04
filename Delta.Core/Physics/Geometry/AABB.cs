﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Delta.Physics.Geometry
{
    /// <summary>
    /// An Axis-Aligned Bounding Box. ie. it will not reflect rotations;
    /// </summary>
    public class AABB
    {
        public Vector2 Position;
        public int HalfWidth { get; protected set; }
        public int HalfHeight { get; protected set; }

        public AABB(int halfWidth, int halfHeight)
        {
            Position = default(Vector2);
            HalfWidth = halfWidth;
            HalfHeight = halfHeight;
        }

        public Vector2 Min
        {
            get
            {
                return Position - new Vector2(HalfWidth, HalfHeight);
            }
        }

        public Vector2 Max
        {
            get
            {
                return Position + new Vector2(HalfWidth, HalfHeight);
            }
        }

        public Vector2[] Vertices
        {
            get
            {
                return new Vector2[] {
                    Position + new Vector2(HalfWidth, -HalfHeight),
                    Position + new Vector2(HalfWidth, HalfHeight),
                    Position + new Vector2(-HalfWidth, HalfHeight),
                    Position + new Vector2(-HalfWidth, -HalfHeight),
                };
            }
        }
     
        /// <summary>
        /// A less expensive check before we waste cycles on a narrow phase detection.
        /// </summary>
        /// <returns>If the two polygons are about to intersect.</returns>
        public static bool TestOverlap(AABB a, AABB b)
        {
            // Exit with no intersection if separated along an axis
            if (a.Max.X < b.Min.X || a.Min.X > b.Max.X) return false;
            if (a.Max.Y < b.Min.Y || a.Min.Y > b.Max.Y) return false;
            // Overlapping on all axes means AABBs are intersecting
            return true;
        }

    }
}