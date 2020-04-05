#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Interop;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace IncrementalNumbering
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public static UIDocument _activeRevitUIDoc { get; private set; }

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            MainWindow mw = new MainWindow(uidoc.Document);

            //https://thebuildingcoder.typepad.com/blog/2012/10/ensure-wpf-add-in-remains-in-foreground.html
            //https://thebuildingcoder.typepad.com/blog/2018/11/revit-window-handle-and-parenting-an-add-in-form.html
            IWin32Window revit_window = new JtWindowHandle(Autodesk.Windows.ComponentManager.ApplicationWindow);

            WindowInteropHelper helper = new WindowInteropHelper(mw);

            helper.Owner = revit_window.Handle;

            mw.Show();

            return Result.Succeeded;
        }
    }

    public class JtWindowHandle : IWin32Window
    {
        IntPtr _hwnd;

        public JtWindowHandle(IntPtr h)
        {
            Debug.Assert(IntPtr.Zero != h, "expected non-null window handle");

            _hwnd = h;
        }

        public IntPtr Handle
        {
            get
            {
                return _hwnd;
            }
        }
    }
}
