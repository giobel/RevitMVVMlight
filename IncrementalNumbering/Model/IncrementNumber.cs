using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Linq;

namespace IncrementalNumbering.Model
{
    class IncrementNumber
    {
        public static void Increment(UIApplication uiapp, string parameterName, string startNumber)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uiapp.ActiveUIDocument.Document;

            double convertedStartNumber = Double.Parse(startNumber);
            bool flag = true;

            while (flag)
            {
                try
                {
                    using (Transaction tran = new Transaction(doc, "Text note updated"))
                    {
                        tran.Start();

                        Reference reference = uidoc.Selection.PickObject(ObjectType.Element, "Pick elements in the desired order and hit ESC to stop picking.");

                        Element pile = doc.GetElement(reference);

                        pile.LookupParameter(parameterName).Set($"Pile {convertedStartNumber}");

                        tran.Commit();

                        convertedStartNumber++;
                    }

                    //RequestHandler handler = new Model.RequestHandler(); 
                    //handler.Request.Make(Request.RequestId.Increment);

                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Error", ex.Message);
                    flag = false;
                }
            }

        }//close method

    }
}
