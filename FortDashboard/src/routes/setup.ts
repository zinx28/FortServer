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
    app.get("/login/setup", async (c) => {
        const token = getCookie(c, "AuthToken");

        if (token) {
            const apiResponse = await fetch(`${process.env.URL}/admin/new/login/check`, {
              method: "POST",
              headers: {
                Authorization: `Bearer ${token}`,
                "Content-Type": "application/json",
              },
            });
      
            const JsonParsed = await apiResponse.json();
            if(JsonParsed){
              if(!JsonParsed.error){
                  if(JsonParsed.setup == false) {
                    return c.redirect("/login")
                  }
              }else {
                return c.redirect("/login") // Prob failed to login
              }
            }   
            
            const data = {
                title: "Login",
                errorMessage: null
            }
            return c.html(await renderEJS("Setup.ejs", data));
        }

        return c.redirect("/login")
    })
}