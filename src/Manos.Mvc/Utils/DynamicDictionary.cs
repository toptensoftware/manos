using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace Manos.Mvc
{
	public class DynamicDictionary : DynamicObject
	{
		private Dictionary<string, object> _data;

		public DynamicDictionary()
		{
			_data = new Dictionary<string, object>();
		}
        public DynamicDictionary(Dictionary<string, object> data)
		{
            this._data = data ;
        }

		public static Dictionary<string, object> DictionaryFromObject(object o)
		{
			if (o == null)
				return null;

			var d = new Dictionary<string, object>();
			foreach (var p in o.GetType().GetProperties())
			{
				d[p.Name] = p.GetValue(o, null);
			}
			return d;
		}

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this._data.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this._data[binder.Name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this._data[binder.Name] = value;
            return true;
        }


	}
}
