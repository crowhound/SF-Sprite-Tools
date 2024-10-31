using System.IO;
using System.Text;

using UnityEditor;

using UnityEngine;

namespace SFEditor
{
    public class SFSpriteEditorSettings : ScriptableObject
    {
		public const string SFSpriteEditorSettingsPath = "Assets/SF Settings/SFSpriteEditorSettings.asset";

		private static string SFSpriteEditorSettingsFolderPath;

		#region Sprite Animation Settings
		public string DefaultAnimationClipFolderPath = "Assets/Animations/";
		/// <summary>
		/// The default value for how many frames per second are set when creating a new animation clip in the SF Sprite Editor. Due note if the _rememberManuallySetSPS bool is set to true the value set for new clips will be the _lastSetSamplesPerSecond.
		/// </summary>
		public uint SamplesPerSecond = 24;

		/// <summary>
		/// This is the last value set by the user for frames per second in the Animation overlay inside the SF Sprite Editor.
		/// </summary>
		[SerializeField] private uint _lastSetSamplesPerSecond = 0;

		/// <summary>
		/// Should the SF Sprite Inspector remember the last manually set value of the frames per second and use it when creating new Animation Clips. This settings is project based currently so each project will be able to remeber a different value.
		/// </summary>
		[SerializeField] private bool _rememberManuallySetSPS = true;
		#endregion

		public bool WasAnimatorInspectorOpen = false;

		[SerializeField] private Color _spriteFrameColor = new Color(.55f, .55f, .55f, 1);
		[SerializeField] private Color _lastSelectedSpriteFrameColor = new Color(0, .7f, 0, 1);
		[SerializeField] private Color _multipleSelectedSpriteFrameColor = new Color(0, 0.1f, .7f, 1);

		public static SFSpriteEditorSettings GetOrCreateSettings()
		{
			var settings = AssetDatabase.LoadAssetAtPath<SFSpriteEditorSettings>(SFSpriteEditorSettingsPath);

			if(settings == null)
			{
				settings = CreateInstance<SFSpriteEditorSettings>();
				settings.SamplesPerSecond = 24;
				settings._lastSetSamplesPerSecond = 0;
				settings._rememberManuallySetSPS = true;

				if(!CheckIfSettingsDirectoryExists())
					Directory.CreateDirectory(SFSpriteEditorSettingsFolderPath);

				if(!CheckIfAnimationFolderExists(settings))
					Directory.CreateDirectory(settings.DefaultAnimationClipFolderPath);

				AssetDatabase.CreateAsset(settings, SFSpriteEditorSettingsPath);
				AssetDatabase.SaveAssets();

			}
			return settings;
		} 

		private static bool CheckIfAnimationFolderExists(SFSpriteEditorSettings settings)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(Application.dataPath);
			sb.Replace("Assets", "");
			sb.Append(settings.DefaultAnimationClipFolderPath);

			settings.DefaultAnimationClipFolderPath = sb.ToString();

			if(!Directory.Exists(settings.DefaultAnimationClipFolderPath))
				Debug.LogWarning(settings.DefaultAnimationClipFolderPath);

			return false;
		}
		private static bool CheckIfSettingsDirectoryExists()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(Application.dataPath);
			sb.Replace("Assets","");
			sb.Append(SFSpriteEditorSettingsPath);
			sb.Replace("SFSpriteEditorSettings.asset", "");

			SFSpriteEditorSettingsFolderPath = sb.ToString();

			if(!Directory.Exists(SFSpriteEditorSettingsFolderPath))
				Debug.LogWarning(SFSpriteEditorSettingsFolderPath);

			return false;
		}

		public static SerializedObject GetSerializedSettings()
		{
			return new SerializedObject(GetOrCreateSettings());
		}
	}
}
