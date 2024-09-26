using UnityEditor;

namespace SFEditor.Utilities
{
	/// <summary>
	/// Helper struct to serialize GUIDS with strings for any type of asset.
	/// </summary>
	[System.Serializable]
    public struct StringGUID
    {
		[UnityEngine.SerializeField] private string _stringGUID;

		public StringGUID(GUID guid)
		{
			_stringGUID = guid.ToString();
		}

		public static implicit operator GUID(StringGUID data) => new GUID(data._stringGUID);
		public static implicit operator StringGUID(GUID data) => new StringGUID(data);
	}
}
