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
    return c.redirect("/dashboard/panel/roles");
  });
  app.get("/dashboard/panel/:panelId", async (c) => {
    const token = getCookie(c, "AuthToken");
    var DisplayName = "NotSure";
    if (token) {
      // Calls the api to see if we should redirect to setup or dashboard

      const apiResponse = await fetch(
        `${process.env.URL}/admin/new/dashboard/panel`,
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

          var data = {
            title: "Dashboard",
            roleId: JsonParsed.roleId,
            moderator: JsonParsed.moderator,
            navbar: await ejs.renderFile(
              path.join(__dirname, `../views/partials/nav.ejs`),
              {
                displayName: JsonParsed.displayName,
                activeTab: "adminPanel",
              }
            ),
            NavItem: await ejs.renderFile(
               path.join(__dirname, `../views/partials/panel/NavItem.ejs`),
              {
                activeTab: c.req.param("panelId"),
              }
            ),
            NewsTab: "Empty Page",
            NewsForm: await ejs.renderFile(
              path.join(__dirname, `../views/partials/forms/NewsForm.ejs`)
            ),
          };

          console.log(c.req.param("panelId"))
          switch (c.req.param("panelId")) {
            case "roles":
              data.NewsTab = await ejs.renderFile(path.join(__dirname, `../views/partials/panel/RolesItem.ejs`), {
                moderator: JsonParsed.moderator,
                roleId: JsonParsed.roleId,
                AdminLists: JsonParsed.AdminLists,
                AddMod: await ejs.renderFile(
                  path.join(__dirname, `../views/partials/forms/AddMod.ejs`)
                ),
                EditMod: await ejs.renderFile(
                  path.join(__dirname, `../views/partials/forms/EditMod.ejs`)
                ),
              })
              break;
            case "sm":
              if (JsonParsed.roleId == 3) {
                const apiResponsePanel = await fetch(
                  `${process.env.URL}/admin/new/dashboard/content/ConfigData`,
                  {
                    method: "POST",
                    headers: {
                      Authorization: `Bearer ${token}`,
                      "Content-Type": "application/json",
                    },
                  }
                );

                const ConfigDataJsonParsed = await apiResponsePanel.json();
                if (ConfigDataJsonParsed) {
                  if (!ConfigDataJsonParsed.error) {
                    console.log(JSON.stringify(ConfigDataJsonParsed))
                    data.NewsTab = await ejs.renderFile(
                      path.join(
                        __dirname,
                        `../views/partials/panel/SMItem.ejs`
                      ),
                      {
                        Items: ConfigDataJsonParsed,
                        moderator: JsonParsed.moderator,
                        roleId: JsonParsed.roleId,
                      }
                    );
                  }
                }
              } else {
                console.log("NOT ADMIN");
              }
              break;
            default:
              break;
          }
          return c.html(await renderEJS("dashboard/AdminPanel.ejs", data));
        } else {
          return c.redirect("/login"); // Prob failed to login
        }
      }
    }

    return c.redirect("/login");
  });
}
