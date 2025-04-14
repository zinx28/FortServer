'use client';

import { Activity, Users, Calendar, AlertTriangle, Bell, Settings } from "lucide-react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert"
import { Badge } from "@/components/ui/badge"
import { Switch } from "@/components/ui/switch"
import { Label } from "@/components/ui/label"
import { Select } from "@/components/ui/select"
import { SelectContent, SelectItem, SelectTrigger, SelectValue } from "@radix-ui/react-select"
import { useEffect, useState } from "react"
import { useRouter } from "next/router";

export default function Dashboard() {
  const [theme, setTheme] = useState(localStorage.getItem("theme") || "light");

  useEffect(() => {
    if (theme === "dark") {
      document.documentElement.classList.add("dark");
    } else {
      document.documentElement.classList.remove("dark");
    }
    localStorage.setItem("theme", theme);
  }, [theme]);

  return (
    <div className="flex min-h-screen bg-background">
      {/* Sidebar */}
      <div className="hidden border-r bg-muted/40 lg:block lg:w-64">
        <div className="flex h-full flex-col">
          <div className="flex h-14 items-center border-b px-6">
            <h2 className="text-lg font-semibold tracking-tight">FortBackend</h2>
          </div>
          <div className="flex-1 overflow-auto py-2">
            <nav className="grid items-start px-2 text-sm font-medium">
              <Button variant="ghost" className="flex justify-start gap-2 px-4 py-2 text-primary" asChild>
                <a href="#">
                  <Activity className="h-4 w-4" />
                  Dashboard
                </a>
              </Button>
              <Button variant="ghost" className="flex justify-start gap-2 px-4 py-2" asChild>
                <a href="#">
                  <Users className="h-4 w-4" />
                  Content Management
                </a>
              </Button>
              <Button variant="ghost" className="flex justify-start gap-2 px-4 py-2" asChild>
                <a href="#">
                  <Settings className="h-4 w-4" />
                  Admin Panel
                </a>
              </Button>
            </nav>
          </div>
          <div className="mt-auto border-t p-4">
            <div className="flex items-center gap-2 rounded-lg bg-primary/10 p-2">
              <div className="flex h-8 w-8 items-center justify-center rounded-full bg-primary text-primary-foreground">
                A
              </div>
              <div className="flex-1">
                <p className="text-sm font-medium">Admin</p>
              </div>
              <Button variant="ghost" size="icon" className="h-8 w-8">
                <Settings className="h-4 w-4" />
                <span className="sr-only">Settings</span>
              </Button>
            </div>
          </div>
        </div>
      </div>

      {/* Main content */}
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
              <div className="flex items-center gap-2">
                <Button variant="outline" size="sm">
                  Refresh
                </Button>
                <Button size="sm">Actions</Button>
              </div>
            </div>

            <TabsContent value="overview" className="space-y-4">
              <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                <Card>
                  <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                    <CardTitle className="text-sm font-medium">People Connected</CardTitle>
                    <Users className="h-4 w-4 text-muted-foreground" />
                  </CardHeader>
                  <CardContent>
                    <div className="text-2xl font-bold">0</div>
                    <p className="text-xs text-muted-foreground">No active connections</p>
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                    <CardTitle className="text-sm font-medium">Season Status</CardTitle>
                    <Calendar className="h-4 w-4 text-muted-foreground" />
                  </CardHeader>
                  <CardContent>
                    <div className="flex items-center space-x-2">
                      <Badge variant="outline" className="text-amber-500 border-amber-500">
                        Inactive
                      </Badge>
                      <span className="text-sm text-muted-foreground">Forced Season: false</span>
                    </div>
                    <div className="mt-3 flex items-center space-x-2">
                      <Switch id="forced-season" />
                      <Label htmlFor="forced-season">Enable Forced Season</Label>
                    </div>
                    <p className="mt-2 text-xs text-amber-500">Forced season is recommended</p>
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                    <CardTitle className="text-sm font-medium">System Status</CardTitle>
                    <Activity className="h-4 w-4 text-muted-foreground" />
                  </CardHeader>
                  <CardContent>
                    <div className="flex items-center space-x-2">
                      <Badge variant="outline" className="bg-green-500/10 text-green-500 border-green-500/20">
                        Operational
                      </Badge>
                    </div>
                    <p className="mt-2 text-xs text-muted-foreground">All systems running normally</p>
                  </CardContent>
                </Card>
              </div>

              <div className="grid gap-4 md:grid-cols-1 lg:grid-cols-2">
                <Card className="col-span-1">
                  <CardHeader>
                    <CardTitle>Upcoming Tasks</CardTitle>
                    <CardDescription>Tasks scheduled for implementation</CardDescription>
                  </CardHeader>
                  <CardContent>
                    <div className="space-y-4">
                      <div className="flex items-center gap-4">
                        <div className="flex h-9 w-9 items-center justify-center rounded-full bg-primary/10">
                          <Calendar className="h-5 w-5 text-primary" />
                        </div>
                        <div className="flex-1 space-y-1">
                          <p className="text-sm font-medium leading-none">Table Implementation</p>
                          <p className="text-sm text-muted-foreground">
                            Improve data tables with sorting and filtering
                          </p>
                        </div>
                        <Badge>Soon</Badge>
                      </div>

                      <div className="flex items-center gap-4">
                        <div className="flex h-9 w-9 items-center justify-center rounded-full bg-primary/10">
                          <Settings className="h-5 w-5 text-primary" />
                        </div>
                        <div className="flex-1 space-y-1">
                          <p className="text-sm font-medium leading-none">Button Refinements</p>
                          <p className="text-sm text-muted-foreground">Fix styling issues with buttons and controls</p>
                        </div>
                        <Badge>Soon</Badge>
                      </div>
                    </div>
                  </CardContent>
                </Card>

                <Card className="col-span-1">
                  <CardHeader>
                    <CardTitle>System Notifications</CardTitle>
                    <CardDescription>Recent alerts and messages</CardDescription>
                  </CardHeader>
                  <CardContent>
                    <Alert className="mb-4">
                      <AlertTriangle className="h-4 w-4" />
                      <AlertTitle>UI Improvements Needed</AlertTitle>
                      <AlertDescription>Tables and buttons require styling improvements</AlertDescription>
                    </Alert>

                    <div className="rounded-lg border p-3">
                      <div className="flex items-center gap-2">
                        <Bell className="h-4 w-4 text-muted-foreground" />
                        <p className="text-sm font-medium">Feedback Request</p>
                      </div>
                      <p className="mt-2 text-sm text-muted-foreground">
                        Please share any UI recommendations or issues on our Discord server.
                      </p>
                      <div className="mt-3">
                        <Button variant="outline" size="sm">
                          Join Discord
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
                  <CardDescription>View detailed system analytics and metrics</CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="flex h-[300px] items-center justify-center rounded-md border border-dashed">
                    <div className="flex flex-col items-center gap-1 text-center">
                      <p className="text-sm text-muted-foreground">Analytics data will be displayed here</p>
                      <Button variant="outline" size="sm">
                        Configure Analytics
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
                  <CardDescription>Manage your dashboard preferences</CardDescription>
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

                  <div className="flex items-center justify-between">
                    <Label htmlFor="notifications">Enable Notifications</Label>
                    <Switch id="notifications" defaultChecked />
                  </div>

                  <div className="flex items-center justify-between">
                    <Label htmlFor="analytics">Enable Analytics</Label>
                    <Switch id="analytics" defaultChecked />
                  </div>
                </CardContent>
              </Card>
            </TabsContent>
          </Tabs>
        </main>
      </div>
    </div>
  )
}
