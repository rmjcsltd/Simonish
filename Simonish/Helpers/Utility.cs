using System.Diagnostics;
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
        /// <remarks>This is useful when troubleshooting during development to see thread and event sequences.</remarks>
        [Conditional("DEBUG")]
        public static void WriteDebugEntryMessage(MethodBase methodBase)
        {
            //MethodBase methodBase = System.Reflection.MethodBase.GetCurrentMethod();

            string typeName = methodBase.ReflectedType?.FullName;
            string methodName = methodBase.Name;
            string parameterInfo = string.Join(",", methodBase.GetParameters().Select(o => $"{o.ParameterType} {o.Name}").ToArray());

            Debug.WriteLine($"Thread Id {Thread.CurrentThread.ManagedThreadId:000} - Enter {typeName}.{methodName}({parameterInfo}");
        }
    }
}
