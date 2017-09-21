﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Toolkit.Graphics;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;
using Texture2D = SharpDX.Toolkit.Graphics.Texture2D;
using SharpDX;

namespace TombLib.Graphics
{
    public abstract class Mesh<T> : GraphicsResource, IRenderableObject where T : struct, IVertex
    {
        public int BaseIndex { get; set; }
        public int NumIndices { get; set; }
        public List<T> Vertices { get; set; } = new List<T>();
        public List<int> Indices { get; set; } = new List<int>();
        public Buffer<T> VertexBuffer { get; protected set; }
        public Buffer IndexBuffer { get; protected set; }

        public List<Submesh> SubMeshes { get; set; }
        public BoundingSphere BoundingSphere { get; set; }
        public BoundingBox BoundingBox { get; set; }
        
        protected Buffer<T> _vb;
        protected Buffer _ib;

        public Mesh(GraphicsDevice device, string name)
            : base(device, name)
        {
            SubMeshes = new List<Submesh>();
        }

        public abstract void BuildBuffers();
    }
}
