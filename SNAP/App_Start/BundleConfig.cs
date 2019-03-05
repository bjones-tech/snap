using System.Web;
using System.Web.Optimization;

namespace SNAP
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/snap").Include(
                        "~/Scripts/snap.js",
                        "~/Scripts/snap-datepickers.js"));

            bundles.Add(new ScriptBundle("~/bundles/snap-emailaddress").Include(
                        "~/Scripts/snap-emailaddress.js"));

            bundles.Add(new ScriptBundle("~/bundles/snap-newhire").Include(
                        "~/Scripts/snap-newhire.js"));

            bundles.Add(new ScriptBundle("~/bundles/snap-licenses").Include(
                        "~/Scripts/snap-licenses.js"));

            bundles.Add(new ScriptBundle("~/bundles/snap-lastday").Include(
                        "~/Scripts/snap-lastday.js"));

            bundles.Add(new ScriptBundle("~/bundles/snap-aduser").Include(
                        "~/Scripts/snap-aduser.js"));

            bundles.Add(new ScriptBundle("~/bundles/snap-distributionGroup").Include(
                        "~/Scripts/snap-distributionGroup.js"));

            bundles.Add(new ScriptBundle("~/bundles/snap-softwareupdates").Include(
                        "~/Scripts/snap-softwareupdates.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/themes/base/all.css",
                        "~/Content/bootstrap.css",
                        "~/Content/SiteDD.css",
                        "~/Content/Site.css"));

            BundleTable.EnableOptimizations = false;
        }
    }
}
