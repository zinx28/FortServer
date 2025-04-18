"use client";

import { useUserStore } from "@/hooks/useUserStore";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { AlertTriangle, Bell, Edit, FileText, Plus, Save } from "lucide-react";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  TableBody,
  TableHead,
  TableHeader,
  Table,
  TableRow,
  TableCell,
} from "@/components/ui/table";
import { ScrollArea } from "@radix-ui/react-scroll-area";
import { Textarea } from "@/components/ui/textarea";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";

export default function DashboardBase() {
  const { user, isAuthenticated } = useUserStore();
  const [activeTab, setActiveTab] = useState("ini-management");
  const router = useRouter();

  useEffect(() => {
    if (!isAuthenticated) router.push("/login");
  }, [isAuthenticated, router]);

  if (!isAuthenticated) return <div>Redirecting</div>;

  type IniFIleDataDATATAT = {
    FileName: string;
    IniValue: string;
  };

  type IniFileData = {
    FileName: string;
    Data: [
      {
        Title: string;
      }
    ];
  };
  const [iniFileData, setiniFileData] = useState<IniFileData[]>();
  const [validationError, setValidationError] = useState("");
  const [isSaving, setIsSaving] = useState(false);
  const [showSavedMessage, setShowSavedMessage] = useState(false);
  const [iniFullFileData, setiniFullFileData] =
    useState<IniFIleDataDATATAT[]>();
  const [advancedType, setAdvancedType] = useState(false);

  useEffect(() => {
    const test = async () => {
      var apiUrl = process.env.NEXT_PUBLIC_API_URL;
      const response = await fetch(
        `${apiUrl}/admin/new/dashboard/content/dataV2/ini/1`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          credentials: "include",
        }
      );
      const frfr = await response.json();

      setiniFileData(frfr);
      console.log(frfr);
    };
    test();
  }, []);

  const handleTabChange = async (value: string) => {
    setActiveTab(value);
  };

  const handleIniTabChange = async () => {
    if (!advancedType) {
      console.log("hi");
      var apiUrl = process.env.NEXT_PUBLIC_API_URL;
      const response = await fetch(
        `${apiUrl}/admin/new/dashboard/content/dataV2/ini/2`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          credentials: "include",
        }
      );
      const frfr = await response.json();
      console.log(frfr);

      setiniFullFileData(frfr);
    }
    setAdvancedType(!advancedType);
  };

  const [activeIniTab, setActiveIniTab] = useState("DefaultGame.ini");

  const handleSaveContent = async () => {
    setIsSaving(true);
    setValidationError("");
    setShowSavedMessage(false);

    //iniv128

    try {
      const content = iniFullFileData?.find(
        (e) => e.FileName == activeIniTab
      )?.IniValue;
      if (!content) return;
      const errors: string[] = [],
        lines = content.split("\n");

      // all you need to know, this does the syntax checks, it works about how i need it for now
      lines.forEach((line, i) => {
        const t = line.trim();
        if (t && !t.startsWith(";") && !t.startsWith("#")) {
          if (t.startsWith("[") && t.endsWith("]")) {
            const section = t.slice(1, -1).trim();

            if (!/^[A-Za-z0-9_./ ]+$/.test(section))
              errors.push(`Line ${i + 1}: Invalid section name.`);
          } else if (t.startsWith("!") && t.includes("=")) {
            const [key, value] = t
              .slice(1)
              .split("=")
              .map((s) => s.trim());

            if (!/^[A-Za-z0-9_.]+$/.test(key))
              errors.push(`Line ${i + 1}: Invalid key in commented line.`);
          } else if (t.startsWith("+") && t.includes("=")) {
            const key = t.slice(1).split("=")[0].trim();

            if (!/^[A-Za-z0-9_.]+$/.test(key))
              errors.push(`Line ${i + 1}: Invalid key in complex key-value.`);
          } else if (!t.includes("=")) {
            errors.push(
              `Line ${i + 1}: Missing equals sign in key-value pair.`
            );
          } else {
            const [key, value] = t.split("=").map((s) => s.trim());

            if (!/^[A-Za-z0-9_.]+$/.test(key) || !value) {
              errors.push(`Line ${i + 1}: Invalid key-value format.`);
            }
          }
        }
      });

      setValidationError(errors.join("\n"));

      // no errors
      if (errors.length <= 0) {
        var apiUrl = process.env.NEXT_PUBLIC_API_URL;
        const response = await fetch(
          `${apiUrl}/dashboard/v2/content/update/iniv128/uhm/thisisrequired`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(
              iniFullFileData?.find((e) => e.FileName == activeIniTab)
            ),
            credentials: "include",
          }
        );

        const JsonParsed = await response.json();

        console.log(JsonParsed);
      }
    } catch (err) {}

    setIsSaving(false);
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
          onValueChange={handleTabChange}
          className="space-y-6"
        >
          <TabsList className="grid w-full grid-cols-4 mb-4">
            <TabsTrigger
              value="news-update"
              onClick={() => router.push("/dashboard/content")}
            >
              News Update
            </TabsTrigger>
            <TabsTrigger
              value="server-management"
              onClick={() => router.push("/dashboard/content/management")}
            >
              Server Management
            </TabsTrigger>
            <TabsTrigger value="ini-management">Ini Management</TabsTrigger>
            <TabsTrigger value="tournaments">Tournaments</TabsTrigger>
          </TabsList>

          <TabsContent value="ini-management" className="space-y-6">
            <Card>
              <CardHeader className="flex flex-row items-center justify-between space-y-0">
                <div>
                  <CardTitle>INI File Management</CardTitle>
                  <CardDescription>Manage configuration files</CardDescription>
                </div>
                <Button onClick={() => handleIniTabChange()}>
                  <FileText className="mr-2 h-4 w-4" />
                  Advanced INI Manager
                </Button>
              </CardHeader>

              <CardContent>
                <div className="space-y-6">
                  <Tabs value={activeIniTab} onValueChange={setActiveIniTab}>
                    <TabsList className="grid w-full grid-cols-4">
                      {iniFileData?.map((e) => (
                        <TabsTrigger key={e.FileName} value={e.FileName}>
                          {e.FileName}
                        </TabsTrigger>
                      ))}
                    </TabsList>

                    <TabsContent value={activeIniTab} className="mt-6">
                      {!advancedType ? (
                        <>
                          <div className="flex justify-end mb-4">
                            <Button disabled>
                              <Plus className="mr-2 h-4 w-4" />
                              New
                            </Button>
                          </div>

                          <div className="rounded-md border">
                            <Table>
                              <TableHeader>
                                <TableRow>
                                  <TableHead className="w-[70%]">
                                    Section
                                  </TableHead>
                                  <TableHead className="text-right">
                                    Action
                                  </TableHead>
                                </TableRow>
                              </TableHeader>
                              <TableBody>
                                {iniFileData
                                  ?.find((e) => e.FileName == activeIniTab)
                                  ?.Data.map((section) => (
                                    <TableRow key={section.Title}>
                                      <TableCell className="font-medium">
                                        <code className="relative rounded bg-muted px-[0.3rem] py-[0.2rem] font-mono text-sm">
                                          {section.Title}
                                        </code>
                                      </TableCell>
                                      <TableCell className="text-right">
                                        <Button variant="secondary" size="sm">
                                          <Edit className="mr-2 h-4 w-4" />
                                          Edit
                                        </Button>
                                      </TableCell>
                                    </TableRow>
                                  ))}
                              </TableBody>
                            </Table>
                          </div>
                        </>
                      ) : (
                        <>
                          {validationError && (
                            <Alert className="bg-destructive/10 text-destructive border-destructive/20">
                              <AlertTriangle className="h-4 w-4" />
                              <AlertTitle>Syntax Error</AlertTitle>
                              <AlertDescription>
                                {validationError}
                              </AlertDescription>
                            </Alert>
                          )}
                          <Card>
                            <CardHeader>
                              <div className="flex items-center justify-between">
                                <div>
                                  <CardTitle>Edit {activeIniTab}</CardTitle>
                                  <CardDescription>
                                    Directly edit the INI file content. Changes
                                    will be validated before saving.
                                  </CardDescription>
                                </div>

                                <Button
                                  onClick={handleSaveContent}
                                  disabled={isSaving}
                                >
                                  {isSaving ? (
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
                                    <>
                                      <Save className="mr-2 h-4 w-4" />
                                      Save
                                    </>
                                  )}
                                </Button>
                              </div>
                            </CardHeader>

                            <CardContent>
                              <div className="relative">
                                <ScrollArea className="h-[600px] w-full rounded-md border">
                                  <Textarea
                                    className="min-h-[600px] font-mono text-sm resize-none border-0 focus-visible:ring-0 focus-visible:ring-offset-0 p-4"
                                    placeholder="Enter INI content..."
                                    onChange={(e) => {
                                      setiniFullFileData((prev) =>
                                        prev?.map((file) =>
                                          file.FileName === activeIniTab
                                            ? {
                                                ...file,
                                                IniValue: e.target.value,
                                              }
                                            : file
                                        )
                                      );
                                      setValidationError("");
                                    }}
                                    value={
                                      iniFullFileData?.find(
                                        (e) => e.FileName == activeIniTab
                                      )?.IniValue
                                    }
                                  />
                                </ScrollArea>
                              </div>
                            </CardContent>
                          </Card>
                        </>
                      )}
                    </TabsContent>
                  </Tabs>
                </div>
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>
      </main>
    </div>
  );
}
