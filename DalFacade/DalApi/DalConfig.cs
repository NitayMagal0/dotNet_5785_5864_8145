namespace DalApi;
using System.Xml.Linq;

/// <summary>
/// Provides configuration management for Data Access Layer (DAL) implementations
/// </summary>
static class DalConfig
{
    /// <summary>
    /// Represents a DAL implementation configuration entry
    /// </summary>
    /// <param name="Package">Assembly/package name containing the implementation</param>
    /// <param name="Namespace">Namespace containing the DAL implementation class</param>
    /// <param name="Class">Concrete DAL implementation class name</param>
    internal record DalImplementation(
        string Package,
        string Namespace,
        string Class
    );

    /// <summary>Current DAL implementation name</summary>
    internal static string s_dalName;

    /// <summary>Dictionary of available DAL implementations</summary>
    internal static Dictionary<string, DalImplementation> s_dalPackages;

    /// <summary>
    /// Static constructor initializes DAL configuration from XML file
    /// </summary>
    /// <exception cref="DalConfigException">Thrown on configuration loading/parsing errors</exception>
    static DalConfig()
    {
        // Load XML configuration file
        XElement dalConfig = XElement.Load(@"..\xml\dal-config.xml") ??
            throw new DalConfigException("dal-config.xml file is not found");

        // Extract root DAL implementation name
        s_dalName = dalConfig.Element("dal")?.Value ??
            throw new DalConfigException($"Missing required element: 'dal' in {dalConfig}");

        // Parse DAL implementation packages
        var packages = dalConfig.Element("dal-packages")?.Elements() ??
            throw new DalConfigException("<dal-packages> element is missing");

        s_dalPackages = (from item in packages
                         let pkg = item.Value  // Package name from element value
                         let ns = item.Attribute("namespace")?.Value ?? "Dal" // Default namespace
                         let cls = item.Attribute("class")?.Value ?? pkg // Default class name
                         select (item.Name, new DalImplementation(pkg, ns, cls))
                        ).ToDictionary(
                            p => "" + p.Name, // Dictionary key as element name
                            p => p.Item2      // Dictionary value as DalImplementation
                        );
    }
}

/// <summary>
/// Represents errors that occur during DAL configuration loading
/// </summary>
[Serializable]
public class DalConfigException : Exception
{
    /// <summary>Initializes with error message</summary>
    public DalConfigException(string msg) : base(msg) { }

    /// <summary>Initializes with error message and inner exception</summary>
    public DalConfigException(string msg, Exception ex) : base(msg, ex) { }
}