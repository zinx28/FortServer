import { Activity, LogOut, Settings, Users } from "lucide-react";
import React from "react";
import { Button } from "@/components/ui/button";
import { useUserStore } from "@/hooks/useUserStore";
import { usePathname } from "next/navigation";
import Link from "next/link";
import { DropdownMenu, DropdownMenuTrigger, DropdownMenuContent, DropdownMenuItem, DropdownMenuSeparator } from "@/components/ui/dropdown-menu";

const Sidebar = () => {
  const { user } = useUserStore();
  const pathname = usePathname();

  const LogoutDashboard = async () => {
    var apiUrl = process.env.NEXT_PUBLIC_API_URL;
    const response = await fetch(`${apiUrl}/admin/new/logout`, {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      credentials: "include",
    });

    const JsonParsed = await response.json();

    if (JsonParsed) {
      window.location.reload();
    }
  }

  return (
    <div className="fixed left-0 top-0 h-full w-64 border-r bg-muted/40 z-50">
      <div className="flex h-full flex-col">
        <div className="flex h-14 items-center borderz-b px-6">
          <h2 className="text-lg font-semibold tracking-tight">FortBackend</h2>
        </div>
        <div className="flex-1 overflow-auto py-2">
          <nav className="grid items-start px-2 text-sm font-medium">
            <Button
              variant={pathname === "/dashboard" ? "secondary" : "ghost"}
              className="flex justify-start gap-2 px-4 py-2 text-primary"
              asChild
            >
              <Link href="/dashboard">
                <Activity className="h-4 w-4" />
                Dashboard
              </Link>
            </Button>
            <Button
              variant={
                pathname === "/dashboard/content" ? "secondary" : "ghost"
              }
              className="flex justify-start gap-2 px-4 py-2"
              asChild
            >
              <Link href="/dashboard/content">
                <Users className="h-4 w-4" />
                Content Management
              </Link>
            </Button>
            <Button
              variant={
                pathname === "/dashboard/admin-panel" ? "secondary" : "ghost"
              }
              className="flex justify-start gap-2 px-4 py-2"
              asChild
            >
              <Link href="/dashboard/admin-panel">
                <Settings className="h-4 w-4" />
                Admin Panel
              </Link>
            </Button>
          </nav>
        </div>
        <div className="mt-auto border-t p-4">
          <div className="flex items-center gap-2 rounded-lg bg-primary/10 p-2">
            <div className="flex h-8 w-8 items-center justify-center rounded-full bg-primary text-primary-foreground">
              A
            </div>
            <div className="flex-1">
              <p className="text-sm font-medium">{user?.displayName}</p>
            </div>
            <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button variant="ghost" size="icon" className="h-8 w-8">
                    <Settings className="h-4 w-4" />
                    <span className="sr-only">Settings</span>
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end">
                  <DropdownMenuItem>
                    <Settings className="mr-2 h-4 w-4" />
                    <span>Settings</span>
                  </DropdownMenuItem>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem className="text-red-500" onClick={() => LogoutDashboard()}>
                    <LogOut className="mr-2 h-4 w-4" />
                    <span>Logout</span>
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Sidebar;
