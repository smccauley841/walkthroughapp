using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.Questions
{
    public class CreateQuestion : ICreate<Question, NewQuestion>
    {
        private readonly DataContext _context;

        public CreateQuestion(DataContext context)
        {
            _context = context;
        }
        public Question Create(NewQuestion newItem)
        {
            var walkthrough = _context.Walkthroughs.FirstOrDefault(x => x.Id == newItem.WalkthroughId);

            var question = new Question
            {
                QuestionText = newItem.QuestionText,
                Walkthrough = walkthrough
            };

            _context.Questions.Add(question);

            _context.SaveChanges();

            return question;
        }

        public bool DoesItemExist(NewQuestion newItem)
        {
            throw new NotImplementedException();
        }
    }
}
