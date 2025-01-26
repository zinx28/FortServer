document.addEventListener("DOMContentLoaded", function () {
    document
      .getElementById("AddModForm")
      .addEventListener("submit", async function (event) {
        event.preventDefault();
  
        const form = event.target;
        const formData = new FormData(form);
        const data = new URLSearchParams(formData);
  
        const response = await fetch(
          `${window.appConfig.apiUrl}/admin/new/dashboard/panel/grant`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/x-www-form-urlencoded",
            },
            body: data.toString(),
            credentials: "include",
          }
        ); 
  
        const JsonParsed = await response.json();
  
        console.log(JsonParsed);
        // const JsonParsed = JSON.parse(result);
  
        if (JsonParsed) {
          if (JsonParsed.error == true) {
            
          } else {
             window.location.reload();
          }
          
          document.getElementById('editDiscordId').value = "";

          // if (JsonParsed.message != null) {
          //indow.location.href = "/home";
          //}
        }
      });
  });
  