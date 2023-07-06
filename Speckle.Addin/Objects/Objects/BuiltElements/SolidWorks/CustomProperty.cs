using Speckle.Core.Models;

namespace Objects.BuiltElements.SolidWorks;

/// <summary>
/// SolidWorks CustomProperty.
/// </summary>
public sealed class CustomProperty: Base
{
    public CustomProperty() { }

    public CustomProperty(
        string name, 
        string value, 
        string? evaluatedValue = null)
    {
        Name = name;
        Value = value;
        EvaluatedValue = evaluatedValue;
    }

    /// <summary>
    /// Property name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Source value.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Configuration.
    /// </summary>
    public string? Configuration { get; set; }

    /// <summary>
    /// Eval Value.
    /// </summary>
    public string? EvaluatedValue { get; set; }

    ///<inheritdoc/>
    public override string ToString()
    {
        return $"{{{nameof(Name)}:{Name}, {nameof(Value)}:{EvaluatedValue}}}";
    }
}
