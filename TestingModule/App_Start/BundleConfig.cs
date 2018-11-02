using System.Web;
using System.Web.Optimization;

namespace TestingModule
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/jquery.signalR-{version}.js",
                        "~/Scripts/hubgeneration.js",
                        "~/Scripts/chart.js",
                        "~/Scripts/chart.pieceLabel.js",
                        "~/Scripts/canvas.lib.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                        "~/Scripts/jquery-ui*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/font-awesome.css",
                      "~/Content/themes/base/jquery-ui.css",
                      "~/Content/themes/base/theme.css",
                      "~/Content/styles/*.css"));
        }
    }
}
