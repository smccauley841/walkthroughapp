using Microsoft.EntityFrameworkCore;
using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;
using WalkthroughApp_API.Exceptions;

namespace WalkthroughApp_API.DAL.Choices
{
    public class CreateChoices : ICreateChoices
    {
        private readonly DataContext _context;

        public CreateChoices(DataContext context)
        {
            _context = context;
        }
        public async Task<IList<Choice>> CreateQuestionChoices(NewChoice[] newChoices)
        {
            var choices = new List<Choice>();
            foreach (var choice in newChoices)
            {
                var question = await _context.Questions.FirstOrDefaultAsync(x => x.Id == choice.Question);

                if (question == null)
                    throw new QuestionNotFoundException($"Unable to find a question with Id of {choice.Question}");

                var newChoice = new Choice
                {
                    ChoiceText = choice.ChoiceText,
                    IsAnswer = choice.IsAnswer,
                    QuestionId = question
                };
                _context.Choices.Add(newChoice);
                await _context.SaveChangesAsync();
                choices.Add(newChoice);
            }

            //choices.ForEach(AddChoice);
            //await _context.SaveChangesAsync();

            return choices;
        }

        public Task<bool> DoesQuestionChoiceExist(NewChoice newChoice) => _context.Choices.AnyAsync(x =>
                                                                        x.ChoiceText.ToLower() == newChoice.ChoiceText.ToLower() &&
                                                                        x.QuestionId.Id == newChoice.Question);

        public Task<bool> DoesQuestionAlreadyHaveACorrectAnswer(NewChoice newChoice) => _context.Choices.AnyAsync(x =>
                                                                                        x.IsAnswer == true &&
                                                                                        x.QuestionId.Id == newChoice.Question);

        private  void AddChoice(Choice x)
        {
            _context.Choices.Add(x);
        }
    }
}
