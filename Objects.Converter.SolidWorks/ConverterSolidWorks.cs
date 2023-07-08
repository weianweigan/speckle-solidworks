using Speckle.Core.Kits;
using Speckle.Core.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Objects.Converter.SolidWorks;

public sealed partial class ConverterSolidWorks : ISpeckleConverter
{
    public ConverterSolidWorks()
    {
        Version version = Assembly
            .GetAssembly(typeof(ConverterSolidWorks))
            .GetName()
            .Version;
        Report.Log($"Using converter: {Name} v{version}");
    }

    #region Properties
    public string Description => "Default Speckle Kit for SolidWorks";

    public string Name => nameof(ConverterSolidWorks);

    public string Author => "WeiGan";

    public string WebsiteOrEmail => "https://github.com/weianweigan/speckle-solidworks";

    public ProgressReport Report => new ();

    public ReceiveMode ReceiveMode { get; set; }
    #endregion

    public IEnumerable<string> GetServicedApplications()
    {
        return new[] { "SolidWorks"};
    }

    public void SetContextDocument(object doc)
    {
        throw new NotImplementedException();
    }

    public void SetContextObjects(List<ApplicationObject> objects)
    {
        throw new NotImplementedException();
    }

    public void SetConverterSettings(object settings)
    {
        throw new NotImplementedException();
    }

    public void SetPreviousContextObjects(List<ApplicationObject> objects)
    {
        throw new NotImplementedException();
    }
}
