using SolidWorks.Interop.sldworks;
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

    public IModelDoc2 Doc { get; private set; }

    /// <summary>
    /// <para>To know which other objects are being converted, in order to sort relationships between them.
    /// For example, elements that have children use this to determine whether they should send their children out or not.</para>
    /// </summary>
    public Dictionary<string, ApplicationObject> ContextObjects { get; private set; } = new ();

    /// <summary>
    /// <para>To keep track of previously received objects from a given stream in here. 
    /// If possible, conversions routines
    /// will edit an existing object, otherwise they will delete the old one and create the new one.</para>
    /// </summary>
    public Dictionary<string, ApplicationObject> PreviousContextObjects { get; set; } = new ();

    public Dictionary<string, string> Settings { get; private set; } = new ();
    #endregion

    public IEnumerable<string> GetServicedApplications()
    {
        return new[] { "SolidWorks"};
    }

    public void SetContextDocument(object doc)
    {
        Doc = doc as IModelDoc2;
        if (Doc == null)
        {
            throw new Exception("Invalid document object");
        }
    }

    public void SetContextObjects(List<ApplicationObject> objects)
    {
        ContextObjects = new(objects.Count);
        foreach (var @object in objects)
        {
            ContextObjects[@object.applicationId] = @object;
        }
    }

    public void SetConverterSettings(object settings)
    {
        Settings = settings as Dictionary<string, string>;
    }

    public void SetPreviousContextObjects(List<ApplicationObject> objects)
    {
        PreviousContextObjects = new(objects.Count);
        foreach (var @object in objects)
        {
            PreviousContextObjects[@object.applicationId] = @object;
        }
    }
}
