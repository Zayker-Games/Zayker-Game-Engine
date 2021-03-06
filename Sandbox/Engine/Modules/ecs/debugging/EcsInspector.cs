using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Debugging
{
    class EcsInspector : Container
    {
        public EcsInspector(GuiInstance debugger)
        {
            base.Init(debugger);
        }

        public override void Update(float dt)
        {
            ImGui.Begin("ECS Inspector##" + id);

            ImGui.BeginChild("scrolling");
            foreach (ECS.Entity entity in ((ECS.ECSModule)Core.ModuleSystem.GetModuleById("ecs")).GetEntities())
            {
                if(ImGui.Button(entity.name))
                {
                    EntityInspector entityInspector = new EntityInspector(debugger);
                    entityInspector.entity = entity;
                    entityInspector.opened = true;
                    debugger.AddContainer(entityInspector);
                }
            }
            ImGui.EndChild();
            ImGui.End();
        }
    }

    class EntityInspector : Container
    {
        public ZEngine.ECS.Entity entity;

        public EntityInspector(GuiInstance debugger)
        {
            base.Init(debugger);
            temporary = true;
        }

        public override void Update(float dt)
        {
            ImGui.SetNextWindowSizeConstraints(new System.Numerics.Vector2(300, 300), new System.Numerics.Vector2(800, 1000));
            ImGui.Begin("Entity Inspector: " + entity.name + "##" + id, ref opened);

            ImGui.BeginChild("scrolling");

            foreach (ECS.Component component in entity.GetComponents())
            {
                component.DrawInspector();
                ImGui.Separator();
            }

            ImGui.EndChild();
            ImGui.End();
        }
    }
}
