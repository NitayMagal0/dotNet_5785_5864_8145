namespace DalApi;
using System.Xml.Linq;

static class DalConfig
{
    /// <summary>
    /// internal PDS class
    /// </summary>
    internal record DalImplementation
    (string Package,   // package/dll name
        string Namespace, // namespace where DAL implementation class is contained in
        string Class   // DAL implementation class name
    );

    internal static string s_dalName;
    internal static Dictionary<string, DalImplementation> s_dalPackages;

    static DalConfig()
    {
        XElement dalConfig = XElement.Load(@"..\xml\dal-config.xml") ??     //Load the Xml file, do parssing and builds DOM tree of it
                             throw new DalConfigException("dal-config.xml file is not found");

        //save the type of file (list or xml)
        s_dalName =
            dalConfig.Element("dal")?.Value ?? throw new DalConfigException("<dal> element is missing");//if dal-config has list in the Dal line, return list (same for xml)

        var packages = dalConfig.Element("dal-packages")?.Elements() ??
                       throw new DalConfigException("<dal-packages> element is missing");
        s_dalPackages = (from item in packages
                let pkg = item.Value
                let ns = item.Attribute("namespace")?.Value ?? "Dal"
                let cls = item.Attribute("class")?.Value ?? pkg
                select (item.Name, new DalImplementation(pkg, ns, cls))
            ).ToDictionary(p => "" + p.Name, p => p.Item2);
    }
}

[Serializable]
public class DalConfigException : Exception
{
    public DalConfigException(string msg) : base(msg) { }
    public DalConfigException(string msg, Exception ex) : base(msg, ex) { }
}

<<<<<<< HEAD
=======
//For more information look at stage4 page 5
>>>>>>> 263a03ea0ef70dd4db011917882a6b940a20f92a
