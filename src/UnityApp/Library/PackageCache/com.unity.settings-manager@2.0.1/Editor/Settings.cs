using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.SettingsManagement
{
    /// <summary>
    /// Represents a collection of objects that implement <see cref="ISettingsRepository"/>.
    /// </summary>
    public sealed class Settings
    {
        ISettingsRepository[] m_SettingsRepositories;

        /// <summary>
        /// Called prior to when an instance of <see cref="ISettingsRepository"/> serializes its current state.
        /// </summary>
        public event Action beforeSettingsSaved;

        /// <summary>
        /// Called immediately after an instance of <see cref="ISettingsRepository"/> serializes its current state.
        /// </summary>
        public event Action afterSettingsSaved;

        Settings()
        {
        }

        /// <summary>
        /// Creates a new Settings instance with a <see cref="UserSettingsRepository"/> and <see cref="PackageSettingsRepository"/>.
        /// </summary>
        /// <param name="package">The package name, such as `com.example.my-package`.</param>
        /// <param name="settingsFileName">The name of the settings file. The default value is `Settings`.</param>
        public Settings(string package, string settingsFileName = "Settings")
        {
            m_SettingsRepositories = new ISettingsRepository[]
            {
                new PackageSettingsRepository(package, settingsFileName),
                new UserSettingsRepository()
            };
        }

        /// <summary>
        /// Creates a new Settings instance with a collection of objects that implement <see cref="ISettingsRepository"/>.
        /// </summary>
        /// <param name="repositories">The repositories to populate the Settings instance with.</param>
        public Settings(IEnumerable<ISettingsRepository> repositories)
        {
            m_SettingsRepositories = repositories.ToArray();
        }

        /// <summary>
        /// Finds and returns a settings repository that matches the specified scope.
        /// </summary>
        /// <param name="scope">The <see cref="SettingsScope">scope</see> of the settings repository to match.</param>
        /// <returns>
        /// An <see cref="ISettingsRepository"/> instance that implements the requested scope; or null if no
        /// matching repository is found.
        /// </returns>
        public ISettingsRepository GetRepository(SettingsScope scope)
        {
            foreach (var repo in m_SettingsRepositories)
                if (repo.scope == scope)
                    return repo;
            return null;
        }

        /// <summary>
        /// Finds and returns a settings repository that matches the specified scope and name.
        /// </summary>
        /// <param name="scope">The <see cref="SettingsScope">scope</see> of the settings repository to match.</param>
        /// <param name="name">The name of the <see cref="ISettingsRepository"/> to match.</param>
        /// <returns>
        /// An <see cref="ISettingsRepository"/> instance that implements the specified scope and matches the name; or
        /// null if no matching repository is found.
        /// </returns>
        public ISettingsRepository GetRepository(SettingsScope scope, string name)
        {
            foreach (var repo in m_SettingsRepositories)
                if (repo.scope == scope && string.Equals(repo.name, name))
                    return repo;
            return null;
        }

        /// <summary>
        /// Serializes the state of all settings repositories.
        /// </summary>
        public void Save()
        {
            if (beforeSettingsSaved != null)
                beforeSettingsSaved();

            foreach (var repo in m_SettingsRepositories)
                repo.Save();

            if (afterSettingsSaved != null)
                afterSettingsSaved();
        }

        /// <summary>
        /// Sets a value for a settings entry with a matching key and type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="value">The value to set. This must be serializable.</param>
        /// <param name="scope">The <see cref="SettingsScope">scope</see> of the settings repository to match.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        public void Set<T>(string key, T value, SettingsScope scope = SettingsScope.Project)
        {
            if (scope == SettingsScope.Project)
                Set<T, PackageSettingsRepository>(key, value);
            Set<T, UserSettingsRepository>(key, value);
        }

        /// <summary>
        /// Sets a value for a settings entry with a matching key and type `T` from the specified repository.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="value">The value to set. This must be serializable.</param>
        /// <param name="repositoryName">Optional. The name of the repository to set this value in.</param>
        /// <param name="scope">The <see cref="SettingsScope">scope</see> of the settings repository to match.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        public void Set<T>(string key, T value, string repositoryName, SettingsScope scope = SettingsScope.Project)
        {
            if (scope == SettingsScope.Project)
                Set<T, PackageSettingsRepository>(key, value, repositoryName);
            Set<T, UserSettingsRepository>(key, value, repositoryName);
        }

        /// <summary>
        /// Sets a value for a settings entry with a matching key and type `T` from the specified repository of type `K`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="value">The value to set. This must be serializable.</param>
        /// <param name="repositoryName">Optional. The name of the repository to set this value in.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <typeparam name="K">The type of repository to search for matching keys.</typeparam>
        public void Set<T, K>(string key, T value, string repositoryName = null) where K : ISettingsRepository
        {
            bool foundScopeRepository = false;

            foreach (var repo in m_SettingsRepositories)
            {
                if (repo is K && (string.IsNullOrEmpty(repositoryName) || repo.name == repositoryName))
                {
                    repo.Set<T>(key, value);
                    foundScopeRepository = true;
                }
            }

            if (!foundScopeRepository)
                Debug.LogWarning($"No repository with type {typeof(K)} found.");
        }

        /// <summary>
        /// Returns a value for a settings entry with a matching key and type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="scope">The <see cref="SettingsScope">scope</see> of the settings repository to match.</param>
        /// <param name="fallback">Specify the value of type `T` to return if the entry can't be found.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <returns>The value from a matching settings entry; or the default value if not found.</returns>
        public T Get<T>(string key, SettingsScope scope = SettingsScope.Project, T fallback = default(T))
        {
            if (scope == SettingsScope.Project)
                return Get<T, PackageSettingsRepository>(key, fallback);
            return Get<T, UserSettingsRepository>(key, fallback);
        }

        /// <summary>
        /// Returns a value for a settings entry with a matching key and type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="repositoryName">The repository name to match.</param>
        /// <param name="scope">The <see cref="SettingsScope">scope</see> of the settings repository to match.</param>
        /// <param name="fallback">Specify the value of type `T` to return if the entry can't be found.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <returns>The value from a matching settings entry; or the default value if not found.</returns>
        public T Get<T>(string key, string repositoryName, SettingsScope scope = SettingsScope.Project, T fallback = default(T))
        {
            if (scope == SettingsScope.Project)
                return Get<T, PackageSettingsRepository>(key, fallback, repositoryName);
            return Get<T, UserSettingsRepository>(key, fallback, repositoryName);
        }

        /// <summary>
        /// Returns a value for a settings entry with a matching key and type `T` from the specified repository of type `K`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="fallback">Specify the value of type `T` to return if the entry can't be found.</param>
        /// <param name="repositoryName">If provided, only repositories with a matching name will be searched for the key.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <typeparam name="K">The type of repository to search for matching keys.</typeparam>
        /// <returns>The value from a matching settings entry; or the default value if not found.</returns>
        public T Get<T, K>(string key, T fallback = default(T), string repositoryName = null) where K : ISettingsRepository
        {
            foreach (var repo in m_SettingsRepositories)
            {
                if (repo is K && (string.IsNullOrEmpty(repositoryName) || repo.name == repositoryName))
                    return repo.Get<T>(key, fallback);
            }

            Debug.LogWarning($"No repository with type {typeof(K)} found.");
            return fallback;
        }

        /// <summary>
        /// Determines whether the repository in the specified <see cref="SettingsScope">scope</see> contains a settings entry
        /// that matches the specified key and is of type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="scope">The <see cref="SettingsScope">scope</see> of the settings repository to match.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <returns>True if a setting matching both key and type is found; false if no entry is found.</returns>
        public bool ContainsKey<T>(string key, SettingsScope scope = SettingsScope.Project)
        {
            if (scope == SettingsScope.Project)
                return ContainsKey<T, PackageSettingsRepository>(key);
            return ContainsKey<T, UserSettingsRepository>(key);
        }

        /// <summary>
        /// Determines whether the specified repository contains a settings entry that matches the specified key and is of type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="repositoryName">The repository name to match.</param>
        /// <param name="scope">The <see cref="SettingsScope">scope</see> of the settings repository to match.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <returns>True if a setting matching both key and type is found; false if no entry is found.</returns>
        public bool ContainsKey<T>(string key, string repositoryName, SettingsScope scope = SettingsScope.Project)
        {
            if (scope == SettingsScope.Project)
                return ContainsKey<T, PackageSettingsRepository>(key, repositoryName);
            return ContainsKey<T, UserSettingsRepository>(key, repositoryName);
        }

        /// <summary>
        /// Determines whether the specified repository of type `K` contains a settings entry that matches the specified key and is of type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="repositoryName">The repository name to match.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <typeparam name="K">The type of repository to search for matching keys.</typeparam>
        /// <returns>True if a setting matching both key and type is found; false if no entry is found.</returns>
        public bool ContainsKey<T, K>(string key, string repositoryName = null) where K : ISettingsRepository
        {
            foreach (var repo in m_SettingsRepositories)
            {
                if (repo is K && (string.IsNullOrEmpty(repositoryName) || repositoryName == repo.name))
                    return repo.ContainsKey<T>(key);
            }

            Debug.LogWarning($"No repository with type {typeof(K)} found.");
            return false;
        }

        /// <summary>
        /// Removes a key-value pair from a settings repository. This method identifies the settings entry to remove
        /// from any repository in the specified <see cref="SettingsScope">scope</see> by matching the specified key
        /// for a value of type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="scope">The <see cref="SettingsScope">scope</see> of the settings repository to match.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        public void DeleteKey<T>(string key, SettingsScope scope = SettingsScope.Project)
        {
            if (scope == SettingsScope.Project)
                DeleteKey<T, PackageSettingsRepository>(key);
            DeleteKey<T, UserSettingsRepository>(key);
        }

        /// <summary>
        /// Removes a key-value pair from a settings repository. This method identifies the settings entry to remove
        /// from the specified repository by matching the specified key for a value of type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="repositoryName">The repository name to match.</param>
        /// <param name="scope">The <see cref="SettingsScope">scope</see> of the settings repository to match.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        public void DeleteKey<T>(string key, string repositoryName, SettingsScope scope = SettingsScope.Project)
        {
            if (scope == SettingsScope.Project)
                DeleteKey<T, PackageSettingsRepository>(key, repositoryName);
            DeleteKey<T, UserSettingsRepository>(key, repositoryName);
        }

        /// <summary>
        /// Removes a key-value pair from a settings repository. This method identifies the settings entry to remove
        /// from the specified repository of type `K` by matching the specified key for a value of type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="repositoryName">The repository name to match.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <typeparam name="K">The type of repository to search for matching keys.</typeparam>
        public void DeleteKey<T, K>(string key, string repositoryName = null) where K : ISettingsRepository
        {
            bool foundScopeRepository = false;

            foreach (var repo in m_SettingsRepositories)
            {
                if (repo is K && (string.IsNullOrEmpty(repositoryName) || repositoryName == repo.name))
                {
                    foundScopeRepository = true;
                    repo.Remove<T>(key);
                }
            }

            if (!foundScopeRepository)
                Debug.LogWarning($"No repository with type {typeof(K)} found.");
        }
    }
}
