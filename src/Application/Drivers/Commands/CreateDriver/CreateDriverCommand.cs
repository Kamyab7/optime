using Application.Common.Interfaces;
using Domain;
using FluentValidation;
using MediatR;

namespace Application.Drivers.Commands.CreateDriver;

public record CreateDriverCommand : IRequest<string>
{
    public string Name { get; init; } = null!;

    public string LastName { get; init; } = null!;
}

public class CreateDriverCommandValidator : AbstractValidator<CreateDriverCommand>
{
    public CreateDriverCommandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.LastName)
            .MaximumLength(200)
            .NotNull()
            .NotEmpty();
    }
}

public class CreateDriverCommandHandler : IRequestHandler<CreateDriverCommand, string>
{
    private readonly IApplicationDbContext _context;

    public CreateDriverCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreateDriverCommand request, CancellationToken cancellationToken)
    {
        var driver = new Driver(request.Name, request.LastName);

        _context.Drivers.Add(driver);

        await _context.SaveChangesAsync(cancellationToken);

        return driver.Id;
    }
}
