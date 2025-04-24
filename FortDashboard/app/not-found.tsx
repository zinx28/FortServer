import Link from "next/link";



export default function Custom404() {
    return (
      <div className="flex min-h-screen flex-col items-center justify-center p-4 text-center">
        <h1 className="text-6xl font-bold mb-2">404</h1>
        <p className="mb-4">Page not found</p>
        <Link href="/login" className="text-blue-600 hover:underline">
          Return to home
        </Link>
      </div>
    )
  }
  