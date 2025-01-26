document.addEventListener("DOMContentLoaded", function () {
  document
    .getElementById("NewsForm")
    .addEventListener("submit", async function (event) {
      event.preventDefault();

      const form = event.target;
      const formData = new FormData(form);
      const data = new URLSearchParams(formData);

      console.log("URLSearchParams:", data.toString());

      const response = await fetch(
        `${window.appConfig.apiUrl}/admin/new/dashboard/content/update`,
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

      }
    });
});
