using System.Web;
using System.Web.Optimization;

namespace MOLUX
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/header").Include(
                        "~/Content/plugins/jQuery/jquery-2.2.3.min.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Content/loading/jquery-loading.js"));

            bundles.Add(new ScriptBundle("~/bundles/dropzonescripts").Include(
                     "~/Scripts/dropzone/dropzone.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/main.css"));

            bundles.Add(new ScriptBundle("~/bundles/footer").Include(
                        "~/Content/bootstrap/js/bootstrap.min.js",
                        "~/Content/plugins/datepicker/bootstrap-datepicker.js",
                        "~/Content/plugins/slimScroll/jquery.slimscroll.min.js",
                        "~/Content/plugins/fastclick/fastclick.js",
                        "~/Content/dist/js/app.js",
                        "~/Content/dist/js/pages/dashboard.js",
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/jquery.dataTables.columnFilter.js",
                        "~/Content/plugins/select2/select2.js"));

            bundles.Add(new StyleBundle("~/Content/AdminBeeRem").Include(
                        "~/Content/bootstrap/css/bootstrap.css",
                        "~/Content/font-awesome-4.4.0/css/font-awesome.min.css",
                        "~/Content/dist/css/AdminLTE.min.css",
                        "~/Content/dist/css/skins/_all-skins.min.css",
                        "~/Content/plugins/morris/morris.css",
                        "~/Content/plugins/datepicker/datepicker3.css",
                        "~/Content/plugins/daterangepicker/daterangepicker.css",
                        "~/Content/plugins/datatables/jquery.dataTables.min.css",
                        "~/Content/plugins/select2/select2.min.css"));

            bundles.Add(new StyleBundle("~/Content/dropzonescss").Include(
                     "~/Scripts/dropzone/css/basic.css",
                     "~/Scripts/dropzone/css/dropzone.css"));
        }
    }
}
