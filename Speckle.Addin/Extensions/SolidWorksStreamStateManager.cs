/*
 * Notes:
 *  There are two way which can store data in SolidWorks document. Third party storage and custom attribute.
 *  Third party storage support save stream to solidworks document. But it need event to notify. 
 */

using DesktopUI2.Models;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Speckle.Core.Logging;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using IAttribute = SolidWorks.Interop.sldworks.IAttribute;

namespace Speckle.ConnectorSolidWorks.Extensions;

internal static class SolidWorksStreamStateManager
{
    private const string ATTRIBUTE_PARAMETER_NAME = "StreamStates";

    public static List<StreamState> ReadState(IModelDoc2 doc)
    {
        // Check if attribute exists
        IFeature? attributeFeat = doc.FinsSpeckleAttribute();
        if (attributeFeat == null)
        {
            return new ();
        }

        // Get attribute parameter
        var attribute = (IAttribute)attributeFeat.GetSpecificFeature2();
        var parameter = (IParameter)attribute.GetParameter(ATTRIBUTE_PARAMETER_NAME);
        string stringValue = parameter.GetStringValue();

        // Convert to list
        var states = JsonConvert.DeserializeObject<List<StreamState>>(stringValue);
        return states ?? new();
    }

    /// <summary>
    /// Write stream state list to SolidWorks document.
    /// </summary>
    /// <param name="doc">Solidworks document.</param>
    /// <param name="streamStates">Stream states.</param>
    /// <param name="app">Solidworks interface.</param>
    public static void WriteStreamStateList(
        IModelDoc2 doc, 
        List<StreamState> streamStates, 
        ISldWorks app)
	{
        try
        {
            IAttributeDef? attributeDef;
            IFeature? attributeFeat = doc.FinsSpeckleAttribute();
            if (attributeFeat == null)
            {
                attributeDef = app.DefineAttribute(IActiveDoc2Extensions.SpeckleAttributeFeatureName) as IAttributeDef;
                //See https://help.solidworks.com/2022/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.IAttributeDef~AddParameter.html
                attributeDef?.AddParameter(ATTRIBUTE_PARAMETER_NAME, (int)swParamType_e.swParamTypeString, 2.0, 0);
            }
            else
            {
                attributeDef = attributeFeat.GetDefinition() as IAttributeDef;
            }

            if (attributeDef == null)
            {
                throw new InvalidOperationException($"Cannot Create or Get {nameof(IAttributeDef)}, Feature is {attributeFeat?.Name ?? "null"}");
            }

            bool value = attributeDef.Register();
            if (!value)
            {
                throw new SpeckleException("Register SolidWorks AttributeDef Failed!");
            }

            // Create
            SolidWorks.Interop.sldworks.Attribute attribute = attributeDef.CreateInstance5(
                (ModelDoc2)doc,
                null,
                IActiveDoc2Extensions.SpeckleAttributeFeatureName,
                0,
                (int)swInConfigurationOpts_e.swAllConfiguration);

            // Hide
            var addedFeat = doc.Extension.GetLastFeatureAdded();
            addedFeat.SetUIState((int)swUIStates_e.swIsHiddenInFeatureMgr, true);

            // Add value
            var parameter = (IParameter)attribute.GetParameter(ATTRIBUTE_PARAMETER_NAME);
            parameter.SetStringValue(JsonConvert.SerializeObject(streamStates));

            // Notify Solidworks that something has changed
            doc.SetSaveFlag();
        }
        catch (Exception e)
        {
            SpeckleLog.Logger.Error(e, "Failed to write stream state.");
        }
    }
}
