using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace ControlHub.Common
{
    public static class JsonHelper<T>
    {
        public static string ToJson(T obj)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(obj);

            return json;
        }
    }
}