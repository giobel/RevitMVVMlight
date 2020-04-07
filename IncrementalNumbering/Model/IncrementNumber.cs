using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IncrementalNumbering.Model
{
    class IncrementNumber
    {
        public static void Increment(UIApplication uiapp, Category CategorySelected, Parameter parameterName, string OperatorValue, string SelectedValue)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uiapp.ActiveUIDocument.Document;

            //double convertedStartNumber = Double.Parse(startNumber);
            bool flag = true;

            Selection selElements = uidoc.Selection;

            ICollection<Element> idTxt = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategoryId(CategorySelected.Id).ToElements();

            ICollection<ElementId> selectedIds = new List<ElementId>();

            string parseValue = SelectedValue.Replace(" m", "").Replace("\xb2", "").Replace("\xb3", "");

            //TaskDialog.Show("r", parseValue);
            try
            {

            
            foreach (Element item in idTxt)
            {
                if (SelectedValue == null)
                {
                    selectedIds.Add(item.Id);
                }
                else if (OperatorValue == Helpers.Operators.Equal.ToString())
                {
                    string tempValue = Helpers.ParameterValueToString(item, parameterName);

                    if (tempValue == SelectedValue)
                    {
                        selectedIds.Add(item.Id);
                    }
                }
                else if (OperatorValue == Helpers.Operators.Larger.ToString())
                {
                    string tempValue = Helpers.ParameterValueToString(item, parameterName).Replace(" m", "").Replace("\xb2", "").Replace("\xb3", "");

                    if (Double.Parse(tempValue) > Double.Parse(parseValue))
                        selectedIds.Add(item.Id);
                }
                else if (OperatorValue == Helpers.Operators.Smaller.ToString())
                {
                    string tempValue = Helpers.ParameterValueToString(item, parameterName).Replace(" m", "").Replace("\xb2", "").Replace("\xb3", "");

                    if (Double.Parse(tempValue) < Double.Parse(parseValue))
                        selectedIds.Add(item.Id);
                }
                //else must be not equal
                else
                {
                    selectedIds = idTxt.Where(x => Helpers.ParameterValueToString(x, parameterName) != SelectedValue).Select(x => x.Id).ToList();
                }


            };
            }
            catch(Exception ex) { TaskDialog.Show("R", ex.Message); }
            selElements.SetElementIds(selectedIds);

        }//close method

    }
}
