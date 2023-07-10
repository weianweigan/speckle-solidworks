using Objects.BuiltElements.SolidWorks;
using Speckle.Core.Models;
using System;
using System.Collections.Generic;

namespace Objects.Converter.SolidWorks;

public partial class ConverterSolidWorks
{
    private static HashSet<Type> _supportObjects = new HashSet<Type>()
    {
        typeof(CustomProperty),
        typeof(Equation),
        typeof(BomSheet),
        typeof(Body),
        typeof(BuiltElements.SolidWorks.Component),
        typeof(DrawingSheet),
        typeof(DrawingView),
        typeof(BuiltElements.SolidWorks.Sketch),
    };

    public bool CanConvertToNative(Base @object)
    {
        return _supportObjects.Contains(@object.GetType());
    }

    public object ConvertToNative(Base @object)
    {
        throw new NotImplementedException();
    }

    public List<object> ConvertToNative(List<Base> objects)
    {
        throw new NotImplementedException();
    }
}
