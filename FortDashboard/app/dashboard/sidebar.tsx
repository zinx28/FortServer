import { Activity, Settings, Users } from "lucide-react";
import React from "react";
import { Button } from "@/components/ui/button";
import { useUserStore } from "@/hooks/useUserStore";
import { usePathname } from "next/navigation";
import Link from "next/link";

const Sidebar = () => {
  const { user } = useUserStore();
  const pathname = usePathname();

  console.log(user?.displayName);

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
            <Button variant="ghost" size="icon" className="h-8 w-8">
              <Settings className="h-4 w-4" />
              <span className="sr-only">Settings</span>
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Sidebar;
