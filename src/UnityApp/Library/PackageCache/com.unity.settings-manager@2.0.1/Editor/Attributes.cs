using System;
using UnityEngine;

namespace UnityEditor.SettingsManagement
{
    /// <summary>
    /// A custom attribute for registering a static field of type <see cref="IUserSetting"/> for the <see cref="UserSettingsProvider"/> window.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class UserSettingAttribute : Attribute
    {
        string m_Category;
        GUIContent m_Title;
        bool m_VisibleInSettingsProvider;

        /// <summary>
        /// Gets the name of the group (category) to assign this settings value to.
        /// When Unity finds settings values in assemblies, it displays them in groups, organized by category.
        /// </summary>
        /// <value>The group or category where this setting appears in the UI.</value>
        public string category
        {
            get { return m_Category; }
        }

        /// <summary>
        /// Gets the label to show for this setting.
        /// </summary>
        /// <value>The label that appears beside this setting in the UI.</value>
        public GUIContent title
        {
            get { return m_Title; }
        }

        /// <summary>
        /// True to show this field in the <see cref="UserSettingsProvider"/> interface; false if not.
        /// </summary>
        public bool visibleInSettingsProvider
        {
            get { return m_VisibleInSettingsProvider; }
        }

        /// <summary>
        /// Registers a static field as a setting. Fields must be of a type that implements <see cref="IUserSetting"/>.
        /// </summary>
        public UserSettingAttribute()
        {
            m_VisibleInSettingsProvider = false;
        }

        /// <summary>
        /// Registers a static field as a setting and creates an entry in the UI. The field must be of a type that implements <see cref="IUserSetting"/>.
        /// </summary>
        /// <param name="category">The category to assign this setting to.</param>
        /// <param name="title">The display text for this setting in the UI.</param>
        /// <param name="tooltip">Optional. The tooltip for this setting.</param>
        public UserSettingAttribute(string category, string title, string tooltip = null)
        {
            m_Category = category;
            m_Title = new GUIContent(title, tooltip);
            m_VisibleInSettingsProvider = true;
        }
    }

    /// <summary>
    /// A custom attribute for registering a field with <see cref="Settings"/>, but without automatically creating
    /// a property field in the <see cref="SettingsProvider"/>.
    /// Unlike <see cref="UserSettingAttribute"/>, this attribute is valid for instance properties as well as static. These values
    /// don't appear in the SettingsProvider. Unity clears their stored values when "Reset All" is invoked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SettingsKeyAttribute : Attribute
    {
        string m_Key;
        SettingsScope m_Scope;

        /// <summary>
        /// Gets the key for this value.
        /// </summary>
        /// <value>The key used to identify this settings value from the repository.</value>
        public string key
        {
            get { return m_Key; }
        }

        /// <summary>
        /// Gets the location where this setting is serialized.
        /// </summary>
        /// <value>
        /// Indicates whether this is a <see cref="UnityEditor.SettingsScope.Project"/> setting
        /// or a <see cref="UnityEditor.SettingsScope.User"/> preference.
        /// </value>
        public SettingsScope scope
        {
            get { return m_Scope; }
        }

        /// <summary>
        /// Registers a field as a setting. This allows the <see cref="UserSettingsProvider"/> to reset its value and display it
        /// in debugging modes.
        /// </summary>
        /// <param name="key">The key for this setting.</param>
        /// <param name="scope">The scope in which this setting is serialized.</param>
        public SettingsKeyAttribute(string key, SettingsScope scope = SettingsScope.Project)
        {
            m_Key = key;
            m_Scope = scope;
        }
    }

    /// <summary>
    /// A custom attribute for adding a section of settings to a category.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class UserSettingBlockAttribute : Attribute
    {
        string m_Category;

        /// <summary>
        /// Returns the title for the settings group.
        /// When Unity finds settings values in assemblies, it displays them in groups, organized by category.
        /// </summary>
        /// <value>The group or category where this setting appears in the UI.</value>
        public string category
        {
            get { return m_Category; }
        }

        /// <summary>
        /// Registers a static method for a callback in the <see cref="UserSettingsProvider"/> Editor window under a category.
        /// <code><![CDATA[
        /// [UserSettingBlock("General")]
        /// static void GeneralSettings(string[] searchContext) {}
        /// ]]></code>
        /// </summary>
        /// <param name="category">Specify the title of the group of settings under which this setting appears in the UI.</param>
        public UserSettingBlockAttribute(string category)
        {
            m_Category = category;
        }
    }
}
