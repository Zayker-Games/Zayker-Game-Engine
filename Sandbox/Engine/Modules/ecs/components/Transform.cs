using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.ECS.Components
{
    class Transform : Component
    {
        /// <summary>
        /// Rotation relative to parent. 
        /// </summary>
        public Math.Quaternion localRotation
        {
            get
            {
                return _localRotation;
            } 
            set
            {
                _localRotation = value;
                _localEulerAngles = _localRotation.GetEulerAngles();
            }
        }
        private Math.Quaternion _localRotation = new Math.Quaternion();

        /// <summary>
        /// Euler angles relative to parents. 
        /// </summary>
        public Math.Vector localEulerAngles
        {
            get
            {
                return _localEulerAngles;
            }
            set
            {
                _localEulerAngles = value;
                _localRotation = Math.Quaternion.FromEulerAngles(_localEulerAngles);
            }
        }
        private Math.Vector _localEulerAngles = new Math.Vector();

        public Math.Vector forward
        {
            get
            {
                return _localRotation * Math.Vector.Forwards;
            }
        }

        public Math.Vector position = new Math.Vector(0f, 0f, 0f);
        public Math.Vector scale = new Math.Vector(1f, 1f, 1f);

        public override void DrawInspector()
        {
            // Here we have to create a temporary variable, which is very stupid. I'll have to change that!
            System.Numerics.Vector3 positionReference = (System.Numerics.Vector3)position;
            ImGuiNET.ImGui.InputFloat3("Position", ref positionReference);
            entity.GetComponent<Transform>().position = (Math.Vector)positionReference;
        }
    }
}
