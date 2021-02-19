define([
    "dojo/_base/declare",
    "dojo/when",
    "dojo/request",
    "epi/dependency",
    "epi/shell/command/_Command",
    "epi/shell/_ContextMixin",
    "epi-cms/command/_NonEditViewCommandMixin"
],
    function (
        declare,
        when,
        request,
        dependency,
        _Command,
        _ContextMixin,
        _NonEditViewCommandMixin) {
        return declare([_Command, _ContextMixin, _NonEditViewCommandMixin],
            {
                iconClass: "siteimprove-icon",
                canExecute: true,
                label: "Content Check",

                _execute: function() {
                    var scope = this;

                    when(scope.getCurrentContext(),
                        function(context) {
                            if (!context || !context.previewUrl || !context.capabilities)
                                return;

                            if (!context.capabilities.isPage && !context.capabilities.isBlock)
                                return;

                            var previewParams = {};

                            var viewSettingsManager = dependency.resolve("epi.viewsettingsmanager");
                            if (viewSettingsManager) {
                                previewParams = viewSettingsManager.get("previewParams");
                            }

                            previewParams.siteimprovecontext = true;

                            request(context.previewUrl, { query: previewParams }).then(function(html) {
                                    if (context.capabilities.isPage) {
                                        request.get('/siteimprove/pageUrl',
                                            {
                                                query: { contentId: context.id, locale: context.language },
                                                handleAs: 'json'
                                            }).then(function(response) {
                                            scope.pushHtml(html, response.isDomain ? "" : response.url);
                                        });
                                    } else {
                                        scope.pushHtml(html, "");
                                    }
                                }
                            );
                        });
                },
                pushHtml: function(html, pageUrl) {
                    request.get('/siteimprove/token', { handleAs: 'json' })
                        .then(function(token) {
                            var si = window._si || [];

                            si.push([
                                'onHighlight',
                                function(highlightInfo) {
                                    document.querySelector(highlightInfo.highlights[0].selector).style =
                                        "background-color:yellow";
                                }
                            ]);

                            si.push([
                                'contentcheck', html, pageUrl, token, function(contentId) {}
                            ]);
                        });
                }
            });
    }
);