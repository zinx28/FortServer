import { useEffect, useState } from "react";
import userStore from "../lib/userStore";

export const useUserStore = () => {
  const [user, setUser] = useState(userStore.getState("user"));
  const [isAuthenticated, setIsAuthenticated] = useState(
    userStore.getState("isAuthenticated")
  );

  useEffect(() => {
    const interval = setInterval(() => {
      const currentUser = userStore.getState("user");
      const currentIsAuthenticated = userStore.getState("isAuthenticated");

      if (currentUser !== user) {
        setUser(currentUser);
      }

      if (currentIsAuthenticated !== isAuthenticated) {
        setIsAuthenticated(currentIsAuthenticated);
      }
    }, 100);

    return () => clearInterval(interval);
  }, [user, isAuthenticated]);

  const login = (userData: any) => {
    userStore.dispatch("setUser", userData);
  };

  const logout = () => {
    userStore.dispatch("logout");
  };

  return {
    user,
    isAuthenticated,
    login,
    logout,
  };
};
