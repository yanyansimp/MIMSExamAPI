# **ProductPackagingAPI**

## **Project Description**
ProductPackagingAPI is a .NET 8 Web API built using Clean Architecture principles. It provides product management functionalities, including authentication and authorization using JWT. The API is versioned, allowing different versions of endpoints for flexibility.


## **Project Structure (Clean Architecture)**
The project follows **Clean Architecture**, ensuring separation of concerns with four key layers:

1. **Application Layer**  
   - Contains business logic and application services (e.g., `ProductService.cs`, `UserService.cs`).

2. **Domain Layer**  
   - Contains core domain entities and interfaces (e.g., `Product.cs`, `User.cs`).
   - Defines repository interfaces (`IProductRepository.cs`, `IUserRepository.cs`).

3. **Infrastructure Layer**  
   - Handles external concerns like database access (`ProductRepository.cs`, `UserRepository.cs`).
   - Contains the `DbHelper.cs` for database connections.

4. **WebApi Layer**  
   - The API layer exposing controllers (`ProductsController.cs`).
   - Implements API versioning (`v1`, `v2` folders for different versions of endpoints).

---

## **Setup Instructions**

### **1. Setting Up the Database**
- Clone or download the **MIMSExamDB** repository:  
  [GitHub Link](https://github.com/yanyansimp/MIMSExamDB)  
- Run the provided SQL scripts in a separate database project to set up the database schema.

### **2. Clone This API Project**
```sh
git clone https://github.com/your-repo/ProductPackagingAPI.git
cd ProductPackagingAPI
```

### **3. Install .NET 8**
Ensure you have .NET 8 installed:  
```sh
dotnet --version  # Verify .NET version
```

### **4. Restore Dependencies**
Run the following command to restore NuGet packages:
```sh
dotnet restore
```

### **5. Configure Database Connection**
- Open **`appsettings.json`** in the `WebApi` project.
- Update the `ConnectionStrings` section with your database server name.

Example:
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=MIMSExamDB;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;"
}
```

### **6. Run the Application**
```sh
dotnet run --project WebApi
```
The API should now be running at:  
**https://localhost:5001** (or your configured port)

---

## **Testing the API**

### **Postman Collection**
A Postman collection is included in the project for testing. Import it into Postman.

### **Endpoints to Check Authentication**
#### Unauthorized Requests:
- **GET** `/api/v1/products/`
- **POST** `/api/v1/products/`
- **GET** `/api/v2/products/`

Expect a **401 Unauthorized** response.

#### Register and Login:
1. **Register User**
   - **POST** `/api/v1/auth/register`
   - Provide username and password in the request body.
2. **Login**
   - **POST** `/api/v1/auth/login`
   - Use the registered credentials.
   - This will return a JWT token, which will be **automatically stored** as a Bearer Token for subsequent requests.

#### Testing Authorized Requests:
After logging in, the returned token will be **automatically stored in Postman** as a Bearer Token.  
You can now retry the endpoints without manually adding the token:
  - **GET** `/api/v1/products/`
  - **POST** `/api/v1/products/`
  - **GET** `/api/v2/products/`

---