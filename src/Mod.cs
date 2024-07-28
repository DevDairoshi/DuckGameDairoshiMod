using System.Reflection;

// The title of your mod, as displayed in menus
[assembly: AssemblyTitle("DairoshiMod")]

// The author of the mod
[assembly: AssemblyCompany("Dairoshi")]

// The description of the mod
[assembly: AssemblyDescription("Just a weapon pack")]

// The mod's version
[assembly: AssemblyVersion("1.4.0.0")]

namespace DuckGame.DairoshiMod
{
    public class DairoshiMod : Mod
    {
        // The mod's priority; this property controls the load order of the mod.
        public override Priority priority
        {
            get { return base.priority; }
        }

        // This function is run before all mods are finished loading.
        protected override void OnPreInitialize()
        {
            base.OnPreInitialize();
        }

        // This function is run after all mods are loaded.
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();
        }
    }
}
