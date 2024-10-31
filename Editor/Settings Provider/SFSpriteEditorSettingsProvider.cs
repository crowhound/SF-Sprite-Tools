using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SFEditor
{
	public class SFSpriteEditorSettingsProvider : SettingsProvider
	{
		private SerializedObject _settingsObj;

		private const string SFSpriteEditorSettingsPath = SFSpriteEditorSettings.SFSpriteEditorSettingsPath;
		private const string SFSpriteEditorStyleSheetPath = "Assets/SF.2DTools/Editor/UI Toolkit/Style Sheets/SFSpriteEditor.uss";

		public SFSpriteEditorSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) {}

		public static bool IsSettingsAvaiable()
		{
			return File.Exists(SFSpriteEditorSettingsPath);
		}

		[SettingsProvider]
		public static SettingsProvider CreateSFSpriteEditorSettingProvider()
		{
			var provider = new SettingsProvider(SFSpriteEditorSettingsPath, SettingsScope.Project)
			{
				label = "SF Sprite Editor Settings",

				activateHandler = (searchcontext, rootElement) =>
				{

					var settings = SFSpriteEditorSettings.GetSerializedSettings();
					// TODO: Make the style sheet for the editor if we want one.
					// This might be a good time to make the Common Style Sheet and add it to a project settings folder.

					var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(SFSpriteEditorStyleSheetPath);
					rootElement.styleSheets.Add(styleSheet);

					var title = new Label()
					{
						text = "SF Sprite Editor Settings",
					};
					title.AddToClassList("title__label");
					rootElement.Add(title);

					var propertiesContainer = new VisualElement()
					{
						style =
						{
							flexDirection = FlexDirection.Column
						}
					};
					propertiesContainer.AddToClassList("properties-container");;
					rootElement.Add(propertiesContainer);

					var animationSectionTitle = new Label()
					{
						text = "Animation Settings",
					};
					propertiesContainer.Add(animationSectionTitle);
					propertiesContainer.Add(new PropertyField(settings.FindProperty("WasAnimatorInspectorOpen")));
					propertiesContainer.Add(new PropertyField(settings.FindProperty("DefaultAnimationClipFolderPath")));
					propertiesContainer.Add(new PropertyField(settings.FindProperty("SamplesPerSecond")));

					propertiesContainer.Add(new PropertyField(settings.FindProperty("_rememberManuallySetSPS")));



					var spriteFrameSectionTitle = new Label()
					{
						text = "Sprite Frame Settings",
					};
					propertiesContainer.Add(spriteFrameSectionTitle);
					propertiesContainer.Add(new PropertyField(settings.FindProperty("_spriteFrameColor")));
					propertiesContainer.Add(new PropertyField(settings.FindProperty("_lastSelectedSpriteFrameColor"))); 
					propertiesContainer.Add(new PropertyField(settings.FindProperty("_multipleSelectedSpriteFrameColor")));

					rootElement.Bind(settings);
				},

				// Populate the keywords for the search bar inside of the project settings menu.
				keywords = new HashSet<string>(new[] { "SamplesPerSecond", "FPS", "AnimationFPS", "SF SpriteEditor", "SF" })
			};

			return provider;
		}
	}
}
