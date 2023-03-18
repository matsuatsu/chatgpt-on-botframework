using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3;
using OpenAI.GPT3.ObjectModels;
using System.Linq;

namespace ChatGptBot.Services
{
    public class ChatGptService
    {
        private readonly IConfiguration _configuration;

        public ChatGptService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> SendMessageAsync(string message,List<ChatMessageData> messageHistory=null)
        {
            var openAiService = new OpenAIService(new OpenAiOptions()
            {
                ApiKey =   _configuration["OpenaiApiKey"],
            });


            var messages=new List<ChatMessage>{
                ChatMessage.FromSystem("あなたは優秀な情報提供アシスタントです。")
            };

            if(messageHistory!=null){
                foreach(ChatMessageData msg in messageHistory){
                    if(msg.role=="user"){
                        messages.Add(ChatMessage.FromUser(msg.content));
                    }else{
                        messages.Add(ChatMessage.FromAssistant(msg.content));
                    }
                }
            }

            messages.Add(ChatMessage.FromUser(message));

            var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = messages,
                Model = Models.ChatGpt3_5Turbo,
                MaxTokens = 50//optional
            });

            if (completionResult.Successful)
            {
                return completionResult.Choices.First().Message.Content;
            }
            else{
                return "failed";
            }
        }
    }
}
