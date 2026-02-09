# EnterpriseHub API Documentation

Base URL (dev): `http://localhost:5077`

Authentication: **JWT Bearer**

---

## 🔐 Authentication

### POST /auth/register
Create a new user account.

**Request**
```json
{
  "email": "user@mail.com",
  "password": "Pass33220",
  "firstName": "John",
  "lastName": "Doe"
}
Response 200

{
  "accessToken": "jwt-token",
  "user": {
    "id": "uuid",
    "email": "user@mail.com",
    "firstName": "John",
    "lastName": "Doe",
    "role": "Dev"
  }
}
POST /auth/login
Authenticate a user.

Request

{
  "email": "user@mail.com",
  "password": "Pass33220"
}
Response 200

{
  "accessToken": "jwt-token",
  "user": {
    "id": "uuid",
    "email": "user@mail.com",
    "firstName": "John",
    "lastName": "Doe",
    "role": "Dev"
  }
}
🧑 Clients
GET /clients
Returns all active clients.

Response 200

[
  {
    "id": "uuid",
    "name": "ACME Garage",
    "email": "contact@acme.fr",
    "phone": "0611223344"
  }
]
POST /clients
Create a client.

Request

{
  "name": "ACME Garage",
  "email": "contact@acme.fr",
  "phone": "0611223344"
}
Errors

409 Conflict → email already exists

DELETE /clients/{id}
Soft delete a client.

Response

204 No Content

📁 Projects
POST /projects
Create a project for a client.

Request

{
  "name": "ERP v1",
  "clientId": "uuid",
  "description": "First iteration"
}
Errors

404 Not Found → client not found

🎫 Tickets
POST /tickets
Create a ticket.

Request

{
  "title": "Bug login",
  "projectId": "uuid",
  "description": "Cannot login",
  "priority": "High"
}
Response 201

{
  "id": "uuid",
  "title": "Bug login",
  "description": "Cannot login",
  "priority": "High",
  "status": "Open",
  "projectId": "uuid"
}
Errors

404 Not Found → project not found

❌ Error format
All errors follow application/problem+json.

{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Validation error",
  "status": 400,
  "detail": "Email already exists"
}

///////////////////////////////////////////////////////////////////////////////////////
/////////////////////////TESTS RESULTS/////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////
 # dotnet test tests/EnterpriseHub.Application.Tests/EnterpriseHub.Application.Tests.csproj -v normal #

 Série de tests pour C:\documentdevpro\projetPro\C#.NET\enterprisehub\tests\EnterpriseHub.Application.Tests\bin\Debug\net8.0\EnterpriseHub.Application.Tests.dll (.NETCoreApp,Version=v8.0)
Outil en ligne de commande d'exécution de tests Microsoft (R), version 17.8.0 (x64)
Copyright (c) Microsoft Corporation. Tous droits réservés.

Démarrage de l'exécution de tests, patientez...
Au total, 1 fichiers de test ont correspondu au modèle spécifié.
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.4.5+1caef2f33e (64-bit .NET 8.0.23)
[xUnit.net 00:00:00.50]   Discovering: EnterpriseHub.Application.Tests
[xUnit.net 00:00:00.53]   Discovered:  EnterpriseHub.Application.Tests
[xUnit.net 00:00:00.53]   Starting:    EnterpriseHub.Application.Tests
[xUnit.net 00:00:00.60]   Finished:    EnterpriseHub.Application.Tests
  Réussi EnterpriseHub.Application.Tests.UnitTest1.Test1 [1 ms]
  Réussi ProjectServiceTests.UpdateAsync_Should_Return_Null_When_Project_Not_Found [8 ms]
  Réussi TicketServiceTests.CreateAsync_Should_Create_Ticket_When_Project_Exists [9 ms]
  Réussi ProjectServiceTests.CreateAsync_Should_Create_Project_When_Client_Exists [3 ms]
  Réussi ClientServiceTests.CreateAsync_Should_Throw_When_Email_Already_Exists [20 ms]
  Réussi TicketServiceTests.CreateAsync_Should_Throw_When_Project_Not_Found [11 ms]
  Réussi ProjectServiceTests.CreateAsync_Should_Throw_When_Client_Not_Found [8 ms]
  Réussi ClientServiceTests.DeleteAsync_Should_SoftDelete_Client_And_Not_Return_In_GetAll [4 ms]
  Réussi ClientServiceTests.CreateAsync_Should_Create_Client [< 1 ms]

Série de tests réussie.
Nombre total de tests : 9
     Réussi(s) : 9
 Durée totale : 1.2213 Secondes
     1>Génération du projet "C:\documentdevpro\projetPro\C#.NET\enterprisehub\tests\EnterpriseHub.Application.Tests\EnterpriseHub.Application.Tests.csproj" terminé 
       e (VSTest cible(s)).

La génération a réussi.
    0 Avertissement(s)
    0 Erreur(s)

Temps écoulé 00:00:04.23