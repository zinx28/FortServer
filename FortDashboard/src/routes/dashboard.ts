import { Hono } from "hono";
import { renderEJS } from "../util/rendering";
import ejs from 'ejs'
import * as path from "path";

import {
  getCookie,
  getSignedCookie,
  setCookie,
  setSignedCookie,
  deleteCookie,
} from "hono/cookie";

export default function (app: Hono) {
  app.get("/dashboard/home", async (c) => {
      return c.redirect("/dashboard")
  });
  app.get("/dashboard", async (c) => {
    const token = getCookie(c, "AuthToken");
    var DisplayName = "NotSure";
    if (token) {
      // Calls the api to see if we should redirect to setup or dashboard

      const apiResponse = await fetch(
        `${process.env.URL}/admin/new/dashboard`,
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
          const data = {
            title: "Dashboard",
            connectedPeople: JsonParsed.PeopleConnected,
            ForcedSeason: JsonParsed.ForceSeason,
            SeasonForced: JsonParsed.SeasonForced,
            navbar: await ejs.renderFile(path.join(__dirname, `../views/partials/nav.ejs`), {
              displayName: JsonParsed.displayName,
              activeTab: "dashboard"
           })
          };
          return c.html(await renderEJS("dashboard/Home.ejs", data));
        } else {
          return c.redirect("/login"); // Prob failed to login
        } 
      }
    }

    return c.redirect("/login");
  });
}
