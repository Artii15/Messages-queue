using ServiceStack.FluentValidation;
using ServiceStack.ServiceInterface;

namespace Server
{
	public class CreateTopicValidator : AbstractValidator<CreateTopic>
	{
		public CreateTopicValidator() 
		{
			RuleSet(ApplyTo.Post, () => { RuleFor(r => r.Name).NotEmpty().WithErrorCode("ShouldNotBeEmpty"); });
			RuleSet(ApplyTo.Post, () => { RuleFor(r => r.Name).Matches("^[a-zA-Z0-9]+$").WithErrorCode("InvalidName"); });
		}
	}
}
