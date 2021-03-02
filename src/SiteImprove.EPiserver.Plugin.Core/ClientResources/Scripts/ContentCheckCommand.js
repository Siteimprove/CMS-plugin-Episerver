define([
    'dojo/_base/declare',
    'dojo/when',
    'dojo/request',
    'epi/shell/command/_Command',
    'epi/shell/_ContextMixin',
    'epi-cms/command/_NonEditViewCommandMixin',
], function (
    declare,
    when,
    request,
    _Command,
    _ContextMixin,
    _NonEditViewCommandMixin
) {
    return declare([_Command, _ContextMixin, _NonEditViewCommandMixin], {
        iconClass: 'siteimprove-icon',
        canExecute: true,
        label: 'Content Check',
        _execute: function () {
            var scope = this;
            when(scope.getCurrentContext(), function (context) {
                if (!context || !context.hasTemplate || !context.capabilities) return;
                if (!context.capabilities.isPage && !context.capabilities.isBlock)
                    return;

                var iframe = document.querySelector('iframe[name="sitePreview"]');
                var idocument = iframe.contentWindow.document;
                var html = idocument.documentElement.innerHTML;

                if (context.capabilities.isPage) {
                    request
                        .get('/siteimprove/pageUrl', {
                            query: { contentId: context.id, locale: context.language },
                            handleAs: 'json',
                        })
                        .then(function (response) {
                            scope.pushHtml(html, response.isDomain ? '' : response.url);
                        });
                } else {
                    scope.pushHtml(html, '');
                }
            });
        },
        pushHtml: function (html, pageUrl) {
            request
                .get('/siteimprove/token', { handleAs: 'json' })
                .then(function (token) {
                    var si = window._si || [];
                    si.push([
                        'onHighlight',
                        function (highlightInfo) {
                            // Highlight is running again before previous highlight was cleaned up. Clean it up now!
                            if (window.siteimproveHighlightCleanupFunction) {
                                window.siteimproveHighlightCleanupFunction();
                            }

                            var iframe = document.querySelector('iframe[name="sitePreview"]');
                            var idocument = iframe.contentWindow.document;

                            // Add styling to iFrame
                            var stylingId = 'siteimprove-styling';
                            var styling = idocument.getElementById(stylingId);
                            if (!styling) {
                                idocument.body.insertAdjacentHTML(
                                    'beforeend',
                                    `
              <style type="text/css" id="${stylingId}">
                  .siteimprove-highlight {
                    animation: siteimprove-pulse 0.5s;
                    animation-iteration-count: 8;
                    outline: 5px solid transparent;
                    outline-offset: -3px;
                  }
                  .siteimprove-highlight-inner {
                    animation: siteimprove-pulse 0.5s;
                    animation-iteration-count: 8;
                    outline: 5px solid transparent;
                    outline-offset: -5px;
                  }
                  @keyframes siteimprove-pulse {
                    from { outline-color: transparent; }
                    50% { outline-color: #ffc107; }
                    to { outline-color: transparent; }
                  }
              </style>
            `
                                );
                            }

                            // Highlight classes
                            var highlightClass = 'siteimprove-highlight';
                            var highlightClassInner = 'siteimprove-highlight-inner';

                            // Add highlight
                            highlightInfo.highlights.forEach((info, index) => {
                                // If error is inside the HEAD tag. Then Highlight the body
                                if (info.selector.startsWith('HEAD')) {
                                    info.selector = 'BODY';
                                    info.offset = null;
                                }

                                var element = idocument.querySelector(info.selector);
                                if (element) {
                                    // Scroll into view
                                    if (index === 0) {
                                        element.scrollIntoView({
                                            behavior: 'smooth',
                                            block: 'center',
                                        });
                                    }

                                    // Cleanup after adding highlight
                                    var cleanup = (callback) => {
                                        window.siteimproveHighlightCleanupFunction = () => {
                                            callback();
                                            window.clearTimeout(
                                                window.siteimproveHighlightCleanupTimer
                                            );
                                            window.siteimproveHighlightCleanupTimer = null;
                                            window.siteimproveHighlightCleanupFunction = null;
                                        };
                                        window.siteimproveHighlightCleanupTimer = setTimeout(() => {
                                            window.siteimproveHighlightCleanupFunction();
                                        }, 4000);
                                    };

                                    // Highlight text
                                    if (info.offset) {
                                        var originalHTML = element.innerHTML;
                                        var errorChild = element.childNodes[info.offset.child];
                                        var errorText = errorChild.textContent;
                                        var start = info.offset.start;
                                        var end = info.offset.start + info.offset.length;

                                        var beforeWord = errorText.slice(0, start);
                                        var beforeNode = document.createTextNode(beforeWord);
                                        element.insertBefore(beforeNode, errorChild);

                                        var errorWord = errorText.slice(start, end);
                                        var errorNode = document.createElement('span');
                                        errorNode.innerText = errorWord;
                                        errorNode.classList.add(highlightClass);
                                        element.insertBefore(errorNode, errorChild);

                                        var afterWord = errorText.slice(end);
                                        var afterNode = document.createTextNode(afterWord);
                                        element.insertBefore(afterNode, errorChild);

                                        element.removeChild(errorChild);

                                        cleanup(() => {
                                            element.innerHTML = originalHTML;
                                        });
                                    } else {
                                        // Highlight body
                                        if (element.tagName === 'BODY') {
                                            element.classList.add(highlightClassInner);

                                            cleanup(() => {
                                                element.classList.remove(highlightClassInner);
                                            });
                                        } else {
                                            // Highlight other tag types
                                            element.classList.add(highlightClass);

                                            cleanup(() => {
                                                element.classList.remove(highlightClass);
                                            });
                                        }
                                    }
                                }
                            });
                        },
                    ]);

                    si.push([
                        'contentcheck',
                        html,
                        pageUrl,
                        token,
                        function (contentId) { },
                    ]);
                });
        },
    });
});