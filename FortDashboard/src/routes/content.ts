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
    return c.redirect("/dashboard/content/news");
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
            NewsTab: await ejs.renderFile(
              path.join(__dirname, `../views/partials/content/NewsTab.ejs`)
            ),
            NewsForm: await ejs.renderFile(
              path.join(__dirname, `../views/partials/forms/NewsForm.ejs`)
            ),
          };

          switch (c.req.param("contentID")) {
            case "news":
              data.NewsTab = await ejs.renderFile(
                path.join(__dirname, `../views/partials/content/NewsTab.ejs`)
              );
              break;
            case "server":
              data.NewsTab = await ejs.renderFile(
                path.join(__dirname, `../views/partials/content/ServerTab.ejs`)
              );
              break;
            case "ini":
              const response = await fetch(
                `http://127.0.0.1:1111/admin/new/dashboard/content/dataV2/ini/1`,
                {
                  method: "POST",
                  headers: {
                    Authorization: `Bearer ${token}`,
                    "Content-Type": "application/json",
                  },
                }
              );
              const JsonParsed = await response.json();
              console.log(JSON.stringify(JsonParsed));
              // lil n
              if (JsonParsed) {
                if (JsonParsed.error == null) {
                  data.NewsTab = await ejs.renderFile(
                    path.join(
                      __dirname,
                      `../views/partials/content/IniTab.ejs`
                    ),
                    {
                      IniData: JsonParsed,
                    }
                  );
                }
              }
              break;
            case "tournaments":
              const Tourresponse = await fetch(
                `http://127.0.0.1:1111/admin/new/dashboard/content/dataV2/cup/tournaments`,
                {
                  method: "POST",
                  headers: {
                    Authorization: `Bearer ${token}`,
                    "Content-Type": "application/json",
                  },
                }
              );
              const frfrfrfr = await Tourresponse.json();
              console.log(JSON.stringify(frfrfrfr));
              if (Array.isArray(frfrfrfr)) {
                var FirstItem = frfrfrfr[0].ID;
                return c.redirect(`/dashboard/content/cup/${FirstItem}`);
              } else {
                data.NewsTab = await ejs.renderFile(
                  path.join(__dirname, `../views/partials/content/CupTab.ejs`),
                  {
                    //IniData: JsonParsed
                  }
                );
              }
              break;
            case "shop":
              data.NewsTab = await ejs.renderFile(
                path.join(__dirname, `../views/partials/content/ShopTab.ejs`)
              );
              break;
            default:
              data.NewsTab = await ejs.renderFile(
                path.join(__dirname, `../views/partials/content/NewsTab.ejs`)
              );
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
  app.get("/dashboard/content/cup/:cupID", async (c) => {
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
          const Tourresponse = await fetch(
            `http://127.0.0.1:1111/admin/new/dashboard/content/dataV2/cup/tournaments`,
            {
              method: "POST",
              headers: {
                Authorization: `Bearer ${token}`,
                "Content-Type": "application/json",
              },
            }
          );
          const frfrfrfr = await Tourresponse.json();
          console.log(JSON.stringify(frfrfrfr));
          if (Array.isArray(frfrfrfr)) {

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
                  activeTab: "tournaments",
                }
              ),
              NewsTab: await ejs.renderFile(
                path.join(__dirname, `../views/partials/content/CupTab.ejs`),
                {
                  CupData: frfrfrfr
                }
              ),
              NewsForm: await ejs.renderFile(
                path.join(__dirname, `../views/partials/forms/NewsForm.ejs`)
              ),
            };
            return c.html(await renderEJS("dashboard/Content.ejs", data));
          }
        }
      }
    }
    return c.redirect("/login");
  });
}
