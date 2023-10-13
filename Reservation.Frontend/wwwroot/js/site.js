
const eventDataName ="form-event-data";
const businessExceptionName = "exception";

function triggerEventData(event) {
    const hasFreeHours = event.detail.hasFreeHours || false;

    const element = document.getElementById("reservationFormBtn");
    if (hasFreeHours) {
        element.removeAttribute("disabled");
    } else {
        element.setAttribute("disabled", "disabled");
    }
}

function handleBusinessException(event) {
    console.log(event.detail.value);
    
}

document.addEventListener("DOMContentLoaded", () => {
  
    document.body.addEventListener(eventDataName, triggerEventData);
    document.body.addEventListener(businessExceptionName, handleBusinessException);
    
    document.body.addEventListener("locationChanged", (event) => {
        console.log("location changed")
        const element = document.getElementById("reservationTime");
        console.log('element', element);
    })
    
})

