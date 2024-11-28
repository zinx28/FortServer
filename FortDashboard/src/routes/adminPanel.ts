import { Hono } from "hono";
import { renderEJS } from "../util/rendering";
import ejs from "ejs";
import * as path from "path";

import {
  getCookie,
  getSignedCookie,
  setCookie,
  setSignedCookie,
  deleteCookie,
} from "hono/cookie";

export default function (app: Hono) {
  app.get("/dashboard/panel", async (c) => {
    const token = getCookie(c, "AuthToken");
    var DisplayName = "NotSure";
    if (token) {
      // Calls the api to see if we should redirect to setup or dashboard

      const apiResponse = await fetch(
        "http://localhost:1111/admin/new/dashboard/panel",
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );

      const JsonParsed = await apiResponse.json();
      if (JsonParsed) {
        if (!JsonParsed.error) {
          console.log(JSON.stringify(JsonParsed));
          const data = {
            title: "Dashboard",
            roleId: JsonParsed.roleId,
            moderator: JsonParsed.moderator,
            AdminLists: JsonParsed.AdminLists,
            navbar: await ejs.renderFile(
              path.join(__dirname, `../views/partials/nav.ejs`),
              {
                displayName: JsonParsed.displayName,
                activeTab: "adminPanel",
              }
            ),
            AddMod: await ejs.renderFile(path.join(__dirname, `../views/partials/forms/AddMod.ejs`)),
            EditMod: await ejs.renderFile(path.join(__dirname, `../views/partials/forms/EditMod.ejs`))
          };
          return c.html(await renderEJS("dashboard/AdminPanel.ejs", data));
        } else {
          return c.redirect("/login"); // Prob failed to login
        }
      }
    }

    return c.redirect("/login");
  });
}
