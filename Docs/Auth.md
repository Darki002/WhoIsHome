# Authentication & Authorized

The API uses two Middlewares to prevent others from access it without permission. In use is an API Key and JWT, read more about it in the sections.

## API Key

Used by: all Endpoints
Header Key: `X-API-KEY`

Must provide the API Key in the Header with the key `X-API-KEY` that was defined in the `API_KEY` environment variables. 

## JWT

Used by: all except the `AuthController`

Must first login via the Login Endpoint and receive a token that is valid for 60min. With the token you can then access all resources as the logged in User.
