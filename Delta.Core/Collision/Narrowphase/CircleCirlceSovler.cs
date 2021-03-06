﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Delta.Collision
{
    class CircleCircleSolver : ICollisionSolver
    {

        public bool IsSolveable(CollisionBody colA, CollisionBody colB)
        {
            // we the assume calling function has checked for null shapes.
            if (colA.Shape is Circle && colB.Shape is Circle) return true;
            else return false;
        }

        public CollisionResult SolveCollision(CollisionBody colA, CollisionBody colB)
        {
            Circle circleA = (Circle)colA.Shape;
            Circle circleB = (Circle)colB.Shape;

            Vector2 distance = colA.Position - colB.Position;
            float abSize = circleA.Radius + circleB.Radius;
            float penetration = abSize - distance.Length();
            Vector2 mtv = default(Vector2);
            if (penetration <= 0)
            {
                return CollisionResult.NoCollision;
            }
            mtv = distance;
            Vector2Extensions.SafeNormalize(ref mtv);
            // the distance vector determines the direction of movement. the distance and mtv
            // should always oppose each other to repel collisions.
            if (Vector2.Dot(distance, mtv) < 0f)
                mtv = -mtv;
            return new CollisionResult()
            {
                Us = colA,
                Them = colB,
                CollisionResponse = penetration * mtv,
                IsColliding = true
            };
        }

    }
}
