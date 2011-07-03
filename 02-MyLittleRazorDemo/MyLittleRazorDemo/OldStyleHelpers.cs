using System;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace MyLittleRazorDemo
{
    public static class OldStyleHelpers
    {
        public static string RawPrice(this HtmlHelper html, decimal value)
        {
            if (value <= 0)
                return "<strong>FREE</strong>";

            return "AUD$ " + value;
        }

        public static IHtmlString Price(this HtmlHelper html, decimal value)
        {
            if (value <= 0)
                return new HtmlString("<strong>FREE</strong>");
            
            return new HtmlString("AUD$ " + value);
        }

        public static HelperResult RenderSection(this WebPageBase webPage, string name, 
                Func<dynamic, HelperResult> defaultContents)
        {
            if (webPage.IsSectionDefined(name))
            {
                return webPage.RenderSection(name);
            }

            return defaultContents(null);
        }
    }
}