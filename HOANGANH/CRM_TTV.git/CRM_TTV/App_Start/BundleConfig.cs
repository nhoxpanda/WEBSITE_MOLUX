using System;
using System.Web;
using System.Web.Optimization;

namespace CRM_TTV
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            if (ignoreList == null)
            {
                throw new ArgumentNullException("ignoreList");
            }
            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*-vsdoc.js");
            ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
            ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
        }

        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();
            BundleConfig.AddDefaultIgnorePatterns(bundles.IgnoreList);
            BundleTable.EnableOptimizations = false;
            ScriptBundle scriptBundle = new ScriptBundle("~/bundles/jquery");
            string[] strArrays = new string[] { "~/Scripts/jquery-{version}.js" };
            bundles.Add(scriptBundle.Include(strArrays));
            ScriptBundle scriptBundle1 = new ScriptBundle("~/bundles/jqueryui");
            strArrays = new string[] { "~/Scripts/jquery-ui-{version}.js" };
            bundles.Add(scriptBundle1.Include(strArrays));
            ScriptBundle scriptBundle2 = new ScriptBundle("~/bundles/jqueryval");
            strArrays = new string[] { "~/Scripts/jquery.unobtrusive*", "~/Scripts/jquery.validate*" };
            bundles.Add(scriptBundle2.Include(strArrays));
            ScriptBundle scriptBundle3 = new ScriptBundle("~/bundles/website");
            strArrays = new string[]
            {
                //BEGIN CORE PLUGINS
                "~/Content/assets/global/plugins/bootstrap/js/bootstrap.js",
                "~/Content/assets/global/plugins/bootstrap/js/bootstrap.min.js",
                "~/Content/assets/global/plugins/js.cookie.min.js",
                "~/Content/assets/global/plugins/datatables/datatables.min.js",
                //"~/Content/assets/global/plugins/datatables/plugins/bootstrap/datatables.bootstrap.js",
                "~/Content/assets/global/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js",
                "~/Content/assets/global/plugins/bootstrap-toastr/toastr.min.js",


                "~/Content/assets/global/plugins/bootstrap-hover-dropdown/bootstrap-hover-dropdown.js",
                "~/Content/assets/global/plugins/jquery-slimscroll/jquery.slimscroll.js",
                "~/Content/assets/global/plugins/jquery.blockui.js",
                "~/Content/assets/global/plugins/bootstrap-switch/js/bootstrap-switch.js",
                "~/Scripts/select2/js/select2.js",
                "~/Content/assets/global/plugins/bootstrap-multiselect/bootstrap-multiselect.js",
                "~/Content/assets/global/plugins/bootstrap-daterangepicker/daterangepicker.js",
                "~/Content/assets/global/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js",
                "~/Content/assets/global/plugins/bootstrap-timepicker/js/bootstrap-timepicker.js",
                "~/Content/assets/global/plugins/bootstrap-datetimepicker/js/bootstrap-datetimepicker.js",
                "~/Content/assets/global/plugins/bootstrap-tabdrop/js/bootstrap-tabdrop.js",
                //"~/Content/assets/global/plugins/fullcalendar/fullcalendar.js",
                "~/Scripts/ammap/ammap.js",
                "~/Scripts/ammap/maps/js/worldLow.js",
                "~/Content/assets/global/plugins/jquery-nestable/jquery.nestable.js",
                "~/Content/assets/global/scripts/app.js",
                "~/Content/assets/pages/scripts/ui-nestable.min.js",
                "~/Content/assets/pages/scripts/components-date-time-pickers.js",
                //"~/Content/assets/pages/scripts/dashboard.js",
                "~/Content/assets/layouts/layout4/scripts/layout.js",
                "~/Content/assets/layouts/layout4/scripts/demo.js",
                "~/Content/assets/layouts/global/scripts/quick-sidebar.js"
                 //BEGIN THEME LAYOUT SCRIPTS
            };
            bundles.Add(scriptBundle3.Include(strArrays));
            ScriptBundle scriptBundle4 = new ScriptBundle("~/bundles/modernizr");
            strArrays = new string[] { "~/Scripts/modernizr-*" };
            bundles.Add(scriptBundle4.Include(strArrays));
            StyleBundle styleBundle = new StyleBundle("~/Content/css");
            strArrays = new string[] { "~/Content/site.css" };
            bundles.Add(styleBundle.Include(strArrays));
            StyleBundle styleBundle1 = new StyleBundle("~/Content/themes/base/css");
            strArrays = new string[] 
            {
                "~/Content/themes/base/jquery.ui.core.css",
                "~/Content/themes/base/jquery.ui.resizable.css",
                "~/Content/themes/base/jquery.ui.selectable.css",
                "~/Content/themes/base/jquery.ui.accordion.css",
                "~/Content/themes/base/jquery.ui.autocomplete.css",
                "~/Content/themes/base/jquery.ui.button.css",
                "~/Content/themes/base/jquery.ui.dialog.css",
                "~/Content/themes/base/jquery.ui.slider.css",
                "~/Content/themes/base/jquery.ui.tabs.css",
                "~/Content/themes/base/jquery.ui.datepicker.css",
                "~/Content/themes/base/jquery.ui.progressbar.css",
                "~/Content/themes/base/jquery.ui.theme.css" };
            bundles.Add(styleBundle1.Include(strArrays));
            StyleBundle styleBundle2 = new StyleBundle("~/Content/website");
            strArrays = new string[] 
            {
                "~/Content/assets/layouts/layout4/css/layout.min.css",
                "~/Content/assets/layouts/layout4/font-awesome-4.4.0/css/font-awesome.css",
                "~/Content/assets/global/plugins/simple-line-icons/simple-line-icons.css",
                "~/Content/assets/global/plugins/bootstrap/css/bootstrap.css",
                "~/Content/assets/global/plugins/uniform/css/uniform.default.css",
                "~/Content/assets/global/plugins/bootstrap-switch/css/bootstrap-switch.css",
                "~/Content/assets/global/plugins/select2/css/select2.css",
                "~/Content/assets/global/plugins/select2/css/select2-bootstrap.css",
                "~/Content/assets/global/plugins/bootstrap-multiselect/bootstrap-multiselect.css",
                "~/Content/assets/global/plugins/bootstrap-datepicker/css/bootstrap-datepicker3.css",
                //"~/Content/assets/global/plugins/bootstrap-timepicker/css/bootstrap-timepicker.css",
                "~/Content/assets/global/plugins/bootstrap-datetimepicker/css/bootstrap-datetimepicker.css",
                //"~/Content/assets/global/plugins/fullcalendar/fullcalendar.css",
                "~/Scripts/ammap/ammap.css",
                "~/Content/assets/global/plugins/jquery-nestable/jquery.nestable.css",
                "~/Content/assets/global/css/components.css",
                "~/Content/assets/global/css/plugins.css",
                "~/Content/assets/layouts/layout4/css/layout.css",
                "~/Content/assets/layouts/layout4/css/themes/light.css",
                "~/Content/Site.css",
                "~/Content/assets/global/plugins/bootstrap-toastr/toastr.min.css"
            };
            bundles.Add(styleBundle2.Include(strArrays));
        }
    }
}
