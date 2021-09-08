using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.ECS
{
    class Entity
    {
        public string name = "Entity";
        private List<Component> _components;

        public Entity()
        {
            _components = new List<Component>();
        }

        public void Update(double deltaTime)
        {
            foreach (Component component in _components)
            {
                component.Update(deltaTime);
            }
        }

        public T AddComponent<T> () where T : Component, new()
        {
            T component = new T();
            component.entity = this;
            component._init();
            _components.Add(component);
            return component;
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (Component component in _components)
            {
                if (component.GetType() == typeof(T))
                    return (T)component;
            }
            return null;
        }

    }
}
