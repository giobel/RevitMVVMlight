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
            
            if (e.LookupParameter(p.Definition.Name) == null)
            {
                return null;
            }
            
            StorageType parameterType = p.StorageType;

            if (StorageType.Double == parameterType)
            {
                double paramValue = e.LookupParameter(p.Definition.Name).AsDouble();

                double convertedValue =  Math.Round(UnitUtils.ConvertFromInternalUnits(paramValue, p.DisplayUnitType),2);

                if (p.DisplayUnitType == DisplayUnitType.DUT_CUBIC_METERS)
                {
                    value = $"{convertedValue.ToString("0.00")} m\xb3";
                }
                else if (p.DisplayUnitType == DisplayUnitType.DUT_SQUARE_METERS)
                {
                    value = $"{convertedValue.ToString("0.00")} m\xb2";
                }
                else {
                    value = convertedValue.ToString();
                }
               
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

        public enum Operators
        {
            Equal,
            Larger,
            Smaller,
            Not_Equal
        }
    }
}
