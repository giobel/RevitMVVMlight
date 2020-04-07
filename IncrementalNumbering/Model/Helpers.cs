using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace IncrementalNumbering
{
    class Helpers
    {
        public static string ParameterValueToString(Element e, Parameter p)
        {
            string value;
            StorageType parameterType = p.StorageType;

            if (StorageType.Double == parameterType)
            {
                value = UnitUtils.ConvertFromInternalUnits(e.LookupParameter(p.Definition.Name).AsDouble(),
                    p.DisplayUnitType).ToString();

            }
            else if (StorageType.String == parameterType)
            {
                value = e.LookupParameter(p.Definition.Name).AsString();

            }
            else if (StorageType.Integer == parameterType)
            {
                value = e.LookupParameter(p.Definition.Name).AsInteger().ToString();

            }
            else
            {
                value = e.LookupParameter(p.Definition.Name).AsValueString();
            }


            return value;
        }
    }
}
