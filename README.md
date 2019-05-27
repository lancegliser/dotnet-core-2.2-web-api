# Service example

This site serves as a service layer for Service, the front end React collection of applications supporting the business.

## Organization

## Service

This is actual service layer itself. Models and controllers, the swagger UI, etc are all here.

### Service.Test

This is the tests for the service layer.

## Commands

To run tests use this command from the root:

    dotnet test

To limit the tests run, you may specificy categories of tests in any way you like. You may want to review [the selective unit tests documentation](https://docs.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests).

    dotnet test --filter {Attribute}={Value} -- example: Category=Utilities
    dotnet test --filter {String} 

To run the project, using an express web server that comes with the project use:

    # Set the environment using powershell
    $Env:ASPNETCORE_ENVIRONMENT = "Development"
    # For single run
    dotnet run --project .\Service\
    # To have your changes automatically reloaded
    dotnet watch --project .\Service\ run

This will give you a local copy of the site, likely at: https://localhost:5001/

To build the project, a step you should _not_ have to do, use:

    dotnet build
