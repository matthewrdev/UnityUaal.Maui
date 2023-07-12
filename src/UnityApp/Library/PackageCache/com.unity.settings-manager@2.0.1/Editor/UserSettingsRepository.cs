namespace UnityEditor.SettingsManagement
{
    /// <summary>
    /// Represents a settings repository for user preferences.
    /// </summary>
    /// <seealso cref="UnityEditor.EditorPrefs"/>
    public class UserSettingsRepository : ISettingsRepository
    {
        static string GetEditorPrefKey<T>(string key)
        {
            return GetEditorPrefKey(typeof(T).FullName, key);
        }

        static string GetEditorPrefKey(string fullName, string key)
        {
            return fullName + "::" + key;
        }

        static void SetEditorPref<T>(string key, T value)
        {
            var k = GetEditorPrefKey<T>(key);

            if (typeof(T) == typeof(string))
                EditorPrefs.SetString(k, (string)(object)value);
            else if (typeof(T) == typeof(bool))
                EditorPrefs.SetBool(k, (bool)(object)value);
            else if (typeof(T) == typeof(float))
                EditorPrefs.SetFloat(k, (float)(object)value);
            else if (typeof(T) == typeof(int))
                EditorPrefs.SetInt(k, (int)(object)value);
            else
                EditorPrefs.SetString(k, ValueWrapper<T>.Serialize(value));
        }

        static T GetEditorPref<T>(string key, T fallback = default(T))
        {
            var k = GetEditorPrefKey<T>(key);

            if (!EditorPrefs.HasKey(k))
                return fallback;

            var o = (object)fallback;

            if (typeof(T) == typeof(string))
                o = EditorPrefs.GetString(k, (string)o);
            else if (typeof(T) == typeof(bool))
                o = EditorPrefs.GetBool(k, (bool)o);
            else if (typeof(T) == typeof(float))
                o = EditorPrefs.GetFloat(k, (float)o);
            else if (typeof(T) == typeof(int))
                o = EditorPrefs.GetInt(k, (int)o);
            else
                return ValueWrapper<T>.Deserialize(EditorPrefs.GetString(k));

            return (T)o;
        }

        /// <summary>
        /// Gets the <see cref="UnityEditor.SettingsScope">scope</see> this repository applies to.
        /// </summary>
        /// <value>Indicates that this is a <see cref="UnityEditor.SettingsScope.User"/> preference.</value>
        /// <seealso cref="ISettingsRepository.scope"/>
        public SettingsScope scope
        {
            get { return SettingsScope.User; }
        }

        /// <summary>
        /// Gets the identifying name for this repository.
        /// </summary>
        /// <value>User settings are named "EditorPrefs".</value>
        public string name
        {
            get { return "EditorPrefs"; }
        }

        /// <summary>
        /// Gets the full path to the file containing the serialized settings data.
        /// </summary>
        /// <remarks>This property returns an empty string.</remarks>
        /// <value>The location stored for this repository.</value>
        /// <seealso cref="ISettingsRepository.path"/>
        public string path
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Saves all settings to their serialized state.
        /// </summary>
        /// <seealso cref="ISettingsRepository.Save"/>
        public void Save()
        {
        }

        /// <summary>
        /// Sets a value for a settings entry with a matching key and type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="value">The value to set. This must be serializable.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        public void Set<T>(string key, T value)
        {
            SetEditorPref<T>(key, value);
        }

        /// <summary>
        /// Returns a value for a settings entry with a matching key and type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="fallback">Specify the value of type `T` to return if the entry can't be found.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <returns>The value matching both `key` and type `T`. If there was no match, this returns the `fallback` value.</returns>
        public T Get<T>(string key, T fallback = default(T))
        {
            return GetEditorPref<T>(key, fallback);
        }

        /// <summary>
        /// Determines whether this repository contains a settings entry that matches the specified key and is of type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <returns>True if a settings entry matches both `key` and type `T`; false if no entry is found.</returns>
        public bool ContainsKey<T>(string key)
        {
            return EditorPrefs.HasKey(GetEditorPrefKey<T>(key));
        }

        /// <summary>
        /// Removes a key-value pair from this settings repository. This method identifies the settings entry to remove
        /// by matching the specified key for a value of type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        public void Remove<T>(string key)
        {
            EditorPrefs.DeleteKey(GetEditorPrefKey<T>(key));
        }
    }
}
