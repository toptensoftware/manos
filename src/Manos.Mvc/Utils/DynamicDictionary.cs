using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace Manos.Mvc
{
	public class DynamicDictionary : DynamicObject
	{
        private Dictionary<string, object> _data
        {
			get;
			set;
        }

        public DynamicDictionary(Dictionary<string, object> data)
        {
            this._data = data ;
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
