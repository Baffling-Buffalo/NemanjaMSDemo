using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClient.TagHelpers
{
    [HtmlTargetElement("li", Attributes = "active-url")]
    public class ActiveTagHelper : TagHelper
    {
        public IHttpContextAccessor ContextAccessor { get; }

        public string ActiveUrl { get; set; }


        public ActiveTagHelper(IHttpContextAccessor contextAccessor)
        {
            ContextAccessor = contextAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ActiveUrl == "home" || ContextAccessor.HttpContext.Request.Path.ToString().Contains(ActiveUrl,StringComparison.OrdinalIgnoreCase))
            {
                var existingAttrs = output.Attributes["class"]?.Value;
                output.Attributes.SetAttribute("class",
                    "active " + existingAttrs.ToString());
            }
        }

    }
}
