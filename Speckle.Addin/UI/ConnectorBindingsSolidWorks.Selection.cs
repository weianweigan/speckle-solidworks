using DesktopUI2.Models.Filters;
using Speckle.ConnectorSolidWorks.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speckle.ConnectorSolidWorks.UI
{
    public partial class ConnectorBindingsSolidWorks
    {
        public override void SelectClientObjects(
            List<string> objs, 
            bool deselect = false)
        {
            throw new NotImplementedException();
        }

        public override List<string> GetSelectedObjects()
        {
            var doc = App.IActiveDoc2;

            return doc
                ?.ISelectionManager
                .GetSelections()
                .Select(p => p.Name)
                .ToList();
        }

        public override List<ISelectionFilter> GetSelectionFilters()
        {
            return new List<ISelectionFilter>();
        }
    }
}
