/// <binding Clean='clean' />
"use strict";

// noinspection JSUnresolvedFunction
var gulp = require("gulp"),
    gulpWebpack = require("webpack-stream"),
    clean = require('gulp-clean');

gulp.task("clean",
    function ()
    {
        return gulp.src('./wwwroot/dist/*.js', { read: false })
            .pipe(clean());
    });

gulp.task("react:dev", function ()
    {
        var config = require('./webpack.config');
        config.mode = 'development';
        delete config.optimization;
        return gulp.src('./src/**/*.ts')
            .pipe(gulpWebpack(config, require('webpack')))
            .pipe(gulp.dest('./wwwroot/dist/'));
    });

gulp.task("react:prod", gulp.series("clean",
    function ()
    {
        var config = require('./webpack.config');
        config.mode = 'production';
        delete config.devtool;
        return gulp.src('./src/**/*.ts')
            .pipe(gulpWebpack(config, require('webpack')))
            .pipe(gulp.dest('./wwwroot/dist/'));
    }));

gulp.task("react:watch",
    function ()
    {
        gulp.watch('./src/**/*.*', gulp.parallel("react:dev"));
    });