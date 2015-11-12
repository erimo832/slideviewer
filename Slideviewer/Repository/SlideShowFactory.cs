using ControlHub.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ControlHub.Repository
{
    public static class SlideShowFactory
    {
        public static IList<Slide> GetSlideShow(string name)
        {
            string path = string.Format("slideshows/{0}/", name);

            var o = new DirectoryInfo(string.Format("{0}{1}", System.Web.HttpRuntime.AppDomainAppPath, path));

            var result = new List<Slide>();

            foreach (var item in o.GetFiles())
            {
                if (item.FullName.EndsWith("txt"))
                {
                    foreach (var row in File.ReadLines(item.FullName))
                    {
                        result.Add(new Slide 
                        {
                            Type = "message",
                            Css = "font-size: 60px; width: 1100px;", // must need db or some kind of style file
                            Value = row
                        });
                    }
                }
                else if (item.FullName.EndsWith("pages"))
                {
                    foreach (var row in File.ReadLines(item.FullName))
                    {
                        result.Add(new Slide
                        {
                            Type = "url",
                            Css = "width: 100%; height: 100%", // must need db or some kind of style file
                            Value = row
                        });
                    }
                }
                else if (item.FullName.ToLower().EndsWith("jpg") || item.FullName.ToLower().EndsWith("jpeg") || item.FullName.ToLower().EndsWith("gif")
                    || item.FullName.ToLower().EndsWith("gifv") || item.FullName.ToLower().EndsWith("png"))
                {
                    result.Add(new Slide 
                    { 
                        Type = "image",
                        Css = "max-width: 100%; max-height: 820px;", // must need db or some kind of style file
                        Value = string.Format("{0}{1}", path, item.Name)
                    });
                }
            }

            return result;
        }
    }
}