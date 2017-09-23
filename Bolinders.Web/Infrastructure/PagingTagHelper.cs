using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bolinders.Core.ViewModels;
using Bolinders.Core.Models.ViewModels;

namespace Bolinders.Web.Infrastructure
{
    [HtmlTargetElement("div", Attributes = "page-model,page-action")]
    public class PagingTagHelper : TagHelper
    {
        private IUrlHelperFactory _helper;

        public PagingTagHelper(IUrlHelperFactory helper)
        {
            _helper = helper;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public VehicleListViewModel PageModel { get; set; }
        public string PageAction { get; set; }
        public bool EnablePageClasses { get; set; }
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string queryString = "";
            bool first = true;
            List<string> keys = new List<string>();

            foreach (var prop in PageModel.SeachModel.GetType().GetProperties())
            {
                keys.Add(prop.Name);
                if (prop.GetValue(PageModel.SeachModel, null) != null)
                {
                    if (!first)
                    {
                        queryString = queryString + "&";
                    }
                    queryString = queryString + prop.Name.ToString() + "=" + prop.GetValue(PageModel.SeachModel, null).ToString();
                    first = false;
                }
            }

            if (queryString != "")
            {
                queryString = "?" + queryString;
            }

            var test = keys;


            //int j = 1;
            //foreach (var prop in PageModel.SeachModel.GetType().GetProperties())
            //{
            //    if (j == 1) { queryString = "&"; }
            //    if (prop.GetValue(PageModel.SeachModel, null) != null)
            //    {
            //        queryString = string.Concat(queryString, prop.GetValue(PageModel.SeachModel, null));
            //    }
            //    j++;
            //}
            //if (queryString != null)
            //{
            //    queryString = string.Concat("?", queryString);
            //}

            //var antal = j;



            //var hrefString = "?used=true";


            IUrlHelper urlHelper = _helper.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");
            
            for (int i = 1; i <= PageModel.Pager.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.Attributes["href"] = urlHelper.Action(PageAction,
                    new { page = i }) + queryString;
                if (EnablePageClasses)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.Pager.CurrentPage ? PageClassSelected : PageClassNormal);
                }
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }

            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}
