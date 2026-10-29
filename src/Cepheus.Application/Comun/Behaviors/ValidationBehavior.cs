using FluentValidation;
using MediatR;

namespace Cepheus.Application.Comun.Behaviors
{
    // <summary>
    /// Pipeline behavior de MediatR: antes de que cualquier Command/Query llegue
    /// a su Handler, corre todos los IValidator&lt;TRequest&gt; registrados para ese
    /// tipo (ej. CreateUserCommandValidator para CreateUserCommand). Si hay errores,
    /// lanza ValidationException y el handler nunca se ejecuta.
    /// Se registra una sola vez en el Host (Program.cs) vía
    /// services.AddTransient(typeof(IPipelineBehavior&lt;,&gt;), typeof(ValidationBehavior&lt;,&gt;)).
    /// </summary>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var failures = (await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken))))
                .SelectMany(result => result.Errors)
                .Where(failure => failure != null)
                .ToList();

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            return await next();
        }
    }

}
