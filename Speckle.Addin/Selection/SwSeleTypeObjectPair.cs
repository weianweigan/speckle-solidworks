using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Speckle.ConnectorSolidWorks.Selection;

public class SwSeleTypeObjectPair
{
    #region Ctor
    [JsonConstructor]
    public SwSeleTypeObjectPair(
        int index, 
        swSelectType_e selectType,
        int mark,
        object selectedObject,
        double[]? point,
        string pid)
    {
        SelectType = selectType;
        Mark = mark;
        Index = index;
        Point = point;
        SelectedObject = selectedObject;
        PID = pid;

        if (selectedObject is IFeature feature)
        {
            Name = $"{feature.Name}({pid})";
        }
        else
        {
            Name = pid;
        }
    }
    #endregion

    #region Properties
    /// <summary>
    /// Index base on 1.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Mark from SolidWorks.
    /// </summary>
    public int Mark { get; } = -1;

    public swSelectType_e SelectType { get; private set; }

    /// <summary>
    /// Object in sw
    /// </summary>
    public object SelectedObject { get; private set; }

    /// <summary>
    /// name display in sw SelectionBox
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Custom tag for track, Optional
    /// </summary>
    public object? Tag { get; set; }

    /// <summary>
    /// Selection point from use click
    /// </summary>
    public double[]? Point { get; set; }

    /// <summary>
    /// PID for this object，null default.
    /// </summary>
    public string PID { get; }
    #endregion

    #region Methods
    public swPersistReferencedObjectStates_e ReSolveFormPID(IModelDoc2 doc)
    {
        if (string.IsNullOrEmpty(PID))
        {
            throw new ArgumentNullException("PID Cannot be null!");
        }

        byte[] byteId = Convert.FromBase64String(PID);

        SelectedObject = doc.Extension.GetObjectByPersistReference3(byteId, out int errorCode);
        return (swPersistReferencedObjectStates_e)errorCode;
    }

    /// <summary>
    /// Use for Face Edge Vertex，avoid <see cref="System.Runtime.InteropServices.COMException"/>
    /// </summary>
    public void GetSafeEntity()
    {
        if (SelectType == swSelectType_e.swSelEDGES
            || SelectType == swSelectType_e.swSelFACES
            || SelectType == swSelectType_e.swSelVERTICES)
        {
            var entity = SelectedObject as IEntity;
            if (entity?.IsSafe == false)
                SelectedObject = entity.GetSafeEntity();
        }
    }

    public override string ToString()
    {
        return Name ?? base.ToString();
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static SwSeleTypeObjectPair FromJson(string json)
    {
        return JsonSerializer.Deserialize<SwSeleTypeObjectPair>(json) ?? 
            throw new NullReferenceException("Cannot Deserialize SwSeleTypeObjectPair");
    }
    #endregion
}
