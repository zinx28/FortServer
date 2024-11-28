async function logout() {
  const response = await fetch("http://127.0.0.1:1111/admin/new/logout", {
    method: "POST",
    headers: {
      "Content-Type": "application/x-www-form-urlencoded",
    },
    credentials: "include",
  });

  const JsonParsed = await response.json();

  if (JsonParsed) {
    if (JsonParsed.error == true) {
    } else {
        window.location.reload()
    }
  }
}
