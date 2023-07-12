using System;

namespace UnityEditor.SettingsManagement
{
    /// <summary>
    /// A settings repository that stores data local to a Unity project relative to a package.
    /// The settings data is serialized to a JSON file.
    /// </summary>
    [Serializable]
    public sealed class PackageSettingsRepository : FileSettingsRepository
    {
        /// <summary>
        /// Initializes and returns an instance of the PackageSettingsRepository with the
        /// serialized data location set to a path defined by the specified `package` and
        /// `name` values relative to the `ProjectSettings` directory. For example:
        /// `MyUnityProject/ProjectSettings/Packages/com.example.my-package/Settings.json`.
        /// </summary>
        /// <param name="package">The name of the package to store the serialized data under.</param>
        /// <param name="name">The base filename to use for the serialized data location.</param>
        public PackageSettingsRepository(string package, string name) : base(GetSettingsPath(package, name))
        {
        }

        // Cannot call FindFromAssembly from a constructor or field initializer
        //        static string CreateSettingsPath(Assembly assembly, string name)
        //        {
        //            var info = PackageManager.PackageInfo.FindForAssembly(assembly);
        //            return string.Format("{0}/{1}/{2}.json", k_PackageSettingsDirectory, info.name, name);
        //        }

        /// <summary>
        /// Builds and returns a path for a settings file relative to the calling assembly's package directory.
        /// This method constructs the location from the specified `package` and (filename) `name` under the `ProjectSettings` folder.
        /// </summary>
        /// <param name="packageName">The name of the package requesting this setting.</param>
        /// <param name="name">An optional name for the settings file. Default is "Settings."</param>
        /// <returns>A package-scoped path to the settings file inside the project's `ProjectSettings`.</returns>
        public static string GetSettingsPath(string packageName, string name = "Settings")
        {
            return string.Format("{0}/{1}/{2}.json", k_PackageSettingsDirectory, packageName, name);
        }
    }
}
