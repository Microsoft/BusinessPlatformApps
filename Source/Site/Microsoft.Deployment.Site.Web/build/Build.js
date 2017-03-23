require('babel-polyfill');

var babel = require('gulp-babel');
var gulp = require('gulp');
var sourcemaps = require('gulp-sourcemaps');
var typescript = require('gulp-typescript');

gulp.task('build-TS', function () {
    return gulp.src(['wwwroot/**/*.ts', 'typings/**.*'])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(typescript({
            declaration: false,
            emitDecoratorMetadata: true,
            experimentalDecorators: true,
            module: 'commonjs',
            noImplicitAny: false,
            noResolve: false,
            removeComments: true,
            sourceMap: false,
            target: 'es6'
        }))
        .pipe(babel({
            plugins: ['transform-runtime'],
            presets: ['es2015']
        }))
        //.pipe(sourcemaps.write('.', {
        //    includeContent: false,
        //    sourceRoot: '/'
        //}))
        .pipe(gulp.dest('wwwroot/'));
});

gulp.task('Build', ['build-TS'], function () {});