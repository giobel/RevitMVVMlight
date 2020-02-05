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
    public class RequestHandler : IExternalEventHandler
    {
        public string m_StartNumber { get; set; }
        public string m_ParameterName { get; set; }

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
                            Model.IncrementNumber.Increment(uiapp, m_ParameterName, m_StartNumber);
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
