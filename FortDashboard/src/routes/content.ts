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
  app.get("/dashboard/content", async (c) => {
      return c.redirect("/dashboard/content/news")
  });
  app.get("/dashboard/content/:contentID", async (c) => {
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
                activeTab: "content",
              }
            ),
            NavItem: await ejs.renderFile(
              path.join(__dirname, `../views/partials/content/NavItem.ejs`),
              {
                activeTab: c.req.param("contentID"),
              }
            ),
            NewsTab: await ejs.renderFile(path.join(__dirname, `../views/partials/content/NewsTab.ejs`)),
            NewsForm: await ejs.renderFile(path.join(__dirname, `../views/partials/forms/NewsForm.ejs`))
          };

          switch (c.req.param("contentID")) {
            case "news":
              data.NewsTab = await ejs.renderFile(path.join(__dirname, `../views/partials/content/NewsTab.ejs`));
              break;
            case "server":
              data.NewsTab = await ejs.renderFile(path.join(__dirname, `../views/partials/content/ServerTab.ejs`));
              break;
            default:
              data.NewsTab = await ejs.renderFile(path.join(__dirname, `../views/partials/content/NewsTab.ejs`));
              break;
          }

          return c.html(await renderEJS("dashboard/Content.ejs", data));
        } else {
          return c.redirect("/login"); // Prob failed to login
        }
      }
    }

    return c.redirect("/login");
  });
}
