// <auto-generated />
namespace NuGet.Packaging.Core
{
    using System.Globalization;
    using System.Reflection;
    using System.Resources;

    internal static class Strings
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("NuGet.Packaging.Core.Strings", typeof(Strings).GetTypeInfo().Assembly);

        /// <summary>
        /// Nuspec file does not contain the '{0}' node.
        /// </summary>
        internal static string MissingMetadataNode
        {
            get { return GetString("MissingMetadataNode"); }
        }

        /// <summary>
        /// Nuspec file does not contain the '{0}' node.
        /// </summary>
        internal static string FormatMissingMetadataNode(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("MissingMetadataNode"), p0);
        }

        /// <summary>
        /// Nuspec file does not exist in package.
        /// </summary>
        internal static string MissingNuspec
        {
            get { return GetString("MissingNuspec"); }
        }

        /// <summary>
        /// Nuspec file does not exist in package.
        /// </summary>
        internal static string FormatMissingNuspec()
        {
            return GetString("MissingNuspec");
        }

        /// <summary>
        /// Package contains multiple nuspec files.
        /// </summary>
        internal static string MultipleNuspecFiles
        {
            get { return GetString("MultipleNuspecFiles"); }
        }

        /// <summary>
        /// Package contains multiple nuspec files.
        /// </summary>
        internal static string FormatMultipleNuspecFiles()
        {
            return GetString("MultipleNuspecFiles");
        }

        /// <summary>
        /// String argument '{0}' cannot be null or empty
        /// </summary>
        internal static string StringCannotBeNullOrEmpty
        {
            get { return GetString("StringCannotBeNullOrEmpty"); }
        }

        /// <summary>
        /// String argument '{0}' cannot be null or empty
        /// </summary>
        internal static string FormatStringCannotBeNullOrEmpty(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("StringCannotBeNullOrEmpty"), p0);
        }

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);

            System.Diagnostics.Debug.Assert(value != null);

            if (formatterNames != null)
            {
                for (var i = 0; i < formatterNames.Length; i++)
                {
                    value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
                }
            }

            return value;
        }
    }
}
