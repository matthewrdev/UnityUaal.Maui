namespace UnityEditor.SettingsManagement
{
    /// <summary>
    /// An interface that represents a settings repository, which is responsible for implementing the saving and loading of values.
    /// </summary>
    public interface ISettingsRepository
    {
        /// <summary>
        /// Implement this property to get the <see cref="SettingsScope"/> this repository applies to.
        /// </summary>
        /// <value>
        /// Indicates whether this is a <see cref="UnityEditor.SettingsScope.Project"/> setting
        /// or a <see cref="UnityEditor.SettingsScope.User"/> preference.
        /// </value>
        SettingsScope scope { get; }

        /// <summary>
        /// Implement this property to get the name to identify this repository.
        /// </summary>
        /// <value>The bare filename of this repository.</value>
        string name { get; }

        /// <summary>
        /// Implement this property to get the file path to the serialized settings data.
        /// </summary>
        /// <value>Full path to the JSON file containing the settings data.</value>
        string path { get; }

        /// <summary>
        /// Implement this method to save all settings to their serialized state.
        /// </summary>
        void Save();

        /// <summary>
        /// Implement this method to set a value for a settings entry with a matching key and type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="value">The value to set. Must be serializable.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        void Set<T>(string key, T value);

        /// <summary>
        /// Implement this method to get a value for a settings entry with a matching key and type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="fallback">Specify the value of type `T` to return if the entry can't be found.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <returns>The value matching both `key` and type `T`. If there was no match, this returns the `fallback` value.</returns>
        T Get<T>(string key, T fallback = default(T));

        /// <summary>
        /// Implement this method to evaluate whether the repository contains a settings entry that matches the specified key and is of type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <returns>True if a settings entry matches both `key` and type `T`; false if no entry is found.</returns>
        bool ContainsKey<T>(string key);

        /// <summary>
        /// Implement this method to remove a key-value pair from the settings repository. This method identifies the settings entry to remove
        /// by matching the specified key for a value of type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        void Remove<T>(string key);
    }
}
