using DesktopUI2.Models.Settings;
using System.Collections.Generic;

namespace Speckle.ConnectorSolidWorks.UI;

/// <summary>
/// Custom settings for SolidWorks connector.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>
/// <see cref="ListBoxSetting"/>
/// </item>
/// <item>
/// <see cref="CheckBoxSetting"/>
/// </item>
/// <item>
/// <see cref="MultiSelectBoxSetting"/>
/// </item>
/// <item>
/// <see cref="MappingSeting"/>
/// </item>
/// <item>
/// <see cref="TextBoxSetting"/>
/// </item>
/// <item>
/// <see cref="NumericSetting"/>
/// </item>
/// </list>
/// </remarks>
public partial class ConnectorBindingsSolidWorks
{
    public override List<ISetting> GetSettings()
    {
        return new List<ISetting>();
    }
}
