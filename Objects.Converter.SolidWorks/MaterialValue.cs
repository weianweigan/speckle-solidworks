namespace Speckle.Objects.SolidWorks;

/// <summary>
/// <see href="https://help.solidworks.com/2022/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.IBody2~MaterialPropertyValues2.html"/>
/// </summary>
public struct MaterialValue
{
    public double R;

    public double G;

    public double B;

    public double Ambient;

    public double Diffuse;

    public double Specular;

    public double Shininess;

    public double Transparency;

    public double Emission;

    public MaterialValue(
        double r, 
        double g, 
        double b, 
        double ambient, 
        double diffuse, 
        double specular, 
        double shininess, 
        double transparency, 
        double emission)
    {
        R = r;
        G = g;
        B = b;
        Ambient = ambient;
        Diffuse = diffuse;
        Specular = specular;
        Shininess = shininess;
        Transparency = transparency;
        Emission = emission;
    }

    public static MaterialValue From(double[] values)
    {
        return new MaterialValue(
            values[0],
            values[1],
            values[2],
            values[3],
            values[4],
            values[5],
            values[6],
            values[7],
            values[8]
            );
    }
}
