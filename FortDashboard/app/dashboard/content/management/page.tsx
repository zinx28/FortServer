"use client";

import { useUserStore } from "@/hooks/useUserStore";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
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
  ArrowLeft,
  Edit,
  Check,
  Save,
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
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogDescription,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuTrigger,
  DropdownMenuItem,
} from "@/components/ui/dropdown-menu";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import {
  Accordion,
  AccordionItem,
  AccordionTrigger,
  AccordionContent,
} from "@/components/ui/accordion";

export default function DashboardBase() {
  const { user, isAuthenticated } = useUserStore();
  const [activeTab, setActiveTab] = useState("server-management");
  const router = useRouter();

  useEffect(() => {
    if (!isAuthenticated) router.push("/login");
  }, [isAuthenticated, router]);

  if (!isAuthenticated) return <div>Redirecting</div>;

  type InfoCard = {
    title: string;
    description: string;
    onClick: () => void;
  };

  type YKY = {
    ForcedSeason?: boolean;
    Season?: number;
    ShopRotation?: boolean;
    WeeklyQuests?: number;
  };
  
  const [SecondTabContent, setSecondTabContent] = useState<YKY>();

  const handleTabChange = async (value: string) => {
    setActiveTab(value);
    if (value === "server-management") {
      var apiUrl = process.env.NEXT_PUBLIC_API_URL;
      const response = await fetch(
        `${apiUrl}/dashboard/v2/content/data/server/1/69`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          credentials: "include",
        }
      );
      const ApiResponse = await response.json();

      if (ApiResponse) {
        console.log(ApiResponse);
        setSecondTabContent(ApiResponse);
      }
    }
  };

  const saveSEcondTabContent = async () => {
    var apiUrl = process.env.NEXT_PUBLIC_API_URL;
    const response = await fetch(
      `${apiUrl}/dashboard/v2/content/update/server/1/69`, 
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(SecondTabContent),
        credentials: "include",
      }
    );

    const ApiResponse = await response.json();

    if (ApiResponse) {
      console.log(ApiResponse);
    }
  }

  return (
    <div className="flex flex-1 flex-col">
      <header className="sticky top-0 z-10 flex h-14 items-center gap-4 border-b bg-background px-4 sm:px-6 lg:px-8">
        <div className="flex flex-1 items-center gap-2">
          <h1 className="text-xl font-semibold">Content</h1>
        </div>
        <div className="flex items-center gap-2">
          <Button variant="outline" size="icon">
            <Bell className="h-4 w-4" />
            <span className="sr-only">Notifications</span>
          </Button>
        </div>
      </header>

      <main className="flex-1 p-4 sm:p-6 lg:p-8">
        {/* Main tabs */}
        <Tabs
          value={activeTab}
          onValueChange={handleTabChange}
          className="space-y-6"
        >
          <TabsList className="grid w-full grid-cols-4 mb-4">
            <TabsTrigger value="news-update" onClick={() => router.push("/dashboard/content")}>News Update</TabsTrigger>
            <TabsTrigger value="server-management">
              Server Management
            </TabsTrigger>
            <TabsTrigger value="ini-management" onClick={() => router.push("/dashboard/content/ini")}>Ini Management</TabsTrigger>
            <TabsTrigger value="tournaments">Tournaments</TabsTrigger>
          </TabsList>

          <TabsContent value="server-management" className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle>Server Configuration</CardTitle>
                <CardDescription>
                  Manage server settings and game parameters
                </CardDescription>
              </CardHeader>

              <CardContent>
                <div className="space-y-6">
                  <Alert className="bg-amber-500/10 text-amber-500 border-amber-500/20">
                    <AlertTriangle className="h-4 w-4" />
                    <AlertTitle>Important</AlertTitle>
                    <AlertDescription>
                      Changes to server configuration will affect all connected
                      players. Some changes may require a server restart.
                    </AlertDescription>
                  </Alert>

                  <Accordion type="single" collapsible className="w-full">
                    <AccordionItem
                      value="season-settings"
                      className="border rounded-md px-4"
                    >
                      <AccordionTrigger className="py-4 hover:no-underline">
                        <span className="text-lg font-medium">
                          Season Settings
                        </span>
                      </AccordionTrigger>
                      <AccordionContent className="pb-4 pt-1">
                        <div className="space-y-4">
                          <div className="flex items-center justify-between">
                            <div>
                              <Label
                                htmlFor="force-season"
                                className="text-base"
                              >
                                Force Season
                              </Label>
                              <p className="text-sm text-muted-foreground">
                                Enable forced season for all players
                              </p>
                            </div>
                            <Switch
                              id="force-season"
                              checked={SecondTabContent?.ForcedSeason}
                              onCheckedChange={(checked) =>
                                setSecondTabContent((prev) => ({
                                  ...prev,
                                  ForcedSeason: checked,
                                }))
                              }
                            />
                          </div>

                          {SecondTabContent?.ForcedSeason && (
                            <>
                              <div className="space-y-2">
                                <Label htmlFor="season-number">
                                  Season Number
                                </Label>
                                <Input
                                  id="seasonNumber"
                                  type="number"
                                  placeholder="Season NUmber"
                                  value={SecondTabContent?.Season}
                                  onChange={(e) =>
                                    setSecondTabContent((prev) => ({
                                      ...prev,
                                      Season: Number(e.target.value),
                                    }))
                                  }
                                  min="0"
                                />
                                <p className="text-xs text-muted-foreground">
                                  Season to force for all players
                                </p>
                              </div>

                              <div className="space-y-2">
                                <Label htmlFor="weekly-quests">
                                  Weekly Quests
                                </Label>
                                <Input
                                  id="weekly-quests"
                                  type="number"
                                  placeholder="Enter number of quests"
                                  value={SecondTabContent.WeeklyQuests}
                                  onChange={(e) =>
                                    setSecondTabContent((prev) => ({
                                      ...prev,
                                      WeeklyQuests: Number(e.target.value),
                                    }))
                                  }
                                  min="0"
                                />
                                <p className="text-xs text-muted-foreground">
                                  Number of weekly quests granted to players (0
                                  or more)
                                </p>
                              </div>

                              <div className="space-y-2">
                                <Label
                                  htmlFor="force-season"
                                  className="text-base"
                                >
                                  Shop Rotation
                                </Label>
                                <p className="text-sm text-muted-foreground">
                                  Shop rotations, resets daily
                                </p>

                                <Switch
                                  id="shop-rotation"
                                  checked={SecondTabContent?.ShopRotation}
                                  onCheckedChange={(checked) =>
                                    setSecondTabContent((prev) => ({
                                      ...prev,
                                      ShopRotation: checked,
                                    }))
                                  }
                                />
                              </div>
                            </>
                          )}
                        </div>
                      </AccordionContent>
                    </AccordionItem>
                  </Accordion>

                  <Button className="w-full" onClick={() => saveSEcondTabContent()}>
                    <Save className="mr-2 h-4 w-4"/>
                    Save Configuration
                  </Button>
                </div>
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>
      </main>
    </div>
  );
}
