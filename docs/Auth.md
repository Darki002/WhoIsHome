# Authentication & Authorized

The API uses two Middlewares to prevent others from access it without permission. In use is an API Key and JWT, read more about it in the sections.

## API Key

- Used by: all Endpoints
- Header Key: `X-API-KEY`

Must provide the API Key in the Header with the key `X-API-KEY` that was defined in the `API_KEY` environment variables. 

## JWT

- Used by: all except the `AuthController`
- Valid Duration: 1 houre

Must first login via the Login Endpoint and receive a token that is valid for 60min. With the token you can then access all resources as the logged in User. This will also make sure that the user can only perform actions that he is allowed to.

## Refresh

- Used by: `AuthController`
- Valid Duration: 14 Days

The *RefreshToken* is long valid and can be used to get a new JWT Token. This allows the Mobile App to stay logged in for as long as 14 days without storing any user data on the phone.

### Get a Token

Make a request to `/Auth/Login`, will also Invalidate any other *RefreshToken* from this user, so only the newly generated *RefreshToken* is valid.

### How to use

Make a reques to `/Auth/Refresh` and you will get a new *JWT Token* and also a new *RefreshToken* that is valid for 14 days again. The old RefreshToken will be invalid after the request is done.
