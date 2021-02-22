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
                _execute: function () {
                    var scope = this;
                    when(scope.getCurrentContext(),
                        function (context) {
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
                            request(context.previewUrl, { query: previewParams }).then(function (html) {
                                if (context.capabilities.isPage) {
                                    request.get('/siteimprove/pageUrl',
                                        {
                                            query: { contentId: context.id, locale: context.language },
                                            handleAs: 'json'
                                        }).then(function (response) {
                                            scope.pushHtml(html, response.isDomain ? "" : response.url);
                                        });
                                } else {
                                    scope.pushHtml(html, "");
                                }
                            }
                            );
                        });
                },
                pushHtml: function (html, pageUrl) {
                    request.get('/siteimprove/token', { handleAs: 'json' })
                        .then(function (token) {
                            var si = window._si || [];
                            si.push([
                                'onHighlight',
                                function (highlightInfo) {
                                    var iframe = document.querySelector('iframe[name="sitePreview"]');
                                    var idocument = iframe.contentWindow.document;
                                    // Styling
                                    var stylingId = 'siteimprove-styling';
                                    var styling = idocument.getElementById(stylingId);
                                    if (!styling) {
                                        idocument.body.insertAdjacentHTML("beforeend", `
                                            <style type="text/css" id="${stylingId}">
                                                .siteimprove-highlight {
                                                  animation: siteimprove-pulse 0.7s;
                                                  animation-iteration-count: 4;
                                                  outline: 5px solid transparent;
                                                }
                                                .siteimprove-highlight-text {
                                                  animation: siteimprove-pulse 0.7s;
                                                  animation-iteration-count: 4;
                                                  outline: 5px solid transparent;
                                                }
                                                @keyframes siteimprove-pulse {
                                                  from { outline-color: transparent; }
                                                  50% { outline-color: #ffc107; }
                                                  to { outline-color: transparent; }
                                                }
                                            </style>
                                        `);
                                    }
                                    // Highlight config
                                    var highlightClass = 'siteimprove-highlight';
                                    var highlightText = 'siteimprove-highlight-text';
                                    var removeAfterTime = 3000;
                                    // Add highlight
                                    highlightInfo.highlights.forEach((info, index) => {
                                        var selector = info.selector;
                                        var element = idocument.querySelector(selector);
                                        if (element) {
                                            if (index === 0) element.scrollIntoView({ behavior: "smooth" });
                                            // Text
                                            if (info.offset) {
                                                var text = element.innerHTML;
                                                var start = info.offset.start;
                                                var end = info.offset.start + info.offset.length;
                                                var word = text.slice(start, end);
                                                var beforeWord = text.slice(0, start);
                                                var afterWord = text.slice(end);
                                                element.innerHTML = `${beforeWord}<span class="${highlightText}">${word}</span>${afterWord}`;
                                                // Cleanup
                                                setTimeout(() => {
                                                    element.innerHTML = text;
                                                }, removeAfterTime);
                                            }
                                            // Image
                                            if (element.tagName === 'IMG') {
                                                element.classList.add(highlightClass);
                                                // Cleanup
                                                setTimeout(() => {
                                                    element.classList.remove(highlightClass);
                                                }, removeAfterTime);
                                            }
                                        }
                                    })
                                }
                            ]);
                            si.push([
                                'contentcheck', html, pageUrl, token, function (contentId) { }
                            ]);
                        });
                }
            });
    }
);