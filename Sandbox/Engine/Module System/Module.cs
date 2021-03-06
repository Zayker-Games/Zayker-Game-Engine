using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Core
{
    /// <summary>
    /// An Engine-Module adds a set of features to the engine, if it is enabled for the current project. 
    /// </summary>
    class Module
    {
        public string id;
        public bool isEnabled;
        /// <summary>
        /// List of other modules, that this module needs to function. 
        /// </summary>
        public List<string> dependencies;

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

        public virtual void Update(double deltaTime)
        {

        }

        /// <summary>
        /// Returns the path to this modules directory.
        /// </summary>
        public string GetDirectory()
        {
            if (false && System.IO.Directory.GetCurrentDirectory().Contains("netcoreapp3.1"))
            {
                return System.IO.Directory.GetCurrentDirectory().Substring(0, System.IO.Directory.GetCurrentDirectory().LastIndexOf(@"bin\")) + @"Modules\" + id + @"\";
            }
            else
            {
                return System.IO.Directory.GetCurrentDirectory() + @"\Engine\Modules\" + id + @"\";
            }
        }
    }
}
