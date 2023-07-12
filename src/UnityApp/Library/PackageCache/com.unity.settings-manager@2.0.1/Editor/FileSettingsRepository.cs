using System;
using System.IO;
using UnityEngine;

namespace UnityEditor.SettingsManagement
{
    /// <summary>
    /// Represents a settings repository that stores data serialized to a JSON file.
    /// </summary>
    [Serializable]
    public class FileSettingsRepository : ISettingsRepository
    {
        /// <summary>
        /// Location of where the package settings are saved under the `ProjectSettings` directory.
        /// </summary>
        /// <returns>The folder where package settings are saved under the `ProjectSettings` directory.</returns>
        protected const string k_PackageSettingsDirectory = "ProjectSettings/Packages";

        /// <summary>
        /// Location of where the package settings are saved under the `UserSettings` directory.
        /// </summary>
        /// <returns>Per-project user settings directory. </returns>
        protected const string k_UserProjectSettingsDirectory = "UserSettings/Packages";

        const bool k_PrettyPrintJson = true;

        bool m_Initialized;
        string m_Path;
        [SerializeField]
        SettingsDictionary m_Dictionary = new SettingsDictionary();
        Hash128 m_JsonHash;

        /// <summary>
        /// Initializes and returns an instance of the FileSettingsRepository
        /// with the serialized data location set to the specified path.
        /// </summary>
        /// <param name="path">The project-relative path to save settings to.</param>
        public FileSettingsRepository(string path)
        {
            m_Path = path;
            m_Initialized = false;
            AssemblyReloadEvents.beforeAssemblyReload += Save;
            EditorApplication.quitting += Save;
        }

        void Init()
        {
            if (m_Initialized)
                return;

            m_Initialized = true;

            if (TryLoadSavedJson(out string json))
            {
                m_Dictionary = null;
                m_JsonHash = Hash128.Compute(json);
                EditorJsonUtility.FromJsonOverwrite(json, this);
            }

            if (m_Dictionary == null)
                m_Dictionary = new SettingsDictionary();
        }

        /// <summary>
        /// Sets the <see cref="SettingsScope"/> this repository applies to.
        /// </summary>
        /// <remarks>
        /// By default, this repository implementation is relevant to the Project scope, but any implementations
        /// that override this method can choose to store this serialized data at a user scope instead.
        /// </remarks>
        /// <value>
        /// <see cref="SettingsScope.Project"/>, meaning that this setting applies to project settings (the default);
        /// or <see cref="SettingsScope.User"/>, meaning that this setting applies to user preferences.
        /// </value>
        /// <seealso cref="ISettingsRepository.scope"/>
        public virtual SettingsScope scope => SettingsScope.Project;

        /// <summary>
        /// Gets the full path to the file containing the serialized settings data.
        /// </summary>
        /// <value>The location stored for this repository.</value>
        /// <seealso cref="ISettingsRepository.path"/>
        public string path
        {
            get { return m_Path; }
        }

        /// <summary>
        /// Sets the name of file containing the serialized settings data.
        /// </summary>
        /// <value>The bare filename of the settings file.</value>
        public string name => Path.GetFileNameWithoutExtension(path);

        /// <summary>
        /// Loads the JSON file that stores the values for this settings object.
        /// </summary>
        /// <param name="json">The full path to the JSON file to load.</param>
        /// <returns>True if the file exists; false if it doesn't.</returns>
        public bool TryLoadSavedJson(out string json)
        {
            json = string.Empty;
            if (!File.Exists(path))
                return false;
            json = File.ReadAllText(path);
            return true;
        }

        /// <summary>
        /// Saves all settings to their serialized state.
        /// </summary>
        /// <seealso cref="ISettingsRepository.Save"/>
        public void Save()
        {
            Init();

            if (!File.Exists(path))
            {
                var directory = Path.GetDirectoryName(path);

                if (string.IsNullOrEmpty(directory))
                {
                    Debug.LogError(
                        $"Settings file {name} is saved to an invalid path: {path}. Settings will not be saved.");
                    return;
                }

                Directory.CreateDirectory(directory);
            }

            string json = EditorJsonUtility.ToJson(this, k_PrettyPrintJson);

            // While unlikely, a hash collision is possible. Always test the actual saved contents before early exit.
            if (m_JsonHash == Hash128.Compute(json)
                && TryLoadSavedJson(out string existing)
                && existing.Equals(json))
                return;

#if UNITY_2019_3_OR_NEWER
            // AssetDatabase.IsOpenForEdit can be a very slow synchronous blocking call when Unity is connected to
            // Perforce Version Control. Especially if it's called repeatedly with every EditorGUI redraw.
            if (File.Exists(path) && !AssetDatabase.IsOpenForEdit(path))
            {
                if (!AssetDatabase.MakeEditable(path))
                {
                    Debug.LogWarning($"Could not save package settings to {path}");
                    return;
                }
            }
#endif

            try
            {
                m_JsonHash = Hash128.Compute(json);
                File.WriteAllText(path, json);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.LogWarning($"Could not save package settings to {path}");
            }
        }

        /// <summary>
        /// Sets a value for a settings entry with a matching key and type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="value">The value to set. This value must be serializable.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <seealso cref="ISettingsRepository.Set{T}"/>
        public void Set<T>(string key, T value)
        {
            Init();
            m_Dictionary.Set<T>(key, value);
        }

        /// <summary>
        /// Returns a value with key of type `T`, or the fallback value if no matching key is found.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="fallback">Specify the value of type `T` to return if the entry can't be found.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <returns>The settings value if a match is found; otherwise, it returns the default (fallback) value.</returns>
        /// <seealso cref="ISettingsRepository.Get{T}"/>
        public T Get<T>(string key, T fallback = default(T))
        {
            Init();
            return m_Dictionary.Get<T>(key, fallback);
        }

        /// <summary>
        /// Determines whether this repository contains a settings entry that matches the specified key and is of type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <returns>True if a match is found for both key and type; false if no entry is found.</returns>
        /// <seealso cref="ISettingsRepository.ContainsKey{T}"/>
        public bool ContainsKey<T>(string key)
        {
            Init();
            return m_Dictionary.ContainsKey<T>(key);
        }

        /// <summary>
        /// Removes a key-value pair from the settings repository. This method identifies the settings entry to remove
        /// by matching the specified key for a value of type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <seealso cref="ISettingsRepository.Remove{T}"/>
        public void Remove<T>(string key)
        {
            Init();
            m_Dictionary.Remove<T>(key);
        }
    }
}
