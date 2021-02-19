define([
    "dojo/_base/declare",
    "dijit/form/ToggleButton",
    "epi-cms/component/command/_GlobalToolbarCommandProvider",
    "siteimprove/ContentCheckCommand"
], function (
    declare,
    ToggleButton,
    _GlobalToolbarCommandProvider,
    ContentCheckCommand
) {
    return declare([_GlobalToolbarCommandProvider], {
        constructor: function () {
            this.inherited(arguments);

            var settings = {
                widget: ToggleButton,
                "class": "epi-chromeless epi-mediumButton"
            };

            this.addToTrailing(new ContentCheckCommand({order: -20000}), settings);
        }
    });
});