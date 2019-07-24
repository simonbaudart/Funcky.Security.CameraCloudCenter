/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
    sass = require("gulp-sass"),
    rename = require("gulp-rename"),
    cssmin = require("gulp-cssnano"),
    prefix = require("gulp-autoprefixer"),
    sourcemaps = require("gulp-sourcemaps"),
    gulpWebpack = require("webpack-stream"),
    watch = require("gulp-watch"),
    clean = require('gulp-clean');

var sassOptions = {
    outputStyle: "expanded"
};

gulp.task("clean",
    function ()
    {
        return gulp.src('./wwwroot/dist/*.js', { read: false })
            .pipe(clean());
    });

gulp.task("react:dev", gulp.series("clean",
    function ()
    {
        var config = require('./webpack.config');
        config.mode = 'development';
        delete config.optimization;
        return gulp.src('./src/**/*.ts')
            .pipe(gulpWebpack(config, require('webpack')))
            .pipe(gulp.dest('./wwwroot/dist/'));
    }));

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