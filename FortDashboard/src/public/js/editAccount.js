var AccountId = "";
document.addEventListener("DOMContentLoaded", function () {
  document
    .getElementById("EditModForm")
    .addEventListener("submit", async function (event) {
      event.preventDefault();

      const form = event.target;
      const formData = new FormData(form);
      const data = new URLSearchParams(formData);

      const response = await fetch(
        `http://127.0.0.1:1111/admin/new/dashboard/panel/user/edit`,
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
        }

        document.getElementById("editDiscordId").value = "";

        // if (JsonParsed.message != null) {
        //indow.location.href = "/home";
        //}
      }
    });
});
async function populateModalEditUserShow(UserData) {
  const adminInfo = UserData.getAttribute("data-admin-info");
  if (adminInfo) {
    var adminRadio = document.getElementById("AdminRadio1");
    var moderatorRadio = document.getElementById("ModeratorRadio1");
    var editUsersBox = document.getElementById("editUsersEmail");

    const response = await fetch(
      `http://127.0.0.1:1111/admin/new/dashboard/panel/user/edit/${adminInfo}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
      }
    );

    const JsonParsed = await response.json();

    if(JsonParsed){
      TempData = JsonParsed;
      //console.log("E " + JsonParsed);
      //console.log("E " + JSON.stringify(JsonParsed));
      editUsersBox.value = TempData.UserData.Email;
      AccountId = TempData.AccountId;
      document.getElementById("accountId").value = AccountId;
      if (TempData.adminInfo.Role == 3) {
        moderatorRadio.checked = false;
        adminRadio.checked = true;
      } else {
        moderatorRadio.checked = true;
        adminRadio.checked = false;
      }
    }
  }
}
