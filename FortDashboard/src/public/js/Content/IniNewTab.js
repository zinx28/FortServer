document.addEventListener("DOMContentLoaded", () => {
  const hiddenNewsId = document.getElementById("hiddenNewsId");
  const hiddensectionId = document.getElementById("hiddenSectionId");
  const hiddenArrayIndex = document.getElementById("hiddenArrayIndex");
  const toggleSwitch = document.getElementById("flexSwitchCheckDefault");
  const hiddenContext = document.getElementById("hiddenContext");

  const dropdownMenu = document.getElementById("dropdownmenu");
  if (!dropdownMenu) {
    console.error("Dropdown menu not found");
    return;
  }
  const divider = document.getElementById("dropdown-divider");
  const DropDownButton = document.getElementById("dropdownMenuButton");

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
      const contentID = target.getAttribute("data-contextId");
      const DataItem = target.getAttribute("data-item");

      if (context) {
        populateModalForEditNewsTab(context, contentID, DataItem);
      } else {
        console.warn("Button clicked without a valid data-context attribute.");
      }
    });
  });

  async function populateModalForEditNewsTab(context, contentID, DataItem) {
    console.log(`Editing context: ${context} with ID: ${contentID} | ${DataItem}`);

    const response = await fetch(
        `http://127.0.0.1:1111/admin/new/dashboard/content/data/${context}/${contentID}/${DataItem}`,
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
      
      
      const dynamicItems = dropdownMenu.querySelectorAll(".dynamic-item");
      dynamicItems.forEach((item) => item.remove());

      if (JsonParsed) {
        
        hiddenNewsId.value = DataItem;
        hiddenArrayIndex.value = "-1";
        hiddensectionId.value = contentID;
        hiddenContext.value = context;

         resetModalFields();

          if(context == "ini"){
            editBody.style.display = "block";
            editTitle.style.display = "block";
            editTabContent.style.display = "block";
            console.log(JSON.stringify(JsonParsed));

            const DataFR = JsonParsed.Data || [];

            DataFR.forEach((newsItem, index) => {
              console.log("TESTPENIS " + newsItem);
              const li = document.createElement("li");
              li.classList.add("dynamic-item");
              const a = document.createElement("a");
              a.classList.add("dropdown-item");
              a.href = "#";
              a.textContent = newsItem.Name;
              a.addEventListener("click", () =>
                populateModalForEditNewsTabDropDown(
                  "ini",
                  index,
                  newsItem,
                  a.textContent,
                  {
                    Section: DataItem,
                    Context: context,
                    ContentID: contentID,
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
      
      editTitle.value = newsItem.Name;
      editBody.value = newsItem.Value;
      
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
