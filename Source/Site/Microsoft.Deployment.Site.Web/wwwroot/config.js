System.config({
  defaultJSExtensions: true,
  transpiler: false,
  paths: {
    "*": "dist/*",
    "github:*": "jspm_packages/github/*",
    "npm:*": "jspm_packages/npm/*"
  },
  bundles: {
    "app-build.js": [
      "SiteCommon/Web/base/Dictionary.js",
      "SiteCommon/Web/base/ExperienceType.js",
      "SiteCommon/Web/base/query-parameter.js",
      "SiteCommon/Web/localization/en-us.js",
      "SiteCommon/Web/localization/language.js",
      "SiteCommon/Web/services/actionrequest.js",
      "SiteCommon/Web/services/actionresponse.js",
      "SiteCommon/Web/services/datastore.js",
      "SiteCommon/Web/services/deploymentservice.js",
      "SiteCommon/Web/services/errorservice.js",
      "SiteCommon/Web/services/httpservice.js",
      "SiteCommon/Web/services/loggerservice.js",
      "SiteCommon/Web/services/mainservice.js",
      "SiteCommon/Web/services/navigationservice.js",
      "SiteCommon/Web/services/translate-service.js",
      "SiteCommon/Web/services/utilityservice.js",
      "app.html!github:systemjs/plugin-text@0.0.8.js",
      "app.js",
      "github:jspm/nodelibs-process@0.1.2.js",
      "github:jspm/nodelibs-process@0.1.2/index.js",
      "github:twbs/bootstrap@3.3.7/css/bootstrap.css!github:systemjs/plugin-text@0.0.8.js",
      "main.js",
      "npm:aurelia-binding@1.0.1.js",
      "npm:aurelia-binding@1.0.1/aurelia-binding.js",
      "npm:aurelia-dependency-injection@1.0.0.js",
      "npm:aurelia-dependency-injection@1.0.0/aurelia-dependency-injection.js",
      "npm:aurelia-event-aggregator@1.0.0.js",
      "npm:aurelia-event-aggregator@1.0.0/aurelia-event-aggregator.js",
      "npm:aurelia-framework@1.0.1.js",
      "npm:aurelia-framework@1.0.1/aurelia-framework.js",
      "npm:aurelia-history@1.0.0.js",
      "npm:aurelia-history@1.0.0/aurelia-history.js",
      "npm:aurelia-http-client@1.0.0.js",
      "npm:aurelia-http-client@1.0.0/aurelia-http-client.js",
      "npm:aurelia-loader@1.0.0.js",
      "npm:aurelia-loader@1.0.0/aurelia-loader.js",
      "npm:aurelia-logging@1.0.0.js",
      "npm:aurelia-logging@1.0.0/aurelia-logging.js",
      "npm:aurelia-metadata@1.0.0.js",
      "npm:aurelia-metadata@1.0.0/aurelia-metadata.js",
      "npm:aurelia-pal@1.0.0.js",
      "npm:aurelia-pal@1.0.0/aurelia-pal.js",
      "npm:aurelia-path@1.0.0.js",
      "npm:aurelia-path@1.0.0/aurelia-path.js",
      "npm:aurelia-route-recognizer@1.0.0.js",
      "npm:aurelia-route-recognizer@1.0.0/aurelia-route-recognizer.js",
      "npm:aurelia-router@1.0.2.js",
      "npm:aurelia-router@1.0.2/aurelia-router.js",
      "npm:aurelia-task-queue@1.0.0.js",
      "npm:aurelia-task-queue@1.0.0/aurelia-task-queue.js",
      "npm:aurelia-templating@1.0.0.js",
      "npm:aurelia-templating@1.0.0/aurelia-templating.js",
      "npm:babel-runtime@5.8.38/core-js/json/stringify.js",
      "npm:babel-runtime@5.8.38/core-js/object/create.js",
      "npm:babel-runtime@5.8.38/core-js/object/define-property.js",
      "npm:babel-runtime@5.8.38/core-js/object/get-own-property-descriptor.js",
      "npm:babel-runtime@5.8.38/core-js/object/set-prototype-of.js",
      "npm:babel-runtime@5.8.38/core-js/promise.js",
      "npm:babel-runtime@5.8.38/core-js/reflect/metadata.js",
      "npm:babel-runtime@5.8.38/core-js/symbol.js",
      "npm:babel-runtime@5.8.38/helpers/classCallCheck.js",
      "npm:babel-runtime@5.8.38/helpers/createClass.js",
      "npm:babel-runtime@5.8.38/helpers/typeof.js",
      "npm:babel-runtime@5.8.38/regenerator.js",
      "npm:babel-runtime@5.8.38/regenerator/index.js",
      "npm:babel-runtime@5.8.38/regenerator/runtime.js",
      "npm:core-js@2.4.1/library/fn/json/stringify.js",
      "npm:core-js@2.4.1/library/fn/object/create.js",
      "npm:core-js@2.4.1/library/fn/object/define-property.js",
      "npm:core-js@2.4.1/library/fn/object/get-own-property-descriptor.js",
      "npm:core-js@2.4.1/library/fn/object/set-prototype-of.js",
      "npm:core-js@2.4.1/library/fn/promise.js",
      "npm:core-js@2.4.1/library/fn/reflect/metadata.js",
      "npm:core-js@2.4.1/library/fn/symbol.js",
      "npm:core-js@2.4.1/library/fn/symbol/index.js",
      "npm:core-js@2.4.1/library/modules/_a-function.js",
      "npm:core-js@2.4.1/library/modules/_add-to-unscopables.js",
      "npm:core-js@2.4.1/library/modules/_an-instance.js",
      "npm:core-js@2.4.1/library/modules/_an-object.js",
      "npm:core-js@2.4.1/library/modules/_array-includes.js",
      "npm:core-js@2.4.1/library/modules/_array-methods.js",
      "npm:core-js@2.4.1/library/modules/_array-species-constructor.js",
      "npm:core-js@2.4.1/library/modules/_array-species-create.js",
      "npm:core-js@2.4.1/library/modules/_classof.js",
      "npm:core-js@2.4.1/library/modules/_cof.js",
      "npm:core-js@2.4.1/library/modules/_collection-strong.js",
      "npm:core-js@2.4.1/library/modules/_collection-weak.js",
      "npm:core-js@2.4.1/library/modules/_collection.js",
      "npm:core-js@2.4.1/library/modules/_core.js",
      "npm:core-js@2.4.1/library/modules/_ctx.js",
      "npm:core-js@2.4.1/library/modules/_defined.js",
      "npm:core-js@2.4.1/library/modules/_descriptors.js",
      "npm:core-js@2.4.1/library/modules/_dom-create.js",
      "npm:core-js@2.4.1/library/modules/_enum-bug-keys.js",
      "npm:core-js@2.4.1/library/modules/_enum-keys.js",
      "npm:core-js@2.4.1/library/modules/_export.js",
      "npm:core-js@2.4.1/library/modules/_fails.js",
      "npm:core-js@2.4.1/library/modules/_for-of.js",
      "npm:core-js@2.4.1/library/modules/_global.js",
      "npm:core-js@2.4.1/library/modules/_has.js",
      "npm:core-js@2.4.1/library/modules/_hide.js",
      "npm:core-js@2.4.1/library/modules/_html.js",
      "npm:core-js@2.4.1/library/modules/_ie8-dom-define.js",
      "npm:core-js@2.4.1/library/modules/_invoke.js",
      "npm:core-js@2.4.1/library/modules/_iobject.js",
      "npm:core-js@2.4.1/library/modules/_is-array-iter.js",
      "npm:core-js@2.4.1/library/modules/_is-array.js",
      "npm:core-js@2.4.1/library/modules/_is-object.js",
      "npm:core-js@2.4.1/library/modules/_iter-call.js",
      "npm:core-js@2.4.1/library/modules/_iter-create.js",
      "npm:core-js@2.4.1/library/modules/_iter-define.js",
      "npm:core-js@2.4.1/library/modules/_iter-detect.js",
      "npm:core-js@2.4.1/library/modules/_iter-step.js",
      "npm:core-js@2.4.1/library/modules/_iterators.js",
      "npm:core-js@2.4.1/library/modules/_keyof.js",
      "npm:core-js@2.4.1/library/modules/_library.js",
      "npm:core-js@2.4.1/library/modules/_meta.js",
      "npm:core-js@2.4.1/library/modules/_metadata.js",
      "npm:core-js@2.4.1/library/modules/_microtask.js",
      "npm:core-js@2.4.1/library/modules/_object-assign.js",
      "npm:core-js@2.4.1/library/modules/_object-create.js",
      "npm:core-js@2.4.1/library/modules/_object-dp.js",
      "npm:core-js@2.4.1/library/modules/_object-dps.js",
      "npm:core-js@2.4.1/library/modules/_object-gopd.js",
      "npm:core-js@2.4.1/library/modules/_object-gopn-ext.js",
      "npm:core-js@2.4.1/library/modules/_object-gopn.js",
      "npm:core-js@2.4.1/library/modules/_object-gops.js",
      "npm:core-js@2.4.1/library/modules/_object-gpo.js",
      "npm:core-js@2.4.1/library/modules/_object-keys-internal.js",
      "npm:core-js@2.4.1/library/modules/_object-keys.js",
      "npm:core-js@2.4.1/library/modules/_object-pie.js",
      "npm:core-js@2.4.1/library/modules/_object-sap.js",
      "npm:core-js@2.4.1/library/modules/_property-desc.js",
      "npm:core-js@2.4.1/library/modules/_redefine-all.js",
      "npm:core-js@2.4.1/library/modules/_redefine.js",
      "npm:core-js@2.4.1/library/modules/_set-proto.js",
      "npm:core-js@2.4.1/library/modules/_set-species.js",
      "npm:core-js@2.4.1/library/modules/_set-to-string-tag.js",
      "npm:core-js@2.4.1/library/modules/_shared-key.js",
      "npm:core-js@2.4.1/library/modules/_shared.js",
      "npm:core-js@2.4.1/library/modules/_species-constructor.js",
      "npm:core-js@2.4.1/library/modules/_string-at.js",
      "npm:core-js@2.4.1/library/modules/_task.js",
      "npm:core-js@2.4.1/library/modules/_to-index.js",
      "npm:core-js@2.4.1/library/modules/_to-integer.js",
      "npm:core-js@2.4.1/library/modules/_to-iobject.js",
      "npm:core-js@2.4.1/library/modules/_to-length.js",
      "npm:core-js@2.4.1/library/modules/_to-object.js",
      "npm:core-js@2.4.1/library/modules/_to-primitive.js",
      "npm:core-js@2.4.1/library/modules/_uid.js",
      "npm:core-js@2.4.1/library/modules/_wks-define.js",
      "npm:core-js@2.4.1/library/modules/_wks-ext.js",
      "npm:core-js@2.4.1/library/modules/_wks.js",
      "npm:core-js@2.4.1/library/modules/core.get-iterator-method.js",
      "npm:core-js@2.4.1/library/modules/es6.array.iterator.js",
      "npm:core-js@2.4.1/library/modules/es6.map.js",
      "npm:core-js@2.4.1/library/modules/es6.object.create.js",
      "npm:core-js@2.4.1/library/modules/es6.object.define-property.js",
      "npm:core-js@2.4.1/library/modules/es6.object.get-own-property-descriptor.js",
      "npm:core-js@2.4.1/library/modules/es6.object.set-prototype-of.js",
      "npm:core-js@2.4.1/library/modules/es6.object.to-string.js",
      "npm:core-js@2.4.1/library/modules/es6.promise.js",
      "npm:core-js@2.4.1/library/modules/es6.string.iterator.js",
      "npm:core-js@2.4.1/library/modules/es6.symbol.js",
      "npm:core-js@2.4.1/library/modules/es6.weak-map.js",
      "npm:core-js@2.4.1/library/modules/es7.reflect.metadata.js",
      "npm:core-js@2.4.1/library/modules/es7.symbol.async-iterator.js",
      "npm:core-js@2.4.1/library/modules/es7.symbol.observable.js",
      "npm:core-js@2.4.1/library/modules/web.dom.iterable.js",
      "npm:process@0.11.8.js",
      "npm:process@0.11.8/browser.js"
    ],
    "aurelia.js": [
      "npm:aurelia-binding@1.0.1.js",
      "npm:aurelia-binding@1.0.1/aurelia-binding.js",
      "npm:aurelia-bootstrapper@1.0.0.js",
      "npm:aurelia-bootstrapper@1.0.0/aurelia-bootstrapper.js",
      "npm:aurelia-dependency-injection@1.0.0.js",
      "npm:aurelia-dependency-injection@1.0.0/aurelia-dependency-injection.js",
      "npm:aurelia-event-aggregator@1.0.0.js",
      "npm:aurelia-event-aggregator@1.0.0/aurelia-event-aggregator.js",
      "npm:aurelia-fetch-client@1.0.0.js",
      "npm:aurelia-fetch-client@1.0.0/aurelia-fetch-client.js",
      "npm:aurelia-framework@1.0.1.js",
      "npm:aurelia-framework@1.0.1/aurelia-framework.js",
      "npm:aurelia-history-browser@1.0.0.js",
      "npm:aurelia-history-browser@1.0.0/aurelia-history-browser.js",
      "npm:aurelia-history@1.0.0.js",
      "npm:aurelia-history@1.0.0/aurelia-history.js",
      "npm:aurelia-http-client@1.0.0.js",
      "npm:aurelia-http-client@1.0.0/aurelia-http-client.js",
      "npm:aurelia-loader-default@1.0.0.js",
      "npm:aurelia-loader-default@1.0.0/aurelia-loader-default.js",
      "npm:aurelia-loader@1.0.0.js",
      "npm:aurelia-loader@1.0.0/aurelia-loader.js",
      "npm:aurelia-logging-console@1.0.0.js",
      "npm:aurelia-logging-console@1.0.0/aurelia-logging-console.js",
      "npm:aurelia-logging@1.0.0.js",
      "npm:aurelia-logging@1.0.0/aurelia-logging.js",
      "npm:aurelia-metadata@1.0.0.js",
      "npm:aurelia-metadata@1.0.0/aurelia-metadata.js",
      "npm:aurelia-pal-browser@1.0.0.js",
      "npm:aurelia-pal-browser@1.0.0/aurelia-pal-browser.js",
      "npm:aurelia-pal@1.0.0.js",
      "npm:aurelia-pal@1.0.0/aurelia-pal.js",
      "npm:aurelia-path@1.0.0.js",
      "npm:aurelia-path@1.0.0/aurelia-path.js",
      "npm:aurelia-polyfills@1.0.0.js",
      "npm:aurelia-polyfills@1.0.0/aurelia-polyfills.js",
      "npm:aurelia-route-recognizer@1.0.0.js",
      "npm:aurelia-route-recognizer@1.0.0/aurelia-route-recognizer.js",
      "npm:aurelia-router@1.0.2.js",
      "npm:aurelia-router@1.0.2/aurelia-router.js",
      "npm:aurelia-task-queue@1.0.0.js",
      "npm:aurelia-task-queue@1.0.0/aurelia-task-queue.js",
      "npm:aurelia-templating-binding@1.0.0.js",
      "npm:aurelia-templating-binding@1.0.0/aurelia-templating-binding.js",
      "npm:aurelia-templating-resources@1.0.0.js",
      "npm:aurelia-templating-resources@1.0.0/abstract-repeater.js",
      "npm:aurelia-templating-resources@1.0.0/analyze-view-factory.js",
      "npm:aurelia-templating-resources@1.0.0/array-repeat-strategy.js",
      "npm:aurelia-templating-resources@1.0.0/aurelia-hide-style.js",
      "npm:aurelia-templating-resources@1.0.0/aurelia-templating-resources.js",
      "npm:aurelia-templating-resources@1.0.0/binding-mode-behaviors.js",
      "npm:aurelia-templating-resources@1.0.0/binding-signaler.js",
      "npm:aurelia-templating-resources@1.0.0/compose.js",
      "npm:aurelia-templating-resources@1.0.0/css-resource.js",
      "npm:aurelia-templating-resources@1.0.0/debounce-binding-behavior.js",
      "npm:aurelia-templating-resources@1.0.0/dynamic-element.js",
      "npm:aurelia-templating-resources@1.0.0/focus.js",
      "npm:aurelia-templating-resources@1.0.0/hide.js",
      "npm:aurelia-templating-resources@1.0.0/html-resource-plugin.js",
      "npm:aurelia-templating-resources@1.0.0/html-sanitizer.js",
      "npm:aurelia-templating-resources@1.0.0/if.js",
      "npm:aurelia-templating-resources@1.0.0/map-repeat-strategy.js",
      "npm:aurelia-templating-resources@1.0.0/null-repeat-strategy.js",
      "npm:aurelia-templating-resources@1.0.0/number-repeat-strategy.js",
      "npm:aurelia-templating-resources@1.0.0/repeat-strategy-locator.js",
      "npm:aurelia-templating-resources@1.0.0/repeat-utilities.js",
      "npm:aurelia-templating-resources@1.0.0/repeat.js",
      "npm:aurelia-templating-resources@1.0.0/replaceable.js",
      "npm:aurelia-templating-resources@1.0.0/sanitize-html.js",
      "npm:aurelia-templating-resources@1.0.0/set-repeat-strategy.js",
      "npm:aurelia-templating-resources@1.0.0/show.js",
      "npm:aurelia-templating-resources@1.0.0/signal-binding-behavior.js",
      "npm:aurelia-templating-resources@1.0.0/throttle-binding-behavior.js",
      "npm:aurelia-templating-resources@1.0.0/update-trigger-binding-behavior.js",
      "npm:aurelia-templating-resources@1.0.0/with.js",
      "npm:aurelia-templating-router@1.0.0.js",
      "npm:aurelia-templating-router@1.0.0/aurelia-templating-router.js",
      "npm:aurelia-templating-router@1.0.0/route-href.js",
      "npm:aurelia-templating-router@1.0.0/route-loader.js",
      "npm:aurelia-templating-router@1.0.0/router-view.js",
      "npm:aurelia-templating@1.0.0.js",
      "npm:aurelia-templating@1.0.0/aurelia-templating.js"
    ]
  },
  depCache: {
    "npm:aurelia-polyfills@1.0.0.js": [
      "npm:aurelia-polyfills@1.0.0/aurelia-polyfills"
    ],
    "npm:aurelia-polyfills@1.0.0/aurelia-polyfills.js": [
      "aurelia-pal"
    ],
    "npm:aurelia-pal@1.0.0.js": [
      "npm:aurelia-pal@1.0.0/aurelia-pal"
    ],
    "npm:aurelia-http-client@1.0.0.js": [
      "npm:aurelia-http-client@1.0.0/aurelia-http-client"
    ],
    "npm:aurelia-http-client@1.0.0/aurelia-http-client.js": [
      "aurelia-path",
      "aurelia-pal"
    ],
    "npm:aurelia-path@1.0.0.js": [
      "npm:aurelia-path@1.0.0/aurelia-path"
    ],
    "npm:aurelia-templating-router@1.0.0.js": [
      "npm:aurelia-templating-router@1.0.0/aurelia-templating-router"
    ],
    "npm:aurelia-templating-router@1.0.0/aurelia-templating-router.js": [
      "aurelia-router",
      "./route-loader",
      "./router-view",
      "./route-href"
    ],
    "npm:aurelia-router@1.0.2.js": [
      "npm:aurelia-router@1.0.2/aurelia-router"
    ],
    "npm:aurelia-router@1.0.2/aurelia-router.js": [
      "aurelia-logging",
      "aurelia-route-recognizer",
      "aurelia-dependency-injection",
      "aurelia-history",
      "aurelia-event-aggregator"
    ],
    "npm:aurelia-logging@1.0.0.js": [
      "npm:aurelia-logging@1.0.0/aurelia-logging"
    ],
    "npm:aurelia-route-recognizer@1.0.0.js": [
      "npm:aurelia-route-recognizer@1.0.0/aurelia-route-recognizer"
    ],
    "npm:aurelia-dependency-injection@1.0.0.js": [
      "npm:aurelia-dependency-injection@1.0.0/aurelia-dependency-injection"
    ],
    "npm:aurelia-history@1.0.0.js": [
      "npm:aurelia-history@1.0.0/aurelia-history"
    ],
    "npm:aurelia-event-aggregator@1.0.0.js": [
      "npm:aurelia-event-aggregator@1.0.0/aurelia-event-aggregator"
    ],
    "npm:aurelia-route-recognizer@1.0.0/aurelia-route-recognizer.js": [
      "aurelia-path"
    ],
    "npm:aurelia-dependency-injection@1.0.0/aurelia-dependency-injection.js": [
      "aurelia-metadata",
      "aurelia-pal"
    ],
    "npm:aurelia-event-aggregator@1.0.0/aurelia-event-aggregator.js": [
      "aurelia-logging"
    ],
    "npm:aurelia-metadata@1.0.0.js": [
      "npm:aurelia-metadata@1.0.0/aurelia-metadata"
    ],
    "npm:aurelia-metadata@1.0.0/aurelia-metadata.js": [
      "aurelia-pal"
    ],
    "npm:aurelia-templating-router@1.0.0/route-loader.js": [
      "aurelia-dependency-injection",
      "aurelia-templating",
      "aurelia-router",
      "aurelia-path",
      "aurelia-metadata"
    ],
    "npm:aurelia-templating@1.0.0.js": [
      "npm:aurelia-templating@1.0.0/aurelia-templating"
    ],
    "npm:aurelia-templating@1.0.0/aurelia-templating.js": [
      "aurelia-logging",
      "aurelia-metadata",
      "aurelia-pal",
      "aurelia-path",
      "aurelia-loader",
      "aurelia-dependency-injection",
      "aurelia-binding",
      "aurelia-task-queue"
    ],
    "npm:aurelia-loader@1.0.0.js": [
      "npm:aurelia-loader@1.0.0/aurelia-loader"
    ],
    "npm:aurelia-binding@1.0.1.js": [
      "npm:aurelia-binding@1.0.1/aurelia-binding"
    ],
    "npm:aurelia-task-queue@1.0.0.js": [
      "npm:aurelia-task-queue@1.0.0/aurelia-task-queue"
    ],
    "npm:aurelia-loader@1.0.0/aurelia-loader.js": [
      "aurelia-path",
      "aurelia-metadata"
    ],
    "npm:aurelia-binding@1.0.1/aurelia-binding.js": [
      "aurelia-logging",
      "aurelia-pal",
      "aurelia-task-queue",
      "aurelia-metadata"
    ],
    "npm:aurelia-task-queue@1.0.0/aurelia-task-queue.js": [
      "aurelia-pal"
    ],
    "npm:aurelia-templating-router@1.0.0/route-href.js": [
      "aurelia-templating",
      "aurelia-dependency-injection",
      "aurelia-router",
      "aurelia-pal",
      "aurelia-logging"
    ],
    "npm:aurelia-templating-router@1.0.0/router-view.js": [
      "aurelia-dependency-injection",
      "aurelia-templating",
      "aurelia-router",
      "aurelia-metadata",
      "aurelia-pal"
    ],
    "npm:aurelia-templating-resources@1.0.0.js": [
      "npm:aurelia-templating-resources@1.0.0/aurelia-templating-resources"
    ],
    "npm:aurelia-templating-resources@1.0.0/aurelia-templating-resources.js": [
      "./compose",
      "./if",
      "./with",
      "./repeat",
      "./show",
      "./hide",
      "./sanitize-html",
      "./replaceable",
      "./focus",
      "aurelia-templating",
      "./css-resource",
      "./html-sanitizer",
      "./binding-mode-behaviors",
      "./throttle-binding-behavior",
      "./debounce-binding-behavior",
      "./signal-binding-behavior",
      "./binding-signaler",
      "./update-trigger-binding-behavior",
      "./abstract-repeater",
      "./repeat-strategy-locator",
      "./html-resource-plugin",
      "./null-repeat-strategy",
      "./array-repeat-strategy",
      "./map-repeat-strategy",
      "./set-repeat-strategy",
      "./number-repeat-strategy",
      "./repeat-utilities",
      "./analyze-view-factory",
      "./aurelia-hide-style"
    ],
    "npm:aurelia-templating-resources@1.0.0/signal-binding-behavior.js": [
      "./binding-signaler"
    ],
    "npm:aurelia-templating-resources@1.0.0/repeat-strategy-locator.js": [
      "./null-repeat-strategy",
      "./array-repeat-strategy",
      "./map-repeat-strategy",
      "./set-repeat-strategy",
      "./number-repeat-strategy"
    ],
    "npm:aurelia-templating-resources@1.0.0/repeat.js": [
      "aurelia-dependency-injection",
      "aurelia-binding",
      "aurelia-templating",
      "./repeat-strategy-locator",
      "./repeat-utilities",
      "./analyze-view-factory",
      "./abstract-repeater"
    ],
    "npm:aurelia-templating-resources@1.0.0/if.js": [
      "aurelia-templating",
      "aurelia-dependency-injection"
    ],
    "npm:aurelia-templating-resources@1.0.0/hide.js": [
      "aurelia-dependency-injection",
      "aurelia-templating",
      "aurelia-pal",
      "./aurelia-hide-style"
    ],
    "npm:aurelia-templating-resources@1.0.0/compose.js": [
      "aurelia-dependency-injection",
      "aurelia-task-queue",
      "aurelia-templating",
      "aurelia-pal"
    ],
    "npm:aurelia-templating-resources@1.0.0/show.js": [
      "aurelia-dependency-injection",
      "aurelia-templating",
      "aurelia-pal",
      "./aurelia-hide-style"
    ],
    "npm:aurelia-templating-resources@1.0.0/sanitize-html.js": [
      "aurelia-binding",
      "aurelia-dependency-injection",
      "./html-sanitizer"
    ],
    "npm:aurelia-templating-resources@1.0.0/with.js": [
      "aurelia-dependency-injection",
      "aurelia-templating",
      "aurelia-binding"
    ],
    "npm:aurelia-templating-resources@1.0.0/replaceable.js": [
      "aurelia-dependency-injection",
      "aurelia-templating"
    ],
    "npm:aurelia-templating-resources@1.0.0/focus.js": [
      "aurelia-templating",
      "aurelia-binding",
      "aurelia-dependency-injection",
      "aurelia-task-queue",
      "aurelia-pal"
    ],
    "npm:aurelia-templating-resources@1.0.0/binding-mode-behaviors.js": [
      "aurelia-binding",
      "aurelia-metadata"
    ],
    "npm:aurelia-templating-resources@1.0.0/throttle-binding-behavior.js": [
      "aurelia-binding"
    ],
    "npm:aurelia-templating-resources@1.0.0/debounce-binding-behavior.js": [
      "aurelia-binding"
    ],
    "npm:aurelia-templating-resources@1.0.0/binding-signaler.js": [
      "aurelia-binding"
    ],
    "npm:aurelia-templating-resources@1.0.0/update-trigger-binding-behavior.js": [
      "aurelia-binding"
    ],
    "npm:aurelia-templating-resources@1.0.0/html-resource-plugin.js": [
      "aurelia-templating",
      "./dynamic-element"
    ],
    "npm:aurelia-templating-resources@1.0.0/css-resource.js": [
      "aurelia-templating",
      "aurelia-loader",
      "aurelia-dependency-injection",
      "aurelia-path",
      "aurelia-pal"
    ],
    "npm:aurelia-templating-resources@1.0.0/array-repeat-strategy.js": [
      "./repeat-utilities",
      "aurelia-binding"
    ],
    "npm:aurelia-templating-resources@1.0.0/number-repeat-strategy.js": [
      "./repeat-utilities"
    ],
    "npm:aurelia-templating-resources@1.0.0/map-repeat-strategy.js": [
      "./repeat-utilities"
    ],
    "npm:aurelia-templating-resources@1.0.0/set-repeat-strategy.js": [
      "./repeat-utilities"
    ],
    "npm:aurelia-templating-resources@1.0.0/aurelia-hide-style.js": [
      "aurelia-pal"
    ],
    "npm:aurelia-templating-resources@1.0.0/repeat-utilities.js": [
      "aurelia-binding"
    ],
    "npm:aurelia-templating-resources@1.0.0/dynamic-element.js": [
      "aurelia-templating"
    ],
    "npm:aurelia-templating-binding@1.0.0.js": [
      "npm:aurelia-templating-binding@1.0.0/aurelia-templating-binding"
    ],
    "npm:aurelia-templating-binding@1.0.0/aurelia-templating-binding.js": [
      "aurelia-logging",
      "aurelia-binding",
      "aurelia-templating"
    ],
    "npm:aurelia-logging-console@1.0.0.js": [
      "npm:aurelia-logging-console@1.0.0/aurelia-logging-console"
    ],
    "npm:aurelia-logging-console@1.0.0/aurelia-logging-console.js": [
      "aurelia-logging"
    ],
    "npm:aurelia-loader-default@1.0.0.js": [
      "npm:aurelia-loader-default@1.0.0/aurelia-loader-default"
    ],
    "npm:aurelia-loader-default@1.0.0/aurelia-loader-default.js": [
      "aurelia-loader",
      "aurelia-pal",
      "aurelia-metadata"
    ],
    "npm:aurelia-history-browser@1.0.0.js": [
      "npm:aurelia-history-browser@1.0.0/aurelia-history-browser"
    ],
    "npm:aurelia-history-browser@1.0.0/aurelia-history-browser.js": [
      "aurelia-pal",
      "aurelia-history"
    ],
    "npm:aurelia-framework@1.0.1.js": [
      "npm:aurelia-framework@1.0.1/aurelia-framework"
    ],
    "npm:aurelia-framework@1.0.1/aurelia-framework.js": [
      "aurelia-dependency-injection",
      "aurelia-binding",
      "aurelia-metadata",
      "aurelia-templating",
      "aurelia-loader",
      "aurelia-task-queue",
      "aurelia-path",
      "aurelia-pal",
      "aurelia-logging"
    ],
    "npm:aurelia-fetch-client@1.0.0.js": [
      "npm:aurelia-fetch-client@1.0.0/aurelia-fetch-client"
    ],
    "npm:aurelia-bootstrapper@1.0.0.js": [
      "npm:aurelia-bootstrapper@1.0.0/aurelia-bootstrapper"
    ],
    "npm:aurelia-bootstrapper@1.0.0/aurelia-bootstrapper.js": [
      "aurelia-pal",
      "aurelia-pal-browser",
      "aurelia-polyfills"
    ],
    "npm:aurelia-pal-browser@1.0.0.js": [
      "npm:aurelia-pal-browser@1.0.0/aurelia-pal-browser"
    ],
    "npm:aurelia-pal-browser@1.0.0/aurelia-pal-browser.js": [
      "aurelia-pal"
    ]
  },
  map: {
    "aurelia-animator-css": "npm:aurelia-animator-css@1.0.0",
    "aurelia-bootstrapper": "npm:aurelia-bootstrapper@1.0.0",
    "aurelia-fetch-client": "npm:aurelia-fetch-client@1.0.0",
    "aurelia-framework": "npm:aurelia-framework@1.0.1",
    "aurelia-history-browser": "npm:aurelia-history-browser@1.0.0",
    "aurelia-http-client": "npm:aurelia-http-client@1.0.0",
    "aurelia-loader-default": "npm:aurelia-loader-default@1.0.0",
    "aurelia-logging-console": "npm:aurelia-logging-console@1.0.0",
    "aurelia-pal-browser": "npm:aurelia-pal-browser@1.0.0",
    "aurelia-polyfills": "npm:aurelia-polyfills@1.0.0",
    "aurelia-router": "npm:aurelia-router@1.0.2",
    "aurelia-templating-binding": "npm:aurelia-templating-binding@1.0.0",
    "aurelia-templating-resources": "npm:aurelia-templating-resources@1.0.0",
    "aurelia-templating-router": "npm:aurelia-templating-router@1.0.0",
    "babel": "npm:babel-core@5.8.38",
    "babel-polyfill": "npm:babel-polyfill@6.13.0",
    "babel-runtime": "npm:babel-runtime@5.8.38",
    "bluebird": "npm:bluebird@3.4.1",
    "bootstrap": "github:twbs/bootstrap@3.3.7",
    "core-js": "npm:core-js@2.4.1",
    "fetch": "github:github/fetch@1.0.0",
    "font-awesome": "npm:font-awesome@4.6.3",
    "jquery": "npm:jquery@2.2.4",
    "process": "npm:process@0.11.8",
    "regenerator-runtime": "npm:regenerator-runtime@0.9.5",
    "text": "github:systemjs/plugin-text@0.0.8",
    "github:jspm/nodelibs-assert@0.1.0": {
      "assert": "npm:assert@1.4.1"
    },
    "github:jspm/nodelibs-buffer@0.1.0": {
      "buffer": "npm:buffer@3.6.0"
    },
    "github:jspm/nodelibs-path@0.1.0": {
      "path-browserify": "npm:path-browserify@0.0.0"
    },
    "github:jspm/nodelibs-process@0.1.2": {
      "process": "npm:process@0.11.8"
    },
    "github:jspm/nodelibs-util@0.1.0": {
      "util": "npm:util@0.10.3"
    },
    "github:jspm/nodelibs-vm@0.1.0": {
      "vm-browserify": "npm:vm-browserify@0.0.4"
    },
    "github:twbs/bootstrap@3.3.7": {
      "jquery": "npm:jquery@2.2.4"
    },
    "npm:assert@1.4.1": {
      "assert": "github:jspm/nodelibs-assert@0.1.0",
      "buffer": "github:jspm/nodelibs-buffer@0.1.0",
      "process": "github:jspm/nodelibs-process@0.1.2",
      "util": "npm:util@0.10.3"
    },
    "npm:aurelia-animator-css@1.0.0": {
      "aurelia-metadata": "npm:aurelia-metadata@1.0.0",
      "aurelia-pal": "npm:aurelia-pal@1.0.0",
      "aurelia-templating": "npm:aurelia-templating@1.0.0"
    },
    "npm:aurelia-binding@1.0.1": {
      "aurelia-logging": "npm:aurelia-logging@1.0.0",
      "aurelia-metadata": "npm:aurelia-metadata@1.0.0",
      "aurelia-pal": "npm:aurelia-pal@1.0.0",
      "aurelia-task-queue": "npm:aurelia-task-queue@1.0.0"
    },
    "npm:aurelia-bootstrapper@1.0.0": {
      "aurelia-event-aggregator": "npm:aurelia-event-aggregator@1.0.0",
      "aurelia-framework": "npm:aurelia-framework@1.0.1",
      "aurelia-history": "npm:aurelia-history@1.0.0",
      "aurelia-history-browser": "npm:aurelia-history-browser@1.0.0",
      "aurelia-loader-default": "npm:aurelia-loader-default@1.0.0",
      "aurelia-logging-console": "npm:aurelia-logging-console@1.0.0",
      "aurelia-pal": "npm:aurelia-pal@1.0.0",
      "aurelia-pal-browser": "npm:aurelia-pal-browser@1.0.0",
      "aurelia-polyfills": "npm:aurelia-polyfills@1.0.0",
      "aurelia-router": "npm:aurelia-router@1.0.2",
      "aurelia-templating": "npm:aurelia-templating@1.0.0",
      "aurelia-templating-binding": "npm:aurelia-templating-binding@1.0.0",
      "aurelia-templating-resources": "npm:aurelia-templating-resources@1.0.0",
      "aurelia-templating-router": "npm:aurelia-templating-router@1.0.0"
    },
    "npm:aurelia-dependency-injection@1.0.0": {
      "aurelia-metadata": "npm:aurelia-metadata@1.0.0",
      "aurelia-pal": "npm:aurelia-pal@1.0.0"
    },
    "npm:aurelia-event-aggregator@1.0.0": {
      "aurelia-logging": "npm:aurelia-logging@1.0.0"
    },
    "npm:aurelia-framework@1.0.1": {
      "aurelia-binding": "npm:aurelia-binding@1.0.1",
      "aurelia-dependency-injection": "npm:aurelia-dependency-injection@1.0.0",
      "aurelia-loader": "npm:aurelia-loader@1.0.0",
      "aurelia-logging": "npm:aurelia-logging@1.0.0",
      "aurelia-metadata": "npm:aurelia-metadata@1.0.0",
      "aurelia-pal": "npm:aurelia-pal@1.0.0",
      "aurelia-path": "npm:aurelia-path@1.0.0",
      "aurelia-task-queue": "npm:aurelia-task-queue@1.0.0",
      "aurelia-templating": "npm:aurelia-templating@1.0.0"
    },
    "npm:aurelia-history-browser@1.0.0": {
      "aurelia-history": "npm:aurelia-history@1.0.0",
      "aurelia-pal": "npm:aurelia-pal@1.0.0"
    },
    "npm:aurelia-http-client@1.0.0": {
      "aurelia-pal": "npm:aurelia-pal@1.0.0",
      "aurelia-path": "npm:aurelia-path@1.0.0"
    },
    "npm:aurelia-loader-default@1.0.0": {
      "aurelia-loader": "npm:aurelia-loader@1.0.0",
      "aurelia-metadata": "npm:aurelia-metadata@1.0.0",
      "aurelia-pal": "npm:aurelia-pal@1.0.0"
    },
    "npm:aurelia-loader@1.0.0": {
      "aurelia-metadata": "npm:aurelia-metadata@1.0.0",
      "aurelia-path": "npm:aurelia-path@1.0.0"
    },
    "npm:aurelia-logging-console@1.0.0": {
      "aurelia-logging": "npm:aurelia-logging@1.0.0"
    },
    "npm:aurelia-metadata@1.0.0": {
      "aurelia-pal": "npm:aurelia-pal@1.0.0"
    },
    "npm:aurelia-pal-browser@1.0.0": {
      "aurelia-pal": "npm:aurelia-pal@1.0.0"
    },
    "npm:aurelia-polyfills@1.0.0": {
      "aurelia-pal": "npm:aurelia-pal@1.0.0"
    },
    "npm:aurelia-route-recognizer@1.0.0": {
      "aurelia-path": "npm:aurelia-path@1.0.0"
    },
    "npm:aurelia-router@1.0.2": {
      "aurelia-dependency-injection": "npm:aurelia-dependency-injection@1.0.0",
      "aurelia-event-aggregator": "npm:aurelia-event-aggregator@1.0.0",
      "aurelia-history": "npm:aurelia-history@1.0.0",
      "aurelia-logging": "npm:aurelia-logging@1.0.0",
      "aurelia-path": "npm:aurelia-path@1.0.0",
      "aurelia-route-recognizer": "npm:aurelia-route-recognizer@1.0.0"
    },
    "npm:aurelia-task-queue@1.0.0": {
      "aurelia-pal": "npm:aurelia-pal@1.0.0"
    },
    "npm:aurelia-templating-binding@1.0.0": {
      "aurelia-binding": "npm:aurelia-binding@1.0.1",
      "aurelia-logging": "npm:aurelia-logging@1.0.0",
      "aurelia-templating": "npm:aurelia-templating@1.0.0"
    },
    "npm:aurelia-templating-resources@1.0.0": {
      "aurelia-binding": "npm:aurelia-binding@1.0.1",
      "aurelia-dependency-injection": "npm:aurelia-dependency-injection@1.0.0",
      "aurelia-loader": "npm:aurelia-loader@1.0.0",
      "aurelia-logging": "npm:aurelia-logging@1.0.0",
      "aurelia-metadata": "npm:aurelia-metadata@1.0.0",
      "aurelia-pal": "npm:aurelia-pal@1.0.0",
      "aurelia-path": "npm:aurelia-path@1.0.0",
      "aurelia-task-queue": "npm:aurelia-task-queue@1.0.0",
      "aurelia-templating": "npm:aurelia-templating@1.0.0"
    },
    "npm:aurelia-templating-router@1.0.0": {
      "aurelia-dependency-injection": "npm:aurelia-dependency-injection@1.0.0",
      "aurelia-logging": "npm:aurelia-logging@1.0.0",
      "aurelia-metadata": "npm:aurelia-metadata@1.0.0",
      "aurelia-pal": "npm:aurelia-pal@1.0.0",
      "aurelia-path": "npm:aurelia-path@1.0.0",
      "aurelia-router": "npm:aurelia-router@1.0.2",
      "aurelia-templating": "npm:aurelia-templating@1.0.0"
    },
    "npm:aurelia-templating@1.0.0": {
      "aurelia-binding": "npm:aurelia-binding@1.0.1",
      "aurelia-dependency-injection": "npm:aurelia-dependency-injection@1.0.0",
      "aurelia-loader": "npm:aurelia-loader@1.0.0",
      "aurelia-logging": "npm:aurelia-logging@1.0.0",
      "aurelia-metadata": "npm:aurelia-metadata@1.0.0",
      "aurelia-pal": "npm:aurelia-pal@1.0.0",
      "aurelia-path": "npm:aurelia-path@1.0.0",
      "aurelia-task-queue": "npm:aurelia-task-queue@1.0.0"
    },
    "npm:babel-polyfill@6.13.0": {
      "babel-runtime": "npm:babel-runtime@6.18.0",
      "core-js": "npm:core-js@2.4.1",
      "process": "github:jspm/nodelibs-process@0.1.2",
      "regenerator-runtime": "npm:regenerator-runtime@0.9.5"
    },
    "npm:babel-runtime@5.8.38": {
      "process": "github:jspm/nodelibs-process@0.1.2"
    },
    "npm:babel-runtime@6.18.0": {
      "core-js": "npm:core-js@2.4.1",
      "regenerator-runtime": "npm:regenerator-runtime@0.9.5"
    },
    "npm:bluebird@3.4.1": {
      "process": "github:jspm/nodelibs-process@0.1.2"
    },
    "npm:buffer@3.6.0": {
      "base64-js": "npm:base64-js@0.0.8",
      "child_process": "github:jspm/nodelibs-child_process@0.1.0",
      "fs": "github:jspm/nodelibs-fs@0.1.2",
      "ieee754": "npm:ieee754@1.1.6",
      "isarray": "npm:isarray@1.0.0",
      "process": "github:jspm/nodelibs-process@0.1.2"
    },
    "npm:core-js@2.4.1": {
      "fs": "github:jspm/nodelibs-fs@0.1.2",
      "path": "github:jspm/nodelibs-path@0.1.0",
      "process": "github:jspm/nodelibs-process@0.1.2",
      "systemjs-json": "github:systemjs/plugin-json@0.1.2"
    },
    "npm:font-awesome@4.6.3": {
      "css": "github:systemjs/plugin-css@0.1.26"
    },
    "npm:inherits@2.0.1": {
      "util": "github:jspm/nodelibs-util@0.1.0"
    },
    "npm:path-browserify@0.0.0": {
      "process": "github:jspm/nodelibs-process@0.1.2"
    },
    "npm:process@0.11.8": {
      "assert": "github:jspm/nodelibs-assert@0.1.0",
      "fs": "github:jspm/nodelibs-fs@0.1.2",
      "vm": "github:jspm/nodelibs-vm@0.1.0"
    },
    "npm:regenerator-runtime@0.9.5": {
      "path": "github:jspm/nodelibs-path@0.1.0",
      "process": "github:jspm/nodelibs-process@0.1.2"
    },
    "npm:util@0.10.3": {
      "inherits": "npm:inherits@2.0.1",
      "process": "github:jspm/nodelibs-process@0.1.2"
    },
    "npm:vm-browserify@0.0.4": {
      "indexof": "npm:indexof@0.0.1"
    }
  }
});