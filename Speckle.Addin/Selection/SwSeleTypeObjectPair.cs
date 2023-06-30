using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;

namespace Speckle.ConnectorSolidWorks.Selection;

public class SwSeleTypeObjectPair : IEqualityComparer<SwSeleTypeObjectPair>
{
    public SwSeleTypeObjectPair(
        int index, 
        swSelectType_e selectType,
        int mark,
        object selectedObject,
        double[] point,
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

    public SwSeleTypeObjectPair(
        object selectedObject, 
        swSelectType_e selectType)
    {
        SelectType = selectType;
        SelectedObject = selectedObject;
    }

    /// <summary>
    /// Index from 1
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// mark from SolidWorks
    /// </summary>
    public int Mark { get; private set; }

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
    public object Tag { get; set; }

    /// <summary>
    /// Selection point from use click
    /// </summary>
    public double[] Point { get; set; }

    /// <summary>
    /// PID for this object，null default.
    /// </summary>
    public string PID { get; set; }

    public swPersistReferencedObjectStates_e ReSolveFormPID(IModelDoc2 doc)
    {
        if (string.IsNullOrEmpty(PID))
        {
            throw new ArgumentNullException("请先对属性 PID 赋值");
        }

        var byteId = Convert.FromBase64String(PID);

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
            if (!entity.IsSafe)
                SelectedObject = entity.GetSafeEntity();
        }
    }

    public override string ToString()
    {
        return Name ?? base.ToString();
    }
}
