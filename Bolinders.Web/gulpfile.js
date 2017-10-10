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

//Start order
gulp.task("1-LESS", ["less"]);
gulp.task("2-CLEAN", ["clean:css", "clean:js"]);
gulp.task("3-MINIFY", ["min:js", "min:css"]);





// Compiles LESS to CSS
gulp.task("less", function () {
    //return gulp.src('./wwwroot/less/main.less')
    return gulp.src([paths.less])
        .pipe(less())
        .pipe(gulp.dest('wwwroot/css'));
});

// Clear JS
gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

// Clear CSS
gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

// CONCATS & MINIFY JS
gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

// CONCATS & MINIFY CSS
gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});