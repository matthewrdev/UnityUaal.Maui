using System.IO;
using NUnit.Framework;
using UnityEditor.SettingsManagement;
using Directory = UnityEngine.Windows.Directory;

namespace UnityEngine.SettingsManagement.EditorTests
{
	public class TestProjectUserSettings : SettingsTestBase
	{
		Settings m_Settings;
		ISettingsRepository m_UserSettingsRepo;

		[SetUp]
		public void Setup()
		{
			m_Settings = new Settings(new [] { m_UserSettingsRepo = new ProjectUserSettings(k_PackageName) });
			m_Settings.Save();
		}

		[TearDown]
		public void Cleanup()
		{
			var dir = Path.GetDirectoryName(m_UserSettingsRepo.path);
			if(Directory.Exists(dir))
				Directory.Delete(dir);
		}

		[Test]
		public void ProjectUserSettings_IsCreatedAtCorrectPath()
		{
			var expectedPath = $"UserSettings/Packages/{k_PackageName}/Settings.json";
			Assert.That(m_UserSettingsRepo.path, Is.EqualTo(expectedPath));
			Assert.That(File.Exists(expectedPath));
		}
	}
}
