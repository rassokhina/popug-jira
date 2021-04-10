# Popug Jira - .NET Microservices Application

Training .NET app, based on microservices architecture. The application includes a task tracker service, an analytics service and a billing service. Each of these services uses common auth service for authentication and authorization.

## Technologies

* .NET 5
* Idendity Server 4
* EF Core
* RabbitMQ
* MassTransit
* Quartz.NET

## Overview

This training project contains microservices (each one owning its own SQL database), impelements CQRS pattern, supports asynchronous communication for CUD and Business events propagation across multiple services based on RabbitMQ Event Bus. Identity Server 4 implements OpenID Connect and OAuth 2.0 protocols and is used for authentication and authorization into each service. 

CQRS is used to divide the application into Read and Write flows. Is helps to easily take advantage of Event Sourcing.

Each microservice has its own database since they must be loosely coupled so that they can be developed, deployed and scaled independently (SQL provider for EF Core is used and can be easily changed to another db provider). 

Microservices communicate with each other by events. Event communication is handled with RabbitMQ to convey integration events. EventStrorming approach was implemented to design maintainable event-driven architecture and distinguish main business events. 

**Microservces communication schema**
![image](https://user-images.githubusercontent.com/11731408/114268608-1d8dc180-9a2c-11eb-80b1-1c8c7bc83a18.png)


The Schema Registry for events is based on MassTransit, a library used as a wrapper around message brokers.
All events are taken out in the Shared project, it is connected for each service that consumes and produces events. Thus, this is the only "place of truth" that can be relied on to verify the events and their versions.

## Projects

* **IdentityServer** - service for authentication and authorization, IS4 based

![image](https://user-images.githubusercontent.com/11731408/113515497-10418480-959f-11eb-851c-ddf24f03d498.png)

* **TaskTracker.Core** - library containing business logic for task tracker service (create and assign tasks for users, view user task list)

* **TaskTracker.Client** - MVC Core client for task tracker service

![image](https://user-images.githubusercontent.com/11731408/113515606-b097a900-959f-11eb-9ffe-e6245100ce93.png)

* **Accounting.Core** - library containing business logic for accounting service (view user balance audit, ensures the accrual of money for completed tasks and payment of the balance to users every day)

* **Accounting.Client** - MVC Core client for acconting service

![image](https://user-images.githubusercontent.com/11731408/113515749-9611ff80-95a0-11eb-9982-ec494c5a88cb.png)

* **Analytics.Core** - library containing business logic for anlytics service (view statistics for the most expensive tasks)

* **Analytics.Client** - MVC Core client for anlytics service

![image](https://user-images.githubusercontent.com/11731408/113515837-0c166680-95a1-11eb-8882-117b9599a85f.png)

* **Shared** - shared library containing events and queue settings for event bus

## Installation

1. Install SQL Server 
2. Install Erlang and RabbitMQ <https://www.rabbitmq.com/#getstarted>
3. Open .sln file in Visual Studio 2019 version supported .NET 5
4. Configure SQL connection string in appsettings.json files for each .Client project 
5. Run the following projects in the solution:
    - IdentityServer <https://localhost:5001/>
    - TaskTracker.Client <https://localhost:5002/>
    - Accounting.Client <https://localhost:5003/>
    - Analytics.Client <https://localhost:5004/>

Users to test

| Username | Password |
| -------- | -------- |
| admin   | Password123!     |
| employee    | Password123!    |
| manager    | Password123!    |
| accountant    | Password123!    |
