﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Delta;
using Delta.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Delta.Collision;
using Delta.Collision.Geometry;

namespace Delta.Examples.Entities
{
    public class Obstacle : CollideableEntity
    {
        const float SPEED = 50;

        public bool AutoRotate, MoveAndRotate;


        public Vector2 Input { get; set; }

        public Vector2 Velocity;


        public Obstacle()
        {
            Collider = new Collider() 
            {
                Tag = this,
                Mass = 0f,
                Geom = new OBB(10, 10),
                OnCollision = OnCollision,
                BeforeCollision = BeforeCollision
            };
        }

        protected override void LateInitialize()
        {
            base.LateInitialize();

            Velocity = Vector2Extensions.RandomDirection() * SPEED;
        }

        public void SwitchBody()
        {
            if (Polygon is Circle)
            {
                Polygon = new OBB(10, 10);
            }
            else if (Polygon is OBB)
            {
                Polygon = new Circle(5);
            }
        }

        protected override void LightUpdate(GameTime gameTime)
        {
            if (AutoRotate || MoveAndRotate)
                Rotation += 0.01f;
            if (MoveAndRotate)
                Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.LightUpdate(gameTime);
        }

        public override bool OnCollision(Collider them, Vector2 normal)
        {
            Velocity = -Velocity;
            return true;
        }

    }
}
