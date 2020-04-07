using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
using System.ComponentModel;

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

            operatorValue.ItemsSource = new List<string> {  Helpers.Operators.Equal.ToDescription(),
                                                            Helpers.Operators.Larger.ToDescription(),
                                                            Helpers.Operators.Smaller.ToDescription(),
                                                            Helpers.Operators.Not_Equal.ToDescription()
                                                        };
            
            
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
            try
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
            catch
            {
                return null;
            }
        }

        private IList<Parameter> GetParam(Document doc, Category cat)
        {
            IList<Parameter> ps = new List<Parameter>();

            try
            {
                if (cat != null)
                {
                    Element e = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategoryId(cat.Id).WhereElementIsNotElementType().ToElements().First();

                    //IList<Parameter> ps = e.Parameters .GetOrderedParameters();

                    foreach (Parameter parameter in e.Parameters)
                    {
                        ps.Add(parameter);
                    }
                }
                IEnumerable<Parameter> sortedEnum = ps.OrderBy(f => f.Definition.Name);
                IList<Parameter> sortedList = sortedEnum.ToList();

                return sortedList;
            }
            catch { return null; }


        }

        private List<string> GetParameterValue(Document doc, Category cat, Parameter p)
        {
            
            try
            {
                List<string> results = new List<string>();

                IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategoryId(cat.Id).WhereElementIsNotElementType().ToElements();

                foreach (Element element in elements)
                {           
                //string paramValue = element.LookupParameter(p.Definition.Name).AsString();
                string paramValue = Helpers.ParameterValueToString(element, p);

                if (paramValue != null && !results.Contains(paramValue))
                    results.Add(paramValue);
                }
                List<string> sortedResult = results.OrderBy(x => PadNumbers(x)).ToList();


                return sortedResult;
            }

            catch
            {
                return null; 
            };


        }

        public static string PadNumbers(string input)
        {
            return Regex.Replace(input, "[0-9]+", match => match.Value.PadLeft(10, '0'));
        }

        #endregion



    }
    internal static class Extensions
    {
        public static string ToDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }
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
