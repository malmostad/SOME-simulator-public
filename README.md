## About The Project
A social media simulator which can be used to practice situations that may arise in social media, for the public sector.

### Built With
Built in Visual Studio. Server is written in C#, client in TypeScript.

* [Vue.js](https://vuejs.org/)
* [ASP.NET Core](https://github.com/aspnet/Home)
* [MySQL](https://www.mysql.com/)

## Getting Started
### Server
Make sure you have Visual Studio/JetBrains Rider/Visual Studio Code, MySQL Server and Workbench installed.
If no, download and install. 

Open Workbench.
* Create a new Scheme, name it for example “some”
* Create a new User, name it for example “some”, with password set as “some”
* Give your User rights to your “some” scheme

Your connection string will look like this:
  ```sh
  "Server=localhost;Database=some;User=some;Password=some;"
  ```
Locate and open the ***.sln** file in your project folder. 

Locate appsettings.json. Expand it and open **appsettings.Development.json**.
Replace the placeholdertext “CONNECTIONSTRING” with your new connection string

### Client
The client folder is attached to the server project, but you can open the folder in any other preferred code editor.

Make sure you have NodeJs installed.
* Open your command prompt, go to the client folder
* Run to install packages:
  ```sh
  npm install
  ```
* Run to start project: 
  ```sh
  npm run serve
  ```

### Prerequisites
* Visual Studio/JetBrains Rider/Visual Studio Code
* .NET Core 2.2 SDK
* MySQL Server
* Workbench
* NodeJs (v12)

## Getting started
The solution comes prepared with three users and one mock scenario.

Run endpoint to create users:
 ```sh
  {host}/api/data/createusers
  ```

Run endpoint to create scenario:
 ```sh
  {host}/api/data/create
  ```

Log in as admin in the client interface to manage your scenario, and add/edit/remove scenario events, posts and comments.
Scenarios and Phases needs to be added in the controller. Locate the folder Controllers and open **DataController** to edit/add/remove them.

To truncate:
  ```sh
  {host}/api/data/truncate
  ```
**!! This endpoint clears all data from your database !!**

Now you’re all set to go. Happy coding!

## Contributing

Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. 

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/Feature`)
3. Commit your Changes (`git commit -m 'Add some Feature'`)
4. Push to the Branch (`git push origin feature/Feature`)
5. Open a Pull Request


