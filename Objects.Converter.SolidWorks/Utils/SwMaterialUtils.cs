using SolidWorks.Interop.sldworks;
using Speckle.Objects.SolidWorks;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Objects.Converter.SolidWorks.Utils;

internal static class SwMaterialUtils
{
    public static Other.RenderMaterial GetRenderMaterial(this MaterialValue? materialValue)
    {
        if (materialValue == null)
        {
            return null;
        }

        return new Objects.Other.RenderMaterial(
            opacity: materialValue.Value.Transparency,
            metalness: materialValue.Value.Shininess,
            roughness: materialValue.Value.Specular,
            diffuse: (ToColor(materialValue.Value.Diffuse)),
            emissive: (ToColor(materialValue.Value.Emission))
            );
    }

    #region Document
    public static MaterialValue? GetMaterialValue(this IModelDoc2 doc)
    {
        var materialValue = (double[])doc.MaterialPropertyValues;

        if (materialValue == null)
        {
            return null;
        }

        return MaterialValue.From(materialValue);
    }

    public static Other.RenderMaterial GetRenderMaterial(this IModelDoc2 doc)
    {
        return doc.GetMaterialValue().GetRenderMaterial();
    }
    #endregion

    #region Component
    public static MaterialValue? GetMaterialValue(this IComponent2 component)
    {
        if (!component.HasMaterialPropertyValues())
        {
            return null;
        }

        var materialValue = (double[])component.MaterialPropertyValues;

        if (materialValue == null)
        {
            return null;
        }

        return MaterialValue.From(materialValue);
    }
    #endregion

    #region Body
    public static MaterialValue? GetMaterialValue(this IBody2 body)
    {
        if (!body.HasMaterialPropertyValues())
        {
            return null;
        }

        //  0  1  2  3        4        5         6          7             8
        //[ R, G, B, Ambient, Diffuse, Specular, Shininess, Transparency, Emission ]
        var materialValue = (double[])body.MaterialPropertyValues2;
        if (materialValue == null)
        {
            return null;
        }

        return MaterialValue.From(materialValue);
    }

    /// <summary>
    /// Converter MaterialPropertyValues to RenderMaterial
    /// </summary>
    public static Other.RenderMaterial GetRenderMaterial(this IBody2 body)
    {
        return body.GetMaterialValue().GetRenderMaterial();
    }
    #endregion

    #region Face
    public static MaterialValue? GetMaterialValue(this IFace2 face)
    {
        if (!face.HasMaterialPropertyValues())
        {
            return null;
        }

        //  0  1  2  3        4        5         6          7             8
        //[ R, G, B, Ambient, Diffuse, Specular, Shininess, Transparency, Emission ]
        var materialValue = (double[])face.MaterialPropertyValues;
        if (materialValue == null)
        {
            return null;
        }

        return MaterialValue.From(materialValue);
    }

    /// <summary>
    /// To ARGB
    /// </summary>
    /// <remarks>
    /// https://help.solidworks.com/2022/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.IFace2~MaterialPropertyValues.html
    /// </remarks>
    /// <param name="face"></param>
    /// <returns></returns>
    public static int? ToARGB(this IFace2 face)
    {
        if (!face.HasMaterialPropertyValues())
        {
            return null;
        }

        var materialValue = (double[])face.MaterialPropertyValues;
        if (materialValue == null)
        {
            return null;
        }

        return ToARBG(new double[] { materialValue[0], materialValue[1], materialValue[2], materialValue[7] });
    }
    #endregion

    #region Color
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
    #endregion
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