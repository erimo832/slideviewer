using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlHub.Model
{
    public class ViewCommand
    {
        public string Message { get; set; }
        public string MessageCss { get; set; }
        public string Url { get; set; }

        public bool Validate(out string error)
        {
            try
            {
                error = string.Empty;
                

                //Check Url
                var okUri = Uri.IsWellFormedUriString(Url, UriKind.RelativeOrAbsolute);

                if (!okUri)
                {
                    error += "Invalid url";
                }
                
                //Check css

                return true;
            }
            catch(Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}