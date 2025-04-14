"use client";

import { useUserStore } from "@/hooks/useUserStore";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import Sidebar from "./sidebar";
import { Button } from "@/components/ui/button";
import {
  Activity,
  AlertTriangle,
  Bell,
  Settings,
  Users,
  Calendar,
  AlertCircle,
  Shield,
  Globe,
} from "lucide-react";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Switch } from "@/components/ui/switch";
import { Label } from "@/components/ui/label";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useTheme } from "next-themes";
import Link from "next/link";

export default function DashboardBase() {
  const { user, isAuthenticated } = useUserStore();
  const router = useRouter();

  useEffect(() => {
    if (!isAuthenticated) router.push("/login");
  }, [isAuthenticated, router]);

  if (!isAuthenticated) return <div>Redirecting</div>;

  const { theme, setTheme, resolvedTheme } = useTheme();

  return (
    <div className="flex flex-1 flex-col">
      <header className="sticky top-0 z-10 flex h-14 items-center gap-4 border-b bg-background px-4 sm:px-6 lg:px-8">
        <div className="flex flex-1 items-center gap-2">
          <h1 className="text-xl font-semibold">Dashboard</h1>
        </div>
        <div className="flex items-center gap-2">
          <Button variant="outline" size="icon">
            <Bell className="h-4 w-4" />
            <span className="sr-only">Notifications</span>
          </Button>
        </div>
      </header>

      <main className="flex-1 p-4 sm:p-6 lg:p-8">
        <Tabs defaultValue="overview" className="space-y-4">
          <div className="flex items-center justify-between">
            <TabsList>
              <TabsTrigger value="overview">Overview</TabsTrigger>
              <TabsTrigger value="analytics">Analytics</TabsTrigger>
              <TabsTrigger value="settings">Settings</TabsTrigger>
            </TabsList>
          </div>

          <TabsContent value="overview" className="space-y-4">
            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
              <Card>
                <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                  <CardTitle className="text-sm font-medium">
                    People Connected
                  </CardTitle>
                  <Users className="h-4 w-4 text-muted-foreground" />
                </CardHeader>
                <CardContent>
                  <div className="text-2xl font-bold">0</div>
                  <p className="text-xs text-muted-foreground">
                    No active connections
                  </p>
                </CardContent>
              </Card>

              <Card>
                <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                  <CardTitle className="text-sm font-medium">
                    Season Status
                  </CardTitle>
                  <Calendar className="h-4 w-4 text-muted-foreground" />
                </CardHeader>
                <CardContent>
                  <div className="flex items-center space-x-2">
                    <Badge
                      variant="outline"
                      className="text-amber-500 border-amber-500"
                    >
                      Inactive
                    </Badge>
                    <span className="text-sm text-muted-foreground">
                      Forced Season: false
                    </span>
                  </div>
                  <div className="mt-3 flex items-center space-x-2">
                    <Switch id="forced-season" />
                    <Label htmlFor="forced-season">Enable Forced Season</Label>
                  </div>
                  <p className="mt-2 text-xs text-amber-500">
                    Forced season is recommended
                  </p>
                </CardContent>
              </Card>

              <Card>
                <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                  <CardTitle className="text-sm font-medium">
                    System Status
                  </CardTitle>
                  <Activity className="h-4 w-4 text-muted-foreground" />
                </CardHeader>
                <CardContent>
                  <div className="flex items-center space-x-2">
                    <Badge
                      variant="outline"
                      className="bg-green-500/10 text-green-500 border-green-500/20"
                    >
                      Operational
                    </Badge>
                  </div>
                  <p className="mt-2 text-xs text-muted-foreground">
                    All systems running normally
                  </p>
                </CardContent>
              </Card>
            </div>

            <div className="space-y-4">
              <h2 className="text-xl font-semibold">Quick Access</h2>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                <Link href="/players" className="block">
                  <div className="flex items-center justify-between p-4 rounded-lg border bg-card hover:bg-accent/50 transition-colors">
                    <div className="flex items-center gap-3">
                      <div className="flex h-10 w-10 items-center justify-center rounded-full bg-blue-500/10">
                        <Users className="h-5 w-5 text-blue-500" />
                      </div>
                      <span className="font-medium">Players</span>
                    </div>
                    <div className="text-muted-foreground">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="24"
                        height="24"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        className="h-5 w-5"
                      >
                        <polyline points="9 18 15 12 9 6"></polyline>
                      </svg>
                    </div>
                  </div>
                </Link>

                <Link href="/regions" className="block">
                  <div className="flex items-center justify-between p-4 rounded-lg border bg-card hover:bg-accent/50 transition-colors">
                    <div className="flex items-center gap-3">
                      <div className="flex h-10 w-10 items-center justify-center rounded-full bg-green-500/10">
                        <Globe className="h-5 w-5 text-green-500" />
                      </div>
                      <span className="font-medium">Regions</span>
                    </div>
                    <div className="text-muted-foreground">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="24"
                        height="24"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        className="h-5 w-5"
                      >
                        <polyline points="9 18 15 12 9 6"></polyline>
                      </svg>
                    </div>
                  </div>
                </Link>

                <Link href="/moderation" className="block">
                  <div className="flex items-center justify-between p-4 rounded-lg border bg-card hover:bg-accent/50 transition-colors">
                    <div className="flex items-center gap-3">
                      <div className="flex h-10 w-10 items-center justify-center rounded-full bg-amber-500/10">
                        <Shield className="h-5 w-5 text-amber-500" />
                      </div>
                      <span className="font-medium">Moderation</span>
                    </div>
                    <div className="text-muted-foreground">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="24"
                        height="24"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        className="h-5 w-5"
                      >
                        <polyline points="9 18 15 12 9 6"></polyline>
                      </svg>
                    </div>
                  </div>
                </Link>
              </div>
            </div>

            <div className="grid gap-4 md:grid-cols-1 lg:grid-cols-1">
              <Card className="col-span-1">
                <CardHeader>
                  <CardTitle>System Information</CardTitle>
                  <CardDescription>Important system details</CardDescription>
                </CardHeader>
                <CardContent>
                  <Alert className="mb-4">
                    <AlertTriangle className="h-4 w-4" />
                    <AlertTitle>Open Source Project</AlertTitle>
                    <AlertDescription>
                      This is an open source project. Contributions are welcome.
                    </AlertDescription>
                  </Alert>

                  <div className="rounded-lg border p-3">
                    <div className="flex items-center gap-2">
                      <Bell className="h-4 w-4 text-muted-foreground" />
                      <p className="text-sm font-medium">Feedback Request</p>
                    </div>
                    <p className="mt-2 text-sm text-muted-foreground">
                      Please share any UI recommendations or issues on our
                      Discord server.
                    </p>
                    <div className="mt-3">
                      <Button variant="outline" size="sm">
                        <a href="https://discord.gg/8kCu6PDvPd" target="_blank">
                          Join Discord
                        </a>
                      </Button>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </div>
          </TabsContent>

          <TabsContent value="analytics" className="space-y-4">
            <Card>
              <CardHeader>
                <CardTitle>Analytics</CardTitle>
                <CardDescription>
                  View detailed system analytics and metrics
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="flex h-[300px] items-center justify-center rounded-md border border-dashed">
                  <div className="flex flex-col items-center gap-1 text-center">
                    <AlertCircle className="h-8 w-8 text-muted-foreground" />
                    <p className="text-sm text-muted-foreground">Analytics</p>
                    <Button variant="outline" size="sm" disabled>
                      Coming Soon
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="settings" className="space-y-4">
            <Card>
              <CardHeader>
                <CardTitle>Settings</CardTitle>
                <CardDescription>
                  Manage your dashboard preferences, this will be moved soon
                </CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="theme">Theme</Label>
                  <Select value={theme} onValueChange={setTheme}>
                    <SelectTrigger>
                      <SelectValue>{theme || "Select theme"}</SelectValue>
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="light">Light</SelectItem>
                      <SelectItem value="dark">Dark</SelectItem>
                      <SelectItem value="system">System</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>
      </main>
    </div>
  );
}
