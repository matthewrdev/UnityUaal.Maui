using System;
using UnityEngine;

namespace UnityEditor.SettingsManagement
{
    [Flags]
    enum SettingVisibility
    {
        None = 0 << 0,

        /// <summary>
        /// Matches any static field implementing IUserSetting and tagged with [UserSettingAttribute(visibleInSettingsProvider = true)].
        /// These fields are automatically scraped by the SettingsProvider and displayed.
        /// </summary>
        Visible = 1 << 0,

        /// <summary>
        /// Matches any static field implementing IUserSetting and tagged with [UserSettingAttribute(visibleInSettingsProvider = false)].
        /// These fields will be reset by the "Reset All" menu in SettingsProvider, but are not shown in the interface.
        /// Typically these fields require some conditional formatting or data handling, and are shown in the
        /// SettingsProvider UI with a [UserSettingBlockAttribute].
        /// </summary>
        Hidden = 1 << 1,

        /// <summary>
        /// A static or instance field tagged with [SettingsKeyAttribute].
        /// Unlisted settings are not shown in the SettingsProvider, but are reset to default values by the "Reset All"
        /// context menu.
        /// </summary>
        Unlisted = 1 << 2,

        /// <summary>
        /// A static field implementing IUserSetting that is not marked with any setting attribute.
        /// Unregistered IUserSetting fields are not affected by the SettingsProvider.
        /// </summary>
        Unregistered = 1 << 3,

        All = Visible | Hidden | Unlisted | Unregistered
    }

    /// <summary>
    /// An interface that represents a user setting.
    /// Types implementing IUserSetting are eligible for use with <see cref="UserSettingAttribute"/>, which enables
    /// fields to automatically populate the <see cref="UserSettingsProvider"/> interface.
    /// </summary>
    public interface IUserSetting
    {
        /// <summary>
        /// Implement this property to get the key for this value.
        /// </summary>
        /// <value>The key used to identify the settings entry. This is used along with the <see cref="type"/> to uniquely identify the value.</value>
        string key { get; }

        /// <summary>
        /// Implement this property to get the type of the stored value.
        /// </summary>
        /// <value>The type of value. This is used along with the <see cref="key"/> to uniquely identify the value.</value>
        Type type { get; }

        /// <summary>
        /// Implement this property to get the location in the UI where this setting will appear.
        /// </summary>
        /// <value>
        /// Indicates whether this is a <see cref="UnityEditor.SettingsScope.Project"/> setting
        /// or a <see cref="UnityEditor.SettingsScope.User"/> preference.
        /// </value>
        SettingsScope scope { get; }

        /// <summary>
        /// Implement this property to get the name of the <see cref="ISettingsRepository"/> that this setting should be associated with.
        /// If null, the first repository matching the <see cref="scope"/> is used.
        /// </summary>
        /// <value>The bare filename of this repository.</value>
        string settingsRepositoryName { get; }

        /// <summary>
        /// Implement this property to get the <see cref="Settings"/> instance to save and load this setting from.
        /// </summary>
        /// <value>A reference to <see cref="Settings"/> instance.</value>
        Settings settings { get; }

        /// <summary>
        /// Implement this method to return the stored settings value.
        /// If you are implementing IUserSetting, you should cache this value.
        /// </summary>
        /// <returns>
        /// The stored value.
        /// </returns>
        object GetValue();

        /// <summary>
        /// Implement this method to return the the default value for this setting.
        /// </summary>
        /// <returns>
        /// The default value for this setting.
        /// </returns>
        object GetDefaultValue();

        /// <summary>
        /// Implement this method to set the value for this setting.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <param name="saveProjectSettingsImmediately">
        /// True to immediately serialize the <see cref="ISettingsRepository"/> that is backing this value; or false to postpone.
        /// If not serializing immediately, be sure to call <see cref="Settings.Save"/>.
        /// </param>
        void SetValue(object value, bool saveProjectSettingsImmediately = false);

        /// <summary>
        /// Implement this method to explicitly update the <see cref="ISettingsRepository"/> that is backing this value.
        /// When the inspected type is a reference value, it is possible to change properties without affecting the
        /// backing setting. ApplyModifiedProperties provides a method to force serialize these changes.
        /// </summary>
        void ApplyModifiedProperties();

        /// <summary>
        /// Implement this method to set the current value back to the default.
        /// </summary>
        /// <param name="saveProjectSettingsImmediately">True to immediately re-serialize project settings. By default, no values are updated. </param>
        void Reset(bool saveProjectSettingsImmediately = false);

        /// <summary>
        /// Implement this method to delete the saved setting. This does not clear the current value.
        /// </summary>
        /// <seealso cref="Reset"/>
        /// <param name="saveProjectSettingsImmediately">True to immediately re-serialize project settings. By default, no values are updated.</param>
        void Delete(bool saveProjectSettingsImmediately = false);
    }

    /// <summary>
    /// A generic implementation of <see cref="IUserSetting"/> to use with a <see cref="Settings"/> instance. This default
    /// implementation assumes that the <see cref="Settings"/> instance contains two <see cref="ISettingsRepository"/> interfaces:
    /// - Project settings (<see cref="SettingsScope.Project"/>)
    /// - User preferences (<see cref="SettingsScope.User"/>)
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    public class UserSetting<T> : IUserSetting
    {
        bool m_Initialized;
        string m_Key;
        string m_Repository;
        T m_Value;
        T m_DefaultValue;
        SettingsScope m_Scope;
        Settings m_Settings;

        UserSetting() { }

        /// <summary>
        /// Initializes and returns an instance of the UserSetting&lt;T&gt; type.
        /// </summary>
        /// <param name="settings">The <see cref="Settings"/> instance to save and load this setting from.</param>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The default value for this key.</param>
        /// <param name="scope">The scope for this setting. By default, the scope is the project.</param>
        public UserSetting(Settings settings, string key, T value, SettingsScope scope = SettingsScope.Project)
        {
            m_Key = key;
            m_Repository = null;
            m_Value = value;
            m_Scope = scope;
            m_Initialized = false;
            m_Settings = settings;
        }

        /// <summary>
        /// Initializes and returns an instance of the UserSetting&lt;T&gt; type using the specified repository.
        /// </summary>
        /// <param name="settings">The <see cref="Settings"/> instance to save and load this setting from.</param>
        /// <param name="repository">The <see cref="ISettingsRepository"/> name to save and load this setting from. Specify null to save to the first available instance.</param>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The default value for this key.</param>
        /// <param name="scope">The scope for this setting. By default, the scope is the project.</param>
        public UserSetting(Settings settings, string repository, string key, T value, SettingsScope scope = SettingsScope.Project)
        {
            m_Key = key;
            m_Repository = repository;
            m_Value = value;
            m_Scope = scope;
            m_Initialized = false;
            m_Settings = settings;
        }

        /// <summary>
        /// Gets the key for this value.
        /// </summary>
        /// <seealso cref="IUserSetting.key"/>
        public string key
        {
            get { return m_Key; }
        }

        /// <summary>
        /// Gets the name of the repository that this setting is saved in.
        /// </summary>
        /// <seealso cref="IUserSetting.settingsRepositoryName" />
        public string settingsRepositoryName
        {
            get { return m_Repository; }
        }

        /// <summary>
        /// Gets the type that this setting represents (&lt;T&gt;).
        /// </summary>
        /// <seealso cref="IUserSetting.type" />
        public Type type
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Returns a copy of the default value.
        /// </summary>
        /// <returns>
        /// The default value.
        /// </returns>
        /// <seealso cref="IUserSetting.GetDefaultValue" />
        public object GetDefaultValue()
        {
            return defaultValue;
        }

        /// <summary>
        /// Returns the currently stored value.
        /// </summary>
        /// <returns>
        /// The value that is currently set.
        /// </returns>
        /// <seealso cref="IUserSetting.GetValue" />
        public object GetValue()
        {
            return value;
        }

        /// <summary>
        /// Gets the scope (<see cref="ISettingsRepository"/>) where the <see cref="Settings"/> instance saves
        /// its data.
        /// </summary>
        /// <seealso cref="IUserSetting.scope" />
        public SettingsScope scope
        {
            get { return m_Scope; }
        }

        /// <summary>
        /// Gets the <see cref="Settings"/> instance to read from and save to.
        /// </summary>
        /// <seealso cref="IUserSetting.settings" />
        public Settings settings
        {
            get { return m_Settings; }
        }

        /// <summary>
        /// Sets the value for this setting from the specified object.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        /// <param name="saveProjectSettingsImmediately">
        /// Set this value to true if you want to immediately serialize the <see cref="ISettingsRepository"/>
        /// that is backing this value. By default, this is false.
        ///
        /// **Note**: If not serializing immediately, you need to call <see cref="Settings.Save"/>.
        /// </param>
        /// <seealso cref="IUserSetting.SetValue" />
        public void SetValue(object value, bool saveProjectSettingsImmediately = false)
        {
            // we do want to allow null values
            if (value != null && !(value is T))
                throw new ArgumentException("Value must be of type " + typeof(T) + "\n" + key + " expecting value of type " + type + ", received " + value.GetType());
            SetValue((T)value, saveProjectSettingsImmediately);
        }

        /// <inheritdoc cref="SetValue" />
        public void SetValue(T value, bool saveProjectSettingsImmediately = false)
        {
            Init();
            m_Value = value;
            settings.Set<T>(key, m_Value, m_Scope);

            if (saveProjectSettingsImmediately)
                settings.Save();
        }

        /// <summary>
        /// Deletes the saved setting but doesn't clear the current value.
        /// </summary>
        /// <param name="saveProjectSettingsImmediately">
        /// Set this value to true if you want to immediately serialize the <see cref="ISettingsRepository"/>
        /// that is backing this value. By default, this is false.
        ///
        /// **Note**: If not serializing immediately, you need to call <see cref="Settings.Save"/>.
        /// </param>
        /// <seealso cref="Reset" />
        /// <seealso cref="IUserSetting.Delete"/>
        public void Delete(bool saveProjectSettingsImmediately = false)
        {
            settings.DeleteKey<T>(key, scope);
            // Don't Init() because that will set the key again. We just want to reset the m_Value with default and
            // pretend that this field hasn't been initialised yet.
            m_Value = ValueWrapper<T>.DeepCopy(m_DefaultValue);
            m_Initialized = false;
        }

        /// <summary>
        /// Forces Unity to serialize the changed properties to the <see cref="ISettingsRepository"/> that is backing this value.
        /// When the inspected type is a reference value, it is possible to change properties without affecting the
        /// backing setting.
        /// </summary>
        /// <seealso cref="IUserSetting.ApplyModifiedProperties"/>
        public void ApplyModifiedProperties()
        {
            settings.Set<T>(key, m_Value, m_Scope);
            settings.Save();
        }

        /// <summary>
        /// Sets the current value back to the default.
        /// </summary>
        /// <param name="saveProjectSettingsImmediately">
        /// Set this value to true if you want to immediately serialize the <see cref="ISettingsRepository"/>
        /// that is backing this value. By default, this is false.
        ///
        /// **Note**: If not serializing immediately, you need to call <see cref="Settings.Save"/>.
        /// </param>
        /// <seealso cref="IUserSetting.Reset"/>
        public void Reset(bool saveProjectSettingsImmediately = false)
        {
            SetValue(defaultValue, saveProjectSettingsImmediately);
        }

        void Init()
        {
            if (!m_Initialized)
            {
                if (m_Scope == SettingsScope.Project && settings == null)
                    throw new Exception("UserSetting \"" + m_Key + "\" is attempting to access SettingsScope.Project setting with no Settings instance!");

                m_Initialized = true;

                // DeepCopy uses EditorJsonUtility which is not permitted during construction
                m_DefaultValue = ValueWrapper<T>.DeepCopy(m_Value);

                if (settings.ContainsKey<T>(m_Key, m_Scope))
                    m_Value = settings.Get<T>(m_Key, m_Scope);
                else
                    settings.Set<T>(m_Key, m_Value, m_Scope);
            }
        }

        /// <summary>
        /// Gets the default value for this setting.
        /// </summary>
        public T defaultValue
        {
            get
            {
                Init();
                return ValueWrapper<T>.DeepCopy(m_DefaultValue);
            }
        }

        /// <summary>
        /// Gets or sets the currently stored value.
        /// </summary>
        public T value
        {
            get
            {
                Init();
                return m_Value;
            }

            set { SetValue(value); }
        }

        /// <summary>
        /// Implicit casts this setting to the backing type `T`.
        /// </summary>
        /// <param name="pref">The UserSetting&lt;T&gt; to cast to `T`.</param>
        /// <returns>
        /// The currently stored <see cref="value"/>.
        /// </returns>
        public static implicit operator T(UserSetting<T> pref)
        {
            return pref.value;
        }

        /// <summary>
        /// Returns a string representation of this setting.
        /// </summary>
        /// <returns>A string summary of this setting of format "[scope] setting. Key: [key]  Value: [value]".</returns>
        public override string ToString()
        {
            return string.Format("{0} setting. Key: {1}  Value: {2}", scope, key, value);
        }
    }
}
