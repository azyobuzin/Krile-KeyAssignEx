using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Acuerdo.Plugin;

namespace KeyAssignEx
{
    [Export(typeof(IPlugin))]
    public class EntryPoint : IPlugin
    {
        public string Name
        {
            get { return "KeyAssignEx"; }
        }

        public Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        private bool changed = false;

        public void Loaded()
        {
            KeyAssignExCore.Init();
        }

        public IConfigurator ConfigurationInterface
        {
            get { return null; }
        }
    }
}
