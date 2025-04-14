"use client";

import {
  Activity,
  AlertTriangle,
  Calendar,
  Eye,
  EyeOff,
  Users,
} from "lucide-react";
import Sidebar from "./sidebar";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Progress } from "@/components/ui/progress";
import { useEffect, useState } from "react";
import { useUserStore } from "@/hooks/useUserStore";
import { useRouter } from "next/navigation";
export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { user, isAuthenticated } = useUserStore();
  console.log(user);

  const router = useRouter();
  useEffect(() => {
    if (!isAuthenticated) router.push("/login");
  }, [isAuthenticated, router]);

  const [showWelcome, setShowWelcome] = useState(user?.setup);
  const [showPasswordReset, setShowPasswordReset] = useState(false);
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);

  const [showEmailSetup, setShowEmailSetup] = useState(false);
  const [email, setEmail] = useState("");
  const [apiError, setApiError] = useState("test");

  const handleWelcomeClose = () => {
    setShowWelcome(false);
    setShowEmailSetup(true);
    setShowPasswordReset(false);
  };

  const handlePasswordReset = async (e: any) => {
    e.preventDefault();

    try {
      var apiUrl = process.env.NEXT_PUBLIC_API_URL;
      const response = await fetch(`${apiUrl}/admin/new/login/setup`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          Email: email,
          Password: password,
          Password_Cn: confirmPassword,
        }),
        credentials: "include",
      });

      const ResponseDATA = await response.json();

      if (ResponseDATA) {
        if (ResponseDATA.message) {
          setApiError(ResponseDATA.message);
        }

        if (ResponseDATA.login) {
          //window.location.reload(); // for now
          setShowPasswordReset(false);
        }
      }
    } catch (err) {}
  };

  const handleEmailSetup = (e: any) => {
    e.preventDefault();
    setShowEmailSetup(false);
    setShowPasswordReset(true);
  };

  return (
    <div className="flex min-h-screen bg-background">
      <Dialog open={showWelcome}>
        <DialogContent
          onInteractOutside={(e) => e.preventDefault()}
          onEscapeKeyDown={(e) => e.preventDefault()}
          className="sm:max-w-md [&>button]:hidden"
        >
          <DialogHeader>
            <DialogTitle>Welcome to FortBackend</DialogTitle>
            <DialogDescription>
              This is your new admin dashboard. Let's get you set up with a few
              quick steps.
            </DialogDescription>
          </DialogHeader>
          <div className="flex flex-col space-y-4">
            <div className="flex items-center space-x-4">
              <div className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10">
                <Activity className="h-5 w-5 text-primary" />
              </div>
              <div className="space-y-1">
                <p className="text-sm font-medium">Powerful Admin Tools</p>
                <p className="text-sm text-muted-foreground">
                  Manage your content and users with ease
                </p>
              </div>
            </div>
            <div className="flex items-center space-x-4">
              <div className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10">
                <Users className="h-5 w-5 text-primary" />
              </div>
              <div className="space-y-1">
                <p className="text-sm font-medium">User Management</p>
                <p className="text-sm text-muted-foreground">
                  Track connections and user activity
                </p>
              </div>
            </div>
            <div className="flex items-center space-x-4">
              <div className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10">
                <Calendar className="h-5 w-5 text-primary" />
              </div>
              <div className="space-y-1">
                <p className="text-sm font-medium">Season Control</p>
                <p className="text-sm text-muted-foreground">
                  Manage seasons and content releases
                </p>
              </div>
            </div>
          </div>
          <DialogFooter>
            <Button onClick={handleWelcomeClose}>Get Started</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={showEmailSetup}>
        <DialogContent className="sm:max-w-md [&>button]:hidden">
          <DialogHeader>
            <DialogTitle>Set Up Your Email</DialogTitle>
            <DialogDescription>
              Please provide an email address for your admin account.
            </DialogDescription>
          </DialogHeader>
          <form onSubmit={handleEmailSetup}>
            <div className="grid gap-4 py-4">
              <div className="space-y-2">
                <Label htmlFor="email">Email Address</Label>
                <Input
                  id="email"
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="admin@example.com"
                  required
                />
              </div>
              <p className="text-sm text-muted-foreground">
                Your email will be used for account recovery and notifications.
              </p>
            </div>
            <DialogFooter>
              <Button type="submit" disabled={!email}>
                Continue
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      <Dialog open={showPasswordReset} onOpenChange={setShowPasswordReset}>
        <DialogContent
          onInteractOutside={(e) => e.preventDefault()}
          onEscapeKeyDown={(e) => e.preventDefault()}
          className="sm:max-w-md [&>button]:hidden"
        >
          <DialogHeader>
            <DialogTitle>Reset Your Password</DialogTitle>
            <DialogDescription>
              For security reasons, please set a new password for your admin
              account.
            </DialogDescription>
          </DialogHeader>
          <form onSubmit={handlePasswordReset}>
            <div className="grid gap-4 py-4">
              <div className="space-y-2">
                <Label htmlFor="new-password">New Password</Label>
                <div className="relative">
                  <Input
                    id="new-password"
                    type={showPassword ? "text" : "password"}
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    className="pr-10"
                    required
                  />
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon"
                    className="absolute right-0 top-0 h-full px-3"
                    onClick={() => setShowPassword(!showPassword)}
                  >
                    {showPassword ? (
                      <EyeOff className="h-4 w-4" />
                    ) : (
                      <Eye className="h-4 w-4" />
                    )}
                    <span className="sr-only">Toggle password visibility</span>
                  </Button>
                </div>
              </div>
              <div className="space-y-2">
                <Label htmlFor="confirm-password">Confirm Password</Label>
                <Input
                  id="confirm-password"
                  type={showPassword ? "text" : "password"}
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  required
                />
              </div>
              <div className="space-y-2">
                <div className="flex items-center justify-between">
                  <Label>Password Strength</Label>
                  <span className="text-xs text-muted-foreground">
                    {password.length > 0
                      ? password.length < 8
                        ? "Weak"
                        : "Strong"
                      : ""}
                  </span>
                </div>
                <Progress
                  value={
                    password.length > 0 ? (password.length < 8 ? 30 : 100) : 0
                  }
                  className="h-2"
                />
              </div>

              {apiError && (
                <div className="rounded-md bg-destructive/15 p-3 text-sm text-destructive">
                  <div className="flex items-center gap-2">
                    <AlertTriangle className="h-4 w-4" />
                    <p>{apiError}</p>
                  </div>
                </div>
              )}
            </div>
            <DialogFooter className="flex justify-between sm:justify-between">
              <Button
                type="button"
                variant="outline"
                onClick={handleWelcomeClose}
              >
                Back
              </Button>
              <Button
                type="submit"
                disabled={!password || password !== confirmPassword}
              >
                Set Password
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      <Sidebar />
      <main className="flex-1 p-6 pl-64">{children}</main>
    </div>
  );
}
