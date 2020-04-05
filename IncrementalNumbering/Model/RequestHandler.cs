using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace IncrementalNumbering.Model
{
    /// <summary>
    /// https://github.com/jeremytammik/RevitSdkSamples/tree/master/SDK/Samples/ModelessDialog/ModelessForm_ExternalEvent/CS
    /// </summary>
    public class RequestHandler : IExternalEventHandler
    {
        public string m_OperatorValue { get; set; }
        public Category m_CategorySelected { get; set; }
        public Parameter m_ParameterName { get; set; }
        public string m_SelectedValue { get; set; }

        private Request m_request = new Request();
        

        public Request Request {
            get {
                return this.m_request;
            }
        }
        public void Execute(UIApplication uiapp)
        {
            try
            {
                switch (Request.Take())
                {
                    case Model.Request.RequestId.None:
                        {
                            return;  // no request at this time -> we can leave immediately
                        }
                    case Model.Request.RequestId.Increment:
                        {
                            Model.IncrementNumber.Increment(uiapp, m_CategorySelected, m_ParameterName, m_OperatorValue, m_SelectedValue);
                            break;
                        }
                    default:
                        {
                            // some kind of a warning here should
                            // notify us about an unexpected request 
                            break;
                        }
                }
            }
            finally
            {
                //Application.thisApp.WakeFormUp();
            }
        }

        public string GetName()
        {
            return "External Event MVVM";
        }
    }
}
