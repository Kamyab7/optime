# Optime

Optime is a repository created for a technical interview assignment. It implements the Clean Architecture, Command Query Responsibility Segregation (CQRS), Domain-Driven Design (DDD), and Vertical Slice patterns.

For more details about Clean Architecture, CQRS, DDD, and Vertical Slice.

- [Clean Architecture](insert_link_to_clean_architecture_here)
- [CQRS](insert_link_to_cqrs_here)
- [DDD](insert_link_to_ddd_here)
- [Vertical Slice](insert_link_to_vertical_slice_here)

:warning: **Note:** Since this project is an interview assignment, I've intentionally avoided some complexities. For example, authentication is implemented using API keys, and there is no hashing mechanism. The admin API key is `adminkey`.

:warning: **Note:** While I intended to write tests for this project, due to time constraints, testing has not been implemented yet.

## Project Initialization

Optime project was initiated on [insert_date_here]. When the project started, the `ApplicationDbContextInitializer` class was created to be responsible for creating the database and seeding some mock driver data. Additionally, it registers some cronjobs for mocking the mechanisms of the whole system. These jobs can be seen and managed in the Hangfire URL: `/hangfire`.

Twenty missions will be seeded every twenty seconds.

The `AutoAssignerService` is responsible for automatically assigning missions to drivers every 5 seconds.

The `AddMockDriverArrivedCronJob` is responsible for mocking driver arrivals.

## Libraries Used

- [MediatR](https://github.com/jbogard/MediatR): A simple mediator implementation in .NET
- [Bogus](https://github.com/bchavez/Bogus): A simple and sane fake data generator for .NET
- [FluentValidation](https://fluentvalidation.net/): A popular .NET library for building strongly-typed validation rules
- [Hangfire](https://www.hangfire.io/): An easy way to perform background processing in .NET and .NET Core applications
- [AutoMapper](https://automapper.org/): A convention-based object-to-object mapper

