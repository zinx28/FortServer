"use client";

import type React from "react";

import { useEffect, useState } from "react";
import Link from "next/link";
import { AlertTriangle, BarChart3, Loader2, MessageSquare } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useUserStore } from "@/hooks/useUserStore";
import { useRouter } from "next/navigation";
import { cn } from "@/lib/utils";

export default function DashboardLogin() {
  const [isLoading, setIsLoading] = useState(false);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [rememberMe, setRememberMe] = useState(false);
  const { login } = useUserStore();
  const [apiError, setApiError] = useState("");
  const router = useRouter();

  const CheckLogin = async () => {
    var apiUrl = process.env.NEXT_PUBLIC_API_URL;
    console.log(apiUrl);
    const response = await fetch(`${apiUrl}/admin/new/login/check`, {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      credentials: "include",
    });
    var responsebc = await response.json();

    if (responsebc && !responsebc.error) {
      console.log("yo");
      login(responsebc);
      router.push("/dashboard");
    }
    console.log(responsebc);
  };

  useEffect(() => {
    setIsLoading(true);
    CheckLogin().finally(() => {
      setIsLoading(false);
    });
  }, []);

  console.log(process.env.NEXT_PUBLIC_API_URL);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    try {
      var apiUrl = process.env.NEXT_PUBLIC_API_URL;
      const form = e.target as HTMLFormElement;
      const formData = new FormData(form);
      const data = new URLSearchParams(formData as any);
      console.log(data.toString());
      const response = await fetch(`${apiUrl}/admin/new/login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/x-www-form-urlencoded",
        },
        body: data.toString(),
        credentials: "include",
      });

      const JsonParsed = await response.json();
      console.log(JsonParsed);
      if (JsonParsed) {
        if (!JsonParsed.error) {
          window.location.reload();
        } else {
          setApiError(JsonParsed.message)
        }
      }
      console.log("Login successful", { email, password, rememberMe });
    } catch (error) {
      console.error("Login failed", error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-muted/40 p-6">
      <Card className="w-full max-w-md mx-auto shadow-lg">
        <CardHeader className="flex flex-col items-center gap-4 text-center">
          <div className="rounded-full bg-primary p-3 text-primary-foreground flex items-center justify-center">
            <BarChart3 className="h-6 w-6" />
          </div>
          <CardTitle className="text-2xl font-bold">Dashboard Login</CardTitle>
          <CardDescription className="text-sm text-muted-foreground">
            Enter your credentials to access your dashboard
          </CardDescription>
        </CardHeader>

        <form onSubmit={handleSubmit}>
          <CardContent className="flex flex-col gap-4">
            <div className="flex flex-col gap-1.5">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                name="email"
                type="email"
                placeholder="name@example.com"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                disabled={isLoading}
              />
            </div>

            <div className="flex flex-col gap-1.5">
              <Label htmlFor="password">Password</Label>
              <Input
                id="password"
                name="password"
                type="password"
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                disabled={isLoading}
              />
            </div>

            <div className="flex items-center gap-2">
              <Checkbox
                id="remember"
                checked={rememberMe}
                onCheckedChange={(checked) => setRememberMe(checked as boolean)}
                disabled={isLoading}
              />
              <Label htmlFor="remember" className="text-sm font-normal">
                Remember me
              </Label>
            </div>

            {apiError && (
              <div className="flex items-center gap-2 rounded-md bg-destructive/10 p-3 text-sm text-destructive">
                <AlertTriangle className="h-4 w-4" />
                <p>{apiError}</p>
              </div>
            )}
          </CardContent>

          <CardFooter className="flex flex-col gap-4">
            <Button type="submit" className="w-full" disabled={isLoading}>
              {isLoading ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Signing in...
                </>
              ) : (
                "Sign in"
              )}
            </Button>

            <Link
              href="https://discord.gg/yapyap"
              target="_blank"
              rel="noopener noreferrer"
              className="flex items-center justify-center gap-2 text-sm text-primary hover:underline"
            >
              <MessageSquare className="h-4 w-4" />
              Any issues, suggestions? Join the Discord!
            </Link>
          </CardFooter>
        </form>
      </Card>
    </div>

  );
}
