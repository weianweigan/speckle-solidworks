using Objects.Other;
using SolidWorks.Interop.sldworks;

namespace Objects.Converter.SolidWorks.Converters;

internal static class SwTransformConverter
{
    public static Transform ToSpeckleTransform(this MathTransform transform)
    {
        return new Transform(transform.ToArray());
    }

    /// <summary>
    ///    |a b c.n |
    ///    |d e f.o |
    ///    |g h i.p |  
    ///    |j k l.m |
    /// </summary>
    /// <remarks>
    /// <see href="https://help.solidworks.com/2022/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.IMathTransform.html"/>
    /// </remarks>
    /// <returns></returns>
    public static double[] ToArray(this IMathTransform transform)
    {
        var data = transform.ArrayData as double[];
        var array = new double[]{
            data[0], data[1], data[2], data[13],
            data[3], data[4], data[5], data[14],
            data[6], data[7], data[8], data[15],
            data[9], data[10], data[11], data[12]
        };
        return array;
    }
}
