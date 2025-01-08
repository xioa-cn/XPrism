using System.Xml.Serialization;

namespace XPrism.Core.Modules.Config;

/// <summary>
/// 模块配置根节点
/// </summary>
[XmlRoot("configuration")]
public class ModulesConfiguration {
    [XmlAttribute("name")] public string Name { get; set; }

    [XmlElement("modules")] public ModulesSection Modules { get; set; }
}

/// <summary>
/// 模块节
/// </summary>
public class ModulesSection {
    [XmlElement("module")] public List<ModuleElement> Modules { get; set; }
}

/// <summary>
/// 模块元素
/// </summary>
public class ModuleElement {
    [XmlAttribute("assemblyFile")] public string AssemblyFile { get; set; }

    [XmlElement("moduleName")] public List<string> ModuleNames { get; set; }
}