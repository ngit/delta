﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Delta.Physics
{
   

    public delegate bool OnCollisionEventHandler(Entity them, Vector2 normal);

    public delegate void AfterCollisionEventHandler(Entity them, Vector2 normal);

    public delegate void OnSeparationEventHandler(Entity them);

}