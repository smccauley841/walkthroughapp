using Microsoft.AspNetCore.Mvc;
using WalkthroughApp_API.DAL;
using WalkthroughApp_API.DAL.Questions;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.Controllers
{
    public class QuestionsController : WalkthroughAppApiController
    {
        private readonly IGetQuestions _getQuestion;
        private readonly ICreate<Question, NewQuestion> _createQuestion;
        private readonly IDelete<Question> _deleteQuestion;
        private readonly IUpdate<Question> _updateQuestion;

        public QuestionsController(IGetQuestions getQuestion, ICreate<Question, NewQuestion> createQuestion, IDelete<Question> deleteQuestion,
                                   IUpdate<Question> updateQuestion)
        {
            _getQuestion = getQuestion;
            _createQuestion = createQuestion;
            _deleteQuestion = deleteQuestion;
            _updateQuestion = updateQuestion;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var questions = _getQuestion.GetAll().Result;
            if (!questions.Any())
                return NotFound("No questions were found");

            return Ok(questions);
        }

        [HttpGet("walkthrough/{walkthroughId}")]
        public async Task<IActionResult> GetQuestionsByWalkthroughId(int walkthroughId)
        {
            var questions = _getQuestion.GetByWalkthroughId(walkthroughId);

            if (!questions.Any())
                return NotFound("No questions were found for this walkthrough");

            return Ok(questions);
        }

        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetQuestionsByQuestionId(int questionId)
        {
            var questions = _getQuestion.GetById(questionId);

            if (questions == null)
                return NotFound("No question was found with this Id");

            return Ok(questions);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewQuestion([FromBody] NewQuestion newQuestion) =>
            Created("", _createQuestion.Create(newQuestion));

        [HttpDelete]
        public async Task<IActionResult> DeleteQuestion(int deletedQuestion)
        {
            var question = _getQuestion.GetById(deletedQuestion);
            if(question == null)
                return NotFound("Question does not exist");
            
            await _deleteQuestion.Delete(question);

            return NoContent();
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateQuestion(Question updatedQuestion)
        {
            var question = _getQuestion.GetById(updatedQuestion.Id);
            if (question == null)
                return NotFound("Question does not exist");
            
            question.QuestionText = updatedQuestion.QuestionText;

            await _updateQuestion.UpdateItem(question);

            return Ok(question);
        }
    }
}
