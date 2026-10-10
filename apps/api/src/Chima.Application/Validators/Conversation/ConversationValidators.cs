using Chima.Application.DTOs.Conversation;
using Chima.Application.DTOs.Message;
using FluentValidation;

namespace Chima.Application.Validators.Conversation;

public class CreateConversationRequestValidator : AbstractValidator<CreateConversationRequest>
{
    public CreateConversationRequestValidator()
    {
        RuleFor(x => x.Title).MaximumLength(200);
    }
}

public class UpdateConversationRequestValidator : AbstractValidator<UpdateConversationRequest>
{
    public UpdateConversationRequestValidator()
    {
        RuleFor(x => x.Title).MaximumLength(200);
    }
}

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(32_000);
    }
}

public class EditMessageRequestValidator : AbstractValidator<EditMessageRequest>
{
    public EditMessageRequestValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(32_000);
    }
}