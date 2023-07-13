using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SettingsManagement;

namespace UnityEngine.SettingsManagement.EditorTests
{
	public class FileRepositoryTests : SettingsTestBase
	{
		Settings m_Settings;
		FileSettingsRepository m_Repository;

		[SetUp]
		public void SetUp()
		{
			m_Settings = new Settings(new [] { m_Repository = new ProjectUserSettings(k_PackageName) } );
		}

		[TearDown]
		public void TearDown()
		{
			if (File.Exists(m_Repository.path))
				File.Delete(m_Repository.path);
		}

		[Test]
		public void CallingSaveMultipleTimes_OnlySavesWhenContentsChange()
		{
			m_Settings.Set<int, FileSettingsRepository>("save-int", 42);
			m_Settings.Save();
			var write = File.GetLastWriteTime(m_Repository.path);

			// verify that setting was serialized
			Assume.That(File.Exists(m_Repository.path));
			var readback = new Settings(new [] { new ProjectUserSettings(k_PackageName) } );
			Assume.That(readback.Get<int, FileSettingsRepository>("save-int"), Is.EqualTo(42));

			// set the same value and save
			m_Settings.Set<int, FileSettingsRepository>("save-int", 42);
			m_Settings.Save();

			// save file should not be touched since nothing change
			Assert.That(write, Is.EqualTo(File.GetLastWriteTime(m_Repository.path)));
		}

		[Test]
		public void SaveFileSettingsRepository_WritesFile()
		{
			m_Settings.Set<int, FileSettingsRepository>("save-int", 42);
			m_Settings.Save();

			Assume.That(File.Exists(m_Repository.path));

			var readback = new Settings(new [] { new ProjectUserSettings(k_PackageName) } );
			Assert.That(readback.Get<int, FileSettingsRepository>("save-int"), Is.EqualTo(42));
		}
	}
}
