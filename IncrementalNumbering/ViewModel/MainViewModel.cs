using Autodesk.Revit.UI;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace IncrementalNumbering.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        private Model.RequestHandler Handler { get; set; }
        private readonly ExternalEvent exEvent;
        private readonly UIDocument uidoc = null;


        #region ParameterName
        //string _parameterName;

        public string ParameterName { get; set; }
        //{
        //    get
        //    {
        //        return _parameterName;
        //    }
        //    set
        //    {
        //        if (_parameterName == value)
        //            return;
        //        _parameterName = value;
        //        RaisePropertyChanged("ParameterName");
        //    }
        //}
        #endregion
        
        public string StartNumber { get; set; }

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
            Handler.m_StartNumber = StartNumber;
            Handler.m_ParameterName = ParameterName;
            MakeRequest(Model.Request.RequestId.Increment);
        }

        private void MakeRequest(Model.Request.RequestId request)
        {
            Handler.Request.Make(request);
            exEvent.Raise();
        }

    }
}