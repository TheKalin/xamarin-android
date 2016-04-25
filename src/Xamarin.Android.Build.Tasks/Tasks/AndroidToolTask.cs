using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Xamarin.Android.Tasks
{
	public abstract class AndroidToolTask : ToolTask
	{
		protected static bool IsWindows = Path.DirectorySeparatorChar == '\\';

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			base.LogEventsFromTextOutput (singleLine, messageImportance);

			if (messageImportance != StandardErrorLoggingImportance)
				return;

			Log.LogFromStandardError (singleLine);
		}

		// Disabled because this regex does not match our errors:
		// monodroid : error 1: System.InvalidOperationException: PackageName can only contain lowercase alphanumeric characters (regex: [a-z0-9.]).

		// Code from class/Microsoft.Build.Utilities/Microsoft.Build.Utilities/ToolTask.cs
		//protected override void LogEventsFromTextOutput (string singleLine, MessageImportance importance)
		//{
		//        singleLine = singleLine.Trim ();
		//        if (singleLine.Length == 0) {
		//                Log.LogMessage (singleLine, importance);
		//                return;
		//        }

		//        // When IncludeDebugInformation is true, prevents the debug symbols stats from braeking this.
		//        if (singleLine.StartsWith ("WROTE SYMFILE") ||
		//                singleLine.StartsWith ("OffsetTable") ||
		//                singleLine.StartsWith ("Compilation succeeded") ||
		//                singleLine.StartsWith ("Compilation failed"))
		//                return;

		//        Match match = ErrorRegex.Match (singleLine);
		//        if (!match.Success) {
		//                Log.LogMessage (importance, singleLine);
		//                return;
		//        }

		//        string filename = match.Result ("${file}") ?? "";
		//        string line = match.Result ("${line}");
		//        int lineNumber = !string.IsNullOrEmpty (line) ? Int32.Parse (line) : 0;

		//        string category = match.Result ("${level}");
		//        string text = match.Result ("${message}");

		//        if (!Path.IsPathRooted (filename) && !String.IsNullOrEmpty (BaseDirectory))
		//                filename = Path.Combine (BaseDirectory, filename);

		//        if (String.Compare (category, "warning", StringComparison.OrdinalIgnoreCase) == 0) {
		//                Log.LogWarning (null, null, null, filename, lineNumber, 0, -1,
		//                        -1, text, null);
		//        } else if (String.Compare (category, "error", StringComparison.OrdinalIgnoreCase) == 0) {
		//                Log.LogError (null, null, null, filename, lineNumber, 0, -1,
		//                        -1, text, null);
		//        } else {
		//                Log.LogMessage (importance, singleLine);
		//        }
		//}

		protected virtual Regex ErrorRegex {
			get { return AndroidErrorRegex; }
		}

		/* This gets pre-pended to any filenames that we get from error strings */
		protected string BaseDirectory { get; set; }

		static Regex androidErrorRegex;
		static Regex AndroidErrorRegex {
			get {
				if (androidErrorRegex == null)
					androidErrorRegex = new Regex (@"^(\s*(?<file>[^:]+):(?<line>\d*)?:\s+)*(?<level>\w+)\s*:\s*(?<message>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
				return androidErrorRegex;
			}
		}

		protected static string QuoteString (string value)
		{
			return string.Format ("\"{0}\"", value);
		}
	}
}

