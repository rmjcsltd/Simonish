using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Rmjcs.Simonish.Helpers
{
    /// <summary>
    /// A small class to hold development utility methods.
    /// </summary>
    internal static class Utility
    {
        /// <summary>
        /// Write a method entry message to the configured listeners.
        /// </summary>
        /// <param name="methodBase">The method being entered.</param>
        /// <param name="callerInstance">The class instance of the caller.</param>
        /// <remarks>This is useful when troubleshooting during development to see thread and event sequences.</remarks>
        [Conditional("DEBUG")]
        public static void WriteDebugEntryMessage(MethodBase methodBase, object callerInstance)
        {
            string typeName = methodBase.ReflectedType?.FullName;
            string methodName = methodBase.Name;
            string parameterInfo = string.Join(",", methodBase.GetParameters().Select(o => $"{o.ParameterType} {o.Name}").ToArray());
            string threadId = Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.CurrentCulture);
            string callerId = callerInstance?.GetHashCode().ToString("D10", CultureInfo.CurrentCulture) ?? "          ";

            Debug.WriteLine($"{DateTime.Now:HH:mm:ss.ffffff} - Thread {threadId} - Object {callerId} - Enter {typeName}.{methodName}({parameterInfo})".Replace("Rmjcs.Simonish.", string.Empty));
        }
    }
}
