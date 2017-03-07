
define([
    "dojo",
    "dojo/_base/declare",
    "epi/_Module",
    "dojo/topic",
    "dojo/request"
], function (
    dojo,
    declare,
    _Module,
    topic,
    request
) {
    return declare([_Module], {

        isPublishing: false,

        initialize: function () {
            this.inherited(arguments);

            topic.subscribe('/epi/shell/context/changed', function (content, ctx) {
                if (!this.isPublishOrViewContext(content, ctx)) {
                    return;
                }

                request.get('/siteimprove/pageUrl',
                    {
                        query: {
                            contentId: content.id.split('_')[0],
                            locale: content.language
                        },
                        handleAs: 'json'
                    })
                .then(function (response) {

                    if (this.isPublishing) {
                        this.pushSi("recheck", response);
                        this.isPublishing = false;

                    } else {
                        this.pushSi("input", response);
                    }

                }.bind(this));
               
            }.bind(this));

            topic.subscribe('/epi/cms/content/statuschange/', function (status, page) {
                if (status === 'Publish') {
                    this.isPublishing = true;
                }
            }.bind(this));
        },

        /**
         * Request token from backoffice and sends request to Siteimprove
         */
        pushSi: function (method, url) {
            var si = window._si || [];

            request.get('/siteimprove/token', { handleAs: 'json' })
            .then(function (response) {

                // Send of to siteimprove
                si.push([method, url, response, function () {
                    console.log('SiteImprove pass: ' + method + ' - ' + url);
                }]);
            }.bind(this));
        },

        /**
         * Helper method for event: /epi/shell/context/changed'
         */
        isPublishOrViewContext: function (content, ctx) {
            // Not intereseted if there is no
            if (!content || !content.publicUrl) {
                return false;
            }

            // If it's not a page ignore it
            if (content.capabilities && !content.capabilities.isPage) {
                return false;
            }

            if (ctx.trigger && !this.isPublishing) {
                return false;
            }

            return true;
        }
    });
});