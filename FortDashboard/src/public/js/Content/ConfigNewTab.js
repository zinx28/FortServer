document.addEventListener("DOMContentLoaded", () => {
  const hiddenNewsId = document.getElementById("hiddenNewsId");
  const hiddensectionId = document.getElementById("hiddenSectionId");
  const hiddenArrayIndex = document.getElementById("hiddenArrayIndex");
  const toggleSwitch = document.getElementById("flexSwitchCheckDefault");
  const hiddenContext = document.getElementById("hiddenContext");
  const editCheckBoxContent = document.getElementById("editCheckBoxContent");
  const dropdownMenu = document.getElementById("dropdownmenu");
  if (!dropdownMenu) {
    console.error("Dropdown menu not found");
    return;
  }
  const IntNumberOnly = document.getElementById("IntNumberOnly");
  const divider = document.getElementById("dropdown-divider");
  const DropDownButton = document.getElementById("dropdownMenuButton");

  const buttons = document.querySelectorAll("[data-context]");

  if (buttons.length === 0) {
    console.warn("No buttons with [data-context] attribute found.");
    return;
  }

  const flexSwitchCheckDefaultLabel = document.querySelector(
    'label[for="flexSwitchCheckDefault"]'
  );

  buttons.forEach((button) => {
    console.log("Button initialized.");
    button.addEventListener("click", (event) => {
      const target = event.currentTarget;
      const context = target.getAttribute("data-context");
      const DataItem = target.getAttribute("data-item");

      console.log(context);

      if (context) {
        populateModalForEditNewsTab(context, DataItem);
      } else {
        console.warn("Button clicked without a valid data-context attribute.");
      }
    });
  });

  async function populateModalForEditNewsTab(context, DataItem) {
    console.log(`Editing context: ${context} with ID: ${DataItem}!`);

    const response = await fetch(
      `${window.appConfig.apiUrl}/admin/new/dashboard/content/ConfigData/${DataItem}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
      }
    );

    const JsonParsed = await response.json();

    console.log(JsonParsed.Data);

    const dynamicItems = dropdownMenu.querySelectorAll(".dynamic-item");
    dynamicItems.forEach((item) => item.remove());

    if (JsonParsed) {
      hiddenNewsId.value = DataItem;
      hiddenArrayIndex.value = "-1";
      //hiddensectionId.value = contentID;
      hiddenContext.value = context;

      resetModalFields();

      const editTitleLabel = document.querySelector('label[for="editTitle"]');
      if (editTitleLabel) {
        editTitleLabel.style.display = "none";
      }

      if (context == "config") {
        editTabContent.style.display = "block";

        const DataFR = JsonParsed.Data || [];

        DataFR.forEach((newsItem, index) => {
          console.log("TESTPENIS " + JSON.stringify(newsItem));
          const li = document.createElement("li");
          li.classList.add("dynamic-item");
          const a = document.createElement("a");
          a.classList.add("dropdown-item");
          a.href = "#";
          a.textContent = newsItem.Title;
          a.addEventListener("click", () =>
            populateModalForEditNewsTabDropDown(
              "none",
              index,
              newsItem,
              a.textContent,
              {
                Section: "",
                Context: context,
                ContentID: DataItem,
              }
            )
          );
          li.appendChild(a);

          dropdownMenu.insertBefore(li, divider);
        });
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
    //const editTitle = document.getElementById("editTitle");
    const editBody = document.getElementById("editBody");
    console.log("E " + ForcedData);
    if (ForcedData != null) {
      //if (ForcedData.Section != null) {
        hiddenNewsId.value = newsItem.METADATA;
      //}

      hiddenArrayIndex.value = index;
      hiddensectionId.value = ForcedData.ContentID;
      hiddenContext.value = ForcedData.Context;
     // hiddenContext.value = newsItem.METADATA;
    }
    if (editBody) {
      editBody.style.display = "none";
      toggleSwitch.style.display = "none";
      flexSwitchCheckDefaultLabel.style.display = "none";
      IntNumberOnly.style.display = "none";
      // needed with ulong bc number box cant be that big
      if (newsItem.Type == "string" || newsItem.Type == "ulong") {
        editBody.style.display = "block";
        editBody.value = newsItem.Value;
      } else if (newsItem.Type == "bool") {
        toggleSwitch.style.display = "block";
        editCheckBoxContent.style.display = "block";
        flexSwitchCheckDefaultLabel.style.display = "block";
        flexSwitchCheckDefaultLabel.textContent = newsItem.Title;

        var IsCheckedPLease = newsItem.Value || false;

        toggleSwitch.checked = IsCheckedPLease;
      } else if (newsItem.Type == "int") {
        IntNumberOnly.style.display = "block";
        IntNumberOnly.value = newsItem.Value;
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

    editCheckBoxContent.style.display = "none";

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
