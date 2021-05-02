using FluentValidation;

namespace Application.Satellites
{
    public class SatelliteValidator : AbstractValidator<SatelliteDto>
    {
        public SatelliteValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Distance).NotNull();
        }
    }
}