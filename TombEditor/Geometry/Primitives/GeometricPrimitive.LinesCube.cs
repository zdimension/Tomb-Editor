﻿// Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------------------------------------------
// The following code is a port of DirectXTk http://directxtk.codeplex.com
// -----------------------------------------------------------------------------
// Microsoft Public License (Ms-PL)
//
// This license governs use of the accompanying software. If you use the 
// software, you accept this license. If you do not accept the license, do not
// use the software.
//
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and 
// "distribution" have the same meaning here as under U.S. copyright law.
// A "contribution" is the original software, or any additions or changes to 
// the software.
// A "contributor" is any person that distributes its contribution under this 
// license.
// "Licensed patents" are a contributor's patent claims that read directly on 
// its contribution.
//
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the 
// license conditions and limitations in section 3, each contributor grants 
// you a non-exclusive, worldwide, royalty-free copyright license to reproduce
// its contribution, prepare derivative works of its contribution, and 
// distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license
// conditions and limitations in section 3, each contributor grants you a 
// non-exclusive, worldwide, royalty-free license under its licensed patents to
// make, have made, use, sell, offer for sale, import, and/or otherwise dispose
// of its contribution in the software or derivative works of the contribution 
// in the software.
//
// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any 
// contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that 
// you claim are infringed by the software, your patent license from such 
// contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all 
// copyright, patent, trademark, and attribution notices that are present in the
// software.
// (D) If you distribute any portion of the software in source code form, you 
// may do so only under this license by including a complete copy of this 
// license with your distribution. If you distribute any portion of the software
// in compiled or object code form, you may only do so under a license that 
// complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The
// contributors give no express warranties, guarantees or conditions. You may
// have additional consumer rights under your local laws which this license 
// cannot change. To the extent permitted under your local laws, the 
// contributors exclude the implied warranties of merchantability, fitness for a
// particular purpose and non-infringement.


using TombEditor.Geometry;

namespace SharpDX.Toolkit.Graphics
{
    public partial class GeometricPrimitive
    {
        /// <summary>
        /// A cube has six faces, each one pointing in a different direction.
        /// </summary>
        public struct LinesCube
        {
            private const int CubeFaceCount = 6;

            private static readonly Vector3[] faceNormals = new Vector3[CubeFaceCount]
                {
                    new Vector3(0, 0, 1),
                    new Vector3(0, 0, -1),
                    new Vector3(1, 0, 0),
                    new Vector3(-1, 0, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(0, -1, 0),
                };

            private static readonly Vector2[] textureCoordinates = new Vector2[4]
                {
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                };

            /// <summary>
            /// Creates a cube with six faces each one pointing in a different direction.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="size">The size.</param>
            /// <param name="toLeftHanded">if set to <c>true</c> vertices and indices will be transformed to left handed. Default is true.</param>
            /// <returns>A cube.</returns>
            public static GeometricPrimitive New(GraphicsDevice device)
            {
                EditorVertex v1 = new TombEditor.Geometry.EditorVertex();
                v1.Position = new Vector4(-128.0f, -128.0f, -128.0f, 1.0f);
                v1.UV = Vector2.Zero;

                EditorVertex v2 = new TombEditor.Geometry.EditorVertex();
                v2.Position = new Vector4(128.0f, -128.0f, -128.0f, 1.0f);
                v2.UV = Vector2.Zero;

                EditorVertex v3 = new TombEditor.Geometry.EditorVertex();
                v3.Position = new Vector4(128.0f, -128.0f, 128.0f, 1.0f);
                v3.UV = Vector2.Zero;

                EditorVertex v4 = new TombEditor.Geometry.EditorVertex();
                v4.Position = new Vector4(-128.0f, -128.0f, 128.0f, 1.0f);
                v4.UV = Vector2.Zero;

                EditorVertex v5 = new TombEditor.Geometry.EditorVertex();
                v5.Position = new Vector4(-128.0f, 128.0f, -128.0f, 1.0f);
                v5.UV = Vector2.Zero;

                EditorVertex v6 = new TombEditor.Geometry.EditorVertex();
                v6.Position = new Vector4(128.0f, 128.0f, -128.0f, 1.0f);
                v6.UV = Vector2.Zero;

                EditorVertex v7 = new TombEditor.Geometry.EditorVertex();
                v7.Position = new Vector4(128.0f, 128.0f, 128.0f, 1.0f);
                v7.UV = Vector2.Zero;

                EditorVertex v8 = new TombEditor.Geometry.EditorVertex();
                v8.Position = new Vector4(-128.0f, 128.0f, 128.0f, 1.0f);
                v8.UV = Vector2.Zero;

                var vertices = new EditorVertex[] { v1, v2, v3, v4, v5, v6, v7 };
                var indices = new short[]
                {
					4, 5, 5, 1, 1, 0, 0, 4,
					5, 6, 6, 2, 2, 1, 1, 5,
					2, 6, 6, 7, 7, 3, 3, 2,
					7, 4, 4, 0, 0, 3, 3, 7,
					7, 6, 6, 5, 5, 4, 4, 7,
					0, 1, 1, 2, 2, 3, 3, 0
                };
				
                // Create the primitive object.
                return new GeometricPrimitive(device, vertices, indices, false) { Name = "Cube" };
            }
        }
    }
}