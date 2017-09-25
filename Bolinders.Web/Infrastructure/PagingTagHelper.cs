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
            // If there is only 1 page, show no pagination
            if (PageModel.Pager.TotalPages == 1)
            {
                output.Content.AppendHtml("");
            }

            // Generate a querystring if search or filter is active.
            string queryString = "";
            bool first = true;

            foreach (var prop in PageModel.SeachModel.GetType().GetProperties())
            {
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

            // If querystring have values, add a ? mark in begining
            if (queryString != "")
            {
                queryString = "?" + queryString;
            }

            // Builds the pagination link
            IUrlHelper urlHelper = _helper.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");

            var stopPage = PageModel.Pager.TotalPages;
            var currentPage = PageModel.Pager.CurrentPage;
            var i = 1;

            // If there is more than 11 pages, only show 5 before and 5 after.
            if (stopPage >= 12)
            {
                if (currentPage >= i + 5 && currentPage <= stopPage - 5)
                {
                    i = currentPage - 5;
                    stopPage = currentPage + 5;
                }
                else if (currentPage >= stopPage - 5)
                {
                    i = stopPage - 11;
                }
                else if (currentPage <= i + 5)
                {
                    stopPage = i + 10;
                }
            }

            // Adds a "<<" link before pagination if currentPage > 1
            if (currentPage > 1)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.Attributes["href"] = urlHelper.Action(PageAction,
                    new { page = currentPage - 1 }) + queryString;
                if (EnablePageClasses)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(PageClassNormal);
                }
                tag.InnerHtml.Append("<<");
                result.InnerHtml.AppendHtml(tag);
            }

            // Generates the pagination links
            for (; i <= stopPage; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.Attributes["href"] = urlHelper.Action(PageAction,
                    new { page = i }) + queryString;
                if (EnablePageClasses)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == currentPage ? PageClassSelected : PageClassNormal);
                }
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }

            // Adds a ">>" link after pagination if currentPage < stopPage
            if (currentPage < stopPage)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.Attributes["href"] = urlHelper.Action(PageAction,
                    new { page = currentPage + 1 }) + queryString;
                if (EnablePageClasses)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(PageClassNormal);
                }
                tag.InnerHtml.Append(">>");
                result.InnerHtml.AppendHtml(tag);
            }

            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}
