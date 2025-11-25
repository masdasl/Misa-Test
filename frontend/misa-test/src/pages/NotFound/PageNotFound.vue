<template>
  <div class="flex items-center justify-center min-h-screen bg-gray-100">
    <div class="text-center px-6 py-12 bg-white shadow-lg rounded-lg">
      <h1 class="text-9xl font-extrabold text-red-500 mb-4">404</h1>
      <h2 class="text-3xl font-semibold mb-4">Oops! Page not found</h2>
      <p class="text-gray-600 mb-8">
        The page you are looking for does not exist or has been moved.
      </p>
      <router-link
        to="/customer"
        class="inline-block px-6 py-3 bg-blue-500 text-white font-semibold rounded hover:bg-blue-600 transition"
      >
        Go Back Home
      </router-link>
    </div>
  </div>
</template>
