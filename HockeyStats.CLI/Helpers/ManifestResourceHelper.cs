using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HockeyStats.CLI.Helpers
{
    public static class ManifestResourceHelper
    {
        public static string[] GetManifestResourceNames(Assembly asm = null)
        {
            asm = asm ?? Assembly.GetCallingAssembly();

            return asm.GetManifestResourceNames();
        }

        public static IEnumerable<string> GetManifestResourcePaths(Assembly asm = null)
        {
            asm = asm ?? Assembly.GetCallingAssembly();
            return asm.GetManifestResourceNames();
        }

        public static FileInfo ExtractResourceToDisk(string pathToResource, string targetPathOnDisk, bool overwrite = true)
        {
            var content = GetManifestResourceString(pathToResource, Assembly.GetCallingAssembly());
            var file = new FileInfo(targetPathOnDisk);
            if (File.Exists(file.FullName) && overwrite)
            {
                File.Delete(file.FullName);
            }
            File.WriteAllText(file.FullName, content);
            return file;
        }

        public static string GetManifestResourceString(string resourceName, Assembly asm = null)
        {
            if (string.IsNullOrWhiteSpace(resourceName)) return "";

            string result;
            using (var stream = GetManifestResourcesStream(resourceName, asm))
            using (var reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        public static Stream GetManifestResourceStream(params string[] resourceNameParts)
        {
            if (resourceNameParts == null || resourceNameParts.Length == 0) return null;

            var asm = Assembly.GetCallingAssembly();
            var resourceName = asm.GetName().Name + "." + string.Join(".", resourceNameParts).Replace('/','\\').Replace('\\', '.');
            return GetManifestResourcesStream(resourceName, asm);
        }

        public static Stream GetManifestResourcesStream(string resourceName, Assembly asm = null)
        {
            if (string.IsNullOrWhiteSpace(resourceName)) return null;

            asm = asm ?? Assembly.GetCallingAssembly();

            var resourceInfo = asm.GetManifestResourceInfo(resourceName);
            if (resourceInfo == null)
            {
                var message = "Could not find the requested resource among: ";
                var names = asm.GetManifestResourceNames();
                message += string.Join(";", names);
                throw new FileNotFoundException(message, resourceName);
            }

            return asm.GetManifestResourceStream(resourceName);
        }
    }
}
