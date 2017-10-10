"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    less = require('gulp-less'),
    fs = require("fs");

var paths = {
    webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.less = paths.webroot + "less/**/*.less";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";


// Compiles less to css
gulp.task("less", function () {
    //return gulp.src('./wwwroot/less/main.less')
    return gulp.src([paths.less])
        .pipe(less())
        .pipe(gulp.dest('wwwroot/css'));
});

// Deletes old minified js/css files
gulp.task("clean:js", ["less"], function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", ["clean:js"], function (cb) {
    rimraf(paths.concatCssDest, cb);
});


// Concats & uglifys js
gulp.task("min:js", ["clean:css"], function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

// Concats & uglifys css
gulp.task("min:css", ["min:js"], function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

// Chain
gulp.task("START", ["min:css"], function () {
    console.log("                           FINNISHED!")
});