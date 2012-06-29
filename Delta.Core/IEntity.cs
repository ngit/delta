﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Delta
{
    public interface IEntity
    {
        string ID { get; set; }
        bool IsVisible { get; set; }
        IEntityParent Parent { get; set; }
        float Order { get; set; }
        Vector2 Position { get; set; }
        Vector2 Size { get; set; }

        void LoadContent(ContentManager content);

        void InternalUpdate(GameTime gameTime);
        void InternalDraw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}