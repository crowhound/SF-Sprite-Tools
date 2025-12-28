using System;
using System.Collections;
using System.Collections.Generic;

namespace SFEditor.Utilities
{
	
#if UNITY_6000_4_OR_NEWER
	/// <summary>
	/// A serialzed helper list for StringGUID that can be used to keep track for assets and objects.
	/// <seealso cref="StringGUID"/>
	/// </summary>
	[Serializable]
    public class StringGUIDList : IReadOnlyList<UnityEngine.GUID>
    {
		[UnityEngine.SerializeField]
		private List<StringGUID> _list = new List<StringGUID>();

		UnityEngine.GUID IReadOnlyList<UnityEngine.GUID>.this[int index] 
		{ 
			get => _list[index];
		}
		public StringGUID this[int index]
		{
			get => _list[index];
			set => _list[index] = value;
		}

		IEnumerator<UnityEngine.GUID> IEnumerable<UnityEngine.GUID>.GetEnumerator()
		{
			// Not yet used.
			throw new NotImplementedException();
		}

		public int Count => _list.Count;
		public IEnumerator GetEnumerator() => _list.GetEnumerator();
		public void Clear() => _list.Clear();
		public void RemoveAt(int i) => _list.RemoveAt(i);
		public void Add(StringGUID value) => _list.Add(value);
	}
	
#else
	/// <summary>
	/// A serialzed helper list for StringGUID that can be used to keep track for assets and objects.
	/// <seealso cref="StringGUID"/>
	/// </summary>
	[Serializable]
	public class StringGUIDList : IReadOnlyList<UnityEditor.GUID>
	{
		[UnityEngine.SerializeField]
		private List<StringGUID> _list = new List<StringGUID>();

		UnityEditor.GUID IReadOnlyList<UnityEditor.GUID>.this[int index] 
		{ 
			get => _list[index];
		}
		public StringGUID this[int index]
		{
			get => _list[index];
			set => _list[index] = value;
		}

		IEnumerator<UnityEditor.GUID> IEnumerable<UnityEditor.GUID>.GetEnumerator()
		{
			// Not yet used.
			throw new NotImplementedException();
		}

		public int Count => _list.Count;
		public IEnumerator GetEnumerator() => _list.GetEnumerator();
		public void Clear() => _list.Clear();
		public void RemoveAt(int i) => _list.RemoveAt(i);
		public void Add(StringGUID value) => _list.Add(value);
	}
#endif
}
