using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using ChatGptBot.Services;


namespace ChatGptBot.Bots
{
    public class ChatGptBot : ActivityHandler
    {
        private readonly ChatGptService _chatGptService;
        private readonly ConversationState _conversationState;

        public ChatGptBot(ChatGptService chatGptService, ConversationState conversationState)
        {
            _chatGptService = chatGptService;
            _conversationState = conversationState;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationStateAccessors = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
             var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new ConversationData());

            var message=turnContext.Activity.Text;
            var responseMessage = await _chatGptService.SendMessageAsync(message,conversationData.messageHistory);

            conversationData.messageHistory.Add(new ChatMessageData(){role="user",content=message});
            conversationData.messageHistory.Add(new ChatMessageData(){role="assistant",content=responseMessage});

            await turnContext.SendActivityAsync(MessageFactory.Text(responseMessage), cancellationToken);
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
    }
}
