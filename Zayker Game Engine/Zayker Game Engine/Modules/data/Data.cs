using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZEngine.Data
{
    class DataHandler : Core.Module
    {
        public DataHandler()
        {
            // The id must match the modules folder name!
            this.id = "data";
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
        }

        public static void Save(object obj, string path)
        {
            string jsonString = JsonConvert.SerializeObject(obj);
            File.WriteAllText(path, jsonString);
            Console.WriteLine("Saved " + obj.ToString() + " to " + path + ".");
        }

        public static T Load<T>(string path)
        {
            if (File.Exists(path))
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            else
                return default(T);
        }
    }
}
