namespace Dal;

using DO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

/// <summary>
/// A static utility class for handling XML operations, including saving, loading, and manipulating XML data.
/// </summary>
static class XMLTools
{
    // The directory where XML files are stored
    const string s_xmlDir = @"..\xml\";

    /// <summary>
    /// Static constructor to ensure the XML directory exists.
    /// </summary>
    static XMLTools()
    {
        if (!Directory.Exists(s_xmlDir))
            Directory.CreateDirectory(s_xmlDir); // Create directory if it doesn't exist
    }

    #region SaveLoadWithXMLSerializer

    /// <summary>
    /// Saves a list of objects to an XML file using XML serialization.
    /// </summary>
    /// <typeparam name="T">The type of objects in the list to save.</typeparam>
    /// <param name="list">The list of objects to be serialized and saved.</param>
    /// <param name="xmlFileName">The name of the XML file where the data will be saved.</param>
    public static void SaveListToXMLSerializer<T>(List<T> list, string xmlFileName) where T : class
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            // Create and write to the XML file using a FileStream and XmlSerializer
            using FileStream file = new(xmlFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            new XmlSerializer(typeof(List<T>)).Serialize(file, list); // Serialize the list to the file
        }
        catch (Exception ex)
        {
            // Throw a custom exception if there is an error during file creation
            throw new DalXMLFileLoadCreateException($"Failed to create XML file: {xmlFilePath}, {ex.Message}");
        }
    }

    /// <summary>
    /// Loads a list of objects from an XML file using XML serialization.
    /// </summary>
    /// <typeparam name="T">The type of objects in the list to load.</typeparam>
    /// <param name="xmlFileName">The name of the XML file to load.</param>
    /// <returns>A list of objects loaded from the file. Returns an empty list if the file does not exist.</returns>
    public static List<T> LoadListFromXMLSerializer<T>(string xmlFileName) where T : class
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            // If the file doesn't exist, return an empty list
            if (!File.Exists(xmlFilePath)) return new();

            // Open the file and deserialize it into a list of objects
            using FileStream file = new(xmlFilePath, FileMode.Open);
            XmlSerializer x = new(typeof(List<T>));
            return x.Deserialize(file) as List<T> ?? new();
        }
        catch (Exception ex)
        {
            // Throw a custom exception if there is an error during file loading
            throw new DalXMLFileLoadCreateException($"Failed to load XML file: {xmlFilePath}, {ex.Message}");
        }
    }

    #endregion

    #region SaveLoadWithXElement

    /// <summary>
    /// Saves an XML element to an XML file using LINQ to XML.
    /// </summary>
    /// <param name="rootElem">The root XML element to save.</param>
    /// <param name="xmlFileName">The name of the XML file where the data will be saved.</param>
    public static void SaveListToXMLElement(XElement rootElem, string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            // Save the XElement to the specified file path
            rootElem.Save(xmlFilePath);
        }
        catch (Exception ex)
        {
            // Throw a custom exception if there is an error during file creation
            throw new DalXMLFileLoadCreateException($"Failed to create XML file: {xmlFilePath}, {ex.Message}");
        }
    }

    /// <summary>
    /// Loads an XML element from an XML file.
    /// </summary>
    /// <param name="xmlFileName">The name of the XML file to load.</param>
    /// <returns>The loaded XML element. If the file does not exist, a new empty element is created and saved.</returns>
    public static XElement LoadListFromXMLElement(string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            // If the file exists, load and return the XML element
            if (File.Exists(xmlFilePath))
                return XElement.Load(xmlFilePath);

            // If the file does not exist, create a new empty root element, save it, and return it
            XElement rootElem = new(xmlFileName);
            rootElem.Save(xmlFilePath);
            return rootElem;
        }
        catch (Exception ex)
        {
            // Throw a custom exception if there is an error during file loading
            throw new DalXMLFileLoadCreateException($"Failed to load XML file: {xmlFilePath}, {ex.Message}");
        }
    }

    #endregion

    #region XmlConfig

    /// <summary>
    /// Retrieves an integer value from an XML configuration file and increments it.
    /// </summary>
    /// <param name="xmlFileName">The name of the XML configuration file.</param>
    /// <param name="elemName">The name of the XML element containing the integer value.</param>
    /// <returns>The current integer value before it was incremented.</returns>
    public static int GetAndIncreaseConfigIntVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);

        // Try to convert the element's value to an integer. Throw an exception if it fails.
        int nextId = root.ToIntNullable(elemName) ?? throw new FormatException($"Can't convert: {xmlFileName}, {elemName}");

        // Increment the value and save the updated XML file
        root.Element(elemName)?.SetValue((nextId + 1).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);

        return nextId;
    }

    /// <summary>
    /// Retrieves an integer value from an XML configuration file.
    /// </summary>
    public static int GetConfigIntVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        return root.ToIntNullable(elemName) ?? throw new FormatException($"Can't convert: {xmlFileName}, {elemName}");
    }

    /// <summary>
    /// Retrieves a DateTime value from an XML configuration file.
    /// </summary>
    public static DateTime GetConfigDateVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        return root.ToDateTimeNullable(elemName) ?? throw new FormatException($"Can't convert: {xmlFileName}, {elemName}");
    }

    /// <summary>
    /// Updates an integer value in an XML configuration file.
    /// </summary>
    public static void SetConfigIntVal(string xmlFileName, string elemName, int elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue(elemVal.ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }

    /// <summary>
    /// Updates a DateTime value in an XML configuration file.
    /// </summary>
    public static void SetConfigDateVal(string xmlFileName, string elemName, DateTime elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue(elemVal.ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }

    #endregion

    #region ExtensionFunctions

    /// <summary>
    /// Converts an XML element's value to a nullable enum.
    /// </summary>
    public static T? ToEnumNullable<T>(this XElement element, string name) where T : struct, Enum =>
        Enum.TryParse<T>((string?)element.Element(name), out var result) ? (T?)result : null;

    /// <summary>
    /// Converts an XML element's value to a nullable DateTime.
    /// </summary>
    public static DateTime? ToDateTimeNullable(this XElement element, string name) =>
        DateTime.TryParse((string?)element.Element(name), out var result) ? (DateTime?)result : null;

    /// <summary>
    /// Converts an XML element's value to a nullable double.
    /// </summary>
    public static double? ToDoubleNullable(this XElement element, string name) =>
        double.TryParse((string?)element.Element(name), out var result) ? (double?)result : null;

    /// <summary>
    /// Converts an XML element's value to a nullable integer.
    /// </summary>
    public static int? ToIntNullable(this XElement element, string name) =>
        int.TryParse((string?)element.Element(name), out var result) ? (int?)result : null;

    #endregion

    #region MyFunctions

    /// <summary>
    /// Retrieves a TimeSpan value from an XML configuration file.
    /// </summary>
    public static TimeSpan GetConfigTimeSpanVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        string value = root.Element(elemName)?.Value ?? throw new FormatException($"Element {elemName} not found in {xmlFileName}");
        return TimeSpan.Parse(value);
    }

    /// <summary>
    /// Updates a TimeSpan value in an XML configuration file.
    /// </summary>
    public static void SetConfigTimeSpanVal(string xmlFileName, string elemName, TimeSpan elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue(elemVal.ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }

    #endregion
}
