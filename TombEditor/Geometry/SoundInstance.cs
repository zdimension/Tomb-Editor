﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TombEditor.Geometry
{
    public class SoundInstance : IObjectInstance
    {
        public short SoundID { get; set; }

        public short Flags { get; set; }

        public SoundInstance(int id, short room) : base(ObjectInstanceType.Sound, id, room)
        {

        }

        public override IObjectInstance Clone()
        {
            SoundInstance instance = new SoundInstance(0, Room);

            instance.X = X;
            instance.Y = Y;
            instance.Z = Z;
            instance.OCB = OCB;
            instance.Rotation = Rotation;
            instance.Invisible = Invisible;
            instance.ClearBody = ClearBody;
            instance.Bits[0] = Bits[0];
            instance.Bits[1] = Bits[1];
            instance.Bits[2] = Bits[2];
            instance.Bits[3] = Bits[3];
            instance.Bits[4] = Bits[4];
            instance.Type = Type;

            instance.SoundID = SoundID;
            instance.Flags = Flags;
            
            return instance;
        }
    }
}
