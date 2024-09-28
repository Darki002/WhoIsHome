# WhoIsHome
Backend for [Android App](https://github.com/Darki002/WhoIsHome.Android)

# API Endpoints

This Project uses Swagger for the API Documentation. You can start up the project locally 
and the swagger you should open automatically or use "localhost:7165/swagger/index.html"

For informations about Authentication & Authorized read the following Documentation: [AuthDocs](./Docs/Auth.md)

# Environment Variables

- `JWT_SECRET_KEY` : Key used for the JWT Authentication.
- `API_KEY` : API Key used by the middleware in every request to Authorized.
- `MYSQL_SERVER`: The SQL Server.
- `MYSQL_PORT`: Port for the Database.
- `MYSQL_DATABASE`: Database that will be used by the Application.
- `MYSQL_USER`: The User that is being used by the app to connect to the db.
- `MYSQL_PASSWORD`: The Password for the user that is being used by the app to connect to the db.
- `PROJECT_ID` : the Firebase projectId of your Firebase project. (Not in used yet)
- `GOOGLE_APPLICATION_CREDENTIALS` : Google Authentication for Firebase. Read more [here](https://cloud.google.com/docs/authentication/provide-credentials-adc#wlif-key) (Not in used yet)

## Local Development

For running the project locally the project uses "DotNetEnv".

1. Create `.env` file in the `WhoIsHome.Host` project
2. Add the Environment Variables in the `.env` file. For example `API_KEY=dev1234`
