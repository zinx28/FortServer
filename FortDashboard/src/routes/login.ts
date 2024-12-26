import { Hono } from "hono";
import { renderEJS } from "../util/rendering";

import {
  getCookie,
  getSignedCookie,
  setCookie,
  setSignedCookie,
  deleteCookie,
} from "hono/cookie";

export default function (app: Hono) {
  app.get("/login", async (c) => {
    const token = getCookie(c, "AuthToken");
    if (token) {
      const apiResponse = await fetch("http://localhost:1111/admin/new/login/check", {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      });
     
      const JsonParsed = await apiResponse.json();
   
      if(JsonParsed){
        if(!JsonParsed.error){
            console.log(JsonParsed.setup);
            if(JsonParsed.setup == true) {
                return c.redirect("/login/setup")
            }else {
                return c.redirect("/dashboard")
            }
        } 
      }
    }

    const data = {
      title: "Login",
      errorMessage: null,
    };
    return c.html(await renderEJS("Index.ejs", data));
  });
}
