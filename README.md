# Prerequisites

- .NET Core 3.1.4. Download from [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- dotnet ef tools. Install globally by running `dotnet tool install --global dotnet-ef`
- SQLite 3 client (e.g. SQLite3 CLI). Download from [SQLite 3 official site](https://www.sqlite.org/download.html)

# Steps to run the demo application

1. Setup database

`cd src/LoopbackAuthenticationIntegrationPoC`
`dotnet ef database update`

Entity Framework Core will complain about a missing value comparer. Ignore it.

2. Run .NET Core backend

`dotnet run`

3. Run Loopback 3 backend (in another terminal)

`cd src/loopback3app`
`node .`

4. Create an account

`curl --header "Content-Type: application/json" --request POST --data '{"username":"admin", "password": "admin", "email": "admin@example.com"}' http://localhost:3000/api/users`

5. Login to create an ID token

`curl --header "Content-Type: application/json" --request POST --data '{"username":"admin", "password": "admin"}' http://localhost:3000/api/users/login`

Note: the return value is currently broken for some reason. Could be a bug in the demo or in LB3 (very unlikely to be a bug in LB3). To retrieve the ID token, we must use SQLite client:

`sqlite3 src/loopback3app/loopback.db`
`select id from accesstoken;`

```
SQLite version 3.14.1 2016-08-11 18:53:32
Enter ".help" for usage hints.
sqlite> select id from accesstoken;
eRbTxxCqLNbYwl9lqgHF1qkLjLHCqrywiJbZVBo1zkcDkHMLDZfgWi7HmcnLyuIL
```

Copy access token from terminal.

6. Try to use the access token to authorize a password change on .NET Core API.

Input the access token as a bearer token header:

`curl --header "Content-Type: application/json" --header "Authorization: Bearer eRbTxxCqLNbYwl9lqgHF1qkLjLHCqrywiJbZVBo1zkcDkHMLDZfgWi7HmcnLyuIL" --request PUT --data '{"password": "admin2"}' http://localhost:5000/api/account -v`

7. Test that the new password works:

`curl --header "Content-Type: application/json" --request POST --data '{"username":"admin", "password": "admin2"}' http://localhost:3000/api/users/login`
