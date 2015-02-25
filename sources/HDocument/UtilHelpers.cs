using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HDoc
{
    /// <summary>
    /// Some helpers
    /// </summary>
    public static class UtilHelpers
    {
        /// <summary>
        /// Extract the key-value list from an objet.
        /// </summary>
        public static IDictionary<String, String> ExtractKeyValues(Object source, Func<String, String> keyTransform = null)
        {
            Dictionary<String, String> result = new Dictionary<string, string>();
            keyTransform = keyTransform ?? (n => n);
            if (source is IDictionary<String, String>)
            {
                foreach (var prop in ((IDictionary<String, String>)source))
                    result[keyTransform(prop.Key)] = prop.Value;
            }
            else if (source != null)
            {
                foreach (var member in source.GetType().GetMembers().Where(m => m is PropertyInfo || m is FieldInfo))
                {
                    String mName = keyTransform(member.Name);
                    if (member is PropertyInfo)
                        result[mName] = Convert.ToString(((PropertyInfo)member).GetValue(source, null));
                    else if (member is FieldInfo)
                        result[mName] = Convert.ToString(((FieldInfo)member).GetValue(source));
                }
            }
            return result;
        }
    }
}
