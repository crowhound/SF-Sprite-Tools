using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace SFEditor.Utilities
{
	/// <summary>
	/// A serialzed helper list for StringGUID that can be used to keep track for assets and objects.
	/// <seealso cref="StringGUID"/>
	/// </summary>
	[Serializable]
    public class StringGUIDList : IReadOnlyList<GUID>
    {
		[SerializeField] private List<StringGUID> _list = new List<StringGUID>();

		GUID IReadOnlyList<GUID>.this[int index] 
		{ 
			get => _list[index];
		}
		public StringGUID this[int index]
		{
			get => _list[index];
			set => _list[index] = value;
		}

		IEnumerator<GUID> IEnumerable<GUID>.GetEnumerator()
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
}
