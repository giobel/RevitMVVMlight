using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using System.Linq;

namespace IncrementalNumbering
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Document _doc = null;

        public MainWindow(Document doc)
        {
            _doc = doc;
            InitializeComponent();
            cboxCategories.ItemsSource = GetCategories(doc);
            cboxCategories.DisplayMemberPath = "Name";

            operatorValue.ItemsSource = new List<string> { "=", ">", "<", "!=" };
            
            
        }
        private void CategoriesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            parameters.SelectedIndex = -1;
            parameters.ClearValue(ItemsControl.ItemsSourceProperty);
            
            parameters.ItemsSource = GetParamValues(_doc, cboxCategories.SelectedItem as Category);

            parameters.DisplayMemberPath = "Definition.Name";
        }

        private void parameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            parameterValue.ItemsSource = GetParameterValue(_doc, cboxCategories.SelectedItem as Category, parameters.SelectedItem as Parameter);
        }

        #region HELPERS
        private List<Category> GetCategories(Document doc)
        {
            //List<Category> catNames = new List<Category>();

            //foreach (Category c in doc.Settings.Categories)
            //{
            //    if (c.AllowsBoundParameters)

            //        catNames.Add(c);
            //}
            ////catNames.Sort();
            //return catNames;
            //Create collector to collect all elements on active view
            FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);

            //get distinct categories of elements in the active view
            List<Category> categories =
                            collector
                                .ToElements()
                                .Select(x => x.Category)
                                .Where(x => x != null && x.Name != null)
                                .Distinct(new CategoryComparer())
                                .OrderBy(x => x.Name)
                                .ToList();


            return categories;
        }

        private IList<Parameter> GetParamValues(Document doc, Category cat)
        {
            Element e = new FilteredElementCollector(doc).OfCategoryId(cat.Id).WhereElementIsNotElementType().ToElements().First();
            // Two choices: 
            // Element.Parameters property -- Retrieves 
            // a set containing all  the parameters.
            // GetOrderedParameters method -- Gets the 
            // visible parameters in order.

            IList<Parameter> ps = e.GetOrderedParameters();

            //List<string> param_names = new List<string>(
            //  ps.Count);

            //foreach (Parameter p in ps)
            //{
            //    // AsValueString displays the value as the 
            //    // user sees it. In some cases, the underlying
            //    // database value returned by AsInteger, AsDouble,
            //    // etc., may be more relevant.

            //    param_names.Add(p.Definition.Name);
            //}
            //return param_names;
            return ps;
        }

        private List<string> GetParameterValue(Document doc, Category cat, Parameter p)
        {
            List<string> results = new List<string> ();

            IList<Element> elements = new FilteredElementCollector(doc).OfCategoryId(cat.Id).WhereElementIsNotElementType().ToElements();

            try
            {
                foreach (Element element in elements)
                {
                    string paramValue = element.LookupParameter(p.Definition.Name).AsValueString();
                    if (!results.Contains(paramValue))
                        results.Add(paramValue);
                }

            }

            catch { };

            return results;
        }
        #endregion
    }

    class CategoryComparer : IEqualityComparer<Category>
    {
        #region Implementation of IEqualityComparer<in Category>

        public bool Equals(Category x, Category y)
        {
            if (x == null || y == null) return false;

            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(Category obj)
        {
            return obj.Id.IntegerValue;
        }

        #endregion
    }
}
