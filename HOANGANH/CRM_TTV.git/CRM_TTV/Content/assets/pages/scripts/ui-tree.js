var UITree = function () {

    var handleSample2 = function () {
        $.getJSON(host + 'Roles/FreeViewAll', function (jData) {
            $('#tree_2').jstree({
                'plugins': ["wholerow", "checkbox", "types"],
                'core': {
                    "themes": {
                        "responsive": false
                    },
                    'data': jData
                },
                "types": {
                    "default": {
                        "icon": "fa fa-folder icon-state-warning icon-lg"
                    },
                    "file": {
                        "icon": "fa fa-file icon-state-warning icon-lg"
                    }
                }
            });
        });
    }



    return {
        //main function to initiate the module
        init: function () {
            handleSample2();
        }

    };

}();

if (App.isAngularJsApp() === false) {
    jQuery(document).ready(function () {
        UITree.init();
    });
}