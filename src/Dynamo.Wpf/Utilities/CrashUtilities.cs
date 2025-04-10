using Dynamo.Configuration;
using Dynamo.Utilities;

namespace Dynamo.Wpf.Utilities
{
    static class CrashUtilities
    {
        internal enum ReportType
        {
            Bug,
            Crash
        }

        internal static string GithubNewIssueUrlFromCrashContent(object crashContent, ReportType reportType = ReportType.Crash)
        {
            var baseUri = new UriBuilder(Configurations.GitHubBugReportingLink);

            var issueTitle = "{0} report from Dynamo {1}";
            var dynamoVersion = AssemblyHelper.GetDynamoVersion().ToString() ?? "2.1.0+";

            // append the title and body to the URL as query parameters
            // making sure we properly escape content since stack traces may contain characters not suitable
            // for use in URLs
            var title = "title=" + Uri.EscapeDataString(string.Format(issueTitle, reportType, dynamoVersion));
            var template = "template=issue.yml";
            var fields = "dynamo_version=" + Uri.EscapeDataString(dynamoVersion)
                + "&os=" + Uri.EscapeDataString(Environment.OSVersion.ToString())
                + "&packages=" + Uri.EscapeDataString(crashContent?.ToString() ?? string.Empty)
                + "&details=" + Uri.EscapeDataString("CLR: " + Environment.Version.ToString());
            baseUri.Query = title + "&" + template + "&" + fields;

            // this will properly format the string for use as a uri
            return baseUri.ToString();
        }



        /// <summary>
        /// Builds a Markdown string that will be posted to our new GitHub issue
        /// </summary>
        /// <param name="dynamoVersion">Dynamo version that should be recorded in the issue report</param>
        /// <param name="markdownPackages">Section of the issue with the loaded packages in markdown format</param>
        /// <returns></returns>
        internal static string BuildMarkdownContent(string dynamoVersion, string markdownPackages)
        {
            return
                "# Issue Description" + Environment.NewLine +
                "Please fill in the following information to help us reproduce the issue:" + Environment.NewLine +
                Environment.NewLine +

                "## Dynamo version" + Environment.NewLine +
                "Dynamo: " + "`" + dynamoVersion + "`" + Environment.NewLine + Environment.NewLine +

                "## Operating system" + Environment.NewLine +
                "OS: " + "`" + Environment.OSVersion + "`" + Environment.NewLine + Environment.NewLine +

                "## What did you do?" + Environment.NewLine +
                "(Fill in here)" + Environment.NewLine + Environment.NewLine +

                "## What did you expect to see?" + Environment.NewLine +
                "(Fill in here)" + Environment.NewLine + Environment.NewLine +

                "## What did you see instead?" + Environment.NewLine +
                "(Fill in here)" + Environment.NewLine + Environment.NewLine +

                "## What packages or external references (if any) were used?" + Environment.NewLine +
                markdownPackages + Environment.NewLine + Environment.NewLine +

                "## Stack Trace" + Environment.NewLine +
                "```" + Environment.NewLine +
                "(From the Dynamo crash window select 'Details' -> 'Copy' and paste here)" + Environment.NewLine +
                "```" + Environment.NewLine + Environment.NewLine +

                "---" + Environment.NewLine +
                "CLR: " + "`" + Environment.Version + "`" + Environment.NewLine;
        }
    }
}
