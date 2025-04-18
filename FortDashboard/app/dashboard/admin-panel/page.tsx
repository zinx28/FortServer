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
  Plus,
  Edit,
  Trash2,
  Code,
  ArrowLeft,
  Check,
  Copy,
  EyeOff,
  Eye,
} from "lucide-react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
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
import { Input } from "@/components/ui/input";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { ScrollArea } from "@radix-ui/react-scroll-area";
import { Textarea } from "@/components/ui/textarea";
import { checkDomainOfScale } from "recharts/types/util/ChartUtils";

export default function DashboardBase() {
  const { user, isAuthenticated } = useUserStore();
  const [activeTab, setActiveTab] = useState("admins");
  const [discordId, setDiscordId] = useState("");
  const [showAddDialog, setShowAddDialog] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [showEditRoleDialog, setShowEditRoleDialog] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState("");
  type Admin = {
    AccountID: string;
    Username?: string;
    Role?: number;
    Disabled: boolean;
  };
  const [admins, setAdmins] = useState<Admin[]>([]);
  const [currentAdmin, setCurrentAdmin] = useState<Admin>();
  const [selectedRole, setSelectedRole] = useState("");

  const [contentView, setContentView] = useState("list");

  type ConfigItem = {
    Title: string;
    METADATA: string;
    Value: string | number | boolean;
    Type: string;
    Private: boolean;
  };

  type ConfigSections = {
    Key: string;
    Description: string;
    Data: ConfigItem[];
  };
  const [currentSection, setCurrentSection] = useState<ConfigSections | null>(
    null
  );
  const [showRawJsonDialog, setShowRawJsonDialog] = useState(false);
  const [copied, setCopied] = useState(false);
  const [rawJsonString, setRawJsonString] = useState("");
  const [modifiedSections, setModifiedSections] = useState<Map<number, any>>(
    new Map()
  );
  const [SectionNumber, setSectionNumber] = useState<number>();

  const router = useRouter();

  useEffect(() => {
    if (!isAuthenticated) router.push("/login");
  }, [isAuthenticated, router]);

  if (!isAuthenticated) return <div>Redirecting</div>; // idk it would still show

  const [rawConfig, setRawConfig] = useState({});

  useEffect(() => {
    if (showRawJsonDialog) {
      setRawJsonString(JSON.stringify(rawConfig, null, 2));
    }
  }, [showRawJsonDialog, rawConfig]);

  const handleCopyJson = () => {
    navigator.clipboard.writeText(rawJsonString);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const roles = [
    { value: "Owner", label: "Owner", color: "amber" },
    { value: "Admin", ID: 3, label: "Admin", color: "blue" },
    { value: "Moderator", ID: 1, label: "Moderator", color: "green" },
  ];

  const getInputType = (type: string) => {
    if (type === "int") {
      return "number";
    }
    return "text";
  };

  const handleSaveRawJson = async () => {
    setIsLoading(true);
    setError("penis");

    try {
      const parsedJson = JSON.parse(rawJsonString);

      await new Promise((resolve) => setTimeout(resolve, 800));

      setRawConfig(parsedJson);

      const updatedSections = configSections.map((section) => {
        return {
          ...section,
          Data: section.Data.map((field) => {
            if (parsedJson[field.METADATA] !== undefined) {
              return {
                ...field,
                Value: parsedJson[field.METADATA],
              };
            }
            return field;
          }),
        };
      });

      setConfigSections(updatedSections);
      setShowRawJsonDialog(false);
    } catch (error: any) {
      setError(error.message || "Failed to save configuration");
    } finally {
      setIsLoading(false);
    }
  };

  const [configSections, setConfigSections] = useState<ConfigSections[]>([]);

  const GrabPanelData = async () => {
    var apiUrl = process.env.NEXT_PUBLIC_API_URL;
    const apiResponse = await fetch(`${apiUrl}/admin/new/dashboard/panel`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
    });

    const JsonParsed = await apiResponse.json();
    console.log(JsonParsed);
    if (JsonParsed) {
      if (JsonParsed.AdminLists) {
        let sections: Admin[] = [];
        JsonParsed.AdminLists.forEach((e: any) => {
          sections.push({
            AccountID: e.adminInfo.AccountId,
            Username: e.DATA.UserName,
            Role: e.adminInfo.Role,
            Disabled: !JsonParsed.admin,
          });
        });
        setAdmins(sections);
      }
    }
  };

  useEffect(() => {
    GrabPanelData().finally(() => {
      console.log("she moans, when she falls");
    });
  }, []);

  const handleAddAdmin = async (e: any) => {
    e.preventDefault();
    setIsLoading(true);
    var apiUrl = process.env.NEXT_PUBLIC_API_URL;
    const response = await fetch(`${apiUrl}/admin/new/dashboard/panel/grant`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        DiscordID: discordId,
      }),
      credentials: "include",
    });

    const AdminPanelData = await response.json();
    console.log(AdminPanelData);
    if (AdminPanelData) {
      setError(AdminPanelData.message);
      if (AdminPanelData.error == false) {
        const newAdmin: Admin = {
          AccountID: AdminPanelData.data.AccountId,
          Username: AdminPanelData.data.DisplayName,
          Role: 1,
          Disabled: false,
        };
        setAdmins((prev) => [...prev, newAdmin]);
        setShowAddDialog(false);
      }
    }
    setIsLoading(false);
  };

  const handleSaveSection = async () => {
    setIsLoading(true);
    setError("");

    try {
      console.log("ye");
      if (modifiedSections.size == 0) return;

      const modifiedData = Array.from(modifiedSections.entries()).map(
        ([index, value]) => {
          return { index, value };
        }
      );
      console.log(modifiedData);
      var apiUrl = process.env.NEXT_PUBLIC_API_URL;

      const response = await fetch(
        `${apiUrl}/dashboard/v2/content/update/config/${SectionNumber}/69`, // real ones
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(modifiedData),
          credentials: "include",
        }
      );

      const JsonParsed = await response.json();

      if (JsonParsed) {
        if (JsonParsed.error == false) {
          console.log(JsonParsed);
        }
      }

      //const updatedSections = configSections.map((section) => {
      //  if (section.Key === currentSection?.Key) {
      //    return currentSection;
      //  }
      //  return section;
      //});

     // setConfigSections(updatedSections);
      //setContentView("list");
    } catch (error: any) {
      setError(error?.message || "Failed to save section");
    } finally {
      setIsLoading(false);
    }
  };

  //  Panel Edit User, returns data for the user
  const handleEditRole = async (admin: Admin) => {
    var apiUrl = process.env.NEXT_PUBLIC_API_URL;
    console.log(admin.AccountID);
    const apiResponse = await fetch(
      `${apiUrl}/admin/new/dashboard/panel/user/edit/${admin.AccountID}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
      }
    );
    var responsefr = await apiResponse.json();
    if (responsefr) {
      setCurrentAdmin(admin);
      console.log(responsefr.adminInfo.Role);
      setSelectedRole(
        roles.find((r) => r.ID === responsefr.adminInfo.Role)?.value || "???"
      );
      setIsLoading(false);
      setShowEditRoleDialog(true);
    }
  };

  // Panel Save User, for now its just roles
  const handleSaveRole = async () => {
    setIsLoading(true);
    try {
      var apiUrl = process.env.NEXT_PUBLIC_API_URL;
      console.log("test");
      console.log(currentAdmin);
      console.log(roles.find((r) => r.label === selectedRole)?.ID);
      var RoleID = roles.find((r) => r.label === selectedRole)?.ID ?? 1;
      const response = await fetch(
        `${apiUrl}/admin/new/dashboard/panel/user/edit`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            selectedRole: RoleID,
            accountId: currentAdmin?.AccountID,
          }),
          credentials: "include",
        }
      );

      const ResponseData = await response.json();

      if (ResponseData) {
        if (ResponseData.error == false) {
          setAdmins((a) =>
            a.map((a) =>
              a.AccountID === currentAdmin?.AccountID
                ? { ...a, Role: RoleID }
                : a
            )
          );
          setShowEditRoleDialog(false);
        }
      }
      setIsLoading(false);
    } catch (err) {}
  };

  //currentSection

  const handleUpdateField = (index: number, value: any) => {
    if (!currentSection) return;
    const updatedSection = { ...currentSection };
    if (Array.isArray(updatedSection.Data)) {
      updatedSection.Data[index].Value = value;
    }
    setCurrentSection(updatedSection);

    console.log(value);

    if (updatedSection.Data[index]?.Type === "bool") {
      
      value = value.toString();
    } else if (typeof updatedSection.Data[index]?.Value === "number") {
      value = BigInt(value).toString() || "0";
    }

    console.log(index);
    console.log(value);
    modifiedSections.set(index, value);
  };

  const handleEditSection = async (section: any) => {
    setCurrentSection(null);
    setError("");
    setModifiedSections(new Map<number, any>());
    var apiUrl = process.env.NEXT_PUBLIC_API_URL;

    console.log(section);
    setSectionNumber(section);

    const apiResponse = await fetch(
      `${apiUrl}/admin/new/dashboard/content/ConfigData/${section}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
      }
    );
    var responsefr = await apiResponse.json();
    if (responsefr) {
      console.log(responsefr);
      setCurrentSection(responsefr);
    }
    setContentView("edit");
  };

  const handleTabChange = async (value: string) => {
    setActiveTab(value);
    if (value === "content") {
      var apiUrl = process.env.NEXT_PUBLIC_API_URL;
      const apiResponse = await fetch(
        `${apiUrl}/admin/new/dashboard/content/ConfigData`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          credentials: "include",
        }
      );

      var ApiRe = await apiResponse.json();
      if (ApiRe) {
        console.log(ApiRe);

        setConfigSections(ApiRe);
      }
    }
  };

  return (
    <div className="flex flex-1 flex-col">
      <header className="sticky top-0 z-10 flex h-14 items-center gap-4 border-b bg-background px-4 sm:px-6 lg:px-8">
        <div className="flex flex-1 items-center gap-2">
          <h1 className="text-xl font-semibold">Admin Panel</h1>
        </div>
        <div className="flex items-center gap-2">
          <Button variant="outline" size="icon">
            <Bell className="h-4 w-4" />
            <span className="sr-only">Notifications</span>
          </Button>
        </div>
      </header>

      <main className="flex-1 p-4 sm:p-6 lg:p-8">
        <Tabs
          value={activeTab}
          onValueChange={handleTabChange}
          className="space-y-6"
        >
          <TabsList className="grid w-full grid-cols-2 mb-4">
            <TabsTrigger value="admins">Admins</TabsTrigger>
            <TabsTrigger value="content">Content</TabsTrigger>
          </TabsList>

          <TabsContent value="admins" className="space-y-6">
            <div className="flex justify-between items-center">
              <h2 className="text-xl font-semibold">Admin Management</h2>
              <Button onClick={() => setShowAddDialog(true)}>
                <Plus className="mr-2 h-4 w-4" />
                Add Admin
              </Button>
            </div>

            <Card>
              <CardHeader>
                <CardTitle>Admin List</CardTitle>
                <CardDescription>
                  Manage administrators who have access to the dashboard
                </CardDescription>
              </CardHeader>
              <CardContent>
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Display Name</TableHead>
                      <TableHead>Role</TableHead>
                      <TableHead className="text-right">Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    <TableRow key="-1">
                      <TableCell className="font-medium">Admin</TableCell>
                      <TableCell>
                        <Badge
                          variant="outline"
                          className="bg-amber-500/10 text-amber-500 border-amber-500/20"
                        >
                          Owner
                        </Badge>
                      </TableCell>
                      <TableCell className="text-right">
                        <div className="flex justify-end gap-2">
                          <Button variant="ghost" size="sm" disabled={true}>
                            <Edit className="h-4 w-4" />
                            <span className="sr-only">Edit role</span>
                          </Button>
                          <Button variant="ghost" size="sm" disabled={true}>
                            <Trash2 className="h-4 w-4 text-destructive" />
                            <span className="sr-only">Remove admin</span>
                          </Button>
                        </div>
                      </TableCell>
                    </TableRow>

                    {admins.map((admin) => (
                      <TableRow key={admin.AccountID}>
                        <TableCell className="font-medium">
                          {admin.Username}
                        </TableCell>
                        <TableCell>
                          <Badge
                            variant="outline"
                            className={
                              admin.Role == 3
                                ? "bg-blue-500/10 text-blue-500 border-blue-500/20"
                                : "bg-green-500/10 text-green-500 border-green-500/20"
                            }
                          >
                            {admin.Role == 3 ? "Admin" : "Moderator"}
                          </Badge>
                        </TableCell>
                        <TableCell className="text-right">
                          <div className="flex justify-end gap-2">
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => handleEditRole(admin)}
                              disabled={admin.Disabled}
                            >
                              <Edit className="h-4 w-4" />
                              <span className="sr-only">Edit role</span>
                            </Button>
                            <Button variant="ghost" size="sm" disabled={true}>
                              <Trash2 className="h-4 w-4 text-destructive" />
                              <span className="sr-only">Remove admin</span>
                            </Button>
                          </div>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="content" className="space-y-6">
            {contentView === "list" ? (
              <>
                <div className="flex justify-between items-center">
                  <h2 className="text-xl font-semibold">
                    Content Configuration
                  </h2>
                  <Button onClick={() => setShowRawJsonDialog(true)}>
                    <Code className="mr-2 h-4 w-4" />
                    Edit Raw JSON
                  </Button>
                </div>

                <Card>
                  <CardHeader>
                    <CardTitle>Configuration Sections</CardTitle>
                    <CardDescription>
                      Manage configuration settings for different parts of the
                      application
                    </CardDescription>
                  </CardHeader>
                  <CardContent>
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Section</TableHead>
                          <TableHead>Description</TableHead>
                          <TableHead className="text-right">Action</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {configSections.map((section, index) => (
                          <TableRow key={section.Key}>
                            <TableCell className="font-medium">
                              {section.Key}
                            </TableCell>
                            <TableCell>{section.Description}</TableCell>
                            <TableCell className="text-right">
                              <Button
                                variant="secondary"
                                size="sm"
                                onClick={() => handleEditSection(index)}
                              >
                                <Edit className="mr-2 h-4 w-4" />
                                Edit
                              </Button>
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </CardContent>
                </Card>
              </>
            ) : (
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
                    <h2 className="text-xl font-semibold">
                      Edit {currentSection?.Key}
                    </h2>
                  </div>
                  <div className="flex gap-2">
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
                  </div>
                </div>

                <Card>
                  <CardHeader>
                    <CardTitle>{currentSection?.Key}</CardTitle>
                    <CardDescription>
                      {currentSection?.Description}
                    </CardDescription>
                  </CardHeader>
                  <CardContent>
                    {
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <TableHead>Setting</TableHead>
                            <TableHead>Value</TableHead>
                            <TableHead>Type</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {currentSection?.Data.map(
                            (field: any, index: number) => (
                              <TableRow key={field.METADATA}>
                                <TableCell className="font-medium">
                                  {field.Title}
                                  {field.Private && (
                                    <Badge
                                      variant="outline"
                                      className="ml-2 bg-amber-500/10 text-amber-500 border-amber-500/20 text-xs"
                                    >
                                      private
                                    </Badge>
                                  )}
                                </TableCell>
                                <TableCell>
                                  {field.Type === "bool" ? (
                                    <Select
                                      value={String(field.Value)}
                                      onValueChange={(value) =>
                                        handleUpdateField(
                                          index,
                                          value === "true"
                                        )
                                      }
                                    >
                                      <SelectTrigger>
                                        <SelectValue placeholder="Select value" />
                                      </SelectTrigger>
                                      <SelectContent>
                                        <SelectItem value="true">
                                          true
                                        </SelectItem>
                                        <SelectItem value="false">
                                          false
                                        </SelectItem>
                                      </SelectContent>
                                    </Select>
                                  ) : (
                                    <div className="relative">
                                      <Input
                                        type={
                                          field.Private
                                            ? showPassword
                                              ? "text"
                                              : "password"
                                            : getInputType(field.Type)
                                        }
                                        value={field.Value}
                                        onChange={(e) => {
                                          const value =
                                          field.Type === "int"
                                            ? Number.parseInt(
                                                e.target.value
                                              ) || 0
                                            : e.target.value;
                                          handleUpdateField(index, value);
                                        }}
                                      />

                                      {field.Private && (
                                        <button
                                          type="button"
                                          className="absolute right-2 top-1/2 -translate-y-1/2 text-gray-500"
                                          onClick={() =>
                                            setShowPassword((prev) => !prev)
                                          }
                                        >
                                          {showPassword ? (
                                            <EyeOff size={18} />
                                          ) : (
                                            <Eye size={18} />
                                          )}
                                        </button>
                                      )}
                                    </div>
                                  )}
                                </TableCell>
                                <TableCell>
                                  <Badge variant="outline">{field.Type}</Badge>
                                </TableCell>
                              </TableRow>
                            )
                          )}
                        </TableBody>
                      </Table>
                    }

                    {error && (
                      <div className="mt-4 rounded-md bg-destructive/15 p-3 text-sm text-destructive">
                        <div className="flex items-center gap-2">
                          <AlertCircle className="h-4 w-4" />
                          <p>{error}</p>
                        </div>
                      </div>
                    )}
                  </CardContent>
                </Card>
              </>
            )}
          </TabsContent>
        </Tabs>
      </main>

      <Dialog open={showEditRoleDialog} onOpenChange={setShowEditRoleDialog}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Edit Admin Role</DialogTitle>
            <DialogDescription>
              Change the role for {currentAdmin?.Username}
            </DialogDescription>
          </DialogHeader>
          <div className="grid gap-4 py-4">
            <div className="space-y-2">
              <Label htmlFor="role">Role</Label>
              <Select value={selectedRole} onValueChange={setSelectedRole}>
                <SelectTrigger id="role">
                  <SelectValue placeholder="Select a role" />
                </SelectTrigger>
                <SelectContent>
                  {roles.map((role) => (
                    <SelectItem
                      key={role.value}
                      value={role.value}
                      disabled={role.value === "Owner"}
                    >
                      {role.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <p className="text-xs text-muted-foreground">
                Changing the role will also update the permissions for this
                admin.
              </p>
            </div>
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setShowEditRoleDialog(false)}
            >
              Cancel
            </Button>
            <Button onClick={handleSaveRole} disabled={isLoading}>
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
                "Save Role"
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={showAddDialog} onOpenChange={setShowAddDialog}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Add New Admin</DialogTitle>
            <DialogDescription>
              Enter the Discord ID of the user you want to add as an admin.
            </DialogDescription>
          </DialogHeader>
          <form onSubmit={handleAddAdmin}>
            <div className="grid gap-4 py-4">
              <div className="space-y-2">
                <Label htmlFor="discord-id">Discord ID</Label>
                <Input
                  id="discord-id"
                  placeholder="Enter Discord ID"
                  value={discordId}
                  onChange={(e) => setDiscordId(e.target.value)}
                  required
                />
                <p className="text-xs text-muted-foreground">
                  You can find a Discord ID by enabling Developer Mode in
                  Discord and right-clicking on a user.
                </p>
              </div>

              {error && (
                <div className="rounded-md bg-destructive/15 p-3 text-sm text-destructive">
                  <div className="flex items-center gap-2">
                    <AlertCircle className="h-4 w-4" />
                    <p>{error}</p>
                  </div>
                </div>
              )}
            </div>
            <DialogFooter>
              <Button
                type="button"
                variant="outline"
                onClick={() => setShowAddDialog(false)}
              >
                Cancel
              </Button>
              <Button type="submit" disabled={!discordId || isLoading}>
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
                    Adding...
                  </>
                ) : (
                  "Add Admin"
                )}
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      <Dialog open={showRawJsonDialog} onOpenChange={setShowRawJsonDialog}>
        <DialogContent className="sm:max-w-[800px]">
          <DialogHeader>
            <DialogTitle>Edit Raw Configuration</DialogTitle>
            <DialogDescription>
              Edit the raw JSON configuration for the application
            </DialogDescription>
          </DialogHeader>
          <div className="grid gap-4 py-4">
            <div className="relative">
              <Button
                variant="outline"
                size="sm"
                className="absolute right-2 top-2 z-10"
                onClick={handleCopyJson}
              >
                {copied ? (
                  <Check className="mr-2 h-4 w-4" />
                ) : (
                  <Copy className="mr-2 h-4 w-4" />
                )}
                {copied ? "Copied" : "Copy"}
              </Button>
              <ScrollArea className="h-[500px] w-full rounded-md border">
                <Textarea
                  value={rawJsonString}
                  onChange={(e) => setRawJsonString(e.target.value)}
                  className="min-h-[500px] font-mono text-sm resize-none border-0 focus-visible:ring-0 focus-visible:ring-offset-0"
                />
              </ScrollArea>

              {error && (
                <div className="mt-4 rounded-md bg-destructive/15 p-3 text-sm text-destructive">
                  <div className="flex items-center gap-2">
                    <AlertCircle className="h-4 w-4" />
                    <p>{error}</p>
                  </div>
                </div>
              )}
            </div>
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setShowRawJsonDialog(false)}
            >
              Cancel
            </Button>
            <Button onClick={handleSaveRawJson} disabled={isLoading}>
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
                "Save Changes2"
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
