using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NiqonNO.Core.Utility
{
    public class NONamingUtility
    {
        public static string EnsureUniqueName(IEnumerable<Object> collection, string compareName)
        {
            return EnsureUniqueNameRecursive();
            string EnsureUniqueNameRecursive(int recursiveCount = 0)
            {
                string checkName = recursiveCount == 0 ? compareName : $"{compareName}_{recursiveCount}";
                return collection.Any(child => child.name.Equals(checkName)) ?
                    EnsureUniqueNameRecursive(++recursiveCount) : checkName;
            }
        }
        public static string EnsureUniqueName(IEnumerable<Object> collection, Object compareItem)
        {
            return EnsureUniqueNameRecursive();
            string EnsureUniqueNameRecursive(int recursiveCount = 0)
            {
                string checkName = recursiveCount == 0 ? compareItem.name : $"{compareItem.name}_{recursiveCount}";
                return collection.Any(child => child.GetType() == compareItem.GetType() && child.name.Equals(checkName)) ?
                    EnsureUniqueNameRecursive(++recursiveCount) : checkName;
            }
        }
    }
}