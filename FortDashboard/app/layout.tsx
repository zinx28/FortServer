"use client";

import type { Metadata } from "next";
import "./globals.css";
import { ThemeProvider } from "@/components/theme-provider";
import { useUserStore } from "@/hooks/useUserStore";
import { useEffect } from "react";

//export const metadata: Metadata = {
//  title: "FortBackend",
//};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { user, isAuthenticated } = useUserStore();

  useEffect(() => {
    console.log("HI");
    console.log(user);
    console.log(isAuthenticated);
  }, [user]);

  return (
    <html lang="en" suppressHydrationWarning>
      <head>
        <title>FortBackend</title>
        <meta
          name="viewport"
          content="width=device-width, initial-scale=1.0, maximum-scale=1.0"
        />
      </head>
      <body>
        <ThemeProvider
          attribute="class"
          defaultTheme="dark"
          enableSystem={false}
          disableTransitionOnChange
        >
          {children}
        </ThemeProvider>
      </body>
    </html>
  );
}
