/// <binding BeforeBuild='default' />
/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require("gulp"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify");

var webroot = "wwwroot/";

var paths = {
    js: webroot + "js_src/**/*.js",
    minJs: webroot + "js/**/*.min.js",
    css: webroot + "css_src/**/*.css",
    minCss: webroot + "css/**/*.min.css",
    concatJsDest: webroot + "js/site.min.js",
    concatCssDest: webroot + "css/site.min.css"
};

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

//gulp.task("default", gulp.series("min:js", "min:css"));

// combined building js task and css task into run task
gulp.task("run", gulp.series("min:js", "min:css"));

// Task to watch both js and css change
gulp.task("watch", function () {
    gulp.watch(paths.js, gulp.series("min:js"));
    gulp.watch(paths.css, gulp.series("min:css"));
});

// Task to build change automatically
gulp.task("default", gulp.series("run","watch"));

