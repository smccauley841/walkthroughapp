using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.Questions;

public class GetQuestions : IGetQuestions

{
    private readonly DataContext _context;

    public GetQuestions(DataContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<Question>> GetAll() => _context.Questions.ToList();

    public Question GetById(int id) => _context.Questions
                                                .FirstOrDefault(x => x.Id == id);

    public IList<Question> GetByWalkthroughId(int walkThroughId) => _context.Questions
                                                                            .Where(x => x.Walkthrough.Id == walkThroughId)
                                                                            .ToList();
}

