﻿
// contains all the grunt config for the source-editor and it's data

module.exports = function (grunt) {

    grunt.config.merge({
        copy: {
            "source-editor-data" : { // currently only used for source-editor-snippets
                files: [
                    {
                        expand: true,
                        flatten: true,
                        cwd: "<%= paths.src %>/sxc-designer/",
                        src: ["**/*.json"],
                        dest: "<%= paths.dist %>/sxc-designer/",
                        rename: function (dest, src) {
                            return dest + src.replace(".json", ".js");
                        }
                    }
                ]
            },
            "source-editor-libs": {
                files: [
                    {
                        expand: true,
                        flatten: true,
                        cwd: "<%= paths.bower %>/angular-ui-ace/",
                        src: ["**/*.js"],
                        dest: "<%= paths.dist %>/lib/angular-ui-ace/"
                    }

                ]
                
            }
        }

    });
};