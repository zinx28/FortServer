document.addEventListener("DOMContentLoaded", () => {
  const buttons = document.querySelectorAll("[data-context]");

  if (buttons.length === 0) {
    console.warn("No buttons with [data-context] attribute found.");
    return;
  }

  buttons.forEach((button) => {
    console.log("TES2T");
    button.addEventListener("click", (event) => {
      const target = event.currentTarget;
      const context = target.getAttribute("data-context");
      const contentID = target.getAttribute("data-id");
      console.log("TEST6969");
      if (context) {
        console.log("22222");
        populateModalForEditNewsTab(context, contentID, -1);
      } else {
        console.log("33333");
        console.warn("Button clicked without a valid data-context attribute.");
      }
    });
  });
});

async function populateModalForEditNewsTab(context, contentID, id) {
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
    const editBodyLabel = document.querySelector('label[for="editBody"]');
    if (editBodyLabel) {
      editBodyLabel.style.display = "block";
    }

    const editTitleLabel = document.querySelector('label[for="editTitle"]');
    if (editTitleLabel) {
      editTitleLabel.style.display = "block";
    }

    const IntNumberOnly = document.getElementById("IntNumberOnly");
    IntNumberOnly.style.display = "none";

    const editTitle = document.getElementById("editTitle");
    editTitle.style.display = "none";

    const editBody = document.getElementById("editBody");
    editBody.style.display = "none";

    const editTabContent = document.getElementById("editTabContent");
    editTabContent.style.display = "none";
    const dropdownMenu = document.getElementById("dropdownmenu");
    const divider = document.getElementById("dropdown-divider");

    if (!dropdownMenu) {
      console.error("Dropdown menu not found");
      return;
    }

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
              a.textContent
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
              a.textContent
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
            1,
            firstNewsItem,
            `News 1`
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
              a.textContent
            )
          );
          li.appendChild(a);

          dropdownMenu.insertBefore(li, divider);
        });

        if (allNews.length > 0) {
          const firstNewsItem = allNews[0];
          populateModalForEditNewsTabDropDown(
            "news",
            1,
            firstNewsItem,
            `Emergency 1`
          );
        }
      } else if (contentID == 3) {
        editBody.style.display = "block";
        editTitle.style.display = "block";
        const editBody = document.getElementById("editBody");

        if (editTitle && editBody) {
          editTitle.value = JsonParsed.title.en;
          editBody.value = JsonParsed.body.en;
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
              a.textContent
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

        const toggleSwitch = document.getElementById("flexSwitchCheckDefault");
        var forceSeason = JsonParsed.ForcedSeason || false;

        toggleSwitch.checked = forceSeason;
        IntNumberOnly.value = JsonParsed.SeasonForced;
      }
    }
  }
}

async function populateModalForEditNewsTabDropDown(
  context,
  index,
  newsItem,
  newsName
) {
  const editTitle = document.getElementById("editTitle");
  const editBody = document.getElementById("editBody");

  const DropDownButton = document.getElementById("dropdownMenuButton");

  console.log(JSON.stringify(newsItem.title));
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