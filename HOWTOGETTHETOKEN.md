### Get a token (development)

Use a **dev** Firebase project and a dedicated test account.

```powershell
$r = Invoke-RestMethod -Method Post `
  -Uri "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=<FIREBASE_WEB_API_KEY>" `
  -ContentType "application/json" `
  -Body (@{ email = "<email>"; password = "<password>"; returnSecureToken = $true } | ConvertTo-Json)

$r.idToken
```

Use the returned `idToken` as `Authorization: Bearer <idToken>`. Tokens expire after one hour.