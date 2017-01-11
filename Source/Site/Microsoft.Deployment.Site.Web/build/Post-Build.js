var bundler = require('aurelia-bundler');
var del = require('del');
var gulp = require('gulp');
var runSequence = require('run-sequence');

var config = {
    force: true,
    baseURL: './wwwroot',
    configPath: './wwwroot/config.js',
    bundles: {
        'dist/app-build': {
            includes: [
              '*.js',
              '*.html!text',
              '*.css!text',
              'bootstrap/css/bootstrap.css!text'
            ],
            options: {
                inject: true,
                minify: true
            }
        },

        'dist/aurelia': {
            includes: [
                'aurelia-bootstrapper',
                'aurelia-event-aggregator',
                'aurelia-fetch-client',
                'aurelia-framework',
                'aurelia-history-browser',
                'aurelia-loader-default',
                'aurelia-logging-console',
                'aurelia-router',
                'aurelia-templating-binding',
                'aurelia-templating-resources',
                'aurelia-templating-router',
                'aurelia-http-client',
                'aurelia-polyfills'
            ],
           
            options: {
                'inject': true,
                'minify': true,
                'depCache': true
            }
        }
    }
};

gulp.task('Clean-Bundle', function () {
    return del([
    'wwwroot/dist/aurelia.js',
    'wwwroot/dist/app-build.js'
    ]);
});

gulp.task('Post-Build', function (callback) {
    runSequence('bundle', callback);
});

gulp.task('bundle', function () {
    return bundler.bundle(config);
});

gulp.task('unbundle', function () {
    return bundler.unbundle(config);
});