# url-shortener

Url shortener created with .NET and Vue, a wallet for profit sharing using basic authentication for user authentication

## Project requirements

- Backend (.NET Core API)
- Endpoint to create shortened URLs.
- Store URLs, clicks, and wallet balance in SQL/SQLite.
- Implement profit-sharing logic: each click earns R10, user starts with 10% share, increases by 10% every 5 clicks up to 80%.
- Frontend Vue.js
- Simple dashboard to create links, view clicks, and wallet balance.
- Display profit percentage growth.
- Database
- SQL or SQLite database to store and query the database.

## Technologies:

Basic Authentication: Secure and simple, storing Base64-encoded credentials in localStorage.
Vue CLI: Streamlines development with hot-reloading and optimized builds.
.NET Core: Open-source framework for building the backend API, handling URL shortening and user authentication. 
Dapper: Micro-ORM for efficient SQL queries on SQLite, managing Users and ShortUrls data.
BCrypt: Password-hashing library for secure storage of user passwords in the backend.
Axios: JavaScript HTTP client for frontend API calls.
Vue Router: Vue.js library for client-side routing.
Dapper/SQLite: Lightweight and efficient for database operations.
TerserPlugin/MiniCssExtractPlugin: Optimizes production builds by minifying JS and extracting CSS.

## Running and Deploying for demonstration:

Backend: Run `dotnet run` for the API from with the api/ directory.
Frontend: From within the client/ directory run `npm run serve` for development, `npm run build` and `npx serve -s dist -p 8080` for further testing in deplayment.

# Security Measures Implemented

## Basic Authentication
- The backend uses Basic Authentication, requiring a Base64-encoded `username:password` in the Authorization header for protected endpoints (e.g., `/api/urls`, `/api/wallet`).
- Credentials are verified against hashed passwords stored in the SQLite database, ensuring secure user authentication.

## Password Hashing with BCrypt
- User passwords are hashed using BCrypt (`BCrypt.Net.BCrypt.HashPassword`) before storage in the Users table, preventing plaintext exposure.
- During login, passwords are verified against stored hashes, enhancing security against database breaches.

## Content Security Policy (CSP)
A CSP is defined in `public/index.html` to restrict resource loading:

    <meta http-equiv="Content-Security-Policy" content="
      default-src 'self';
      connect-src 'self' http://localhost:8080 http://localhost:5000 http://localhost:1337;
      script-src 'self' 'unsafe-eval';
      style-src 'self' 'unsafe-inline';
      img-src 'self' data:;
      font-src 'self';
    ">

Prevents cross-site scripting (XSS) by allowing only trusted origins (self, http://localhost:1337) for scripts, styles, and API connections.

## CORS Configuration
The backend (`Program.cs`) restricts cross-origin requests to trusted frontend origins (http://localhost:8080, http://localhost:5000) using a CORS policy:

    policy.WithOrigins("http://localhost:8080", "http://localhost:5000")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();

Mitigates unauthorized API access from untrusted domains.

## Input Validation
Backend endpoints validate inputs:
- `/api/auth/register`: Checks for non-empty Username and Password, prevents duplicate usernames.
- `/api/urls`: Ensures OriginalUrl is non-empty before creating a shortened URL.
- Reduces risks of invalid or malicious data affecting the system.

## Database Security
- SQLite database (`app.db`) stores sensitive data in a serverless file, accessible only by the backend.
- Dapper uses parameterized queries to prevent SQL injection attacks.

## Frontend Security
- Authentication tokens (Base64 username:password) are stored in localStorage and cleared on logout, minimizing session hijacking risks.
- Vue Router protects routes by redirecting unauthenticated users to the log in page.
- Simulated clicks use a dedicated endpoint to avoid CORS issues with external redirects, ensuring secure click tracking.

## File Structure

.
├── .gitignore
├── api
│   ├── api.csproj
│   ├── appsettings.json
│   ├── BasicAuthenticationHandler.cs
│   ├── Dtos.cs
│   ├── Program.cs
│   └── Properties
│       └── launchSettings.json
├── client
│   ├── babel.config.js
│   ├── jsconfig.json
│   ├── package.json
│   ├── public
│   │   ├── favicon.ico
│   │   └── index.html
│   ├── README.md
│   ├── src
│   │   ├── App.vue
│   │   ├── assets
│   │   │   ├── logo.png
│   │   │   └── style.css
│   │   ├── components
│   │   │   ├── ClientPage.vue
│   │   │   ├── DashBoard.vue
│   │   │   ├── LogIn.vue
│   │   │   ├── RegisterPage.vue
│   │   │   ├── UrlForm.vue
│   │   │   ├── UrlList.vue
│   │   │   └── WalletPage.vue
│   │   ├── main.js
│   │   └── router
│   │       └── index.js
│   └── vue.config.js
└── README.md
