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
            //https://stackoverflow.com/questions/11089104/operation-is-not-valid-while-itemssource-is-in-use-access-and-modify-elements-w
            parameters.SelectedIndex = -1;
            parameters.ClearValue(ItemsControl.ItemsSourceProperty);
            
            parameters.ItemsSource = GetParam(_doc, cboxCategories.SelectedItem as Category);

            parameters.DisplayMemberPath = "Definition.Name";
        }

        private void parameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            parameterValue.ItemsSource = GetParameterValue(_doc, cboxCategories.SelectedItem as Category, parameters.SelectedItem as Parameter);

            operatorValue.SelectedIndex = 0;
        }

        #region HELPERS
        private List<Category> GetCategories(Document doc)
        {
 
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

        private IList<Parameter> GetParam(Document doc, Category cat)
        {
            Element e = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategoryId(cat.Id).WhereElementIsNotElementType().ToElements().First();

            //IList<Parameter> ps = e.Parameters .GetOrderedParameters();
            IList<Parameter> ps = new List<Parameter>(e.Parameters.Size);

            foreach (Parameter parameter in e.Parameters)
            {
                ps.Add(parameter);
            }

            IEnumerable<Parameter> sortedEnum = ps.OrderBy(f => f.Definition.Name);
            IList<Parameter> sortedList = sortedEnum.ToList();

            return sortedList;
        }

        private List<string> GetParameterValue(Document doc, Category cat, Parameter p)
        {
            List<string> results = new List<string> ();

            IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategoryId(cat.Id).WhereElementIsNotElementType().ToElements();

            try
            {
                foreach (Element element in elements)
                {

                    //string paramValue = element.LookupParameter(p.Definition.Name).AsString();
                    string paramValue = Helpers.ParameterValueToString(element, p);

                    if (!results.Contains(paramValue))
                        results.Add(paramValue);
                }

            }

            catch { };

            results.Sort();

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
