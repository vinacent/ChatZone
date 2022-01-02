using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using static System.Int32;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ChatZone.TagHelpers
{
    public class PaginateTagHelper : TagHelper
    {
        private const string ActionAttributeName = "page-action";
        private const string ControllerAttributeName = "page-controller";
        private const string AreaAttributeName = "page-area";
        private const string PageAttributeName = "page-page";
        private const string PageHandlerAttributeName = "page-page-handler";
        private const string FragmentAttributeName = "page-fragment";
        private const string HostAttributeName = "page-host";
        private const string ProtocolAttributeName = "page-protocol";
        private const string RouteAttributeName = "page-route";
        private const string RouteValuesDictionaryName = "page-all-route-data";
        private const string RouteValuesPrefix = "page-route-";
        private IDictionary<string, string> _routeValues;
        private const string Href = "href";
        private bool _routeLink, _pageLink;
        private IHtmlGenerator Generator { get; }

        public PaginateTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        #region AttributeName

        /// <summary>
        /// The name of the action method.
        /// </summary>
        [HtmlAttributeName(ActionAttributeName)]
        public string Action { get; set; }

        [HtmlAttributeName(ControllerAttributeName)]
        public string Controller { get; set; }

        [HtmlAttributeName(ProtocolAttributeName)]
        public string Protocol { get; set; }

        [HtmlAttributeName(HostAttributeName)] public string Host { get; set; }
        [HtmlAttributeName(AreaAttributeName)] public string Area { get; set; }

        [HtmlAttributeName(FragmentAttributeName)]
        // ReSharper disable once MemberCanBePrivate.Global
        public string Fragment { get; set; }

        [HtmlAttributeName(RouteAttributeName)]
        // ReSharper disable once MemberCanBePrivate.Global
        public string Route { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        [HtmlAttributeName(PageAttributeName)] public string Page { get; set; }

        [HtmlAttributeName(PageHandlerAttributeName)]
        // ReSharper disable once MemberCanBePrivate.Global
        public string PageHandler { get; set; }

        /// <summary>
        /// Additional parameters for the route.
        /// </summary>
        [HtmlAttributeName(RouteValuesDictionaryName, DictionaryAttributePrefix = RouteValuesPrefix)]
        public IDictionary<string, string> RouteValues
        {
            get
            {
                if (_routeValues == null)
                {
                    _routeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }

                return _routeValues;
            }
            set => _routeValues = value;
        }

        [HtmlAttributeName("skip-count")] public int SkipCount { get; set; }

        [HtmlAttributeName("max-result-count")]
        public int MaxResultCount { get; set; }

        [HtmlAttributeName("total-count")] public long TotalCount { get; set; }

        private bool HasPreviousPage => SkipCount > 0;

        private bool HasNextPage => SkipCount + MaxResultCount < TotalCount;

        #endregion

        [HtmlAttributeNotBound] [ViewContext] public ViewContext ViewContext { get; set; }
        private int PageIndex => (int)Math.Ceiling((float)SkipCount / MaxResultCount) + 1;
        private int TotalPages => (int)Math.Ceiling((float)TotalCount / MaxResultCount);

        private string GetString(Microsoft.AspNetCore.Html.IHtmlContent content)
        {
            using var writer = new System.IO.StringWriter();
            content.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);

            return writer.ToString();
        }

        private string GenerateTag(string linkText, string cssClass, string page)
        {
            TagBuilder tagBuilder;
            TryParse(page, out var currentPage);

            long skipCount = (currentPage - 1) * MaxResultCount;

            _routeValues ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!_routeValues.ContainsKey("maxResultCount"))
                _routeValues.Add("maxResultCount", MaxResultCount.ToString());
            else
            {
                _routeValues["maxResultCount"] = MaxResultCount.ToString();
            }

            if (!_routeValues.ContainsKey("skipCount")) _routeValues.Add("skipCount", skipCount.ToString());
            else
            {
                _routeValues["skipCount"] = skipCount.ToString();
            }

            var routeValues = new RouteValueDictionary(_routeValues);
            if (_pageLink)
            {
                tagBuilder = Generator.GeneratePageLink(
                    ViewContext,
                    linkText: linkText,
                    pageName: Page,
                    pageHandler: PageHandler,
                    protocol: Protocol,
                    hostname: Host,
                    fragment: Fragment,
                    routeValues: routeValues,
                    htmlAttributes: null);
            }
            else if (_routeLink)
            {
                tagBuilder = Generator.GenerateRouteLink(
                    ViewContext,
                    linkText: linkText,
                    routeName: Route,
                    protocol: Protocol,
                    hostName: Host,
                    fragment: Fragment,
                    routeValues: routeValues,
                    htmlAttributes: null);
            }
            else
            {
                tagBuilder = Generator.GenerateActionLink(
                    ViewContext,
                    linkText: linkText,
                    actionName: Action,
                    controllerName: Controller,
                    protocol: Protocol,
                    hostname: Host,
                    fragment: Fragment,
                    routeValues: routeValues,
                    htmlAttributes: null);
            }

            var currentHref = tagBuilder.Attributes["href"];
            if (currentHref.Contains("?"))
            {
                var split = currentHref.Split("?");
                split[0] = HttpUtility.UrlDecode(split[0]);
                currentHref = string.Join("?", split);
            }
            else
            {
                currentHref = HttpUtility.UrlDecode(currentHref);
            }

            if (!string.IsNullOrWhiteSpace(Area))
            {
                currentHref = $"/{Area}/{currentHref}";
            }

            tagBuilder.Attributes["href"] = currentHref.Replace("/?", "?").TrimEnd('/');

            tagBuilder.TagRenderMode = TagRenderMode.StartTag;
            var listCssClass = cssClass.Split(' ');
            foreach (var css in listCssClass)
                tagBuilder.AddCssClass(css);
            if (linkText == page && PageIndex.ToString() == page)
            {
                // tagBuilder.MergeAttribute("style", "background-color:" + ActiveColor + ";");
            }

            return GetString(tagBuilder) + linkText + "</a>";
        }

        private string IsActive(int i, int pageindex)
        {
            return i == pageindex ? " active" : "";
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            _routeLink = Route != null;
            _pageLink = Page != null || PageHandler != null;

            var myClass = context.AllAttributes["class"]?.Value ??
                          "pagination";
            output.TagName = "ul";
            output.Attributes.SetAttribute("class", myClass);
            var item = "";

            if (TotalPages <= 5)
                for (var i = 1; i <= TotalPages; i++)
                {
                    var disabled = ""; //PageIndex == i ? "disabled" : "";
                    item += $"<li class=\"page-item{disabled}{IsActive(i, PageIndex)}\">" +
                            GenerateTag(i.ToString(), "page-link", i.ToString()) +
                            "</li>";
                }
            else
            {
                var prev = PageIndex - TotalPages + 2 > 0 ? PageIndex - TotalPages + 2 : 0;
                var next = 0;
                if ((PageIndex - 2) > 1)
                {
                    item += "<li class=\"page-item\"><a class=\"page-link\">...</a></li>";
                }

                const string disabled = ""; //PageIndex == i ? "disabled" : "";
                for (var i = PageIndex - 2 - prev; i <= PageIndex + 2 + next; i++)
                {
                    if (i < 1)
                    {
                        next++;
                        continue;
                    }

                    if (i > TotalPages)
                    {
                        continue;
                    }

                    item += "<li class=\"page-item " + disabled + " " + IsActive(i, PageIndex) + "\">" +
                            GenerateTag(i.ToString(), "page-link", i.ToString()) + "</li>";
                }

                if ((PageIndex + 2) < TotalPages)
                    item += "<li class=\"page-item\"><a class=\"page-link\">...</a></li>";
            }

            var prevDisabled = !HasPreviousPage ? "disabled" : "";
            var nextDisabled = !HasNextPage ? "disabled" : "";

            output.Content.SetHtmlContent(
                "<li class=\"page-item " + prevDisabled + "\">" +
                GenerateTag("<i class=\"mdi mdi-chevron-double-left\"></i>", "page-link", "1") + "</li>&nbsp;" +
                "<li class=\"page-item " + prevDisabled + "\">" +
                GenerateTag("<i class=\"mdi mdi-chevron-left\"></i>", "page-link", (PageIndex - 1).ToString()) +
                "</li>" +
                item +
                "<li class=\"page-item " + nextDisabled + "\">" +
                GenerateTag("<i class=\"mdi mdi-chevron-right\"></i>", "page-link", (PageIndex + 1).ToString()) +
                "</li>&nbsp;" +
                "<li class=\"page-item " + nextDisabled + "\">" +
                GenerateTag("<i class=\"mdi mdi-chevron-double-right\"></i>", "page-link", TotalPages.ToString()) +
                "</li>"
            );
        }
    }
}