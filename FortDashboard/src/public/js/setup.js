document.addEventListener("DOMContentLoaded", function () {
  document
    .getElementById("setupForm")
    .addEventListener("submit", async function (event) {
      event.preventDefault();

      const form = event.target;
      const formData = new FormData(form);
      const data = new URLSearchParams(formData);

      const response = await fetch(
        `${window.appConfig.apiUrl}/admin/new/login/setup`,
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
          const errorTextElement = document.getElementById("error-text");

          if (errorTextElement) {
            errorTextElement.textContent = JsonParsed.message;
            errorTextElement.style.display = "block";
          }
        } else {
          if (JsonParsed.login == true) {
            window.location.href = "/dashboard";
          } else {
            window.location.href = "/login";
          }
        }

        // if (JsonParsed.message != null) {
        //indow.location.href = "/home";
        //}
      }
    });
});
