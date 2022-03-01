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
        public static void Increment(UIApplication uiapp, Category CategorySelected, Parameter parameterName, string OperatorValue, string SelectedValue, bool WholeProject, bool SelectedViewports)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uiapp.ActiveUIDocument.Document;

            Selection selElements = null;
            IList<ElementId> viewportViewIds = null;

            if (!SelectedViewports)
            {
                selElements = uidoc.Selection;
            }
            else
            {
                ICollection<ElementId> viewportIds = uidoc.Selection.GetElementIds();

                viewportViewIds = new List<ElementId>();

                foreach (ElementId r in viewportIds)
                {
                    Viewport vp = doc.GetElement(r) as Viewport;
                    viewportViewIds.Add(vp.ViewId);
                }
            }


            ICollection<Element> idTxt = new List<Element>();

            if (CategorySelected.Name == "Sheets")
            {
                idTxt = new FilteredElementCollector(doc).OfCategoryId(CategorySelected.Id).ToElements();
            }
            else
            {
                if (WholeProject)
                {
                    idTxt = new FilteredElementCollector(doc).OfCategoryId(CategorySelected.Id).ToElements();
                }
                else if (SelectedViewports)
                {
                    foreach (ElementId viewId in viewportViewIds)
                    {
                        IList<Element> elements = new FilteredElementCollector(doc, viewId).OfCategoryId(CategorySelected.Id).ToElements();

                        foreach (Element item in elements)
                        {
                            idTxt.Add(item);
                        }                        
                    }
                    
                }
                else
                {
                    idTxt = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategoryId(CategorySelected.Id).ToElements();
                }

            }

            ICollection<ElementId> selectedIds = new List<ElementId>();

            //TaskDialog.Show("r", parseValue);
            try
            {
            foreach (Element item in idTxt)
            {

                if (SelectedValue == null)
                {
                    selectedIds.Add(item.Id);
                }
                    else
                    {
                        string parseValue = SelectedValue.Replace(" m", "").Replace("\xb2", "").Replace("\xb3", "");
                        
                        if (OperatorValue == Helpers.Operators.Equal.ToDescription())
                        {
                            string tempValue = Helpers.ParameterValueToString(item, parameterName);

                            if (tempValue == SelectedValue)
                            {
                                selectedIds.Add(item.Id);
                            }
                        }
                        else if (OperatorValue == Helpers.Operators.Larger.ToDescription())
                        {
                            string tempValue = Helpers.ParameterValueToString(item, parameterName).Replace(" m", "").Replace("\xb2", "").Replace("\xb3", "");

                            if (Double.Parse(tempValue) > Double.Parse(parseValue))
                                selectedIds.Add(item.Id);
                        }
                        else if (OperatorValue == Helpers.Operators.Smaller.ToDescription())
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
                    }



            };
            }
            catch(Exception ex) { TaskDialog.Show("R", ex.Message); }

            //selElements.SetElementIds(selectedIds);
            uidoc.Selection.SetElementIds(selectedIds);

        }//close method

    }
}
