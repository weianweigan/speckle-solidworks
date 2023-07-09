using SolidWorks.Interop.sldworks;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Objects.Converter.SolidWorks.Utils;

internal static class SwMaterialUtils
{
    /// <summary>
    /// Converter MaterialPropertyValues to RenderMaterial
    /// </summary>
    public static Other.RenderMaterial GetRenderMaterial(this IBody2 body)
    {
        if (!body.HasMaterialPropertyValues())
        {
            return null;
        }

        var materialValue = (double[])body.MaterialPropertyValues;
        if (materialValue == null)
        {
            return null;
        }

        //  0  1  2  3        4        5         6          7             8
        //[ R, G, B, Ambient, Diffuse, Specular, Shininess, Transparency, Emission ]

        return new Objects.Other.RenderMaterial(
            opacity: materialValue[7],
            metalness: materialValue[6],
            roughness: materialValue[6],
            diffuse: (ToColor(materialValue[4])),
            emissive: (ToColor(materialValue[8]))
            );
    }

    /// <summary>
    /// To ARGB
    /// </summary>
    /// <remarks>
    /// https://help.solidworks.com/2022/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.IFace2~MaterialPropertyValues.html
    /// </remarks>
    /// <param name="face"></param>
    /// <returns></returns>
    public static int ToARGB(this IFace2 face)
    {
        if (!face.HasMaterialPropertyValues())
        {
            return 0;
        }

        var materialValue = (double[])face.MaterialPropertyValues;
        if (materialValue == null)
        {
            return 0;
        }

        return ToARBG(new double[] { materialValue[0], materialValue[1], materialValue[2], materialValue[7] });
    }

    public static int ToARBG(double[] color)
    {
        return (int)(color[0] * 255) << 24 | (int)(color[1] * 255) << 16 | (int)(color[2] * 255) << 8 | (int)(color[3] * 255);
    }

    /// <summary>
    /// Converter double value to color
    /// </summary>
    /// <remarks>
    /// <see href="https://stackoverflow.com/questions/20120781/convert-double-value-to-rgb-color-in-c-sharp"/>
    /// </remarks>
    /// <param name="this"></param>
    /// <returns></returns>
    public static Color ToColor(this double @this)
    {
        var denominator = new CommonDenominatorBetweenColorsAndDoubles();

        denominator.AsDouble = @this;

        Color color = Color.FromArgb(
            red: denominator.R,
            green: denominator.G,
            blue: denominator.B
        );
        return color;
    }
}

[StructLayout(LayoutKind.Explicit)]
internal struct CommonDenominatorBetweenColorsAndDoubles
{

    [FieldOffset(0)]
    public byte R;
    [FieldOffset(1)]
    public byte G;
    [FieldOffset(2)]
    public byte B;

    [FieldOffset(0)]
    public double AsDouble;
}