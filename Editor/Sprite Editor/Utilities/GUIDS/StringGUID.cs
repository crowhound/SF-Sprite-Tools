
namespace SFEditor.Utilities
{
	// In Unity 6.4 they removed the Editor only requirements for a lot of the GUID and type caching API.
#if UNITY_6000_4_OR_NEWER
	/// <summary>
	/// Helper struct to serialize GUIDS with strings for any type of asset.
	/// </summary>
	[System.Serializable]
    public struct StringGUID
    {
		[UnityEngine.SerializeField] private string _stringGUID;

		public StringGUID(UnityEngine.GUID guid)
		{
			_stringGUID = guid.ToString();
		}
		public static implicit operator UnityEngine.GUID(StringGUID data) => new UnityEngine.GUID(data._stringGUID);
		public static implicit operator StringGUID(UnityEngine.GUID data) => new StringGUID(data);
	}
#else 
	/// <summary>
	/// Helper struct to serialize GUIDS with strings for any type of asset.
	/// </summary>
	[System.Serializable]
	public struct StringGUID
	{
		[UnityEngine.SerializeField] private string _stringGUID;

		public StringGUID(UnityEditor.GUID guid)
		{
			_stringGUID = guid.ToString();
		}

		public static implicit operator UnityEditor.GUID(StringGUID data) => new UnityEditor.GUID(data._stringGUID);
		public static implicit operator StringGUID(UnityEditor.GUID data) => new StringGUID(data);
	}
#endif
}
