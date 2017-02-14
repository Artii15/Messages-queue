using ServiceStack.FluentValidation;
using ServiceStack.ServiceInterface;

namespace Server
{
	public class CreateQueueValidator : AbstractValidator<CreateQueue>
	{
		public CreateQueueValidator()
		{
			RuleSet(ApplyTo.Post, () => { RuleFor(r => r.Name).NotEmpty().WithErrorCode("ShouldNotBeEmpty"); });
			RuleSet(ApplyTo.Post, () => { RuleFor(r => r.Name).Matches("^[a-zA-Z0-9]+$").WithErrorCode("InvalidName"); });
		}
	}
}
