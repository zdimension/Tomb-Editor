﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using TombLib.Graphics;
using SharpDX.Toolkit.Graphics;
using TombEditor.Geometry;
using System.Windows.Forms;

namespace TombEditor
{
    public class Gizmo : BaseGizmo
    {
        private Editor _editor;

        public Gizmo(GraphicsDevice device, Effect effect)
            : base(device, effect)
        {
            _editor = Editor.Instance;
        }

        protected override bool DrawGizmo
        { 
            get
            {
                return _editor.SelectedObject is PositionBasedObjectInstance;
            }
        }

        protected override Vector3 Position
        {
            get
            {
                var obj = (PositionBasedObjectInstance)_editor.SelectedObject;
                return obj.Position + obj.Room.WorldPos;
            }
        }

        protected override void DoGizmoAction(Vector3 newPos, float angle, float scale)
        {
            switch (Action)
            {
                case GizmoAction.Translate:
                    EditorActions.MoveObject(_editor.SelectedObject as PositionBasedObjectInstance,
                                             newPos - _editor.SelectedObject.Room.WorldPos, Control.ModifierKeys);
                    break;
                case GizmoAction.Rotate:
                    angle = MathUtil.RadiansToDegrees(angle);

                    if (Axis == GizmoAxis.X)
                        EditorActions.RotateObject(_editor.SelectedObject, EditorActions.RotationAxis.X, angle);
                    else if (Axis == GizmoAxis.Y)
                        EditorActions.RotateObject(_editor.SelectedObject, EditorActions.RotationAxis.Y, angle);
                    else if (Axis == GizmoAxis.Z)
                        EditorActions.RotateObject(_editor.SelectedObject, EditorActions.RotationAxis.Roll, angle);

                    /*if (_editor.SelectedObject is IRotateableYXRoll)
                    {
                        //EditorActions.RotateObject(_editor.SelectedObject, EditorActions.RotationAxis.Y, angle);
                    }
                    else if (_editor.SelectedObject is IRotateableYX)
                    {
                        //EditorActions.RotateObject(_editor.SelectedObject, EditorActions.RotationAxis.Y, angle);
                    }
                    else
                    {
                        angle = MathUtil.RadiansToDegrees(angle);
                        EditorActions.RotateObject(_editor.SelectedObject, EditorActions.RotationAxis.Y, angle);
                    }*/
                    break;
                case GizmoAction.Scale:
                    EditorActions.ScaleObject(_editor.SelectedObject as IScaleable, scale, Control.ModifierKeys);
                    break;
            }
        }

        protected override float CentreCubeSize => _editor.Configuration.Gizmo_CenterCubeSize;

        protected override float TranslationSphereSize => _editor.Configuration.Gizmo_TranslationSphereSize;

        protected override float Size => _editor.Configuration.Gizmo_Size;

        protected override float ScaleCubeSize => _editor.Configuration.Gizmo_ScaleCubeSize;

        protected override bool SupportScale => _editor.SelectedObject is IScaleable;

        protected override bool SupportRotationY => _editor.SelectedObject is IRotateableY;

        protected override bool SupportRotationYX => _editor.SelectedObject is IRotateableYX;

        protected override bool SupportRotationYXRoll => _editor.SelectedObject is IRotateableYXRoll;
    }
}
