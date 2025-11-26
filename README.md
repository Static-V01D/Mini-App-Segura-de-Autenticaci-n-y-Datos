# Mini-App-Segura-de-Autenticacion-y-Datos (Library App)

## About Us / Autores

* **Keven Y. Paulino Ferrer** — [KPAULINO4952@interbayamon.edu](mailto:KPAULINO4952@interbayamon.edu)
* **Luis A. Catala Garcia** — [LCATALA0861@interbayamon.edu](mailto:LCATALA0861@interbayamon.edu)

---

## Descripción

Mini-App para gestionar una biblioteca con autenticación básica, roles y operaciones CRUD sobre libros. El objetivo es demostrar buenas prácticas de seguridad para una aplicación pequeña: manejo de credenciales, archivos de datos, logs y modelos de amenazas.

---

## 📦 Contenido del repositorio

* `src/` — código fuente de la aplicación
* `data/` — archivos JSON de datos (ej.: `users.json`, `books.json`)
* `logs/` — archivos de registro (`log.txt` o `logs.json`)
* `docs/` — documentación adicional (`.env.example`, `ThreatModels.pdf`, etc.)
* `README.md` — este archivo

---

## How to use / Cómo usar

1. **Registrar** y **Login**: debes registrarte y luego iniciar sesión para poder acceder a las operaciones CRUD según el rol del usuario. También puedes usar un usuario ya registrado copiando nombre y contraseña desde la Carpeta de datos.

2. Al iniciar el programa verás un menú para navegar por los servicios (registro, inicio de sesión, listar recursos, crear, actualizar, eliminar, etc.).

3. Sigue las opciones del menú para realizar las operaciones. El menú y las funciones disponibles dependerán del `role` del usuario (por ejemplo: `admin`, `user`).

4. Ejemplo de flujo (CLI):

```
> 2 (Login)
Username: user1
Password: ****
Login successful.

1) List books
2) Add book
3) Update book
4) Delete book
5) Logout
> 1
ID: 1 | Title: The Hobbit | Author: J.R.R. Tolkien | Available: true
> 2 (Add book)
Title: Foundation
Author: Isaac Asimov
Available (y/n): y
Book added with ID 3.
```

---

## Setup / Preparación

1. Clona el repositorio:

```bash
git clone https://github.com/USERNAME/REPO.git
cd REPO
```

2. Compila y ejecuta (ejemplo para .NET):

```bash
dotnet restore
dotnet build
dotnet run --project src/
```

Ajusta los comandos según tu stack (Node, Python, Java, etc.).

3. Asegúrate de que existan las carpetas `data/` y `logs/`. Coloca `users.json`, `books.json` y otros archivos necesarios dentro de `data/`.

---

## Carpeta de datos (data/) — Ejemplos

`data/users.json` (ejemplo):

```json
[
  {
    "id": 1,
    "username": "admin",
    "passwordHash": "hashed_password_here",
    "role": "admin",
    "createdAt": "2025-11-25T18:00:00Z"
  },
  {
    "id": 2,
    "username": "user1",
    "passwordHash": "hashed_password_here",
    "role": "user",
    "createdAt": "2025-11-25T18:05:00Z"
  }
]
```

> ⚠️ **Nunca** almacenar contraseñas en texto plano. Usar hashing (PBKDF2, bcrypt, Argon2).

`data/books.json` (ejemplo):

```json
[
  { "id": 1, "title": "The Hobbit", "author": "J.R.R. Tolkien", "available": true },
  { "id": 2, "title": "Dune", "author": "Frank Herbert", "available": false }
]
```

---

## Logs / Carpeta de logs

Guarda un archivo de logs en `logs/log.txt` o `logs/logs.json`.

Ejemplo `logs/log.txt`:

```
2025-11-25T18:10:00Z | INFO  | user1 logged in
2025-11-25T18:11:32Z | INFO  | user1 created book id=3 title=Foundation
```

Ejemplo `logs/logs.json`:

```json
[
  { "timestamp": "2025-11-25T18:10:00Z", "level": "INFO", "message": "user1 logged in" }
]
```

---

## Archivo `.env.example`

Crea en `src/` (o en la raíz si lo prefieres) un archivo `docs/.env.example` con las variables requeridas por la aplicación. Ejemplo mínimo:

```env
# Database
DB_CONNECTION_STRING=

# JWT / Auth
JWT_SECRET=
JWT_EXPIRATION_MINUTES=60

# App settings
APP_ENV=development
LOG_LEVEL=info
```

En el README principal enlazamos al ejemplo: 📄 [Example of a .env file](docs/.env.example)

---

## Threat Model

Incluimos un modelo de amenazas en `docs/ThreatModels.pdf`.

📄 [Project ThreatModels (PDF)](docs/ThreatModels.pdf)

---

## Auditoría / Resultados de seguridad

Incluye en `docs/` una captura de pantalla con el resultado del comando de auditoría (por ejemplo `pip audit`, `dotnet list package --vulnerable`, `npm audit`) como `docs/audit-result.png`.

---

## Seguridad y buenas prácticas

* Mantener `.env` fuera del control de versiones (`.gitignore`).
* Guardar solo hashes de contraseñas.
* Validar entradas para evitar inyección y manejo incorrecto de datos.
* Usar HTTPS en producción.
* Usar una base de datos en producción en lugar de JSON plano.

---

## Contacto

* Keven Y. Paulino Ferrer — [KPAULINO4952@interbayamon.edu](mailto:KPAULINO4952@interbayamon.edu)
* Luis A. Catala Garcia — [LCATALA0861@interbayamon.edu](mailto:LCATALA0861@interbayamon.edu)

