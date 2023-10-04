// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", () => {
    
    document.body.addEventListener("allowSubmit", (event) => {
        console.log("allowSubmit event script")
        const element = document.getElementById("reservationFormBtn");
        element.removeAttribute("disabled");
    })
    
})