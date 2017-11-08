'use strict';

var gulp = require('gulp');
var gulpBabel = require('gulp-babel');
var del = require('del');
var sourcemaps = require('gulp-sourcemaps');
var minify = require('gulp-minify');
var mocha = require('gulp-mocha');
var browserify = require('gulp-browserify');

gulp.task('clean', function () {
    return del(['./lib']);
});
gulp.task('prepare-scripts', ['clean'], function () {
    return gulp.src('./src/**/*.js')
        .pipe(sourcemaps.init())
        .pipe(gulpBabel({
            presets: [ "es2015-loose" ],
            plugins: [
                "transform-object-assign", 
                "transform-es3-property-literals",
                "transform-es3-member-expression-literals",
                "transform-object-rest-spread",
                "transform-class-properties"                
            ]
        }))
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest('./lib/src'));
});
gulp.task('browserify-scripts', ['prepare-scripts'], function() {
    return gulp.src('./lib/src/logifyAlert.js')
        .pipe(browserify())
        .pipe(gulp.dest('./lib'));
});
gulp.task('minify-scripts', ['browserify-scripts'], function() {
    return gulp.src('./lib/logifyAlert.js')
        .pipe(minify({
            ext:{
                min:'.min.js'
            }
        }))
        .pipe(gulp.dest('./lib'));
});

gulp.task('watch', function () {
   gulp.watch('./src/**/*.js', ['browserify-scripts']);
});

gulp.task('prepare-tests', ['clean'], function () {
    return gulp.src('./test/**/*.js')
        .pipe(sourcemaps.init())
        .pipe(gulpBabel({
            presets: ['es2015']
        }))
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest('./lib/test'));
});
gulp.task('test', ['prepare-scripts', 'prepare-tests'], function () {
    return gulp.src('./lib/test/**/*.js', {read: false})
        .pipe(mocha({reporter: 'nyan'}));
});
