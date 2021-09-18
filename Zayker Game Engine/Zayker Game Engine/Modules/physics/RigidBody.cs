using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Physics
{
    class RigidBody : ZEngine.ECS.Component
    {
        public Math.Vector velocity = new Math.Vector();
        public float mass = 1f;

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            ZEngine.ECS.Components.Transform transform = entity.GetComponent<ZEngine.ECS.Components.Transform>();
            velocity += new Math.Vector(0f, -1f, 0f) * (float)deltaTime;
            transform.position += velocity * (float)deltaTime;
        }

        public override void DrawInspector()
        {
            // Here we have to create a temporary variable, which is very stupid. I'll have to change that!
            System.Numerics.Vector3 velocityReference = (System.Numerics.Vector3)velocity;
            ImGuiNET.ImGui.InputFloat3("Velocity", ref velocityReference);
            velocity = (Math.Vector)velocityReference;
        }

        public void AddForce(Math.Vector force)
        {
            Math.Vector acceleration = force / mass;
            velocity += acceleration;
        }
    }
}
