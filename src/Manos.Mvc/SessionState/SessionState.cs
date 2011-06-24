using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Manos.Mvc
{
	public class SessionState : IDictionary<string, object>
	{
		public SessionState(string SessionID)
		{
			this.SessionID = SessionID;
		}

		// Constructor for loaded session states
		public SessionState(string SessionID, int timeout, Dictionary<string, object> Values)
		{
			this.SessionID = SessionID;
			this.Timeout = Timeout;
			m_Values = Values;
		}

		// The session id for this session
		public string SessionID
		{																		  
			get;
			private set;
		}

		public byte[] Save()
		{
			using (var ms = new MemoryStream())
			{
				var bf = new BinaryFormatter();

				int version = 1;

				bf.Serialize(ms, version);
				bf.Serialize(ms, Timeout);
				bf.Serialize(ms, m_Values);

				ms.Flush();
				return ms.GetBuffer();
			}
		}

		public void Load(byte[] buf)
		{
			using (var ms = new MemoryStream(buf))
			{
				var bf = new BinaryFormatter();

				int version = (int)bf.Deserialize(ms);
				Timeout = (int)bf.Deserialize(ms);
				m_Values = (Dictionary<string, object>)bf.Deserialize(ms);
			}
		}

		public SessionState Clone()
		{
			var ss = new SessionState(SessionID);
			ss.Load(this.Save());
			return ss;
		}

		int _timeout;
		public int Timeout
		{
			get
			{
				return _timeout;
			}
			set
			{
				_timeout = value;
				Modified = true;
			}
		}

		Dictionary<string, object> m_Values;

		public bool Modified
		{
			get;
			set;
		}

		#region IDictionary<string,object> Members

		public void Add(string key, object value)
		{
			m_Values[key] = value;// Overwrite
			Modified=true;
		}

		public bool ContainsKey(string key)
		{
			return m_Values.ContainsKey(key);
		}

		public ICollection<string> Keys
		{
			get
			{
				return m_Values.Keys;
			}
		}

		public bool Remove(string key)
		{
			if (m_Values.Remove(key))
			{
				Modified = true;
				return true;
			}
			return false;
		}

		public bool TryGetValue(string key, out object value)
		{
			return m_Values.TryGetValue(key, out value);
		}

		public ICollection<object> Values
		{
			get 
			{
				return m_Values.Values;
			}
		}

		public object this[string key]
		{
			get
			{
				if (!ContainsKey(key))
					return null;

				return m_Values[key];
			}
			set
			{
				m_Values[key] = value;
				Modified = true;
			}
		}

		#endregion

		#region ICollection<KeyValuePair<string,object>> Members

		public void Add(KeyValuePair<string, object> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			m_Values.Clear();
			Modified = true;
		}

		public bool Contains(KeyValuePair<string, object> item)
		{
			return ((ICollection<KeyValuePair<string,object>>)m_Values).Contains(item);
		}

		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<string, object>>)m_Values).CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return m_Values.Count(); }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<string, object> item)
		{
			if (((ICollection<KeyValuePair<string, object>>)m_Values).Remove(item))
			{
				Modified = true;
				return true;
			}
			return false;
		}

		#endregion

		#region IEnumerable<KeyValuePair<string,object>> Members

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return m_Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return m_Values.GetEnumerator();
		}

		#endregion
	}
}
