// Copyright (c) 2017 AutomationML e.V.
using System.Windows;
using System.Windows.Markup;

[assembly: XmlnsPrefix("http://www.automationml.org/amlTool/", "AML")]
[assembly: XmlnsDefinition("http://www.automationml.org/amlTool/", "Aml.Toolkit")]
[assembly: XmlnsDefinition("http://www.automationml.org/amlTool/", "Aml.Toolkit.Operations")]
[assembly: XmlnsDefinition("http://www.automationml.org/amlTool/", "Aml.Toolkit.ViewModel")]
[assembly: XmlnsDefinition("http://www.automationml.org/amlTool/", "Aml.Toolkit.View")]
[assembly: XmlnsDefinition("http://www.automationml.org/amlTool/", "Aml.Toolkit.XamlClasses")]
[assembly: XmlnsDefinition("http://www.automationml.org/amlTool/", "Aml.Toolkit.ViewModel.ValidationRules")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]