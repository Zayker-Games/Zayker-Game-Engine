using System;
using System.Collections.Generic;
using System.Text;

namespace Zayker_Game_Engine.Core.EngineModules
{
    /// <summary>
    /// An Engine-Module adds a set of features to the engine, if it is enabled for the current project. 
    /// </summary>
    class EngineModule
    {
        public string id;

        public bool isEnabled;

        /// <summary>
        /// List of other modules, that this module needs to function. 
        /// </summary>
        protected List<EngineModule> dependencies;

        /// <summary>
        /// If this is false, the file will not be copied to the build directory. This can be used for editor modules. 
        /// </summary>
        protected bool includeInBuild = true;

        public virtual void OnEnable()
        {

        }

        public virtual void OnDisable()
        {

        }

        public virtual void Update(float deltaTime)
        {

        }
    }
}
