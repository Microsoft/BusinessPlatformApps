"use strict";
function configure(aurelia) {
    aurelia.use
        .standardConfiguration();
    //.developmentLogging();
    //aurelia.use.plugin('aurelia-animator-css');
    //aurelia.use.plugin('aurelia-html-import-template-loader')
    aurelia.start().then(function () { return aurelia.setRoot(); });
}
exports.configure = configure;
