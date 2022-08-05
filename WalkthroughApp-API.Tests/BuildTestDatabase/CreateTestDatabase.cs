using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.Tests.BuildTestDatabase
{
    public class CreateTestDatabase
    {

        public DataContext BuildContext(string key)
        {
            var builder = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(key);
            var context = new DataContext(builder.Options);
            createJotTitlesTable(context);
            CreateWalkThroughTable(context);
            CreateQuestionTable(context);
            CreateChoicesTable(context);
            CreateUserWalkthroughQuizTable(context);
            CreateUserQuestionResultTable(context);

            return context;
        }

        private static void CreateUserQuestionResultTable(DataContext context)
        {
            var questionOne = context.Questions.FirstOrDefault(x => x.Id == 1);
            var questionTwo = context.Questions.FirstOrDefault(x => x.Id == 2);
            var questionThree = context.Questions.FirstOrDefault(x => x.Id == 3);
            var walkThroughQuizzes = context.UserWalkthroughQuizzes.ToList();
            var choices = context.Choices.ToList();
            context.UserQuestionResults.AddRange(new[]
            {
                new UserQuestionResult
                {
                    Id = 1, UserId = 1, IsAnswerCorrect = true, QuestionId = questionOne, QuizId = walkThroughQuizzes[0],
                    SelectedChoice = choices[0]
                },
                new UserQuestionResult
                {
                    Id = 2, UserId = 1, IsAnswerCorrect = false, QuestionId = questionThree, QuizId = walkThroughQuizzes[0],
                    SelectedChoice = choices[1]
                },
                new UserQuestionResult
                {
                    Id = 3, UserId = 2, IsAnswerCorrect = false, QuestionId = questionTwo, QuizId = walkThroughQuizzes[3],
                    SelectedChoice = choices[5]
                },
            });
            context.SaveChanges();
        }

        private static void CreateUserWalkthroughQuizTable(DataContext context)
        {
            var walkthroughOne = context.Walkthroughs.FirstOrDefault(x => x.Id == 1);
            var walkthroughTwo = context.Walkthroughs.FirstOrDefault(x => x.Id == 1);
            context.UserWalkthroughQuizzes.AddRange(new[]
            {
                new UserWalkthroughQuiz
                    { Id = 1, TimeTaken = new DateTime(2022, 06, 01, 06, 00, 00), UserId = 1, Walkthrough = walkthroughOne },
                new UserWalkthroughQuiz
                    { Id = 2, TimeTaken = new DateTime(2022, 06, 02, 06, 00, 00), UserId = 1, Walkthrough = walkthroughOne },
                new UserWalkthroughQuiz
                    { Id = 3, TimeTaken = new DateTime(2022, 06, 03, 06, 00, 00), UserId = 1, Walkthrough = walkthroughOne },
                new UserWalkthroughQuiz
                    { Id = 4, TimeTaken = new DateTime(2022, 06, 04, 06, 00, 00), UserId = 2, Walkthrough = walkthroughTwo },
                new UserWalkthroughQuiz
                    { Id = 5, TimeTaken = new DateTime(2022, 06, 05, 06, 00, 00), UserId = 2, Walkthrough = walkthroughTwo },
                new UserWalkthroughQuiz
                    { Id = 6, TimeTaken = new DateTime(2022, 06, 06, 06, 00, 00), UserId = 2, Walkthrough = walkthroughTwo }
            });
            context.SaveChanges();
        }

        private static void CreateChoicesTable(DataContext context)
        {
            var questionOne = context.Questions.FirstOrDefault(x => x.Id == 1);
            var questionTwo = context.Questions.FirstOrDefault(x => x.Id == 2);
            var questionThree = context.Questions.FirstOrDefault(x => x.Id == 3);
            context.Choices.AddRange(new[]
            {
                new Choice() { Id = 1, ChoiceText = "choice 1", IsAnswer = true, QuestionId = questionOne },
                new Choice() { Id = 2, ChoiceText = "choice 2", IsAnswer = false, QuestionId = questionOne },
                new Choice() { Id = 3, ChoiceText = "choice 3", IsAnswer = false, QuestionId = questionOne },
                new Choice() { Id = 4, ChoiceText = "choice 4", IsAnswer = false, QuestionId = questionOne },
                new Choice() { Id = 5, ChoiceText = "choice 1", IsAnswer = false, QuestionId = questionTwo },
                new Choice() { Id = 6, ChoiceText = "choice 2", IsAnswer = false, QuestionId = questionTwo },
                new Choice() { Id = 7, ChoiceText = "choice 3", IsAnswer = false, QuestionId = questionTwo },
                new Choice() { Id = 8, ChoiceText = "choice 4", IsAnswer = true, QuestionId = questionTwo },
                new Choice() { Id = 9, ChoiceText = "choice 1", IsAnswer = false, QuestionId = questionThree },
                new Choice() { Id = 10, ChoiceText = "choice 2", IsAnswer = false, QuestionId = questionThree },
                new Choice() { Id = 11, ChoiceText = "choice 3", IsAnswer = true, QuestionId = questionThree }
            });
            context.SaveChanges();
        }

        private static void CreateQuestionTable(DataContext context)
        {
            var walkthroughOne = context.Walkthroughs.FirstOrDefault(x => x.Id == 1);
            var walkthroughTwo = context.Walkthroughs.FirstOrDefault(x => x.Id == 1);
            context.Questions.AddRange(new[]
            {
                new Question() { Id = 1, QuestionText = "any question", Walkthrough = walkthroughOne },
                new Question() { Id = 2, QuestionText = "any other question", Walkthrough = walkthroughTwo },
                new Question() { Id = 3, QuestionText = "any other question for walkthrough one", Walkthrough = walkthroughOne },
            });
            context.SaveChanges();
        }

        private static void CreateWalkThroughTable(DataContext context)
        {
            var jobTitleOne = context.JobTitles.FirstOrDefault(x => x.Id == 1);
            var jobTitleTwo = context.JobTitles.FirstOrDefault(x => x.Id == 2);

            context.Walkthroughs.AddRange(new[]
            {
                new Walkthrough
                {
                    Id = 1, WalkthroughName = "any walkthrough name",
                    EmployeeRole = jobTitleOne
                },
                new Walkthrough
                {
                    Id = 2, WalkthroughName = "any other walkthrough name",
                    EmployeeRole = jobTitleTwo
                }
            });
            context.SaveChanges();
        }

        private static void createJotTitlesTable(DataContext context)
        {
            context.JobTitles.AddRange(new[]
            {
                new JobTitle { Id = 1, Name = "any job title" },
                new JobTitle { Id = 2, Name = "any other job title" }
            });
            context.SaveChanges();
        }
    }
}
