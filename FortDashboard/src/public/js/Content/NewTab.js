document.addEventListener("DOMContentLoaded", () => {
  const editTitle = document.getElementById("editTitle");
  const editBody = document.getElementById("editBody");
  const editTabContent = document.getElementById("editTabContent");
  const dropdownMenu = document.getElementById("dropdownmenu");
  if (!dropdownMenu) {
    console.error("Dropdown menu not found");
    return;
  }
  const divider = document.getElementById("dropdown-divider");
  const hiddenNewsId = document.getElementById("hiddenNewsId");
  const hiddensectionId = document.getElementById("hiddenSectionId");
  const hiddenArrayIndex = document.getElementById("hiddenArrayIndex");
  const hiddenContext = document.getElementById("hiddenContext");
  const DropDownButton = document.getElementById("dropdownMenuButton");
  const IntNumberOnly = document.getElementById("IntNumberOnly");
  const toggleSwitch = document.getElementById("flexSwitchCheckDefault");

  const buttons = document.querySelectorAll("[data-context]");

  if (buttons.length === 0) {
    console.warn("No buttons with [data-context] attribute found.");
    return;
  }

  buttons.forEach((button) => {
    console.log("Button initialized.");
    button.addEventListener("click", (event) => {
      const target = event.currentTarget;
      const context = target.getAttribute("data-context");
      const contentID = target.getAttribute("data-id");

      if (context) {
        populateModalForEditNewsTab(context, contentID);
      } else {
        console.warn("Button clicked without a valid data-context attribute.");
      }
    });
  });

  // This file won't support ini stuff even though its close this api also gives a less big response if you tried
  async function populateModalForEditNewsTab(context, contentID) {
    console.log(`Editing context: ${context} with ID: ${contentID}`);

    const response = await fetch(
      `http://127.0.0.1:1111/admin/new/dashboard/content/data/${context}/${contentID}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
      }
    );

    const JsonParsed = await response.json();

    console.log(JsonParsed);

    if (JsonParsed) {
      console.log(JSON.stringify(JsonParsed));
      resetModalFields();

      const editBodyLabel = document.querySelector('label[for="editBody"]');
      if (editBodyLabel) {
        editBodyLabel.style.display = "block";
      }

      const editTitleLabel = document.querySelector('label[for="editTitle"]');
      if (editTitleLabel) {
        editTitleLabel.style.display = "block";
      }

      const editCheckBoxContent = document.getElementById(
        "editCheckBoxContent"
      );
      editCheckBoxContent.style.display = "none";

      const flexSwitchCheckDefaultLabel = document.querySelector(
        'label[for="flexSwitchCheckDefault"]'
      );
      flexSwitchCheckDefaultLabel.style.display = "none";

      const dynamicItems = dropdownMenu.querySelectorAll(".dynamic-item");
      dynamicItems.forEach((item) => item.remove());

      if (context == "news") {
        if (contentID == 1) {
          editBody.style.display = "block";
          editTitle.style.display = "block";
          editTabContent.style.display = "block";
          // Battle Royale News
          const allNews = JsonParsed.messages || [];
          const allMotds = JsonParsed.motds || [];

          allNews.forEach((newsItem, index) => {
            console.log("TESTPENIS " + newsItem);
            const li = document.createElement("li");
            li.classList.add("dynamic-item");
            const a = document.createElement("a");
            a.classList.add("dropdown-item");
            a.href = "#";
            a.textContent = `News ${index + 1}`;
            a.addEventListener("click", () =>
              populateModalForEditNewsTabDropDown(
                "news",
                index,
                newsItem,
                a.textContent,
                {
                  Section: "Messages",
                  Context: context,
                  ContentID: contentID,
                }
              )
            );

            li.appendChild(a);

            dropdownMenu.insertBefore(li, divider);
          });

          allMotds.forEach((motdItem, index) => {
            const li = document.createElement("li");
            li.classList.add("dynamic-item");
            const a = document.createElement("a");
            a.classList.add("dropdown-item");
            a.href = "#";
            a.textContent = `Motds ${index + 1}`;
            a.addEventListener("click", () =>
              populateModalForEditNewsTabDropDown(
                "motds",
                index,
                motdItem,
                a.textContent,
                {
                  Section: "Motds",
                  Context: context,
                  ContentID: contentID,
                }
              )
            );
            li.appendChild(a);
            dropdownMenu.insertBefore(li, divider);
          });

          // Make it show the content for the first dropdown item!!
          if (allNews.length > 0) {
            const firstNewsItem = allNews[0];
            populateModalForEditNewsTabDropDown(
              "news",
              0,
              firstNewsItem,
              `News 1`,
              {
                Section: "Messages",
                Context: context,
                ContentID: contentID,
              }
            );
          }
        } else if (contentID == 2) {
          editBody.style.display = "block";
          editTitle.style.display = "block";
          editTabContent.style.display = "block";
          const allNews = JsonParsed || [];

          allNews.forEach((newsItem, index) => {
            console.log("TESTPENIS " + newsItem);
            const li = document.createElement("li");
            li.classList.add("dynamic-item");
            const a = document.createElement("a");
            a.classList.add("dropdown-item");
            a.href = "#";
            a.textContent = `Emergency ${index + 1}`;
            a.addEventListener("click", () =>
              populateModalForEditNewsTabDropDown(
                "news",
                index,
                newsItem,
                a.textContent,
                {
                  Section: "Emergency",
                  Context: context,
                  ContentID: contentID,
                }
              )
            );
            li.appendChild(a);

            dropdownMenu.insertBefore(li, divider);
          });

          if (allNews.length > 0) {
            const firstNewsItem = allNews[0];
            populateModalForEditNewsTabDropDown(
              "news",
              0,
              firstNewsItem,
              `Emergency 1`,
              {
                Section: "Emergency",
                Context: context,
                ContentID: contentID,
              }
            );
          }
        } else if (contentID == 3) {
          editBody.style.display = "block";
          editTitle.style.display = "block";
          editTabContent.style.display = "none";

          if (editTitle && editBody) {
            editTitle.value = JsonParsed.title.en;
            editBody.value = JsonParsed.body.en;

            hiddenNewsId.value = "";
            hiddenArrayIndex.value = "-1";
            hiddensectionId.value = contentID;
            hiddenContext.value = context;
          }
        } else if (contentID == 4) {
          editBody.style.display = "block";
          editTitle.style.display = "block";
          editTabContent.style.display = "block";

          const allNews = JsonParsed || [];

          allNews.forEach((newsItem, index) => {
            console.log("TESTPENIS " + JSON.stringify(newsItem));
            const li = document.createElement("li");
            li.classList.add("dynamic-item");
            const a = document.createElement("a");
            a.classList.add("dropdown-item");
            a.href = "#";
            a.textContent = newsItem.playlist_name;
            a.addEventListener("click", () =>
              populateModalForEditNewsTabDropDown(
                "news~p",
                index,
                newsItem,
                a.textContent,
                {
                  Section: "",
                  Context: context,
                  ContentID: contentID,
                }
              )
            );
            li.appendChild(a);

            dropdownMenu.insertBefore(li, divider);
          });
        }
      } else if (context == "server") {
        if (contentID == 1) {
          IntNumberOnly.style.display = "block";
          editBodyLabel.style.display = "none";
          editTitleLabel.style.display = "none";
          editCheckBoxContent.style.display = "block";
          toggleSwitch.style.display = "block";

          hiddenNewsId.value = "";
          hiddenArrayIndex.value = "-1";
          hiddensectionId.value = contentID;
          hiddenContext.value = context;

          flexSwitchCheckDefaultLabel.style.display = "block";
          flexSwitchCheckDefaultLabel.textContent = "Forced Season";

          var forceSeason = JsonParsed.ForcedSeason || false;

          toggleSwitch.checked = forceSeason;
          IntNumberOnly.value = JsonParsed.SeasonForced;
        }
      } else if(context == "ini"){
        console.log("IDK");
      }
    }
  }

  async function populateModalForEditNewsTabDropDown(
    context,
    index,
    newsItem,
    newsName,
    ForcedData
  ) {
    const editTitle = document.getElementById("editTitle");
    const editBody = document.getElementById("editBody");
    console.log("E " + ForcedData);
    if (ForcedData != null) {
      if (ForcedData.Section != null) {
        hiddenNewsId.value = ForcedData.Section;
      }

      hiddenArrayIndex.value = index;
      hiddensectionId.value = ForcedData.ContentID;
      hiddenContext.value = ForcedData.Context;
    }

    // console.log(JSON.stringify(newsItem.title));
    if (editTitle && editBody) {
      if (context == "news~p") {
        editTitle.value = newsItem.display_name.en;
        editBody.value = newsItem.description.en;
      } else {
        editTitle.value = newsItem.title.en;
        editBody.value = newsItem.body.en;
      }
      DropDownButton.textContent = newsName;
    } else {
      DropDownButton.textContent = "Not Selected (FAILED)";
    }
  }

  function resetModalFields() {
    document.getElementById("hiddenNewsId").value = "";
    document.getElementById("hiddenSectionId").value = "";
    document.getElementById("hiddenArrayIndex").value = "-1";
    document.getElementById("hiddenContext").value = "";

    const editBody = document.getElementById("editBody");
    const editTitle = document.getElementById("editTitle");
    editBody.style.display = "none";
    editBody.value = "";
    editTitle.style.display = "none";
    editTitle.value = "";
    editTabContent.style.display = "none";
    toggleSwitch.style.display = "none";
    IntNumberOnly.style.display = "none";
    editTabContent.style.display = "none";
    DropDownButton.textContent = "Not Selected";
  }
});
