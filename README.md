# asw-portal-rest-service

#**Running ASW Rest Portal API**
###**Prerequisites**
Before you begin, ensure you have met the following requirements:
○	You have a basic understanding of dotnet  and rest api apps.
○	You have 6.0 runtime, hosting bundle and installed.
○	You have MSSQL server 2019 installed on a server.
○	Docker latest version

   
###**Configuring and running the application  ***
   Please execute the following instructions from the git bash command line
•	Git clone from the repository.
•	cd <base_code_folder>.
•	Configuration
  "ApplicationUrl": {
    "OCPPApiBaseUrl": "<OCPP base url>,
    "AssetBaseUrl": "<asset base url>,
  }  
 
•	Update the docker.yml file as per the env needed for secret manager.
•	cd <base_code_folder>.
•	dotnet  restore
•	dotnet  build
•	dotnet test
•	dotnet  publish  --output <path/to/directory> <path/to/project_file>
•	run dll 


###**● Running the application using docker:***
    Please execute the following instructions from the command line
Please execute the following instructions from the unix command line
•	Git clone from the repository.
•	cd <base_code_folder>.
•	Configuration
 "ApplicationUrl": {
    "OCPPApiBaseUrl": "<OCPP base url>,
    "AssetBaseUrl": "<asset base url>,
  }  
 
●	cd <base_code_folder>.
●	dotnet  restore
●	dotnet  build
●	dotnet test
●	docker login <docker_container _registory>
●	username : <username>     Password :<password>
●	docker image build -t <docker_container _registory>/asset_api:M3-release .
●	docker push <docker_container _registory>/rest_api:M3release
●	run this command on server cmd - "docker run -d --name restapi -p 5003:80 <docker_container _registory>/rest_api:M3-release"


