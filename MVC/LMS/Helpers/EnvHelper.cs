using System;
using System.IO;
using System.Web.Hosting;

namespace LMS.Helpers
{
    public static class EnvHelper
    {
        private static bool _isLoaded = false;

        public static void Load(string filePath = null)
        {
            if (_isLoaded && string.IsNullOrEmpty(filePath))
                return;

            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    string baseDir = HostingEnvironment.IsHosted
                        ? HostingEnvironment.MapPath("~/")
                        : AppDomain.CurrentDomain.BaseDirectory;

                    filePath = Path.Combine(baseDir ?? string.Empty, ".env");
                }

                if (!File.Exists(filePath))
                    return;

                foreach (var line in File.ReadAllLines(filePath))
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                        continue;

                    int separatorIndex = trimmed.IndexOf('=');
                    if (separatorIndex <= 0)
                        continue;

                    string key = trimmed.Substring(0, separatorIndex).Trim();
                    string value = trimmed.Substring(separatorIndex + 1).Trim();

                    // Strip optional quotes
                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        if (value.Length >= 2)
                        {
                            value = value.Substring(1, value.Length - 2);
                        }
                    }

                    Environment.SetEnvironmentVariable(key, value, EnvironmentVariableTarget.Process);
                }

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceWarning("Failed to load .env file: " + ex.Message);
            }
        }

        public static string Get(string key, string fallback = null)
        {
            if (!_isLoaded)
            {
                Load();
            }

            return Environment.GetEnvironmentVariable(key) ?? fallback;
        }
    }
}
