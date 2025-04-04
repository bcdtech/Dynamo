using System;
using System.Collections.Generic;

namespace DynamoServices
{
    /// <summary>
    /// This attribute can be applied to methods that register callsite with
    /// trace mechanism.
    /// </summary>
    public class RegisterForTraceAttribute : Attribute
    {
    }

    /// <summary>
    /// Utility class to Get/Set TraceData
    /// </summary>
    public static class TraceUtils
    {
        internal const string __TEMP_REVIT_TRACE_ID = "{0459D869-0C72-447F-96D8-08A7FB92214B}-REVIT";
        // ReSharper restore InconsistentNaming

        [ThreadStatic] private static Dictionary<string, string> _localStorageSlot;

        internal static Dictionary<string, string> LocalStorageSlot
        {
            get
            {
                return _localStorageSlot ?? (_localStorageSlot = new Dictionary<string, string>());
            }
            set
            {
                _localStorageSlot = value;
            }
        }

        /// <summary>
        /// Returns a list of the keys bound to trace elements
        /// This should be extracted from the attribute on the methods
        /// </summary>
        /// <returns></returns>
        internal static List<String> TEMP_GetTraceKeys()
        {
            //TODO:Luke Extract this from RequiresTraceAttribute

            return new List<string>() { __TEMP_REVIT_TRACE_ID };
        }

        /// <summary>
        /// Clear a specific key
        /// </summary>
        /// <param name="key"></param>
        internal static void ClearTLSKey(string key)
        {
            LocalStorageSlot.Remove(key);
        }

        /// <summary>
        /// Clear the named slots for all the know keys
        /// </summary>
        internal static void ClearAllKnownTLSKeys()
        {
            LocalStorageSlot.Clear();
        }

        /// <summary>
        /// Returns the data that is bound to a particular key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetTraceData(string key)
        {
            if (LocalStorageSlot.TryGetValue(key, out string data))
                return data;
            return null;
        }

        /// <summary>
        /// Set the data bound to a particular key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetTraceData(string key, string value)
        {
            LocalStorageSlot[key] = value;
        }
    }
}
