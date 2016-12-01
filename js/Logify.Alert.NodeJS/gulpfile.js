'use strict';

var gulp = require('gulp');
var gulpBabel = require('gulp-babel');
var del = require('del');
var sourcemaps = require('gulp-sourcemaps');
var mocha = require('gulp-mocha');

gulp.task('clean', function () {
    return del(['./lib']);
});
gulp.task('prepare-scripts', ['clean'], function () {
    return gulp.src('./src/**/*.js')
        .pipe(sourcemaps.init())
        .pipe(gulpBabel({
            presets: ['es2015']
        }))
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest('./lib/src'));
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
// Release
gulp.task('clean-for-release', function () {
    return del(['./bundle']);
});
gulp.task('convert-for-release', ['clean-for-release'], function () {
    return gulp.src('./src/**/*.js')
        .pipe(gulpBabel({
            presets: ['es2015']
        }))
        .pipe(gulp.dest('./bundle/src'));
});
gulp.task('remove-test-file', ['convert-for-release'], function () {
    return del(['./bundle/src/testApp.js']);
});
gulp.task('copy-package', ['remove-test-file'], function () {
    return gulp.src('./package.json')
        .pipe(gulp.dest('./bundle'));
});
gulp.task('prepare-release', ['copy-package'], function () {
    return gulp.src('./README.md')
        .pipe(gulp.dest('./bundle'));
});