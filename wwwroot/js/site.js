// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function showname () {
    var name = document.getElementById('btnUpload').value;
    // regex to remove everything before (and including) the last \.
    name = name.replace(/^.*[\\\/]/, '');
    document.getElementById('upload').innerHTML = name;

}
