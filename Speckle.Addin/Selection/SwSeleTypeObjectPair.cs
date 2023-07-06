using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Speckle.ConnectorSolidWorks.Selection;

public class SwSeleTypeObjectPair : IEqualityComparer<SwSeleTypeObjectPair>
{
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

    public bool Equals(SwSeleTypeObjectPair x, SwSeleTypeObjectPair y)
    {
        return x?.Name == y?.Name;
    }

    public int GetHashCode(SwSeleTypeObjectPair obj)
    {
        return obj?.Name.GetHashCode() ?? -1;
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
}
