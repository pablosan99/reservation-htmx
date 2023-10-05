document.addEventListener("DOMContentLoaded", () => {
    
    document.body.addEventListener("allowSubmit", (event) => {
        console.log("allowSubmit event script")
        const element = document.getElementById("reservationFormBtn");
        element.removeAttribute("disabled");
    })

    document.body.addEventListener("locationChanged", (event) => {
        console.log("location changed")
        const element = document.getElementById("reservationTime");
        console.log('element', element);
    })
    
})