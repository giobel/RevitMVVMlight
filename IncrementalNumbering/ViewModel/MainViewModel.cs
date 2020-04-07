using Autodesk.Revit.UI;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace IncrementalNumbering.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        private Model.RequestHandler Handler { get; set; }
        private readonly ExternalEvent exEvent;
        private readonly UIDocument uidoc = null;


        public Parameter SelectedParameter { get; set; }
        public Category SelectedCategory { get; set; }
        public string OperatorValue { get; set; }
        public string SelectedValue { get; set; }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Handler = new Model.RequestHandler();

            exEvent = ExternalEvent.Create(Handler);

            uidoc = Command._activeRevitUIDoc;            
        }

        //Command to be called from MainWindow
        public RelayCommand IncrementCommand
        {
            get
            {
                return new RelayCommand(Increment);
            }
        }

        private void Increment()
        {
            Handler.m_OperatorValue = OperatorValue;
            Handler.m_ParameterName = SelectedParameter;
            Handler.m_CategorySelected = SelectedCategory;
            Handler.m_SelectedValue = SelectedValue;
            MakeRequest(Model.Request.RequestId.Increment);
        }

        private void MakeRequest(Model.Request.RequestId request)
        {
            Handler.Request.Make(request);
            exEvent.Raise();
        }

    }
}