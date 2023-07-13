using System;

namespace UnityEditor.SettingsManagement
{
    /// <summary>
    /// Represents a settings repository that stores data local to a Unity project.
    /// The settings data is serialized to a JSON file.
    /// </summary>
    [Serializable]
    public class ProjectUserSettings : FileSettingsRepository
    {
        /// <summary>
        /// Initializes and returns an instance of the ProjectUserSettings repository with the
        /// serialized data location set to a path defined by the specified `package` and
        /// `name` values relative to the `UserSettings` directory. For example:
        /// `MyUnityProject/UserSettings/Packages/com.example.my-package/Settings.json`.
        /// </summary>
        /// <param name="package">The name of the package to store the serialized data under.</param>
        /// <param name="name">The base filename to use for the serialized data location (defaults to "Settings").</param>
        public ProjectUserSettings(string package, string name = "Settings") : base(GetUserSettingsPath(package, name))
        {
        }

        /// <summary>
        /// Builds and returns a path for a settings file relative to the calling assembly's package directory.
        /// This method constructs the location from the specified `package` and (filename) `name` under the `UserSettings` folder.
        /// </summary>
        /// <param name="package">The name of the package requesting this setting.</param>
        /// <param name="name">An optional name for the settings file. Default is "Settings."</param>
        /// <returns>A package-scoped path to the settings file inside the project's `UserSettings` folder.</returns>
        public static string GetUserSettingsPath(string package, string name)
        {
            return string.Format("{0}/{1}/{2}.json", k_UserProjectSettingsDirectory, package, name);
        }
    }
}
