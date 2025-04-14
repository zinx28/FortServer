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

export default function DashboardBase() {
  const { user, isAuthenticated } = useUserStore();
  const [activeTab, setActiveTab] = useState("news-update");
  const [activeLanguage, setActiveLanguage] = useState("en");
  const [contentView, setContentView] = useState("list");
  const [contentArraySections, setcontentArraySections] = useState([]);
  const [showAddDialog, setShowAddDialog] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
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

  const infoCards: InfoCard[] = [
    {
      title: "Game News",
      description: "Manage in-game news announcements in multiple languages",
      onClick: () => NewsPage(1),
    },
    {
      title: "Emergency Notices",
      description: "Manage emergency notifications in multiple languages",
      onClick: () => NewsPage(2),
    },
    {
      title: "Login Messages",
      description: "Manage login screen messages in multiple languages",
      onClick: () => NewsPage(3),
    },
    {
      title: "Playlist Info",
      description: "Manage playlist information in multiple languages",
      onClick: () => NewsPage(4),
    },
  ];
  const [SelectedPart, setSelectedPart] = useState<InfoCard>();
  const [IDOfSection, setIDOfSection] = useState(0);
  const [IDOfSectionIndex, setIDOfSectionIndex] = useState(0);
  type EditDataYUea = {
    Type: string;
    Data: any; //{
    //title: any;
    // body: any;
    // display_name: any;
    //};
  };
  const [editData, setEditData] = useState<EditDataYUea>();

  const handleLanguageChange = (langCode: any) => {
    setActiveLanguage(langCode);
  };

  const NewsPage = async (Index: number) => {
    console.log(infoCards[Index - 1]);
    setSelectedPart(infoCards[Index - 1]);
    setIDOfSection(Index);
    //http://127.0.0.1:1111/dashboard/v2/content/id/news/2
    var apiUrl = process.env.NEXT_PUBLIC_API_URL;
    const apiResponse = await fetch(
      `${apiUrl}/dashboard/v2/content/id/news/${Index}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
      }
    );

    const JsonParsed = await apiResponse.json();
    if (JsonParsed) {
      console.log(JsonParsed);
      if (Array.isArray(JsonParsed)) {
        setcontentArraySections(JsonParsed as []);
      }
    }

    setContentView("edit");
  };

  const languages = [
    { code: "en", name: "English" },
    { code: "es", name: "Spanish" },
    { code: "es-419", name: "Latin American Spanish" },
    { code: "fr", name: "French" },
    { code: "de", name: "German" },
    { code: "it", name: "Italian" },
    { code: "ja", name: "Japanese" },
    { code: "ko", name: "Korean" },
    { code: "pl", name: "Polish" },
    { code: "pt-BR", name: "Brazilian Portuguese" },
    { code: "ru", name: "Russian" },
    { code: "tr", name: "Turkish" },
  ];

  const getLanguageName = (code: any) => {
    const lang = languages.find((l: any) => l.code === code);
    return lang ? lang.name : code;
  };

  const [dataToSend, setDataToSend] = useState<Record<string, any>>({});

  const FindNewsPageDat = async (Index: number) => {
    setIDOfSectionIndex(Index);
    var apiUrl = process.env.NEXT_PUBLIC_API_URL;
    const response = await fetch(
      `${apiUrl}/dashboard/v2/content/data/news/${IDOfSection}/${Index}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/x-www-form-urlencoded",
        },
        credentials: "include",
      }
    );

    const JsonParsed = await response.json();

    if (JsonParsed) {
      setShowAddDialog(true);
      console.log(JsonParsed);
      const jsonObject = JsonParsed.Data as Record<string, any>;
      console.log(JsonParsed.Data);
      setDataToSend(
        Object.keys(jsonObject).reduce((acc, key) => {
          if (!key.startsWith("_") && jsonObject[key] !== undefined) {
            acc[key] = jsonObject[key];
          }
          return acc;
        }, {} as Record<string, any>)
      );
      setEditData(JsonParsed);
    }
  };

  const handleUpdate = async (key: string, value: string) => {
    //dataToSend[key][activeLanguage] = value;
    setDataToSend((prev) => {
      if (activeLanguage) {
        if (
          prev[key] &&
          typeof prev[key] === "object" &&
          !Array.isArray(prev[key])
        ) {
          return {
            ...prev,
            [key]: {
              ...prev[key],
              [activeLanguage]: value,
            },
          };
        } else {
          return {
            ...prev,
            [key]: value,
          };
        }
      }
      return {
        ...prev,
        [key]: value,
      };
    });
  };

  const handleSaveSection = async () => {
    setIsLoading(true);

    const test = Object.keys(dataToSend).reduce((acc, key) => {
      if (!key.startsWith("_") && dataToSend[key] !== undefined) {
        acc[key] = dataToSend[key];
      }
      return acc;
    }, {} as Record<string, any>);
    var apiUrl = process.env.NEXT_PUBLIC_API_URL;
    const response = await fetch(
      `${apiUrl}/dashboard/v2/content/update/news/${IDOfSection}/${IDOfSectionIndex}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(test),
        credentials: "include",
      }
    );

    const JsonParsed = await response.json();

    console.log(JsonParsed);

    setIsLoading(false);
  };

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
          onValueChange={setActiveTab}
          className="space-y-6"
        >
          <TabsList className="grid w-full grid-cols-4 mb-4">
            <TabsTrigger value="news-update">News Update</TabsTrigger>
            <TabsTrigger value="server-management">
              Server Management
            </TabsTrigger>
            <TabsTrigger value="ini-management">Ini Management</TabsTrigger>
            <TabsTrigger value="tournaments">Tournaments</TabsTrigger>
          </TabsList>

          <TabsContent value="news-update" className="space-y-6">
            {contentView === "list" ? (
              <Card>
                <CardHeader>
                  <CardTitle>News Management</CardTitle>
                  <CardDescription>
                    Manage game news, notices, and messages in multiple
                    languages
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4">
                    {infoCards.map((card, i) => (
                      <Card key={i}>
                        <CardHeader className="pb-2">
                          <CardTitle className="text-sm">
                            {card.title}
                          </CardTitle>
                        </CardHeader>
                        <CardContent>
                          <p className="text-xs text-muted-foreground mb-3">
                            {card.description}
                          </p>
                          <Button
                            variant="outline"
                            size="sm"
                            className="w-full"
                            onClick={card.onClick}
                          >
                            Manage
                          </Button>
                        </CardContent>
                      </Card>
                    ))}
                  </div>
                </CardContent>
              </Card>
            ) : showAddDialog != true ? (
              <>
                <div className="flex justify-between items-center">
                  <div className="flex items-center gap-2">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => setContentView("list")}
                    >
                      <ArrowLeft className="mr-2 h-4 w-4" />
                      Back
                    </Button>
                    <h2 className="text-xl font-semibold">Edit</h2>
                  </div>
                </div>

                <Card>
                  <CardHeader>
                    <CardTitle>{SelectedPart?.title}</CardTitle>
                    <CardDescription>
                      {SelectedPart?.description}
                    </CardDescription>
                  </CardHeader>
                  <CardContent>
                    {
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <TableHead>Title</TableHead>
                            <TableHead>Value</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {contentArraySections.map(
                            (Value: any, KeyMaybe: number) => (
                              <TableRow key={KeyMaybe}>
                                <TableCell>{Value}</TableCell>
                                <TableCell>
                                  <Button
                                    variant="ghost"
                                    size="sm"
                                    onClick={() => FindNewsPageDat(KeyMaybe)}
                                  >
                                    Edit
                                    <Edit className="h-4 w-4" />
                                  </Button>
                                </TableCell>
                              </TableRow>
                            )
                          )}
                        </TableBody>
                      </Table>
                    }
                  </CardContent>
                </Card>
              </>
            ) : (
              <>
                {" "}
                <div className="space-y-6">
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-2">
                      <Button
                        variant="outline"
                        onClick={() => setShowAddDialog(false)}
                      >
                        <ArrowLeft className="mr-2 h-4 w-4" />
                        Back
                      </Button>
                      <h2 className="text-xl font-semibold">
                        Edit {editData?.Type}
                      </h2>
                    </div>

                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button variant="outline" size="sm">
                          <Globe className="mr-2 h-4 w-4" />
                          {getLanguageName(activeLanguage)}
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end" className="w-56">
                        {languages.map((lang) => (
                          <DropdownMenuItem
                            key={lang.code}
                            onClick={() => handleLanguageChange(lang.code)}
                            className="flex justify-between"
                          >
                            {lang.name}
                            {activeLanguage === lang.code && (
                              <Check className="h-4 w-4" />
                            )}
                          </DropdownMenuItem>
                        ))}
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </div>

                  <Card>
                    <CardHeader>
                      <CardTitle>
                        Editing content in {getLanguageName(activeLanguage)}
                      </CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-6">
                      {/* my attempt of dnymiac ggs */}
                      {Object.entries(dataToSend).map(([key, value], index) => (
                        <div key={key} className="text-xs">
                          <label className="text-xs font-semibold capitalize">
                            {key}
                          </label>

                          {typeof value === "boolean" ? (
                            <Select
                              value={value ? "true" : "false"}
                              /* onValueChange={(val) =>
                                  handleUpdate(itemIndex, key, val === "true")
                                }*/
                            >
                              <SelectTrigger>
                                <SelectValue placeholder="Select value" />
                              </SelectTrigger>
                              <SelectContent>
                                <SelectItem value="true">true</SelectItem>
                                <SelectItem value="false">false</SelectItem>
                              </SelectContent>
                            </Select>
                          ) : typeof value === "object" &&
                            value !== null &&
                            !Array.isArray(value) ? (
                            <Input
                              value={
                                ((value as Record<string, string>) ?? {})[
                                  activeLanguage
                                ] || ""
                              }
                              onChange={(e) =>
                                handleUpdate(key, e.target.value)
                              }
                            />
                          ) : (
                            <Input
                              value={String(value)}
                              onChange={(e) =>
                                handleUpdate(key, e.target.value)
                              }
                            />
                          )}
                        </div>
                      ))}

                      <Button onClick={handleSaveSection} disabled={isLoading}>
                        {isLoading ? (
                          <>
                            <svg
                              className="mr-2 h-4 w-4 animate-spin"
                              xmlns="http://www.w3.org/2000/svg"
                              fill="none"
                              viewBox="0 0 24 24"
                            >
                              <circle
                                className="opacity-25"
                                cx="12"
                                cy="12"
                                r="10"
                                stroke="currentColor"
                                strokeWidth="4"
                              ></circle>
                              <path
                                className="opacity-75"
                                fill="currentColor"
                                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                              ></path>
                            </svg>
                            Saving...
                          </>
                        ) : (
                          "Save Changes"
                        )}
                      </Button>
                    </CardContent>
                  </Card>
                </div>
              </>
            )}
          </TabsContent>
        </Tabs>
      </main>
    </div>
  );
}
