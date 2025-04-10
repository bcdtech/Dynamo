using System;

namespace Dynamo.Logging
{
    /// <summary>
    /// Utility class to support analytics tracking.
    /// </summary>
    internal class AnalyticsService
    {


        /// <summary>
        /// Indicates that we don't want to shut down analytics when DynamoModel shuts down.
        /// Sometimes we want to keep Analytics service running even when we don't have a DynamoModel started.
        /// </summary>
        internal static bool KeepAlive { get; set; }

        /// <summary>
        /// Starts the Analytics client. This method initializes
        /// the Analytics service and application life cycle start is tracked.
        /// </summary>
        internal static void Start()
        {


        }

        /// <summary>
        /// Indicates whether the user has opted-in to ADP analytics.
        /// As of ADP4 this will return true for most users.
        /// </summary>
        internal static bool IsADPOptedIn
        {
            get; set;

        }

        internal static bool IsADPAvailable()
        {
            return false;
        }

        /// <summary>
        /// Shuts down the client. Application life cycle end is tracked.
        /// </summary>
        internal static void ShutDown()
        {
            if (!KeepAlive)
            {
                Analytics.ShutDown();
            }
        }

        /// <summary>
        /// Show the ADP dynamic consents dialog.
        /// </summary>
        /// <param name="host">main window</param>
        internal static void ShowADPConsentDialog(IntPtr? host)
        {

        }

        internal static string GetUserIDForSession()
        {

            return null;
        }
    }
}
