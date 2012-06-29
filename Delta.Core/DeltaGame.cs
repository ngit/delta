﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Delta
{
    public class DeltaGame : Game //just a name holder
    {
        bool _isInitialized = false;
        ResourceContentManager _embedded = null;

        public DeltaGame()
            : base()
        {
#if DEBUG
            IsMouseVisible = true;
#endif
            _embedded = new ResourceContentManager(Services, EmbeddedContent.ResourceManager);
            base.Content.RootDirectory = "Content";
            G.Setup(this, new Rectangle(0, 0, 1280, 720));
        }

        protected override void LoadContent()
        {
            G.LoadContent(this, _embedded);
            base.LoadContent();
            GC.Collect(); // force a collection after content is loaded.
            ResetElapsedTime(); // avoid the update loop trying to play catch-up
        }

        void InternalInitialize()
        {
            _isInitialized = true;
            LateInitialize();
        }

        protected virtual void LateInitialize()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // HANLDE WITH CARE. LaterInitializes need a preprare audio engine!!
            G.SimpleEffect.Time = (float)gameTime.TotalGameTime.TotalMilliseconds;
            G.Input.Update(gameTime);
            G.Audio.Update(gameTime);
            G.Physics.Simulate((float)gameTime.ElapsedGameTime.TotalSeconds);
            if (!_isInitialized)
                InternalInitialize();
            G.World.InternalUpdate(gameTime);
            G.UI.InternalUpdate(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            G.World.InternalDraw(gameTime, G.SpriteBatch);
            G.UI.InternalDraw(gameTime, G.SpriteBatch);
        }

    }
}